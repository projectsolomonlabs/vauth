using System.Collections.Generic;

namespace Vauth.Persistence
{
    /// <summary>
    /// This interface provides methods to read from the database.
    /// </summary>
    public interface IReadOnlyStore
    {
        /// <summary>
        /// Seeks to the entry with the specified key.
        /// </summary>
        /// <param name="key">The key to be sought.</param>
        /// <param name="direction">The direction of seek.</param>
        /// <returns>An enumerator containing all the entries after seeking.</returns>
        IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] key, SeekDirection direction);

        /// <summary>
        /// Reads a specified entry from the database.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns>The data of the entry. Or <see langword="null"/> if it doesn't exist.</returns>
        byte[] TryGet(byte[] key);

        /// <summary>
        /// Determines whether the database contains the specified entry.
        /// </summary>
        /// <param name="key">The key of the entry.</param>
        /// <returns><see langword="true"/> if the database contains an entry with the specified key; otherwise, <see langword="false"/>.</returns>
        bool Contains(byte[] key);
    }
}
