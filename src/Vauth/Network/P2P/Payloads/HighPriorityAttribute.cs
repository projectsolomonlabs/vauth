using Vauth.Persistence;
using Vauth.SmartContract.Native;
using System.IO;
using System.Linq;

namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Indicates that the transaction is of high priority.
    /// </summary>
    public class HighPriorityAttribute : TransactionAttribute
    {
        public override bool AllowMultiple => false;
        public override TransactionAttributeType Type => TransactionAttributeType.HighPriority;

        protected override void DeserializeWithoutType(BinaryReader reader)
        {
        }

        protected override void SerializeWithoutType(BinaryWriter writer)
        {
        }

        public override bool Verify(DataCache snapshot, Transaction tx)
        {
            UInt160 committee = NativeContract.Vauth.GetCommitteeAddress(snapshot);
            return tx.Signers.Any(p => p.Account.Equals(committee));
        }
    }
}
