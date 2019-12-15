using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class AnimatedTurn5x5 : MonoBehaviour
    {
        [SerializeField]
        private Action<Content, Content> m_OnTurn;
        private Action<Content, Content> OnTurn
        {
            get
            {
                if (m_OnTurn == null)
                {
                    m_OnTurn = SetTurn;
                }
                return m_OnTurn;
            }
        }

        private Action<Content> m_OnWin;
        private Action<Content> OnWin
        {
            get
            {
                if (m_OnWin == null)
                {
                    m_OnWin = SetWin;
                }
                return m_OnWin;
            }
        }

        [SerializeField]
        private Cell.AnimatedPlayerTile[] m_Indicators;

        [SerializeField]
        private Cell.AnimatedPlayerTile[] m_WinIndicators;

        private void OnEnable()
        {
            AddTurnListener();
            AddWinListener();
        }

        private void AddTurnListener()
        {
            Referee.OnTurn += OnTurn;
        }

        private void AddWinListener()
        {
            Referee.OnWin += OnWin;
        }

        private void OnDisable()
        {
            Referee.OnTurn -= OnTurn;
            Referee.OnWin -= OnWin;
        }

        private void SetTurn(Content previousTurn, Content nextTurn)
        {
            foreach (var indicator in m_Indicators)
            {
                indicator.Update(previousTurn, nextTurn);
            }
        }

        private void SetWin(Content winner)
        {
            if (winner == Content.Empty)
            {
                return;
            }

            Content turn = Content.Empty;
            if (Referee.InstanceExists())
            {
                turn = Referee.instance.Turn;
            }

            foreach (var indicator in m_Indicators)
            {
                indicator.Update(turn, winner, true);
            }

            foreach (var indicator in m_WinIndicators)
            {
                indicator.Update(Content.Empty, winner);
            }
        }
    }
}
