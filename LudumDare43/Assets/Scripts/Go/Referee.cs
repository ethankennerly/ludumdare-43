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

        private Action<int, int> m_OnClickCell;

        private void OnEnable()
        {
            if (m_OnClickCell == null)
                m_OnClickCell = MakeMove;
            Cell.OnClick -= m_OnClickCell;
            Cell.OnClick += m_OnClickCell;
        }

        private void OnDisable()
        {
            Cell.OnClick -= m_OnClickCell;
        }

        private void MakeMove(int x, int y)
        {
            Debug.Log("MakeMove: " + x + ", " + y);

            bool legal;
            Game = Game.MakeMove(x, y, out legal);
        }
    }
}
