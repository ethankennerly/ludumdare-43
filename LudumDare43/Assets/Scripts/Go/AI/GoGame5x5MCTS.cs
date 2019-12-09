using MonteCarlo;
using System.Collections.Generic;
using System.Diagnostics;

namespace FineGameDesign.Go.AI
{
    public struct GoAction5x5 : IAction
    {
        public uint PositionMask;

        public GoAction5x5(uint positionMask)
        {
            PositionMask = positionMask;
        }

        private static List<uint> s_IndividualBits = new List<uint>(32);

        public static void ConvertMoveMask(uint legalMoveMask, IList<GoAction5x5> actions)
        {
            ExtractOnBitMasks(legalMoveMask, s_IndividualBits);

            actions.Clear();
            foreach(uint individualBit in s_IndividualBits)
            {
                actions.Add(new GoAction5x5(individualBit));
            }
        }

        public static void ExtractOnBitMasks(uint multipleOnBits, List<uint> individualBits)
        {
            individualBits.Clear();
            for (int bitIndex = 0; multipleOnBits > 0; ++bitIndex, multipleOnBits >>= 1)
            {
                if ((multipleOnBits & 1) > 0)
                {
                    individualBits.Add((uint)(1 << bitIndex));
                }
            }
        }
    }

    public struct GoPlayer5x5 : IPlayer
    {
        public int TurnIndex;

        public GoPlayer5x5(int turnIndex)
        {
            TurnIndex = turnIndex;
        }

        public static GoPlayer5x5 Black = new GoPlayer5x5(0);
        public static GoPlayer5x5 White = new GoPlayer5x5(1);

        public static GoPlayer5x5 GetOtherPlayer(GoPlayer5x5 player)
        {
            return player.TurnIndex == 0 ? White : Black;
        }
    }

    public class GoState5x5 : IState<GoPlayer5x5, GoAction5x5>
    {
        public GoPlayer5x5 CurrentPlayer
        {
            get
            {
                return m_Game.TurnIndex == 0 ? GoPlayer5x5.Black : GoPlayer5x5.White;
            }
        }

        private GoGameState5x5 m_Game;

        private IList<GoAction5x5> m_Actions = new List<GoAction5x5>(32);

        /// <summary>
        /// Caches.
        /// Otherwise, expensive to calculate legal moves.
        /// </summary>
        public IList<GoAction5x5> Actions
        {
            get
            {
                return m_Actions;
            }
        }

        public GoState5x5(GoGameState5x5 game)
        {
            m_Game = game;
            GoAction5x5.ConvertMoveMask(m_Game.CreateLegalMoveMask(), m_Actions);
        }

        /// <summary>
        /// Remove action.
        /// Otherwise, searcher infinitely repeats getting an action.
        /// </summary>
        public void ApplyAction(GoAction5x5 action)
        {
            Log("ApplyAction: Before: " + m_Game.TurnIndex + action.PositionMask +
                "\n" + m_Game);
            m_Game.Move(action.PositionMask);
            GoAction5x5.ConvertMoveMask(m_Game.CreateLegalMoveMask(), m_Actions);
        }

        public IState<GoPlayer5x5, GoAction5x5> Clone()
        {
            return new GoState5x5(m_Game.Clone());
        }

        public double GetResult(GoPlayer5x5 forPlayer)
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
