#pragma warning disable IDE0051

using Vauth.Cryptography;
using Vauth.IO;
using Vauth.Network.P2P.Payloads;
using Vauth.Persistence;
using Vauth.SmartContract.Manifest;
using Vauth.VM;
using Vauth.VM.Types;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// The native Oracle service for Vauth system.
    /// </summary>
    public sealed class OracleContract : NativeContract
    {
        private const int MaxUrlLength = 256;
        private const int MaxFilterLength = 128;
        private const int MaxCallbackLength = 32;
        private const int MaxUserDataLength = 512;

        private const byte Prefix_Price = 5;
        private const byte Prefix_RequestId = 9;
        private const byte Prefix_Request = 7;
        private const byte Prefix_IdList = 6;

        internal OracleContract()
        {
            var events = new List<ContractEventDescriptor>(Manifest.Abi.Events)
            {
                new ContractEventDescriptor
                {
                    Name = "OracleRequest",
                    Parameters = new ContractParameterDefinition[]
                    {
                        new ContractParameterDefinition()
                        {
                            Name = "Id",
                            Type = ContractParameterType.Integer
                        },
                        new ContractParameterDefinition()
                        {
                            Name = "RequestContract",
                            Type = ContractParameterType.Hash160
                        },
                        new ContractParameterDefinition()
                        {
                            Name = "Url",
                            Type = ContractParameterType.String
                        },
                        new ContractParameterDefinition()
                        {
                            Name = "Filter",
                            Type = ContractParameterType.String
                        }
                    }
                },
                new ContractEventDescriptor
                {
                    Name = "OracleResponse",
                    Parameters = new ContractParameterDefinition[]
                    {
                        new ContractParameterDefinition()
                        {
                            Name = "Id",
                            Type = ContractParameterType.Integer
                        },
                        new ContractParameterDefinition()
                        {
                            Name = "OriginalTx",
                            Type = ContractParameterType.Hash256
                        }
                    }
                }
            };

            Manifest.Abi.Events = events.ToArray();
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States)]
        private void SetPrice(ApplicationEngine engine, long price)
        {
            if (price <= 0)
                throw new ArgumentOutOfRangeException(nameof(price));
            if (!CheckCommittee(engine)) throw new InvalidOperationException();
            engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_Price)).Set(price);
        }

        /// <summary>
        /// Gets the price for an Oracle request.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <returns>The price for an Oracle request.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public long GetPrice(DataCache snapshot)
        {
            return (long)(BigInteger)snapshot[CreateStorageKey(Prefix_Price)];
        }

        [ContractMethod(RequiredCallFlags = CallFlags.States | CallFlags.AllowCall | CallFlags.AllowNotify)]
        private ContractTask Finish(ApplicationEngine engine)
        {
            Transaction tx = (Transaction)engine.ScriptContainer;
            OracleResponse response = tx.GetAttribute<OracleResponse>();
            if (response == null) throw new ArgumentException("Oracle response was not found");
            OracleRequest request = GetRequest(engine.Snapshot, response.Id);
            if (request == null) throw new ArgumentException("Oracle request was not found");
            engine.SendNotification(Hash, "OracleResponse", new VM.Types.Array { response.Id, request.OriginalTxid.ToArray() });
            StackItem userData = BinarySerializer.Deserialize(request.UserData, engine.Limits.MaxStackSize, engine.ReferenceCounter);
            return engine.CallFromNativeContract(Hash, request.CallbackContract, request.CallbackMethod, request.Url, userData, (int)response.Code, response.Result);
        }

        private UInt256 GetOriginalTxid(ApplicationEngine engine)
        {
            Transaction tx = (Transaction)engine.ScriptContainer;
            OracleResponse response = tx.GetAttribute<OracleResponse>();
            if (response is null) return tx.Hash;
            OracleRequest request = GetRequest(engine.Snapshot, response.Id);
            return request.OriginalTxid;
        }

        /// <summary>
        /// Gets a pending request with the specified id.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <param name="id">The id of the request.</param>
        /// <returns>The pending request. Or <see langword="null"/> if no request with the specified id is found.</returns>
        public OracleRequest GetRequest(DataCache snapshot, ulong id)
        {
            return snapshot.TryGet(CreateStorageKey(Prefix_Request).AddBigEndian(id))?.GetInteroperable<OracleRequest>();
        }

        /// <summary>
        /// Gets all the pending requests.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <returns>All the pending requests.</returns>
        public IEnumerable<(ulong, OracleRequest)> GetRequests(DataCache snapshot)
        {
            return snapshot.Find(CreateStorageKey(Prefix_Request).ToArray()).Select(p => (BinaryPrimitives.ReadUInt64BigEndian(p.Key.Key.AsSpan(1)), p.Value.GetInteroperable<OracleRequest>()));
        }

        /// <summary>
        /// Gets the requests with the specified url.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <param name="url">The url of the requests.</param>
        /// <returns>All the requests with the specified url.</returns>
        public IEnumerable<(ulong, OracleRequest)> GetRequestsByUrl(DataCache snapshot, string url)
        {
            IdList list = snapshot.TryGet(CreateStorageKey(Prefix_IdList).Add(GetUrlHash(url)))?.GetInteroperable<IdList>();
            if (list is null) yield break;
            foreach (ulong id in list)
                yield return (id, snapshot[CreateStorageKey(Prefix_Request).AddBigEndian(id)].GetInteroperable<OracleRequest>());
        }

        private static byte[] GetUrlHash(string url)
        {
            return Crypto.Hash160(Utility.StrictUTF8.GetBytes(url));
        }

        internal override ContractTask Initialize(ApplicationEngine engine)
        {
            engine.Snapshot.Add(CreateStorageKey(Prefix_RequestId), new StorageItem(BigInteger.Zero));
            engine.Snapshot.Add(CreateStorageKey(Prefix_Price), new StorageItem(0_50000000));
            return ContractTask.CompletedTask;
        }

        internal override async ContractTask PostPersist(ApplicationEngine engine)
        {
            (UInt160 Account, BigInteger VALT)[] nodes = null;
            foreach (Transaction tx in engine.PersistingBlock.Transactions)
            {
                //Filter the response transactions
                OracleResponse response = tx.GetAttribute<OracleResponse>();
                if (response is null) continue;

                //Remove the request from storage
                StorageKey key = CreateStorageKey(Prefix_Request).AddBigEndian(response.Id);
                OracleRequest request = engine.Snapshot.TryGet(key)?.GetInteroperable<OracleRequest>();
                if (request == null) continue;
                engine.Snapshot.Delete(key);

                //Remove the id from IdList
                key = CreateStorageKey(Prefix_IdList).Add(GetUrlHash(request.Url));
                IdList list = engine.Snapshot.GetAndChange(key).GetInteroperable<IdList>();
                if (!list.Remove(response.Id)) throw new InvalidOperationException();
                if (list.Count == 0) engine.Snapshot.Delete(key);

                //Mint VALT for oracle nodes
                nodes ??= RoleManagement.GetDesignatedByRole(engine.Snapshot, Role.Oracle, engine.PersistingBlock.Index).Select(p => (Contract.CreateSignatureRedeemScript(p).ToScriptHash(), BigInteger.Zero)).ToArray();
                if (nodes.Length > 0)
                {
                    int index = (int)(response.Id % (ulong)nodes.Length);
                    nodes[index].VALT += GetPrice(engine.Snapshot);
                }
            }
            if (nodes != null)
            {
                foreach (var (account, valt) in nodes)
                {
                    if (valtSign > 0)
                        await VALT.Mint(engine, account, valt, false);
                }
            }
        }

        [ContractMethod(RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        private async ContractTask Request(ApplicationEngine engine, string url, string filter, string callback, StackItem userData, long valtForResponse)
        {
            //Check the arguments
            if (Utility.StrictUTF8.GetByteCount(url) > MaxUrlLength
                || (filter != null && Utility.StrictUTF8.GetByteCount(filter) > MaxFilterLength)
                || Utility.StrictUTF8.GetByteCount(callback) > MaxCallbackLength || callback.StartsWith('_')
                || valtForResponse < 0_10000000)
                throw new ArgumentException();

            engine.AddValt(GetPrice(engine.Snapshot));

            //Mint valt for the response
            engine.AddValt(valtForResponse);
            await VALT.Mint(engine, Hash, valtForResponse, false);

            //Increase the request id
            StorageItem item_id = engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_RequestId));
            ulong id = (ulong)(BigInteger)item_id;
            item_id.Add(1);

            //Put the request to storage
            if (ContractManagement.GetContract(engine.Snapshot, engine.CallingScriptHash) is null)
                throw new InvalidOperationException();
            engine.Snapshot.Add(CreateStorageKey(Prefix_Request).AddBigEndian(id), new StorageItem(new OracleRequest
            {
                OriginalTxid = GetOriginalTxid(engine),
                ValtForResponse = valtForResponse,
                Url = url,
                Filter = filter,
                CallbackContract = engine.CallingScriptHash,
                CallbackMethod = callback,
                UserData = BinarySerializer.Serialize(userData, MaxUserDataLength)
            }));

            //Add the id to the IdList
            var list = engine.Snapshot.GetAndChange(CreateStorageKey(Prefix_IdList).Add(GetUrlHash(url)), () => new StorageItem(new IdList())).GetInteroperable<IdList>();
            if (list.Count >= 256)
                throw new InvalidOperationException("There are too many pending responses for this url");
            list.Add(id);

            engine.SendNotification(Hash, "OracleRequest", new VM.Types.Array { id, engine.CallingScriptHash.ToArray(), url, filter ?? StackItem.Null });
        }

        [ContractMethod(CpuFee = 1 << 15)]
        private bool Verify(ApplicationEngine engine)
        {
            Transaction tx = (Transaction)engine.ScriptContainer;
            return tx?.GetAttribute<OracleResponse>() != null;
        }

        private class IdList : List<ulong>, IInteroperable
        {
            public void FromStackItem(StackItem stackItem)
            {
                foreach (StackItem item in (VM.Types.Array)stackItem)
                    Add((ulong)item.GetInteger());
            }

            public StackItem ToStackItem(ReferenceCounter referenceCounter)
            {
                return new VM.Types.Array(referenceCounter, this.Select(p => (Integer)p));
            }
        }
    }
}
