using Vauth.Network.P2P.Payloads;
using Vauth.Persistence;
using System;
using System.Collections.Generic;
using static Vauth.Ledger.Blockchain;

namespace Vauth.Plugins
{
    /// <summary>
    /// An interface that allows plugins to observe the persisted blocks.
    /// </summary>
    public interface IPersistencePlugin
    {
        /// <summary>
        /// Called when a block is being persisted.
        /// </summary>
        /// <param name="system">The <see cref="VauthSystem"/> object that contains the blockchain.</param>
        /// <param name="block">The block being persisted.</param>
        /// <param name="snapshot">The snapshot used for persistence.</param>
        /// <param name="applicationExecutedList">The execution result of the contracts in the block.</param>
        void OnPersist(VauthSystem system, Block block, DataCache snapshot, IReadOnlyList<ApplicationExecuted> applicationExecutedList) { }

        /// <summary>
        /// Called when a block has been persisted.
        /// </summary>
        /// <param name="system">The <see cref="VauthSystem"/> object that contains the blockchain.</param>
        /// <param name="block">The block being persisted.</param>
        /// <param name="snapshot">The snapshot used for persistence.</param>
        void OnCommit(VauthSystem system, Block block, DataCache snapshot) { }

        /// <summary>
        /// Indicates whether to allow exceptions to be thrown from the plugin when committing.
        /// </summary>
        /// <param name="ex">The exception to be thrown.</param>
        /// <returns><see langword="true"/> if the exception should be thrown; otherwise, <see langword="false"/>.</returns>
        bool ShouldThrowExceptionFromCommit(Exception ex) => false;
    }
}
