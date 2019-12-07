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
        public void MaskToBitString_3x2AtR0C2AndR1C0_AlignedRowMajorFromTop()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            Assert.AreEqual("001/100", gameState.MaskToBitString((uint)(4 + 8)));
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
        public void MaskToIndex_From8_Equals3()
        {
            Assert.AreEqual(3, GoGameState5x5.MaskToIndex(8));
        }

        [Test]
        public void IllegalMoveMask_Default_None()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            Assert.AreEqual(0, gameState.IllegalMoveMask);
        }

        [Test]
        public void Move_AfterMove3x2_IllegalMoveMaskEqualsBitShiftYPlusX()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 3, y = 2});
            gameState.Move(moveMask);
            Assert.AreEqual(1024 * 8, gameState.IllegalMoveMask);
        }

        [Test]
        public void Move_On3x1AfterCenter_ExcludesSuicide()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 1, y = 0});
            gameState.Move(moveMask);
            Assert.AreEqual(1 + 2 + 4, gameState.IllegalMoveMask);
        }

        [Test]
        public void Move_LastLibertyOn3x1ThatWouldCapture_IsPermitted()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 0});
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 0});
            Assert.AreEqual(1 + 0 + 4, gameState.IllegalMoveMask);
        }

        [Test]
        public void TODO_Move_CaptureOn4x1_PermitsCapturedPosition()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(4, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            Assert.AreEqual(1 + 0 + 0 + 0, gameState.IllegalMoveMask,
                "After black plays at 0, white may play anywhere else." +
                gameState.Audit());
            gameState.MoveAtPosition(new BoardPosition(){x = 3});
            Assert.AreEqual(1 + 0 + 0 + 8, gameState.IllegalMoveMask,
                "After white plays at 3, black may play anywhere empty." +
                gameState.Audit());
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            Assert.AreEqual(1 + 2 + 0 + 8, gameState.IllegalMoveMask,
                "After black plays at 1, white may capture at 2." +
                gameState.Audit());
            gameState.MoveAtPosition(new BoardPosition(){x = 2});
            Assert.AreEqual(0 + 0 + 4 + 8, gameState.IllegalMoveMask,
                "After white captures at 2, black may play on the left side." +
                gameState.Audit());
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            Assert.AreEqual(1 + 2 + 0 + 0, gameState.IllegalMoveMask,
                "After black captures back at 1, white may play on the right side. " +
                gameState.Audit());
        }

        [Test]
        public void TODO_Move_Capture_PreventsRepeatingLastBoardState()
        {
        }

        [Test]
        public void Move_On4x1Adjacent_SharesGroupLiberties()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(4, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 0});
            Assert.AreEqual(1 + 2 + 0 + 0, gameState.IllegalMoveMask,
                "White player cannot play on top or at suicide point.");

            gameState.MoveAtPosition(new BoardPosition(){x = 3, y = 0});
            Assert.AreEqual(0 + 2 + 0 + 8, gameState.IllegalMoveMask,
                "Black player can play adjacent to the previous black stone.");
        }

        [Test]
        public void TODO_Move_On3x2Adjacent_MergesGroups()
        {
        }

        [Test]
        public void CreateLibertyMaskFromIndex_FirstCornerOn3x1_MiddleCell()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            uint libertyMask = gameState.CreateLibertyMaskFromIndex(0);
            Assert.AreEqual(2, libertyMask);
        }

        [Test]
        public void CreateLibertyMaskFromIndex_On3x1AfterCenter_NoLiberties()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 1, y = 0});
            gameState.Move(moveMask);
            uint libertyMask = gameState.CreateLibertyMaskFromIndex(0);
            Assert.AreEqual(0, libertyMask);
        }

        [Test]
        public void RemoveLiberties_FirstMoveOn3x1_CreatesLibertyMask()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            uint moveMask = gameState.CoordinateToMask(new BoardPosition());
            Assert.AreEqual(0, gameState.GetNumGroups(),
                "Num groups before removing liberties");
            gameState.RemoveLiberties(moveMask);
            Assert.AreEqual(1, gameState.GetNumGroups(),
                "Num groups after removing liberties");

            uint libertyMask = gameState.GetGroupLibertyMask(0);
            Assert.AreEqual(2, libertyMask,
                "First group liberty mask after move at 0,0.");
        }

        [Test]
        public void TODO_RemoveLiberties_AdjacentMove_ReducesLibertyMask()
        {
        }

        [Test]
        public void TODO_RemoveLiberties_FormsEyes_MasksIllegalForOpponent()
        {
        }

        [Test]
        public void TODO_RemoveLiberties_Bridge_JoinsGroups()
        {
        }

        [Test]
        public void TODO_Move_AfterCapture_NumberOfGroupsReduced()
        {
        }

        [Test]
        public void TODO_Move_NoLegalMoveForOpponent_LosesGame()
        {
        }
    }
}
