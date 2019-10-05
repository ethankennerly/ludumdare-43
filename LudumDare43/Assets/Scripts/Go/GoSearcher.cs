using Go;
using MonteCarlo;
using System;
using System.Linq;

namespace FineGameDesign.Go
{
    public sealed class GoSearcher
    {
        // 2 idiotic moves at edges.
        // 50000 freezes laptop.
        private const int kMaxIterations = 20000;
        private const int kMaxMilliseconds = 4000;

        public void MakeMove(Referee referee)
        {
            Point move = GetMove(referee.Game);
            referee.MakeMove(move.x, move.y);
        }

        private Point GetMove(Game game)
        {
            var gameState = new GoState(game);
            if (!gameState.Actions.Any())
                return Game.PassMove;

            var topActions = MonteCarloTreeSearch.GetTopActions(gameState, kMaxIterations, kMaxMilliseconds).ToList();
            if (topActions.Count == 0)
                return Game.PassMove;

            return topActions[0].Action.Position;
        }
    }
}
