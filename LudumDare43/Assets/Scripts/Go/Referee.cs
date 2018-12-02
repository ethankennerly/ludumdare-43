using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : ASingleton<Referee>
    {
        public static event Action<Content, int, int> OnIllegalMove;

        /// <summary>
        /// Static events avoid constructor/destructor races and references.
        /// </summary>
        public static event Action<Content, Content> OnTurn;

        public static event Action<Content> OnWin;

        public static event Action<Board> OnBoardSet;

        private Content m_Turn;
        private Content Turn
        {
            set
            {
                if (m_Turn == value)
                    return;

                if (OnTurn != null)
                    OnTurn(m_Turn, value);

                m_Turn = value;
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

                if (!m_Ended && value != null)
                    Turn = value.Turn;

                if (m_Ended && value != null && value.Board != null)
                    value.Board.IsScoring = true;

                if (value != null && OnBoardSet != null)
                    OnBoardSet(value.Board);
            }
        }

        public void MakeMove(int x, int y)
        {
            if (m_Ended)
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

            if (m_NumPasses > 0)
                m_NumPasses--;

            Game = nextGame;
        }

        private const int kMaxPasses = 1;
        private int m_NumPasses = 0;
        private bool m_Ended = false;

        public void Pass()
        {
            m_NumPasses++;
            m_Ended = m_NumPasses > kMaxPasses;
            Game = Game.Pass();
            if (m_Ended)
            {
                EndPlay();
                return;
            }
        }

        private void EndPlay()
        {
            Debug.Log("EndPlay: TODO");
            Turn = Content.Empty;
            m_Ended = true;
        }
    }
}
