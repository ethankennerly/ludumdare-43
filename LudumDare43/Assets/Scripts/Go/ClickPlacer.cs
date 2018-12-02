using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class ClickPlacer : MonoBehaviour
    {
        public static event Action<Content, int, int> OnIllegalMove;

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
            Game previousGame = Referee.instance.Game;
            Game nextGame = previousGame.MakeMove(x, y, out legal);
            if (!legal)
            {
                if (OnIllegalMove != null)
                    OnIllegalMove(previousGame.Turn, x, y);
                return;
            }

            Referee.instance.Game = nextGame;
        }
    }
}
