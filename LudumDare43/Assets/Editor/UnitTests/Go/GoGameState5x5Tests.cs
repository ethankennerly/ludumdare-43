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
        public void SetSize3x2_EmptyMask_Equals3x2()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            Assert.AreEqual("111/111", gameState.MaskToBitString(gameState.EmptyMask));
        }

        [Test]
        public void MaskToBitString_3x2AtX0Y2AndX1Y0_AlignedRowMajorFromTop()
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
        public void CreateAdjacencyMaskFromIndex_On3x2AtX2_EqualsLeftAndBelow()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            uint adjacencyMask = gameState.CreateAdjacencyMaskFromIndex(2);
            Assert.AreEqual("010/001", gameState.MaskToBitString(adjacencyMask));
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
        public void ToString_Board3x1MoveAtX1_EqualsBoardDiagram()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            Assert.AreEqual(".x.", gameState.ToString(),
                "After black plays at 1.");
        }

        [Test]
        public void Move_CaptureOn4x1_PermitsCapturedPosition()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(4, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            AssertBoardDiagramAndIllegalMoveMask("x...", "1000", gameState,
                "After black plays at 0, white may play anywhere else.");

            gameState.MoveAtPosition(new BoardPosition(){x = 3});
            AssertBoardDiagramAndIllegalMoveMask("x..o", "1001", gameState,
                "After white plays at 3, black may play anywhere empty.");

            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask("xx.o", "1101", gameState,
                "After black plays at 1, white may capture at 2.");

            gameState.MoveAtPosition(new BoardPosition(){x = 2});
            AssertBoardDiagramAndIllegalMoveMask("..oo", "0011", gameState,
                "After white captures at 2, black may play on the left side.\n" +
                "Were the black stones on the left captured and marked empty in the first player's group?");

            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask(".x..", "1100", gameState,
                "After black captures at 1, white may play on the left side.\n" +
                "Were the captured stones cleared from each group?");
        }

        private static void AssertBoardDiagramAndIllegalMoveMask(
            string boardDiagram,
            string illegalMoveMask,
            GoGameState5x5 gameState,
            string message)
        {
            string details = message + "\n" + gameState.Audit();
            Assert.AreEqual(boardDiagram, gameState.ToString(),
                details);
            Assert.AreEqual(illegalMoveMask, gameState.MaskToBitString(gameState.IllegalMoveMask),
                "Illegal Move Mask differs.\n" + details);
        }

        [Test]
        public void Move_On4x1Adjacent_SharesGroupLiberties()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(4, 1);
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask(".x..", "1100", gameState,
                "After black plays at 1, white may play on right side.");

            gameState.MoveAtPosition(new BoardPosition(){x = 3});
            AssertBoardDiagramAndIllegalMoveMask(".x.o", "0101", gameState,
                "Black player can play adjacent to the previous black stone.");
        }

        [Test]
        public void TODO_Move_On3x2Adjacent_MergesGroups()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            AssertBoardDiagramAndIllegalMoveMask("x..\n...", "100/000", gameState,
                "After black at 0,0.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 2});
            AssertBoardDiagramAndIllegalMoveMask("x.o\n...", "101/000", gameState,
                "After white at 2,0.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("x.o\n.x.", "101/110", gameState,
                "After black at 1,1.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 2});
            AssertBoardDiagramAndIllegalMoveMask("x.o\n.xo", "101/011", gameState,
                "After white at 1,2.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("x.o\nxxo", "101/111", gameState,
                "After black at 0,1.");
        }

        [Test]
        public void TODO_Move_Capture_PreventsRepeatingLastBoardState()
        {
        }

        [Test]
        public void TODO_Move_NoLegalMove_LosesGame()
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
    }
}
