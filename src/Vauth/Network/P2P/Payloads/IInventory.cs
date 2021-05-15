namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Represents a message that can be relayed on the Vauth network.
    /// </summary>
    public interface IInventory : IVerifiable
    {
        /// <summary>
        /// The type of the inventory.
        /// </summary>
        InventoryType InventoryType { get; }
    }
}
