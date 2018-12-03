using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : ASingleton<Referee>
    {
        public static event Action<Content, int, int> OnIllegalMove;

        public static event Action<Content, float> OnScoreSet;

        /// <summary>
        /// Static events avoid constructor/destructor races and references.
        /// </summary>
        public static event Action<Content, Content> OnTurn;

        public static event Action<Content> OnWin;

        public static event Action<Board> OnBoardSet;

        private bool m_Verbose = false;

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

        private Content m_Win = Content.Empty;
        public Content Win
        {
            get { return m_Win; }

            private set
            {
                if (m_Win == value)
                    return;

                Turn = value;
                m_Win = value;

                if (OnWin != null)
                    OnWin(value);
            }
        }

        private Game m_Game;
        /// <summary>
        /// GoSharp replaces Game every turn.
        /// </summary>
        public Game Game
        {
            get { return m_Game; }
            set
            {
                m_Game = value;

                if (value.Ended)
                    PublishPlayEnded();
                else if (value != null)
                    Turn = value.Turn;

                if (value != null && OnScoreSet != null)
                {
                    OnScoreSet(Content.Black, value.GetScore(Content.Black));
                    OnScoreSet(Content.White, value.GetScore(Content.White));
                }

                if (value != null && OnBoardSet != null)
                    OnBoardSet(value.Board);
            }
        }

        public void MakeMove(int x, int y)
        {
            if (Game.Ended)
                return;

            bool legal;
            Game previousGame = Game;
            Game nextGame = previousGame.MakeMove(x, y, out legal);
            if (!legal)
            {
                if (OnIllegalMove != null)
                    OnIllegalMove(previousGame.Turn, x, y);
                return;
            }

            Game = nextGame;
        }

        public void Pass()
        {
            Game = Game.Pass();
        }

        /// <summary>
        /// Republishes board. Otherwise territories did not appear.
        /// </summary>
        private void PublishPlayEnded()
        {
            if (m_Verbose)
                Debug.Log("PublishPlayEnded: IsScoring=" + Game.Board.IsScoring + " Board=\n" + Game.Board);

            PublishWin();

            if (OnBoardSet != null)
                OnBoardSet(Game.Board);
        }

        private void PublishWin()
        {
            float blackScore = Game.GetScore(Content.Black);
            float whiteScore = Game.GetScore(Content.White);
            if (blackScore == whiteScore)
                return;

            Win = blackScore > whiteScore ? Content.Black : Content.White;
        }
    }
}
