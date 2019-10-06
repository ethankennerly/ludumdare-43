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
        // 2 at 3x3 idiotic moves at edges.
        // 10 at 5x5 freezes laptop.
        // 1000 at 3x3 reasonable, fast.
        // 50000 at 5x5 freezes laptop.
        private const int kMaxIterations = 1000;
        private const int kMaxMilliseconds = 2000;
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
                Log(gameState.CurrentPlayer.Turn.ToString() +
                    ": No good moves. Passing.");
                return Game.PassMove;
            }

            return topAction.Action.Position;
        }

        [Conditional("LOG_GO_SEARCHER")]
        private void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("LOG_GO_SEARCHER")]
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
                sb.Append(UnityEngine.Mathf.Round((float)action.NumWins));
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
