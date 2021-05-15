using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract;
using Vauth.VM;
using System;

namespace Vauth.UnitTests.SmartContract
{
    [TestClass]
    public class UT_OpCodePrices
    {
        [TestMethod]
        public void AllOpcodePriceAreSet()
        {
            foreach (OpCode opcode in Enum.GetValues(typeof(OpCode)))
                Assert.IsTrue(ApplicationEngine.OpCodePrices.ContainsKey(opcode), opcode.ToString());
        }
    }
}
