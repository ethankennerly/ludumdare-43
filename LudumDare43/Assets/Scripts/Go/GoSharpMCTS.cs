using Go;
using MonteCarlo;
using System.Collections.Generic;
using UnityEngine;

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

        private readonly Game m_Game;

        private IList<GoAction> m_Actions;

        public IList<GoAction> Actions
        {
            get
            {
                if (m_Actions == null)
                {
                    m_Actions = GoAction.ConvertMoves(m_Game.GetLegalMoves());
                    Debug.Log("GoState.Actions: " + m_Game.Turn + ": num moves=" + m_Actions.Count);
                }

                return m_Actions;
            }
        }

        public GoState(Game game)
        {
            m_Game = game;
        }

        /// <summary>
        /// Remove action.
        /// Otherwise, searcher infinitely repeats getting an action.
        /// </summary>
        // TODO: Handle pass to end game.
        // TODO: Less waste. GoSharp clones game everytime an action is applied.
        public void ApplyAction(GoAction action)
        {
            m_Game.MakeMove(action.Position);
            if (m_Actions != null)
                m_Actions.Remove(action);
        }

        public IState<GoPlayer, GoAction> Clone()
        {
            return new GoState(new Game(m_Game));
        }

        public double GetResult(GoPlayer forPlayer)
        {
            return m_Game.GetResult(forPlayer.Turn);
        }
    }
}
