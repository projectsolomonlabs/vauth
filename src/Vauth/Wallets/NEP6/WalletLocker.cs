using System;

namespace Vauth.Wallets.NEP6
{
    internal class WalletLocker : IDisposable
    {
        private readonly NEP6Wallet wallet;

        public WalletLocker(NEP6Wallet wallet)
        {
            this.wallet = wallet;
        }

        public void Dispose()
        {
            wallet.Lock();
        }
    }
}
