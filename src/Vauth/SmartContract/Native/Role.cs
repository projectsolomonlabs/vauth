namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// Represents the roles in the Vauth system.
    /// </summary>
    public enum Role : byte
    {
        /// <summary>
        /// The validators of state. Used to generate and sign the state root.
        /// </summary>
        StateValidator = 4,

        /// <summary>
        /// The nodes used to process Oracle requests.
        /// </summary>
        Oracle = 8,

        /// <summary>
        /// VauthFS Alphabet nodes.
        /// </summary>
        VauthFSAlphabetNode = 16
    }
}
