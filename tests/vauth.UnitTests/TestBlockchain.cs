using Vauth.Persistence;
using System;

namespace Vauth.UnitTests
{
    public static class TestBlockchain
    {
        public static readonly VauthSystem TheVauthSystem;
        public static readonly UInt160[] DefaultExtensibleWitnessWhiteList;

        static TestBlockchain()
        {
            Console.WriteLine("initialize VauthSystem");
            TheVauthSystem = new VauthSystem(ProtocolSettings.Default, null, null);
        }

        internal static DataCache GetTestSnapshot()
        {
            return TheVauthSystem.GetSnapshot().CreateSnapshot();
        }
    }
}
