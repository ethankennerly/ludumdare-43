using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : MonoBehaviour
    {
        /// <summary>
        /// Static events avoid constructor/destructor races and references.
        /// </summary>
        public static event Action<Content, Content> OnTurn;

        public static event Action<Board> OnBoardSetup;

        public static Board s_Board;

        public static Board Board
        {
            get { return s_Board; }
        }

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

                if (value != null)
                {
                    Turn = value.Turn;
                    s_Board = value.Board;
                }

                if (value != null && OnBoardSetup != null)
                    OnBoardSetup(value.Board);
            }
        }
    }
}
