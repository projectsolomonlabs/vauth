using Vauth.IO;
using Vauth.IO.Json;
using Vauth.SmartContract.Manifest;
using Vauth.VM;
using Vauth.VM.Types;
using System.Linq;
using Array = Vauth.VM.Types.Array;

namespace Vauth.SmartContract
{
    /// <summary>
    /// Represents a deployed contract.
    /// </summary>
    public class ContractState : IInteroperable
    {
        /// <summary>
        /// The id of the contract.
        /// </summary>
        public int Id;

        /// <summary>
        /// Indicates the number of times the contract has been updated.
        /// </summary>
        public ushort UpdateCounter;

        /// <summary>
        /// The hash of the contract.
        /// </summary>
        public UInt160 Hash;

        /// <summary>
        /// The nef of the contract.
        /// </summary>
        public NefFile Nef;

        /// <summary>
        /// The manifest of the contract.
        /// </summary>
        public ContractManifest Manifest;

        /// <summary>
        /// The script of the contract.
        /// </summary>
        public byte[] Script => Nef.Script;

        void IInteroperable.FromStackItem(StackItem stackItem)
        {
            Array array = (Array)stackItem;
            Id = (int)array[0].GetInteger();
            UpdateCounter = (ushort)array[1].GetInteger();
            Hash = new UInt160(array[2].GetSpan());
            Nef = array[3].GetSpan().AsSerializable<NefFile>();
            Manifest = array[4].ToInteroperable<ContractManifest>();
        }

        /// <summary>
        /// Determines whether the current contract has the permission to call the specified contract.
        /// </summary>
        /// <param name="targetContract">The contract to be called.</param>
        /// <param name="targetMethod">The method to be called.</param>
        /// <returns><see langword="true"/> if the contract allows to be called; otherwise, <see langword="false"/>.</returns>
        public bool CanCall(ContractState targetContract, string targetMethod)
        {
            return Manifest.Permissions.Any(u => u.IsAllowed(targetContract, targetMethod));
        }

        /// <summary>
        /// Converts the contract to a JSON object.
        /// </summary>
        /// <returns>The contract represented by a JSON object.</returns>
        public JObject ToJson()
        {
            return new JObject
            {
                ["id"] = Id,
                ["updatecounter"] = UpdateCounter,
                ["hash"] = Hash.ToString(),
                ["nef"] = Nef.ToJson(),
                ["manifest"] = Manifest.ToJson()
            };
        }

        public StackItem ToStackItem(ReferenceCounter referenceCounter)
        {
            return new Array(referenceCounter, new StackItem[] { Id, (int)UpdateCounter, Hash.ToArray(), Nef.ToArray(), Manifest.ToStackItem(referenceCounter) });
        }
    }
}
