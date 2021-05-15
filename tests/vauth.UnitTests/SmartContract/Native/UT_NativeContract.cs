using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract.Native;

namespace Vauth.UnitTests.SmartContract.Native
{
    [TestClass]
    public class UT_NativeContract
    {
        [TestMethod]
        public void TestGetContract()
        {
            Assert.IsTrue(NativeContract.Vauth == NativeContract.GetContract(NativeContract.Vauth.Hash));
        }
    }
}
