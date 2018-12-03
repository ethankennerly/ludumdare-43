using Go;
using MonteCarlo;
using System.Collections.Generic;

namespace FineGameDesign.Go
{
    public struct GoAction : IAction
    {
        public Point Position;

        public GoAction(Point position)
        {
            Position = position;
        }

        public static IList<GoAction> ConvertMoves(List<Point> points)
        {
            int numPoints = points.Count;
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

        public IList<GoAction> Actions
        {
            get
            {
                if (m_Game == null || m_Game.Board == null || m_Game.Board.IsScoring)
                    return new GoAction[0];

                return GoAction.ConvertMoves(m_Game.GetLegalMoves());
            }
        }

        public GoState(Game game)
        {
            m_Game = game;
        }

        // TODO: Handle pass to end game.
        // TODO: Pass represented by PassMove.
        // TODO: Less waste. GoSharp clones game everytime an action is applied.
        public void ApplyAction(GoAction action)
        {
            m_Game.MakeMove(action.Position);
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
