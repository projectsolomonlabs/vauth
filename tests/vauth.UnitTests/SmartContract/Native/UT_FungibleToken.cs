using Akka.TestKit.Xunit2;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract.Native;

namespace Vauth.UnitTests.SmartContract.Native
{
    [TestClass]
    public class UT_FungibleToken : TestKit
    {
        [TestMethod]
        public void TestTotalSupply()
        {
            var snapshot = TestBlockchain.GetTestSnapshot();
            NativeContract.VALT.TotalSupply(snapshot).Should().Be(3000000050000000);
        }
    }
}
