using System;
using System.IO;

namespace Vauth.Network.P2P.Capabilities
{
    /// <summary>
    /// Indicates that the node is a server.
    /// </summary>
    public class ServerCapability : NodeCapability
    {
        /// <summary>
        /// Indicates the port that the node is listening on.
        /// </summary>
        public ushort Port;

        public override int Size =>
            base.Size +     // Type
            sizeof(ushort); // Port

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerCapability"/> class.
        /// </summary>
        /// <param name="type">The type of the <see cref="ServerCapability"/>. It must be <see cref="NodeCapabilityType.TcpServer"/> or <see cref="NodeCapabilityType.WsServer"/></param>
        /// <param name="port">The port that the node is listening on.</param>
        public ServerCapability(NodeCapabilityType type, ushort port = 0) : base(type)
        {
            if (type != NodeCapabilityType.TcpServer && type != NodeCapabilityType.WsServer)
            {
                throw new ArgumentException(nameof(type));
            }

            Port = port;
        }

        protected override void DeserializeWithoutType(BinaryReader reader)
        {
            Port = reader.ReadUInt16();
        }

        protected override void SerializeWithoutType(BinaryWriter writer)
        {
            writer.Write(Port);
        }
    }
}
