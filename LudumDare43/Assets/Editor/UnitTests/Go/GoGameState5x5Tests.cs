using NUnit.Framework;
using FineGameDesign.Go;

namespace FineGameDesign.Go.UnitTests
{
    public class GoGameState5x5Tests
    {
        private static void AssertMoveCoordinateToMask(BoardPosition pos, uint expectedMask)
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            uint moveMask = gameState.CoordinateToMask(pos);
            gameState.Move(moveMask);
            Assert.AreEqual(expectedMask, gameState.IllegalMoveMask,
                pos.ToString());
        }

        [Test]
        public void CoordinateToMask_BoardPositionX_EqualsToBitShiftYPlusX()
        {
            uint expectedMask = 1;
            for (int x = 0; x < 5; ++x)
            {
                AssertMoveCoordinateToMask(new BoardPosition(){x = x, y = 0}, expectedMask);
                expectedMask <<= 1;
            }
        }

        [Test]
        public void CoordinateToMask_BoardPositionX2Y1_EqualsToBitShiftYPlusX()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 2, y = 1});
            gameState.Move(moveMask);
            Assert.AreEqual(32 * 4, gameState.IllegalMoveMask);
        }

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
            Assert.AreEqual(1024 * 8, gameState.IllegalMoveMask);
        }

        [Test]
        public void IllegalMoveMask_On3x1AfterCenter_ExcludesSuicide()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.Config.SizeX = 3;
            gameState.Config.SizeY = 1;
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 1, y = 0});
            gameState.Move(moveMask);
            Assert.AreEqual(1 + 2 + 4, gameState.IllegalMoveMask);
        }
    }
}
