using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract.Native;
using Vauth.Wallets;

namespace Vauth.UnitTests
{
    [TestClass]
    public class UT_ProtocolSettings
    {
        [TestMethod]
        public void CheckFirstLetterOfAddresses()
        {
            UInt160 min = UInt160.Parse("0x0000000000000000000000000000000000000000");
            min.ToAddress(ProtocolSettings.Default.AddressVersion)[0].Should().Be('N');
            UInt160 max = UInt160.Parse("0xffffffffffffffffffffffffffffffffffffffff");
            max.ToAddress(ProtocolSettings.Default.AddressVersion)[0].Should().Be('N');
        }

        [TestMethod]
        public void Default_Network_should_be_mainnet_Network_value()
        {
            var mainNetNetwork = 0x4F454Eu;
            ProtocolSettings.Default.Network.Should().Be(mainNetNetwork);
        }

        [TestMethod]
        public void TestGetMemoryPoolMaxTransactions()
        {
            ProtocolSettings.Default.MemoryPoolMaxTransactions.Should().Be(50000);
        }

        [TestMethod]
        public void TestGetMillisecondsPerBlock()
        {
            ProtocolSettings.Default.MillisecondsPerBlock.Should().Be(15000);
        }

        [TestMethod]
        public void TestGetSeedList()
        {
            ProtocolSettings.Default.SeedList.Should().BeEquivalentTo(new string[] { "seed1.projectsolomon.com/vauth:10333", "seed2.projectsolomon.com/vauth:10333", "seed3.projectsolomon.com/vauth:10333", "seed4.projectsolomon.com/vauth:10333", "seed5.projectsolomon.com/vauth:10333", });
        }

        [TestMethod]
        public void TestNativeUpdateHistory()
        {
            ProtocolSettings.Default.NativeUpdateHistory.Count.Should().Be(NativeContract.Contracts.Count);
        }
    }
}
