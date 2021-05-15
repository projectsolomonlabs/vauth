using Vauth.IO;
using Vauth.VM;
using Vauth.VM.Types;
using Array = Vauth.VM.Types.Array;

namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// Represents an Oracle request in smart contracts.
    /// </summary>
    public class OracleRequest : IInteroperable
    {
        /// <summary>
        /// The original transaction that sent the related request.
        /// </summary>
        public UInt256 OriginalTxid;

        /// <summary>
        /// The maximum amount of VALT that can be used when executing response callback.
        /// </summary>
        public long ValtForResponse;

        /// <summary>
        /// The url of the request.
        /// </summary>
        public string Url;

        /// <summary>
        /// The filter for the response.
        /// </summary>
        public string Filter;

        /// <summary>
        /// The hash of the callback contract.
        /// </summary>
        public UInt160 CallbackContract;

        /// <summary>
        /// The name of the callback method.
        /// </summary>
        public string CallbackMethod;

        /// <summary>
        /// The user-defined object that will be passed to the callback.
        /// </summary>
        public byte[] UserData;

        public void FromStackItem(StackItem stackItem)
        {
            Array array = (Array)stackItem;
            OriginalTxid = new UInt256(array[0].GetSpan());
            ValtForResponse = (long)array[1].GetInteger();
            Url = array[2].GetString();
            Filter = array[3].GetString();
            CallbackContract = new UInt160(array[4].GetSpan());
            CallbackMethod = array[5].GetString();
            UserData = array[6].GetSpan().ToArray();
        }

        public StackItem ToStackItem(ReferenceCounter referenceCounter)
        {
            return new Array(referenceCounter)
            {
                OriginalTxid.ToArray(),
                ValtForResponse,
                Url,
                Filter ?? StackItem.Null,
                CallbackContract.ToArray(),
                CallbackMethod,
                UserData
            };
        }
    }
}
