using Go;
using MonteCarlo;
using System.Collections.Generic;
using System.Diagnostics;

namespace FineGameDesign.Go
{
    public struct GoAction : IAction
    {
        private static readonly List<GoAction> s_Empty = new List<GoAction>();

        public Point Position;

        public GoAction(Point position)
        {
            Position = position;
        }

        public static IList<GoAction> ConvertMoves(List<Point> points)
        {
            int numPoints = points.Count;
            if (numPoints == 0)
                return s_Empty;

            List<GoAction> actions = new List<GoAction>(numPoints);
            for (int index = 0; index < numPoints; ++index)
            {
                actions.Add(new GoAction(points[index]));
            }
            return actions;
        }
    }

    public struct GoPlayer : IPlayer
    {
        public Content Turn;

        public GoPlayer(Content turn)
        {
            Turn = turn;
        }

        public static GoPlayer Black = new GoPlayer(Content.Black);
        public static GoPlayer White = new GoPlayer(Content.White);

        public static GoPlayer GetOtherPlayer(GoPlayer player)
        {
            return player.Turn == Content.Black ? White : Black;
        }
    }

    public class GoState : IState<GoPlayer, GoAction>
    {
        public GoPlayer CurrentPlayer
        {
            get
            {
                return m_Game.Turn == Content.Black ? GoPlayer.Black : GoPlayer.White;
            }
        }

        private Game m_Game;

        private IList<GoAction> m_Actions;

        private List<Game> m_Renters = new List<Game>(64);

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

        public GoState(Game game)
        {
            m_Game = game;
            m_Actions = GoAction.ConvertMoves(m_Game.GetLegalMoves());
        }

        /// <summary>
        /// Remove action.
        /// Otherwise, searcher infinitely repeats getting an action.
        /// </summary>
        public void ApplyAction(GoAction action)
        {
            Log("ApplyAction: Before: " + m_Game.Turn + action.Position +
                "\n" + m_Game.Board);
            Game nextGame = Game.GamePool.Rent();
            m_Renters.Add(nextGame);
            m_Game = m_Game.MakeLegalMove(action.Position, nextGame);
            m_Actions = GoAction.ConvertMoves(m_Game.GetLegalMoves());
        }

        public IState<GoPlayer, GoAction> Clone()
        {
            foreach (Game renter in m_Renters)
            {
                Game.GamePool.Return(renter);
            }
            m_Renters.Clear();

            Game nextGame = Game.GamePool.Rent();
            m_Renters.Add(nextGame);
            nextGame.Clone(m_Game, cloneTurn: true);
            m_Game = nextGame;
            return new GoState(m_Game);
        }

        public double GetResult(GoPlayer forPlayer)
        {
            bool wasScoring = m_Game.Board.IsScoring;
            m_Game.Board.IsScoring = true;
            double result = m_Game.GetResult(forPlayer.Turn);
            Log("GetResult: " + forPlayer.Turn +
                ": result: " + result +
                " was scoring: " + wasScoring +
                "\n" + m_Game.Board);
            return result;
        }

        [Conditional("LOG_GO_SHARP_MCTS")]
        private static void Log(string message)
        {
            UnityEngine.Debug.Log(message);
        }
    }
}
