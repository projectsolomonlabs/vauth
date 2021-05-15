using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vauth.IO;
using Vauth.Network.P2P.Payloads;
using System.Collections;

namespace Vauth.UnitTests.Network.P2P.Payloads
{
    [TestClass]
    public class UT_MerkleBlockPayload
    {
        [TestMethod]
        public void Size_Get()
        {
            var test = MerkleBlockPayload.Create(TestBlockchain.TheVauthSystem.GenesisBlock, new BitArray(1024, false));
            test.Size.Should().Be(239);

            test = MerkleBlockPayload.Create(TestBlockchain.TheVauthSystem.GenesisBlock, new BitArray(0, false));
            test.Size.Should().Be(111);
        }

        [TestMethod]
        public void DeserializeAndSerialize()
        {
            var test = MerkleBlockPayload.Create(TestBlockchain.TheVauthSystem.GenesisBlock, new BitArray(2, false));
            var clone = test.ToArray().AsSerializable<MerkleBlockPayload>();

            Assert.AreEqual(test.TxCount, clone.TxCount);
            Assert.AreEqual(test.Hashes.Length, clone.TxCount);
            CollectionAssert.AreEqual(test.Hashes, clone.Hashes);
            CollectionAssert.AreEqual(test.Flags, clone.Flags);
        }
    }
}
