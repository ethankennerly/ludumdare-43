using FineGameDesign.Go.AI;
using NUnit.Framework;
using UnityEngine;

namespace FineGameDesign.Go.AI.UnitTests
{
    public class GoGameState5x5Tests
    {
        private static void AssertMoveCoordinateToMask(BoardPosition pos, uint expectedMask)
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(5, 5);
            uint moveMask = gameState.CoordinateToMask(pos);
            gameState.Move(moveMask);
            Assert.AreEqual(expectedMask, gameState.IllegalMoveMask,
                pos.ToString());
        }

        [Test]
        public void SetSize3x1_EmptyMask_Equals3x1()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            Assert.AreEqual("111", gameState.MaskToBitString(gameState.EmptyMask));
            Assert.AreEqual(7, gameState.EmptyMask);
        }

        [Test]
        public void SetSize3x2_EmptyMask_Equals3x2()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            Assert.AreEqual("111/111", gameState.MaskToBitString(gameState.EmptyMask));
            Assert.AreEqual(63, gameState.EmptyMask);
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
            gameState.SetSize(3, 2);
            uint moveMask = gameState.CoordinateToMask(new BoardPosition(){x = 2, y = 1});
            gameState.Move(moveMask);
            Assert.AreEqual("000/001", gameState.MaskToBitString(gameState.IllegalMoveMask));
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
            gameState.SetSize(4, 3);
            gameState.MoveAtPosition(new BoardPosition(){x = 3, y = 2});
            Assert.AreEqual("0000/0000/0001", gameState.MaskToBitString(gameState.IllegalMoveMask));
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
        public void Move_On3x2Adjacent_MergesGroups()
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
            
            Assert.AreEqual(2, gameState.GetNumGroupsForPlayer(0),
                "On second stone, expected 2 groups for player 0.\n" + 
                gameState.Audit()
            );
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("x.o\n.xo", "101/011", gameState,
                "After white at 1,2.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("x.o\nxxo", "101/111", gameState,
                "After black at 0,1.");

            Assert.AreEqual(1, gameState.GetNumGroupsForPlayer(0),
                "On bridging stone, expected 1 group for player 0.\n" + 
                gameState.Audit()
            );
        }

        [Test]
        public void Move_On3x2Capture_MergesGroups()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 2);
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 2});
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 1});
            Assert.AreEqual(1, gameState.GetNumGroupsForPlayer(1),
                "On capturing, expected 1 group for player 1.\n" + 
                gameState.Audit()
            );
        }

        [Test]
        public void Move_On3x3Capture_MergesGroups()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 3);
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 2});
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 2});
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 2});
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 0});
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 1});
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 0});
            Assert.AreEqual(1, gameState.GetNumGroupsForPlayer(1),
                "Before black passes, expected 1 group for player 1.\n" + 
                gameState.Audit()
            );
            gameState.Pass();
            Assert.AreEqual(1, gameState.GetNumGroupsForPlayer(1),
                "After black passes, expected 1 group for player 1.\n" + 
                gameState.Audit()
            );
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 0});
            Assert.AreEqual(1, gameState.GetNumGroupsForPlayer(1),
                "On capturing, expected 1 group for player 1.\n" + 
                gameState.Audit()
            );
        }

        /// <summary>
        /// xo.
        /// .xo
        /// x..
        /// </summary>
        [Test]
        public void Move_CaptureOn3x2_PreventsRepeatingLastBoardStateUntilPlayElsewhere()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 3);
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            AssertBoardDiagramAndIllegalMoveMask("x..\n...\n...", "100/000/000", gameState,
                "After black at 0,0.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask("xo.\n...\n...", "110/000/000", gameState,
                "After white at 0,1.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("xo.\n.x.\n...", "110/010/000", gameState,
                "After black at 1,1.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 1});
            AssertBoardDiagramAndIllegalMoveMask("xo.\n.xo\n...", "110/011/000", gameState,
                "After white at 2,1.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 2, y = 0});
            AssertBoardDiagramAndIllegalMoveMask("x.x\n.xo\n...", "111/011/000", gameState,
                "After black captures at 2,0, white cannot capture back,\n" +
                "because that would repeat the previous board.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 1, y = 2});
            AssertBoardDiagramAndIllegalMoveMask("x.x\n.xo\n.o.", "101/011/010", gameState,
                "After white at 1,2.");
            
            gameState.MoveAtPosition(new BoardPosition(){x = 0, y = 2});
            AssertBoardDiagramAndIllegalMoveMask("x.x\n.xo\nxo.", "101/011/111", gameState,
                "After black plays elsewhere at 0,2 white could recapture.");
        }

        /// <summary>
        /// Suppose this sequence of play:
        /// .x.
        /// Score. White has no place to play, but black does have a place to play.
        /// </summary>
        [Test]
        public void Move_NoLegalMove_LosesGame()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            Assert.AreEqual(0.5f, gameState.CalculateWinner());
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask(".x.", "111", gameState,
                "Black forms two eyes.");
            Assert.AreEqual(0f, gameState.CalculateWinner(),
                gameState.Audit());
        }

        [Test]
        public void Move_NoLegalMove_PointsWinGame()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            Assert.AreEqual(0.5f, gameState.CalculateWinner());
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            AssertBoardDiagramAndIllegalMoveMask(".x.", "111", gameState,
                "Black forms two eyes.");
            
            gameState.PointsForPlayer1 = 2;
            Assert.AreEqual(1f, gameState.CalculateWinner(),
                gameState.Audit());
            gameState.PointsForPlayer1 = 1;
            Assert.AreEqual(0f, gameState.CalculateWinner(),
                gameState.Audit());
        }

        [Test]
        public void Move_WhiteCapturesTwoStones_EarnsTwoPointsForPlayer1()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(4, 1);
            Assert.AreEqual(0.5f, gameState.CalculateWinner());
            gameState.MoveAtPosition(new BoardPosition(){x = 0});
            gameState.MoveAtPosition(new BoardPosition(){x = 3});
            gameState.MoveAtPosition(new BoardPosition(){x = 1});
            Assert.AreEqual(0, gameState.PointsForPlayer1);
            gameState.MoveAtPosition(new BoardPosition(){x = 2});
            Assert.AreEqual(2, gameState.PointsForPlayer1, gameState.Audit());
        }

        /// <summary>
        /// Suppose this sequence of play:
        /// x.
        /// ..
        /// 
        /// x.
        /// o.
        /// 
        /// xx
        /// o.
        /// 
        /// ..
        /// oo
        /// 
        /// x.
        /// oo
        /// 
        /// .o
        /// oo
        /// 
        /// x.
        /// ..
        /// The last board repeats first board above.
        /// So that move is illegal.
        /// White wins due to 3 captured stones of black plus black had to pass first.
        /// A reflection or rotation does not count as a repetition of the board.
        /// </summary>
        [Test]
        public void TODO_Move_CaptureOn2x2_PreventsRepeatingLastBoardStateUntilPlayElsewhere()
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
        public void Clone_Audits_AreEqual()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            gameState.MoveAtPosition(new BoardPosition());
            GoGameState5x5 cloneState = gameState.Clone();
            Assert.AreEqual(gameState.Audit(), cloneState.Audit());
        }

        [Test]
        public void Clone_MoveOnClone_AuditUnchangedOnOriginal()
        {
            GoGameState5x5 gameState = new GoGameState5x5();
            gameState.SetSize(3, 1);
            gameState.MoveAtPosition(new BoardPosition());
            GoGameState5x5 cloneState = gameState.Clone();
            string originalAudit = gameState.Audit();
            cloneState.MoveAtPosition(new BoardPosition(){x = 2});
            Assert.AreEqual(originalAudit, gameState.Audit());
        }
    }
}
