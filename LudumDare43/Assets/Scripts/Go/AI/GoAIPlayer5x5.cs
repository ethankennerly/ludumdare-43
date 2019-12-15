using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go.AI
{
    public sealed class GoAIPlayer5x5 : MonoBehaviour
    {
        [SerializeField]
        private bool m_BlackAI = false;

        [SerializeField]
        private bool m_WhiteAI = true;

        private Action<Content, Content> m_OnTurn;
        private Action<Content, Content> OnTurn
        {
            get
            {
                if (m_OnTurn == null)
                {
                    m_OnTurn = TryMakeMove;
                }
                return m_OnTurn;
            }
        }

        private readonly GoSearcher5x5 m_Searcher = new GoSearcher5x5();

        private bool m_WillMove;
        private float m_MoveDelay = 1.5f;
        private float m_MoveDelayRemaining;

        private void OnEnable()
        {
            Referee5x5.OnTurn += OnTurn;
        }

        private void OnDisable()
        {
            Referee5x5.OnTurn -= OnTurn;
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
            m_Searcher.Game = Referee5x5.instance.Game;
            m_Searcher.MakeMove();
            Referee5x5.instance.PublishState();
        }
    }
}
