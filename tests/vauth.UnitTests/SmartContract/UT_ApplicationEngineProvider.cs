using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.Network.P2P.Payloads;
using Vauth.Persistence;
using Vauth.Plugins;
using Vauth.SmartContract;

namespace Vauth.UnitTests.SmartContract
{
    [TestClass]
    public class UT_ApplicationEngineProvider
    {
        [TestInitialize]
        public void TestInitialize()
        {
            ApplicationEngine.ResetApplicationEngineProvider();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            ApplicationEngine.ResetApplicationEngineProvider();
        }

        [TestMethod]
        public void TestSetAppEngineProvider()
        {
            var provider = new TestProvider();
            ApplicationEngine.SetApplicationEngineProvider(provider).Should().BeTrue();

            using var appEngine = ApplicationEngine.Create(TriggerType.Application, null, null, valt: 0, settings: TestBlockchain.TheVauthSystem.Settings);
            (appEngine is TestEngine).Should().BeTrue();
        }

        [TestMethod]
        public void TestDefaultAppEngineProvider()
        {
            using var appEngine = ApplicationEngine.Create(TriggerType.Application, null, null, valt: 0, settings: TestBlockchain.TheVauthSystem.Settings);
            (appEngine is ApplicationEngine).Should().BeTrue();
        }

        [TestMethod]
        public void TestCantSetAppEngineProviderTwice()
        {
            var provider = new TestProvider();
            ApplicationEngine.SetApplicationEngineProvider(provider).Should().BeTrue();

            var provider2 = new TestProvider();
            ApplicationEngine.SetApplicationEngineProvider(provider2).Should().BeFalse();
        }

        [TestMethod]
        public void TestCanResetAppEngineProviderTwice()
        {
            var provider = new TestProvider();
            ApplicationEngine.SetApplicationEngineProvider(provider).Should().BeTrue();

            ApplicationEngine.ResetApplicationEngineProvider();

            var provider2 = new TestProvider();
            ApplicationEngine.SetApplicationEngineProvider(provider2).Should().BeTrue();
        }

        class TestProvider : IApplicationEngineProvider
        {
            public ApplicationEngine Create(TriggerType trigger, IVerifiable container, DataCache snapshot, Block persistingBlock, ProtocolSettings settings, long valt)
            {
                return new TestEngine(trigger, container, snapshot, persistingBlock, settings, valt);
            }
        }

        class TestEngine : ApplicationEngine
        {
            public TestEngine(TriggerType trigger, IVerifiable container, DataCache snapshot, Block persistingBlock, ProtocolSettings settings, long valt)
                : base(trigger, container, snapshot, persistingBlock, settings, valt)
            {
            }
        }
    }
}
