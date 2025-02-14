using Vauth.Cryptography;
using Vauth.IO;
using Vauth.Network.P2P.Payloads;
using System.IO;

namespace Vauth.Network.P2P
{
    /// <summary>
    /// A helper class for <see cref="IVerifiable"/>.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Calculates the hash of a <see cref="IVerifiable"/>.
        /// </summary>
        /// <param name="verifiable">The <see cref="IVerifiable"/> object to hash.</param>
        /// <returns>The hash of the object.</returns>
        public static UInt256 CalculateHash(this IVerifiable verifiable)
        {
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);
            verifiable.SerializeUnsigned(writer);
            writer.Flush();
            return new UInt256(ms.ToArray().Sha256());
        }

        /// <summary>
        /// Gets the data of a <see cref="IVerifiable"/> object to be hashed.
        /// </summary>
        /// <param name="verifiable">The <see cref="IVerifiable"/> object to hash.</param>
        /// <param name="network">The magic number of the network.</param>
        /// <returns>The data to hash.</returns>
        public static byte[] GetSignData(this IVerifiable verifiable, uint network)
        {
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);
            writer.Write(network);
            writer.Write(verifiable.Hash);
            writer.Flush();
            return ms.ToArray();
        }
    }
}
