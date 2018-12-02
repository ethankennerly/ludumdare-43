using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : ASingleton<Referee>
    {
        /// <summary>
        /// Static events avoid constructor/destructor races and references.
        /// </summary>
        public static event Action<Content, Content> OnTurn;

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

                if (value != null)
                    Turn = value.Turn;

                if (value != null && OnBoardSet != null)
                    OnBoardSet(value.Board);
            }
        }
    }
}
