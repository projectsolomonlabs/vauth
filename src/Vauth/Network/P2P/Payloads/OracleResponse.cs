using Vauth.IO;
using Vauth.IO.Json;
using Vauth.Persistence;
using Vauth.SmartContract;
using Vauth.SmartContract.Native;
using Vauth.VM;
using System;
using System.IO;
using System.Linq;

namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Indicates that the transaction is an oracle response.
    /// </summary>
    public class OracleResponse : TransactionAttribute
    {
        /// <summary>
        /// Indicates the maximum size of the <see cref="Result"/> field.
        /// </summary>
        public const int MaxResultSize = ushort.MaxValue;

        /// <summary>
        /// Represents the fixed value of the <see cref="Transaction.Script"/> field of the oracle responding transaction.
        /// </summary>
        public static readonly byte[] FixedScript;

        /// <summary>
        /// The ID of the oracle request.
        /// </summary>
        public ulong Id;

        /// <summary>
        /// The response code for the oracle request.
        /// </summary>
        public OracleResponseCode Code;

        /// <summary>
        /// The result for the oracle request.
        /// </summary>
        public byte[] Result;

        public override TransactionAttributeType Type => TransactionAttributeType.OracleResponse;
        public override bool AllowMultiple => false;

        public override int Size => base.Size +
            sizeof(ulong) +                 //Id
            sizeof(OracleResponseCode) +    //ResponseCode
            Result.GetVarSize();            //Result

        static OracleResponse()
        {
            using ScriptBuilder sb = new();
            sb.EmitDynamicCall(NativeContract.Oracle.Hash, "finish");
            FixedScript = sb.ToArray();
        }

        protected override void DeserializeWithoutType(BinaryReader reader)
        {
            Id = reader.ReadUInt64();
            Code = (OracleResponseCode)reader.ReadByte();
            if (!Enum.IsDefined(typeof(OracleResponseCode), Code))
                throw new FormatException();
            Result = reader.ReadVarBytes(MaxResultSize);
            if (Code != OracleResponseCode.Success && Result.Length > 0)
                throw new FormatException();
        }

        protected override void SerializeWithoutType(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write((byte)Code);
            writer.WriteVarBytes(Result);
        }

        public override JObject ToJson()
        {
            JObject json = base.ToJson();
            json["id"] = Id;
            json["code"] = Code;
            json["result"] = Convert.ToBase64String(Result);
            return json;
        }

        public override bool Verify(DataCache snapshot, Transaction tx)
        {
            if (tx.Signers.Any(p => p.Scopes != WitnessScope.None)) return false;
            if (!tx.Script.AsSpan().SequenceEqual(FixedScript)) return false;
            OracleRequest request = NativeContract.Oracle.GetRequest(snapshot, Id);
            if (request is null) return false;
            if (tx.NetworkFee + tx.SystemFee != request.ValtForResponse) return false;
            UInt160 oracleAccount = Contract.GetBFTAddress(NativeContract.RoleManagement.GetDesignatedByRole(snapshot, Role.Oracle, NativeContract.Ledger.CurrentIndex(snapshot) + 1));
            return tx.Signers.Any(p => p.Account.Equals(oracleAccount));
        }
    }
}
