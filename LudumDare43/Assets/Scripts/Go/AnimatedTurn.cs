using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class AnimatedTurn : MonoBehaviour
    {
        public Action<Content, Content> m_OnTurn;

        [SerializeField]
        private Cell.AnimatedPlayerTile[] m_Indicators;

        private void OnEnable()
        {
            if (m_OnTurn == null)
                m_OnTurn = SetTurn;
            Referee.OnTurn -= m_OnTurn;
            Referee.OnTurn += m_OnTurn;
            if (Referee.InstanceExists())
                m_OnTurn(Content.Empty, Referee.instance.Game.Turn);
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
    }
}
