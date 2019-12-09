using FineGameDesign.Go.AI;
using NUnit.Framework;
using System.Collections.Generic;

namespace FineGameDesign.Go.UnitTests
{
    public sealed class GoAction5x5Tests
    {
        [Test]
        public void ExtractOnBitMasks_TwoOfThree_TwoBits()
        {
            List<uint> firstAndThirdBits = new List<uint>();
            GoAction5x5.ExtractOnBitMasks((uint)(1 + 0 + 4), firstAndThirdBits);
            Assert.AreEqual(new List<uint>(){1, 4}, firstAndThirdBits);
        }
    }
}
