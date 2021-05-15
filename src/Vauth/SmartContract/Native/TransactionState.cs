using Vauth.IO;
using Vauth.Network.P2P.Payloads;
using Vauth.VM;
using Vauth.VM.Types;

namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// Represents a transaction that has been included in a block.
    /// </summary>
    public class TransactionState : IInteroperable
    {
        /// <summary>
        /// The block containing this transaction.
        /// </summary>
        public uint BlockIndex;

        /// <summary>
        /// The transaction.
        /// </summary>
        public Transaction Transaction;

        private StackItem _rawTransaction;

        void IInteroperable.FromStackItem(StackItem stackItem)
        {
            Struct @struct = (Struct)stackItem;
            BlockIndex = (uint)@struct[0].GetInteger();
            _rawTransaction = @struct[1];
            Transaction = _rawTransaction.GetSpan().AsSerializable<Transaction>();
        }

        StackItem IInteroperable.ToStackItem(ReferenceCounter referenceCounter)
        {
            _rawTransaction ??= Transaction.ToArray();
            return new Struct(referenceCounter) { BlockIndex, _rawTransaction };
        }
    }
}
