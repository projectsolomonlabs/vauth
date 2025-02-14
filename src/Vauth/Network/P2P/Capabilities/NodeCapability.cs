using Vauth.IO;
using System;
using System.IO;

namespace Vauth.Network.P2P.Capabilities
{
    /// <summary>
    /// Represents the capabilities of a Vauth node.
    /// </summary>
    public abstract class NodeCapability : ISerializable
    {
        /// <summary>
        /// Indicates the type of the <see cref="NodeCapability"/>.
        /// </summary>
        public readonly NodeCapabilityType Type;

        public virtual int Size => sizeof(NodeCapabilityType); // Type

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCapability"/> class.
        /// </summary>
        /// <param name="type">The type of the <see cref="NodeCapability"/>.</param>
        protected NodeCapability(NodeCapabilityType type)
        {
            this.Type = type;
        }

        void ISerializable.Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != (byte)Type)
            {
                throw new FormatException();
            }

            DeserializeWithoutType(reader);
        }

        /// <summary>
        /// Deserializes an <see cref="NodeCapability"/> object from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> for reading data.</param>
        /// <returns>The deserialized <see cref="NodeCapability"/>.</returns>
        public static NodeCapability DeserializeFrom(BinaryReader reader)
        {
            NodeCapabilityType type = (NodeCapabilityType)reader.ReadByte();
            NodeCapability capability = type switch
            {
                NodeCapabilityType.TcpServer or NodeCapabilityType.WsServer => new ServerCapability(type),
                NodeCapabilityType.FullNode => new FullNodeCapability(),
                _ => throw new FormatException(),
            };
            capability.DeserializeWithoutType(reader);
            return capability;
        }

        /// <summary>
        /// Deserializes the <see cref="NodeCapability"/> object from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> for reading data.</param>
        protected abstract void DeserializeWithoutType(BinaryReader reader);

        void ISerializable.Serialize(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            SerializeWithoutType(writer);
        }

        /// <summary>
        /// Serializes the <see cref="NodeCapability"/> object to a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> for writing data.</param>
        protected abstract void SerializeWithoutType(BinaryWriter writer);
    }
}
