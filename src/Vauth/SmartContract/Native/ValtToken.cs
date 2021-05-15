#pragma warning disable IDE0051

using Vauth.Cryptography.ECC;
using Vauth.Network.P2P.Payloads;
using System;

namespace Vauth.SmartContract.Native
{
    /// <summary>
    /// Represents the VALT token in the Vauth system.
    /// </summary>
    public sealed class ValtToken : FungibleToken<AccountState>
    {
        public override string Symbol => "VALT";
        public override byte Decimals => 8;

        internal ValtToken()
        {
        }

        internal override ContractTask Initialize(ApplicationEngine engine)
        {
            UInt160 account = Contract.GetBFTAddress(engine.ProtocolSettings.StandbyValidators);
            return Mint(engine, account, 30_000_000 * Factor, false);
        }

        internal override async ContractTask OnPersist(ApplicationEngine engine)
        {
            long totalNetworkFee = 0;
            foreach (Transaction tx in engine.PersistingBlock.Transactions)
            {
                await Burn(engine, tx.Sender, tx.SystemFee + tx.NetworkFee);
                totalNetworkFee += tx.NetworkFee;
            }
            ECPoint[] validators = Vauth.GetNextBlockValidators(engine.Snapshot, engine.ProtocolSettings.ValidatorsCount);
            UInt160 primary = Contract.CreateSignatureRedeemScript(validators[engine.PersistingBlock.PrimaryIndex]).ToScriptHash();
            await Mint(engine, primary, totalNetworkFee, false);
        }

        [ContractMethod(CpuFee = 1 << 15, RequiredCallFlags = CallFlags.States | CallFlags.AllowNotify)]
        private async ContractTask Refuel(ApplicationEngine engine, UInt160 account, long amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (!engine.CheckWitnessInternal(account)) throw new InvalidOperationException();
            await Burn(engine, account, amount);
            engine.Refuel(amount);
        }
    }
}
