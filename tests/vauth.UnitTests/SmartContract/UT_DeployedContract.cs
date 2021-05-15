using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.SmartContract;
using System;

namespace Vauth.UnitTests.SmartContract
{
    [TestClass]
    public class UT_DeployedContract
    {
        [TestMethod]
        public void TestGetScriptHash()
        {
            var contract = new DeployedContract(new ContractState()
            {
                Manifest = new Vauth.SmartContract.Manifest.ContractManifest()
                {
                    Abi = new Vauth.SmartContract.Manifest.ContractAbi()
                    {
                        Methods = new Vauth.SmartContract.Manifest.ContractMethodDescriptor[]
                         {
                             new Vauth.SmartContract.Manifest.ContractMethodDescriptor()
                             {
                                  Name = "verify",
                                  Parameters = Array.Empty<Vauth.SmartContract.Manifest.ContractParameterDefinition>()
                             }
                         }
                    }
                },
                Nef = new NefFile { Script = new byte[] { 1, 2, 3 } },
                Hash = new byte[] { 1, 2, 3 }.ToScriptHash()
            });

            Assert.AreEqual("0xb2e3fe334830b4741fa5d762f2ab36b90b86c49b", contract.ScriptHash.ToString());
        }

        [TestMethod]
        public void TestErrors()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DeployedContract(null));
            Assert.ThrowsException<NotSupportedException>(() => new DeployedContract(new ContractState()
            {
                Manifest = new Vauth.SmartContract.Manifest.ContractManifest()
                {
                    Abi = new Vauth.SmartContract.Manifest.ContractAbi()
                    {
                        Methods = new Vauth.SmartContract.Manifest.ContractMethodDescriptor[]
                         {
                             new Vauth.SmartContract.Manifest.ContractMethodDescriptor()
                             {
                                  Name = "noverify",
                                  Parameters = Array.Empty<Vauth.SmartContract.Manifest.ContractParameterDefinition>()
                             }
                         }
                    }
                },
                Nef = new NefFile { Script = new byte[] { 1, 2, 3 } }
            }));
        }
    }
}
