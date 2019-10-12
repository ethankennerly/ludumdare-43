using Go;
using FineGameDesign.Go;
using NUnit.Framework;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace FineGameDesign.UnitTests.Go
{
    public sealed class GoSearcherTests
    {
        [Test]
        public void FirstMoveOn3x3()
        {
            Stopwatch timePerMove = new Stopwatch();

            Referee referee = new Referee();
            referee.Game = new Game(new Board(3, 3), Content.Black);
            GoSearcher searcher = new GoSearcher();

            timePerMove.Start();
            searcher.MakeMove(referee);
            timePerMove.Stop();
            long millisecondsPerMove = timePerMove.ElapsedMilliseconds;

            uint x1y1Mask = Board.GetCellMask(1, 1, 3, 3);
            Board board = referee.Game.Board;
            Assert.AreEqual(x1y1Mask, board.GetContentMask(0), board.ToString());

            Debug.Log("FirstMoveOn3x3: " + millisecondsPerMove + "ms");
        }
    }
}
