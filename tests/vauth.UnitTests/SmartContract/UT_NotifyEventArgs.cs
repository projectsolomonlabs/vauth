using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.Network.P2P.Payloads;
using Vauth.SmartContract;

namespace Vauth.UnitTests.SmartContract
{
    [TestClass]
    public class UT_NotifyEventArgs
    {
        [TestMethod]
        public void TestGetScriptContainer()
        {
            IVerifiable container = new TestVerifiable();
            UInt160 script_hash = new byte[] { 0x00 }.ToScriptHash();
            NotifyEventArgs args = new NotifyEventArgs(container, script_hash, "Test", null);
            args.ScriptContainer.Should().Be(container);
        }
    }
}
