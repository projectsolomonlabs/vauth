using Vauth.Network.P2P.Payloads;
using Vauth.Persistence;
using Vauth.SmartContract;

namespace Vauth.Plugins
{
    /// <summary>
    /// A provider for creating <see cref="ApplicationEngine"/> instances.
    /// </summary>
    public interface IApplicationEngineProvider
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ApplicationEngine"/> class or its subclass. This method will be called by <see cref="ApplicationEngine.Create"/>.
        /// </summary>
        /// <param name="trigger">The trigger of the execution.</param>
        /// <param name="container">The container of the script.</param>
        /// <param name="snapshot">The snapshot used by the engine during execution.</param>
        /// <param name="persistingBlock">The block being persisted. It should be <see langword="null"/> if the <paramref name="trigger"/> is <see cref="TriggerType.Verification"/>.</param>
        /// <param name="settings">The <see cref="ProtocolSettings"/> used by the engine.</param>
        /// <param name="valt">The maximum valt used in this execution. The execution will fail when the valt is exhausted.</param>
        /// <returns>The engine instance created.</returns>
        ApplicationEngine Create(TriggerType trigger, IVerifiable container, DataCache snapshot, Block persistingBlock, ProtocolSettings settings, long valt);
    }
}
