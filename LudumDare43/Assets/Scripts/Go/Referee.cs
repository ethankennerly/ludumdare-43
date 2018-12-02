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

        public static event Action<Content, int, int> OnIllegalMove;

        public static event Action<Board> OnBoardSetup;

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
            bool legal;
            Game nextGame = Game.MakeMove(x, y, out legal);
            if (!legal)
            {
                if (OnIllegalMove != null)
                    OnIllegalMove(Game.Turn, x, y);
                return;
            }

            Game = nextGame;
        }
    }
}
