using MonteCarlo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FineGameDesign.Go.AI
{
    public sealed class GoSearcher5x5
    {
        /// <remarks>
        /// 20000 iterations on 5x5 reasonable.
        /// </remarks>
        private const int kMaxIterations = 20000;
        private const int kMaxMilliseconds = 8000;
        private const double kMinExploitationValue = 0.125;

        public GoGameState5x5 Game = new GoGameState5x5();

        public void MakeMove()
        {
            uint moveMask = GetMoveMask(Game);
            Game.Move(moveMask);
        }

        private uint GetMoveMask(GoGameState5x5 game)
        {
            var gameState = new GoState5x5(game);
            if (!gameState.Actions.Any())
            {
                return GoGameState5x5.kPassMask;
            }

            var topActions = MonteCarloTreeSearch.GetTopActions(gameState, kMaxIterations, kMaxMilliseconds).ToList();
            if (topActions.Count == 0)
            {
                return GoGameState5x5.kPassMask;
            }

            Log("Turn Index: " + gameState.CurrentPlayer.TurnIndex.ToString(), topActions, game);
            MonteCarloTreeSearch.Node<GoPlayer5x5, GoAction5x5> topAction = topActions[0];
            double exploitationValue = topAction.NumWins / topAction.NumRuns;
            if (exploitationValue < kMinExploitationValue)
            {
                Log(gameState.CurrentPlayer.TurnIndex.ToString() +
                    ": No good moves. Passing.");
                return GoGameState5x5.kPassMask;
            }

            return topAction.Action.PositionMask;
        }

        [Conditional("LOG_GO_SEARCHER")]
        private void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("LOG_GO_SEARCHER")]
        private void Log(string prefix, IList<MonteCarloTreeSearch.Node<GoPlayer5x5, GoAction5x5>> actions,
            GoGameState5x5 game = null,
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

                sb.Append(", position mask: ");
                sb.Append(action.Action.PositionMask);
                sb.Append(", ");
                sb.Append(UnityEngine.Mathf.Round((float)action.NumWins));
                sb.Append("/");
                sb.Append(action.NumRuns);
            }

            if (game != null)
            {
                sb.Append("\n");
                sb.Append(game.ToString());
            }

            string message = sb.ToString();
            UnityEngine.Debug.Log(message);
        }
    }
}
