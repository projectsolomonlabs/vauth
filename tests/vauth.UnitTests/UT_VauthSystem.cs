using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vauth.UnitTests
{
    [TestClass]
    public class UT_VauthSystem
    {
        private VauthSystem VauthSystem;

        [TestInitialize]
        public void Setup()
        {
            VauthSystem = TestBlockchain.TheVauthSystem;
        }

        [TestMethod]
        public void TestGetBlockchain() => VauthSystem.Blockchain.Should().NotBeNull();

        [TestMethod]
        public void TestGetLocalNode() => VauthSystem.LocalNode.Should().NotBeNull();

        [TestMethod]
        public void TestGetTaskManager() => VauthSystem.TaskManager.Should().NotBeNull();
    }
}
