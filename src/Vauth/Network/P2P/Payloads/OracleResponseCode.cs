namespace Vauth.Network.P2P.Payloads
{
    /// <summary>
    /// Represents the response code for the oracle request.
    /// </summary>
    public enum OracleResponseCode : byte
    {
        /// <summary>
        /// Indicates that the request has been successfully completed.
        /// </summary>
        Success = 0x00,

        /// <summary>
        /// Indicates that the protocol of the request is not supported.
        /// </summary>
        ProtocolNotSupported = 0x10,

        /// <summary>
        /// Indicates that the oracle nodes cannot reach a consensus on the result of the request.
        /// </summary>
        ConsensusUnreachable = 0x12,

        /// <summary>
        /// Indicates that the requested Uri does not exist.
        /// </summary>
        NotFound = 0x14,

        /// <summary>
        /// Indicates that the request was not completed within the specified time.
        /// </summary>
        Timeout = 0x16,

        /// <summary>
        /// Indicates that there is no permission to request the resource.
        /// </summary>
        Forbidden = 0x18,

        /// <summary>
        /// Indicates that the data for the response is too large.
        /// </summary>
        ResponseTooLarge = 0x1a,

        /// <summary>
        /// Indicates that the request failed due to insufficient balance.
        /// </summary>
        InsufficientFunds = 0x1c,

        /// <summary>
        /// Indicates that the request failed due to other errors.
        /// </summary>
        Error = 0xff
    }
}
