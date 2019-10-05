using Go;
using MonteCarlo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FineGameDesign.Go
{
    public sealed class GoSearcher
    {
        // 2 idiotic moves at edges.
        // 50000 freezes laptop.
        private const int kMaxIterations = 80000;
        private const int kMaxMilliseconds = 4000;
        private const double kMinExploitationValue = 0.125;

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

            Log(gameState.CurrentPlayer.Turn.ToString(), topActions, game.Board);
            MonteCarloTreeSearch.Node<GoPlayer, GoAction> topAction = topActions[0];
            double exploitationValue = topAction.NumWins / topAction.NumRuns;
            if (exploitationValue < kMinExploitationValue)
            {
                return Game.PassMove;
            }

            return topAction.Action.Position;
        }

        [Conditional("DEBUG")]
        private void Log(string prefix, IList<MonteCarloTreeSearch.Node<GoPlayer, GoAction>> actions,
            Board board = null,
            int maxActions = 10)
        {
            var sb = new StringBuilder();
            sb.Append(prefix);
            int numActions = actions.Count();
            if (numActions > maxActions)
            {
                numActions = maxActions;
            }
            int actionIndex = -1;
            foreach (var action in actions)
            {
                actionIndex++;
                if (actionIndex >= numActions)
                    break;

                sb.Append(", ");
                sb.Append(action.Action.Position);
                sb.Append(action.NumWins.ToString("N2"));
                sb.Append("/");
                sb.Append(action.NumRuns);
            }

            if (board != null)
            {
                sb.Append("\n");
                sb.Append(board.ToString());
            }

            string message = sb.ToString();
            UnityEngine.Debug.Log(message);
        }
    }
}
