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

        [Test]
        public void IllegalMoveMask_AfterMove_EqualsBitShiftYPlusX()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 3, y = 2});
            gameState.Move(moveMask);
            Assert.AreEqual(1024 + 3, gameState.IllegalMoveMask);
        }

        [Test]
        public void CoordinateToMask_BoardPosition3x2_EqualsToBitShiftYPlusX()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 2, y = 1});
            gameState.Move(moveMask);
            Assert.AreEqual(32 + 2, gameState.IllegalMoveMask);
        }
    }
}
