using FineGameDesign.Utils;
using Go;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FineGameDesign.Go.AI
{
    public sealed class Board5x5
    {
        GoConfig5x5 Config;
        List<Board.PositionContent> AllContent;
    }

    public sealed class Referee5x5 : ASingleton<Referee5x5>
    {
        public static event Action<Content, int, int> OnIllegalMove;

        public static event Action<Content, float> OnScoreSet;

        /// <summary>
        /// Static events avoid constructor/destructor races and references.
        /// </summary>
        public static event Action<Content, Content> OnTurn;

        public static event Action<Content> OnWin;

        public static event Action<Board5x5> OnBoardSet;

        public Board5x5 Board = new Board5x5();

        private Content m_Turn = Content.Empty;
        public Content Turn
        {
            get { return m_Turn; }

            private set
            {
                if (m_Turn == value)
                    return;

                Content previous = m_Turn;
                m_Turn = value;

                if (OnTurn != null)
                    OnTurn(previous, value);
            }
        }

        public Content OtherTurn
        {
            get
            {
                if (m_Turn == Content.Black)
                {
                    return Content.White;
                }

                if (m_Turn == Content.White)
                {
                    return Content.Black;
                }

                return Content.Empty;
            }
        }

        private Content m_Win = Content.Empty;
        public Content Win
        {
            get { return m_Win; }

            private set
            {
                if (m_Win == value)
                {
                    return;
                }

                Turn = value;
                m_Win = value;

                if (OnWin != null)
                {
                    OnWin(value);
                }
            }
        }

        private GoGameState5x5 m_Game;

        /// <summary>
        /// GoSharp replaces Game every turn.
        /// </summary>
        public void Init(int sizeX, int sizeY)
        {
            m_Game = new GoGameState5x5();
            m_Game.SetSize(sizeX, sizeY);

            if (PlayEnded())
            {
                PublishPlayEnded();
            }

            if (OnScoreSet != null)
            {
                OnScoreSet(Content.Black, -m_Game.PointsForPlayer1);
                OnScoreSet(Content.White, 0);
            }

            if (OnBoardSet != null)
            {
                OnBoardSet(Board);
            }
        }

        public bool PlayEnded()
        {
            return !m_Game.CanMove(m_Game.TurnIndex);
        }

        public void MakeLegalMove(int x, int y)
        {
            m_Game.MoveAtPosition(new BoardPosition(){x = x, y = y});
            UpdateBoard(m_Game, Board);
        }

        public void MakeMove(int x, int y)
        {
            if (PlayEnded())
            {
                return;
            }

            BoardPosition pos = new BoardPosition(){x = x, y = y};
            if (!m_Game.IsLegalMoveAtPosition(pos))
            {
                if (OnIllegalMove != null)
                {
                    OnIllegalMove(OtherTurn, x, y);
                }
                return;
            }

            m_Game.MoveAtPosition(pos);
            UpdateBoard(m_Game, Board);
        }

        // TODO
        private void UpdateBoard(GoGameState5x5 gameState, Board5x5 board)
        {
        }

        public void Pass()
        {
            m_Game.TurnIndex = m_Game.TurnIndex == 0 ? 1 : 0;
        }

        /// <summary>
        /// Republishes board. Otherwise territories did not appear.
        /// </summary>
        private void PublishPlayEnded()
        {
            PublishWin();

            if (OnBoardSet != null)
            {
                OnBoardSet(Board);
            }
        }

        private void PublishWin()
        {
            float winner = m_Game.CalculateWinner();
            if (winner == 0.5f)
            {
                return;
            }

            Win = winner < 0.5f ? Content.Black : Content.White;
        }
    }
}
