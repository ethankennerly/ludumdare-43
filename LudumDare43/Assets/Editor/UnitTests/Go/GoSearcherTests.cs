using Go;
using FineGameDesign.Go;
using NUnit.Framework;

namespace FineGameDesign.UnitTests.Go
{
    public sealed class GoSearcherTests
    {
        [Test]
        public void FirstMoveOn3x3()
        {
            Referee referee = new Referee();
            referee.Game = new Game(new Board(3, 3), Content.Black);
            GoSearcher searcher = new GoSearcher();
            searcher.MakeMove(referee);
            uint x1y1Mask = Board.GetCellMask(1, 1, 3, 3);
            Board board = referee.Game.Board;
            Assert.AreEqual(x1y1Mask, board.GetContentMask(0), board.ToString());
        }
    }
}
