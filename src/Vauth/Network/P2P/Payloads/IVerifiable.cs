using Vauth.IO;
using Vauth.Persistence;
using System.IO;

namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Represents an object that can be verified in the Vauth network.
    /// </summary>
    public interface IVerifiable : ISerializable
    {
        /// <summary>
        /// The hash of the <see cref="IVerifiable"/> object.
        /// </summary>
        UInt256 Hash => this.CalculateHash();

        /// <summary>
        /// The witnesses of the <see cref="IVerifiable"/> object.
        /// </summary>
        Witness[] Witnesses { get; set; }

        /// <summary>
        /// Deserializes the part of the <see cref="IVerifiable"/> object other than <see cref="Witnesses"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> for reading data.</param>
        void DeserializeUnsigned(BinaryReader reader);

        /// <summary>
        /// Gets the script hashes that should be verified for this <see cref="IVerifiable"/> object.
        /// </summary>
        /// <param name="snapshot">The snapshot to be used.</param>
        /// <returns>The script hashes that should be verified.</returns>
        UInt160[] GetScriptHashesForVerifying(DataCache snapshot);

        /// <summary>
        /// Serializes the part of the <see cref="IVerifiable"/> object other than <see cref="Witnesses"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> for writing data.</param>
        void SerializeUnsigned(BinaryWriter writer);
    }
}
