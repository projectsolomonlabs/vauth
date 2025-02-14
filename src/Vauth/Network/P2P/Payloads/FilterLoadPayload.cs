using Vauth.Cryptography;
using Vauth.IO;
using System;
using System.IO;

namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// This message is sent to load the <see cref="BloomFilter"/>.
    /// </summary>
    public class FilterLoadPayload : ISerializable
    {
        /// <summary>
        /// The data of the <see cref="BloomFilter"/>.
        /// </summary>
        public byte[] Filter;

        /// <summary>
        /// The number of hash functions used by the <see cref="BloomFilter"/>.
        /// </summary>
        public byte K;

        /// <summary>
        /// Used to generate the seeds of the murmur hash functions.
        /// </summary>
        public uint Tweak;

        public int Size => Filter.GetVarSize() + sizeof(byte) + sizeof(uint);

        /// <summary>
        /// Creates a new instance of the <see cref="FilterLoadPayload"/> class.
        /// </summary>
        /// <param name="filter">The fields in the filter will be copied to the payload.</param>
        /// <returns>The created payload.</returns>
        public static FilterLoadPayload Create(BloomFilter filter)
        {
            byte[] buffer = new byte[filter.M / 8];
            filter.GetBits(buffer);
            return new FilterLoadPayload
            {
                Filter = buffer,
                K = (byte)filter.K,
                Tweak = filter.Tweak
            };
        }

        void ISerializable.Deserialize(BinaryReader reader)
        {
            Filter = reader.ReadVarBytes(36000);
            K = reader.ReadByte();
            if (K > 50) throw new FormatException();
            Tweak = reader.ReadUInt32();
        }

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.WriteVarBytes(Filter);
            writer.Write(K);
            writer.Write(Tweak);
        }
    }
}
