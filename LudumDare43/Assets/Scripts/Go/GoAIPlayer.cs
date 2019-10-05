using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class GoAIPlayer : MonoBehaviour
    {
        [SerializeField]
        private bool m_BlackAI = false;

        [SerializeField]
        private bool m_WhiteAI = true;

        private Action<Content, Content> m_OnTurn;

        private readonly GoSearcher m_Searcher = new GoSearcher();

        private bool m_WillMove;
        private float m_MoveDelay = 1f;
        private float m_MoveDelayRemaining;

        private void OnEnable()
        {
            if (m_OnTurn == null)
                m_OnTurn = TryMakeMove;

            Referee.OnTurn -= m_OnTurn;
            Referee.OnTurn += m_OnTurn;
            if (Referee.InstanceExists() && Referee.instance.Game != null)
                m_OnTurn(Content.Empty, Referee.instance.Game.Turn);
        }

        private void OnDisable()
        {
            Referee.OnTurn -= m_OnTurn;
        }

        private void TryMakeMove(Content previousTurn, Content nextTurn)
        {
            if (nextTurn == Content.Empty ||
                (nextTurn == Content.Black && !m_BlackAI) ||
                (nextTurn == Content.White && !m_WhiteAI))
                return;

            m_WillMove = true;
            m_MoveDelayRemaining = m_MoveDelay;
        }

        private void Update()
        {
            if (!m_WillMove)
                return;

            m_MoveDelayRemaining -= Time.deltaTime;
            if (m_MoveDelayRemaining > 0f)
                return;

            m_WillMove = false;
            m_Searcher.MakeMove(Referee.instance);
        }
    }
}
