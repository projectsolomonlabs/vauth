using Vauth.Cryptography;
using Vauth.Cryptography.ECC;
using Vauth.IO;
using Vauth.Persistence;
using Vauth.SmartContract.Manifest;
using Vauth.VM;
using Vauth.VM.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// A native contract for managing roles in Vauth system.
    /// </summary>
    public sealed class RoleManagement : NativeContract
    {
        internal RoleManagement()
        {
            var events = new List<ContractEventDescriptor>(Manifest.Abi.Events)
            {
                new ContractEventDescriptor
                {
                    Name = "Designation",
                    Parameters = new ContractParameterDefinition[]
                    {
                        new ContractParameterDefinition()
                        {
                            Name = "Role",
                            Type = ContractParameterType.Integer
                        },
                        new ContractParameterDefinition()
                        {
                            Name = "BlockIndex",
                            Type = ContractParameterType.Integer
                        }
                    }
                }
            };

            Manifest.Abi.Events = events.ToArray();
        }

        /// <summary>
        /// Gets the list of nodes for the specified role.
        /// </summary>
        /// <param name="snapshot">The snapshot used to read data.</param>
        /// <param name="role">The type of the role.</param>
        /// <param name="index">The index of the block to be queried.</param>
        /// <returns>The public keys of the nodes.</returns>
        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.ReadStates)]
        public ECPoint[] GetDesignatedByRole(DataCache snapshot, Role role, uint index)
        {
            if (!Enum.IsDefined(typeof(Role), role))
                throw new ArgumentOutOfRangeException(nameof(role));
            if (Ledger.CurrentIndex(snapshot) + 1 < index)
                throw new ArgumentOutOfRangeException(nameof(index));
            byte[] key = CreateStorageKey((byte)role).AddBigEndian(index).ToArray();
            byte[] boundary = CreateStorageKey((byte)role).ToArray();
            return snapshot.FindRange(key, boundary, SeekDirection.Backward)
                .Select(u => u.Value.GetInteroperable<NodeList>().ToArray())
                .FirstOrDefault() ?? System.Array.Empty<ECPoint>();
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        private void DesignateAsRole(ApplicationEngine engine, Role role, ECPoint[] nodes)
        {
            if (nodes.Length == 0 || nodes.Length > 32)
                throw new ArgumentException(null, nameof(nodes));
            if (!Enum.IsDefined(typeof(Role), role))
                throw new ArgumentOutOfRangeException(nameof(role));
            if (!CheckCommittee(engine))
                throw new InvalidOperationException(nameof(DesignateAsRole));
            if (engine.PersistingBlock is null)
                throw new InvalidOperationException(nameof(DesignateAsRole));
            uint index = engine.PersistingBlock.Index + 1;
            var key = CreateStorageKey((byte)role).AddBigEndian(index);
            if (engine.Snapshot.Contains(key))
                throw new InvalidOperationException();
            NodeList list = new();
            list.AddRange(nodes);
            list.Sort();
            engine.Snapshot.Add(key, new StorageItem(list));
            engine.SendNotification(Hash, "Designation", new VM.Types.Array(engine.ReferenceCounter, new StackItem[] { (int)role, engine.PersistingBlock.Index }));
        }

        private class NodeList : List<ECPoint>, IInteroperable
        {
            public void FromStackItem(StackItem stackItem)
            {
                foreach (StackItem item in (VM.Types.Array)stackItem)
                    Add(item.GetSpan().AsSerializable<ECPoint>());
            }

            public StackItem ToStackItem(ReferenceCounter referenceCounter)
            {
                return new VM.Types.Array(referenceCounter, this.Select(p => (StackItem)p.ToArray()));
            }
        }
    }
}
