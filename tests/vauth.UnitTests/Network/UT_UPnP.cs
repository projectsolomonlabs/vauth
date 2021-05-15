using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.Network;
using System;

namespace Vauth.UnitTests.Network
{
    [TestClass]
    public class UT_UPnP
    {
        [TestMethod]
        public void GetTimeOut()
        {
            Assert.AreEqual(3, UPnP.TimeOut.TotalSeconds);
        }

        [TestMethod]
        public void NoService()
        {
            Assert.ThrowsException<Exception>(() => UPnP.ForwardPort(1, System.Net.Sockets.ProtocolType.Tcp, ""));
            Assert.ThrowsException<Exception>(() => UPnP.DeleteForwardingRule(1, System.Net.Sockets.ProtocolType.Tcp));
            Assert.ThrowsException<Exception>(() => UPnP.GetExternalIP());
        }
    }
}
