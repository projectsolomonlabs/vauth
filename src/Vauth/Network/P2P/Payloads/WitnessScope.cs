using System;

namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Represents the scope of a <see cref="Witness"/>.
    /// </summary>
    [Flags]
    public enum WitnessScope : byte
    {
        /// <summary>
        /// Indicates that no contract was witnessed. Only sign the transaction.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the calling contract must be the entry contract.
        /// The witness/permission/signature given on first invocation will automatically expire if entering deeper internal invokes.
        /// This can be the default safe choice for native Vauth/VALT (previously used on Vauth 2 as "attach" mode).
        /// </summary>
        CalledByEntry = 0x01,

        /// <summary>
        /// Custom hash for contract-specific.
        /// </summary>
        CustomContracts = 0x10,

        /// <summary>
        ///  Custom pubkey for group members.
        /// </summary>
        CustomGroups = 0x20,

        /// <summary>
        /// This allows the witness in all contexts (default Vauth2 behavior).
        /// </summary>
        /// <remarks>Note: It cannot be combined with other flags.</remarks>
        Global = 0x80
    }
}
