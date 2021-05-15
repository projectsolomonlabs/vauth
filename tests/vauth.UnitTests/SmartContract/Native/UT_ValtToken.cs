using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.IO;
using Vauth.Network.P2P.Payloads;
using Vauth.Persistence;
using Vauth.SmartContract;
using Vauth.SmartContract.Native;
using Vauth.UnitTests.Extensions;
using Vauth.VM;
using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Vauth.UnitTests.SmartContract.Native
{
    [TestClass]
    public class UT_ValtToken
    {
        private DataCache _snapshot;
        private Block _persistingBlock;

        [TestInitialize]
        public void TestSetup()
        {
            _snapshot = TestBlockchain.GetTestSnapshot();
            _persistingBlock = new Block { Header = new Header() };
        }

        [TestMethod]
        public void Check_Name() => NativeContract.VALT.Name.Should().Be(nameof(ValtToken));

        [TestMethod]
        public void Check_Symbol() => NativeContract.VALT.Symbol(_snapshot).Should().Be("VALT");

        [TestMethod]
        public void Check_Decimals() => NativeContract.VALT.Decimals(_snapshot).Should().Be(8);

        [TestMethod]
        public void Refuel()
        {
            // Prepare

            var wallet = TestUtils.GenerateTestWallet();
            var snapshot = TestBlockchain.GetTestSnapshot();

            using var unlock = wallet.Unlock("");
            var accBalance = wallet.CreateAccount();
            var accNoBalance = wallet.CreateAccount();

            // Fake balance

            var key = NativeContract.VALT.CreateStorageKey(20, accNoBalance.ScriptHash);
            var entry = snapshot.GetAndChange(key, () => new StorageItem(new AccountState()));
            entry.GetInteroperable<AccountState>().Balance = 1 * NativeContract.VALT.Factor;

            key = NativeContract.VALT.CreateStorageKey(20, accBalance.ScriptHash);
            entry = snapshot.GetAndChange(key, () => new StorageItem(new AccountState()));
            entry.GetInteroperable<AccountState>().Balance = 100 * NativeContract.VALT.Factor;

            snapshot.Commit();

            // Make transaction

            byte[] script;
            using (ScriptBuilder sb = new())
            {
                sb.EmitDynamicCall(NativeContract.VALT.Hash, "refuel", accBalance.ScriptHash, 100 * NativeContract.VALT.Factor);
                sb.Emit(OpCode.DROP);
                sb.EmitSysCall(ApplicationEngine.System_Runtime_ValtLeft);
                script = sb.ToArray();
            }

            var signers = new Signer[]{ new Signer
                {
                    Account = accBalance.ScriptHash,
                    Scopes =  WitnessScope.CalledByEntry
                } ,
                new Signer
                {
                    Account = accNoBalance.ScriptHash,
                    Scopes =  WitnessScope.CalledByEntry
                } };

            var tx = wallet.MakeTransaction(snapshot, script, accBalance.ScriptHash, signers);
            Assert.IsNotNull(tx);

            // Check

            using ApplicationEngine engine = ApplicationEngine.Create(TriggerType.Application, tx, snapshot, settings: TestBlockchain.TheVauthSystem.Settings, valt: tx.NetworkFee);
            engine.LoadScript(tx.Script);
            Assert.AreEqual(VMState.HALT, engine.Execute());
            Assert.AreEqual(1, engine.ResultStack.Count);
            Assert.AreEqual(100_00300140, engine.ResultStack.Pop().GetInteger());

            entry = snapshot.GetAndChange(key, () => new StorageItem(new AccountState()));
            Assert.AreEqual(0, entry.GetInteroperable<AccountState>().Balance);
        }

        [TestMethod]
        public async Task Check_BalanceOfTransferAndBurn()
        {
            var snapshot = _snapshot.CreateSnapshot();
            var persistingBlock = new Block { Header = new Header { Index = 1000 } };
            byte[] from = Contract.GetBFTAddress(ProtocolSettings.Default.StandbyValidators).ToArray();
            byte[] to = new byte[20];
            var keyCount = snapshot.GetChangeSet().Count();
            var supply = NativeContract.VALT.TotalSupply(snapshot);
            supply.Should().Be(3000000050000000); // 3000000000000000 + 50000000 (Vauth holder reward)

            // Check unclaim

            var unclaim = UT_VauthToken.Check_UnclaimedValt(snapshot, from, persistingBlock);
            unclaim.Value.Should().Be(new BigInteger(0.5 * 1000 * 100000000L));
            unclaim.State.Should().BeTrue();

            // Transfer

            NativeContract.Vauth.Transfer(snapshot, from, to, BigInteger.Zero, true, persistingBlock).Should().BeTrue();
            NativeContract.Vauth.BalanceOf(snapshot, from).Should().Be(100000000);
            NativeContract.Vauth.BalanceOf(snapshot, to).Should().Be(0);

            NativeContract.VALT.BalanceOf(snapshot, from).Should().Be(30000500_00000000);
            NativeContract.VALT.BalanceOf(snapshot, to).Should().Be(0);

            // Check unclaim

            unclaim = UT_VauthToken.Check_UnclaimedValt(snapshot, from, persistingBlock);
            unclaim.Value.Should().Be(new BigInteger(0));
            unclaim.State.Should().BeTrue();

            supply = NativeContract.VALT.TotalSupply(snapshot);
            supply.Should().Be(3000050050000000);

            snapshot.GetChangeSet().Count().Should().Be(keyCount + 3); // valt

            // Transfer

            keyCount = snapshot.GetChangeSet().Count();

            NativeContract.VALT.Transfer(snapshot, from, to, 30000500_00000000, false, persistingBlock).Should().BeFalse(); // Not signed
            NativeContract.VALT.Transfer(snapshot, from, to, 30000500_00000001, true, persistingBlock).Should().BeFalse(); // More than balance
            NativeContract.VALT.Transfer(snapshot, from, to, 30000500_00000000, true, persistingBlock).Should().BeTrue(); // All balance

            // Balance of

            NativeContract.VALT.BalanceOf(snapshot, to).Should().Be(30000500_00000000);
            NativeContract.VALT.BalanceOf(snapshot, from).Should().Be(0);

            snapshot.GetChangeSet().Count().Should().Be(keyCount + 1); // All

            // Burn

            using var engine = ApplicationEngine.Create(TriggerType.Application, null, snapshot, persistingBlock, settings: TestBlockchain.TheVauthSystem.Settings, valt: 0);
            keyCount = snapshot.GetChangeSet().Count();

            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
                await NativeContract.VALT.Burn(engine, new UInt160(to), BigInteger.MinusOne));

            // Burn more than expected

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () =>
                await NativeContract.VALT.Burn(engine, new UInt160(to), new BigInteger(30000500_00000001)));

            // Real burn

            await NativeContract.VALT.Burn(engine, new UInt160(to), new BigInteger(1));

            NativeContract.VALT.BalanceOf(snapshot, to).Should().Be(3000049999999999);

            keyCount.Should().Be(snapshot.GetChangeSet().Count());

            // Burn all

            await NativeContract.VALT.Burn(engine, new UInt160(to), new BigInteger(3000049999999999));

            (keyCount - 1).Should().Be(snapshot.GetChangeSet().Count());

            // Bad inputs

            NativeContract.VALT.Transfer(snapshot, from, to, BigInteger.MinusOne, true, persistingBlock).Should().BeFalse();
            NativeContract.VALT.Transfer(snapshot, new byte[19], to, BigInteger.One, false, persistingBlock).Should().BeFalse();
            NativeContract.VALT.Transfer(snapshot, from, new byte[19], BigInteger.One, false, persistingBlock).Should().BeFalse();
        }

        internal static StorageKey CreateStorageKey(byte prefix, uint key)
        {
            return CreateStorageKey(prefix, BitConverter.GetBytes(key));
        }

        internal static StorageKey CreateStorageKey(byte prefix, byte[] key = null)
        {
            StorageKey storageKey = new()
            {
                Id = NativeContract.Vauth.Id,
                Key = new byte[sizeof(byte) + (key?.Length ?? 0)]
            };
            storageKey.Key[0] = prefix;
            key?.CopyTo(storageKey.Key.AsSpan(1));
            return storageKey;
        }
    }
}
