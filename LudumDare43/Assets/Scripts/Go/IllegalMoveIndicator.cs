using FineGameDesign.Go.AI;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class IllegalMoveIndicator : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private string m_AnimationName = "Illegal";

        [SerializeField]
        private Cell m_PositionFilter;

        [Header("Empty permits either turn.")]
        [SerializeField]
        private Content m_TurnFilter;

        private Action<Content, int, int> m_OnIllegalMove;

        private void OnEnable()
        {
            if (m_OnIllegalMove == null)
                m_OnIllegalMove = PlayAnimation;
            Referee5x5.OnIllegalMove -= m_OnIllegalMove;
            Referee5x5.OnIllegalMove += m_OnIllegalMove;
        }

        private void OnDisable()
        {
            Referee5x5.OnIllegalMove -= m_OnIllegalMove;
        }

        private void PlayAnimation(Content turn, int x, int y)
        {
            if (m_PositionFilter != null &&
                (m_PositionFilter.Point.x != x ||
                m_PositionFilter.Point.y != y))
                return;

            if (m_TurnFilter != Content.Empty &&
                m_TurnFilter != turn)
                return;

            m_Animator.Play(m_AnimationName, -1, 0f);
        }
    }
}
