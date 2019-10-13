using Go;
using FineGameDesign.Go;
using NUnit.Framework;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

namespace FineGameDesign.UnitTests.Go
{
    public sealed class GoSearcherTests
    {
        private static void AssertTinyAndOdd(int size, string dimension)
        {
            Assert.IsTrue(size == 1 || size == 3 || size == 5,
                "Only small odd size is simple to calculate optimal move in center. " +
                "size" + dimension + "=" + size
            );
        }

        public void MakeMove(int sizeX, int sizeY)
        {
            AssertTinyAndOdd(sizeX, "X");
            AssertTinyAndOdd(sizeY, "Y");

            Stopwatch timePerMove = new Stopwatch();

            Referee referee = new Referee();
            referee.Game = new Game();
            referee.Game.Clone(new Board(sizeX, sizeY), Content.Black);
            GoSearcher searcher = new GoSearcher();

            timePerMove.Start();
            searcher.MakeMove(referee);
            timePerMove.Stop();
            long millisecondsPerMove = timePerMove.ElapsedMilliseconds;

            uint x1y1Mask = Board.GetCellMask(sizeX / 2, sizeY / 2, sizeX, sizeY);
            Board board = referee.Game.Board;
            Assert.AreEqual(x1y1Mask, board.GetContentMask(0), board.ToString());

            Debug.Log("MakeMoveOn" + sizeX + "x" + sizeY + ": " + millisecondsPerMove + "ms");
        }

        [Test]
        public void MakeMoveOn1x3()
        {
            MakeMove(1, 3);
        }

        [Test]
        public void MakeMoveOn3x3()
        {
            MakeMove(3, 3);
        }

        [Test]
        public void MakeMoveOn3x5()
        {
            MakeMove(3, 5);
        }

        [Test]
        public void MakeMoveOn5x5()
        {
            MakeMove(5, 5);
        }
    }
}
