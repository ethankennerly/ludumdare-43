using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class AnimatedTurn : MonoBehaviour
    {
        public Action<Content, Content> m_OnTurn;
        public Action<Content> m_OnWin;

        [SerializeField]
        private Cell.AnimatedPlayerTile[] m_Indicators;

        private void OnEnable()
        {
            AddTurnListener();
            AddWinListener();
        }

        private void AddTurnListener()
        {
            if (m_OnTurn == null)
                m_OnTurn = SetTurn;
            Referee.OnTurn -= m_OnTurn;
            Referee.OnTurn += m_OnTurn;
            if (Referee.InstanceExists() && Referee.instance.Game != null)
                m_OnTurn(Content.Empty, Referee.instance.Turn);
        }

        private void AddWinListener()
        {
            if (m_OnWin == null)
                m_OnWin = SetWin;
            Referee.OnWin -= m_OnWin;
            Referee.OnWin += m_OnWin;
            if (Referee.InstanceExists() && Referee.instance.Win != Content.Empty)
                m_OnWin(Referee.instance.Win);
        }

        private void OnDisable()
        {
            Referee.OnTurn -= m_OnTurn;
        }

        private void SetTurn(Content previousTurn, Content nextTurn)
        {
            foreach (var indicator in m_Indicators)
                indicator.Update(previousTurn, nextTurn);
        }

        private void SetWin(Content winner)
        {
            if (winner == Content.Empty)
                return;

            Content turn = Content.Empty;
            if (Referee.instance.Game != null)
                turn = Referee.instance.Turn;

            Debug.Log("SetWin: turn=" + turn + " winner=" + winner);
            foreach (var indicator in m_Indicators)
                indicator.Update(turn, winner, true);
        }
    }
}
