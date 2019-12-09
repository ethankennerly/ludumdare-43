using MonteCarlo;
using System.Collections.Generic;
using System.Diagnostics;

namespace FineGameDesign.Go.AI
{
    public struct GoAction : IAction
    {
        private static readonly List<GoAction> s_Empty = new List<GoAction>();

        public uint PositionMask;

        public GoAction(uint positionMask)
        {
            PositionMask = positionMask;
        }

        public static IList<GoAction> ConvertMoveMask(uint legalMoveMask)
        {
            if (legalMoveMask == 0)
            {
                return s_Empty;
            }

            UnityEngine.Debug.LogWarning("ConvertMoveMask: TODO");
            return s_Empty;
        }
    }

    public struct GoPlayer : IPlayer
    {
        public int TurnIndex;

        public GoPlayer(int turnIndex)
        {
            TurnIndex = turnIndex;
        }

        public static GoPlayer Black = new GoPlayer(0);
        public static GoPlayer White = new GoPlayer(1);

        public static GoPlayer GetOtherPlayer(GoPlayer player)
        {
            return player.TurnIndex == 0 ? White : Black;
        }
    }

    public class GoState : IState<GoPlayer, GoAction>
    {
        public GoPlayer CurrentPlayer
        {
            get
            {
                return m_Game.TurnIndex == 0 ? GoPlayer.Black : GoPlayer.White;
            }
        }

        private GoGameState5x5 m_Game;

        private IList<GoAction> m_Actions;

        /// <summary>
        /// Caches.
        /// Otherwise, expensive to calculate legal moves.
        /// </summary>
        public IList<GoAction> Actions
        {
            get
            {
                return m_Actions;
            }
        }

        public GoState(GoGameState5x5 game)
        {
            m_Game = game;
            m_Actions = GoAction.ConvertMoveMask(m_Game.CreateLegalMoveMask());
        }

        /// <summary>
        /// Remove action.
        /// Otherwise, searcher infinitely repeats getting an action.
        /// </summary>
        public void ApplyAction(GoAction action)
        {
            Log("ApplyAction: Before: " + m_Game.TurnIndex + action.PositionMask +
                "\n" + m_Game);
            m_Game.Move(action.PositionMask);
            m_Actions = GoAction.ConvertMoveMask(m_Game.CreateLegalMoveMask());
        }

        public IState<GoPlayer, GoAction> Clone()
        {
            return new GoState(m_Game.Clone());
        }

        public double GetResult(GoPlayer forPlayer)
        {
            double result = m_Game.CalculateResultForPlayer(forPlayer.TurnIndex);
            Log("GetResult: " + forPlayer.TurnIndex +
                ": result: " + result +
                "\n" + m_Game);
            return result;
        }

        [Conditional("LOG_GO_GAME_STATE_MCTS")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
