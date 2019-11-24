using NUnit.Framework;
using FineGameDesign.Go;

namespace FineGameDesign.Go.UnitTests
{
    public class GoGameState5x5Tests
    {
        [Test]
        public void IllegalMoveMask_Default_None()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            Assert.AreEqual(0, gameState.IllegalMoveMask);
        }
    }
}
