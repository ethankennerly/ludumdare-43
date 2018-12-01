using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Referee : MonoBehaviour
    {
        public event Action<Board> OnBoardSetup;

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
                if (value != null && OnBoardSetup != null)
                    OnBoardSetup(value.Board);
            }
        }
    }
}
