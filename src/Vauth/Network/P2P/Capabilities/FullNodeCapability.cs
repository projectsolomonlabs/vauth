using System.IO;

namespace Vauth.Network.P2P.Capabilities
{
    /// <summary>
    /// Indicates that a node has complete block data.
    /// </summary>
    public class FullNodeCapability : NodeCapability
    {
        /// <summary>
        /// Indicates the current block height of the node.
        /// </summary>
        public uint StartHeight;

        public override int Size =>
            base.Size +    // Type
            sizeof(uint);  // Start Height

        /// <summary>
        /// Initializes a new instance of the <see cref="FullNodeCapability"/> class.
        /// </summary>
        /// <param name="startHeight">The current block height of the node.</param>
        public FullNodeCapability(uint startHeight = 0) : base(NodeCapabilityType.FullNode)
        {
            StartHeight = startHeight;
        }

        protected override void DeserializeWithoutType(BinaryReader reader)
        {
            StartHeight = reader.ReadUInt32();
        }

        protected override void SerializeWithoutType(BinaryWriter writer)
        {
            writer.Write(StartHeight);
        }
    }
}
