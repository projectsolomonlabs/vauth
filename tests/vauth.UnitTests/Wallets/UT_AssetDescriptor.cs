using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract.Native;
using System;

namespace Vauth.UnitTests.Wallets
{
    [TestClass]
    public class UT_AssetDescriptor
    {
        [TestMethod]
        public void TestConstructorWithNonexistAssetId()
        {
            var snapshot = TestBlockchain.GetTestSnapshot();
            Action action = () =>
            {
                var descriptor = new Vauth.Wallets.AssetDescriptor(snapshot, ProtocolSettings.Default, UInt160.Parse("01ff00ff00ff00ff00ff00ff00ff00ff00ff00a4"));
            };
            action.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void Check_VALT()
        {
            var snapshot = TestBlockchain.GetTestSnapshot();
            var descriptor = new Vauth.Wallets.AssetDescriptor(snapshot, ProtocolSettings.Default, NativeContract.VALT.Hash);
            descriptor.AssetId.Should().Be(NativeContract.VALT.Hash);
            descriptor.AssetName.Should().Be(nameof(ValtToken));
            descriptor.ToString().Should().Be(nameof(ValtToken));
            descriptor.Symbol.Should().Be("VALT");
            descriptor.Decimals.Should().Be(8);
        }

        [TestMethod]
        public void Check_Vauth()
        {
            var snapshot = TestBlockchain.GetTestSnapshot();
            var descriptor = new Vauth.Wallets.AssetDescriptor(snapshot, ProtocolSettings.Default, NativeContract.Vauth.Hash);
            descriptor.AssetId.Should().Be(NativeContract.Vauth.Hash);
            descriptor.AssetName.Should().Be(nameof(VauthToken));
            descriptor.ToString().Should().Be(nameof(VauthToken));
            descriptor.Symbol.Should().Be("Vauth");
            descriptor.Decimals.Should().Be(0);
        }
    }
}
