using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public class ClickCollider : MonoBehaviour
    {
        public static event Action<ClickCollider> OnClick;

        [SerializeField]
        private Collider2D m_Collider;

        [Header("Optional. Restarts animation on each click.")]
        [SerializeField]
        private Animator m_Animator;
        [SerializeField]
        private string m_AnimationName = "Clicked";

        private Action<Collider2D> m_OnClickAnything;

        private void OnEnable()
        {
            if (m_OnClickAnything == null)
                m_OnClickAnything = PublishClick;
            RemoveListeners();
            ClickInputSystem.instance.onCollisionEnter2D += m_OnClickAnything;
        }

        private void RemoveListeners()
        {
            if (ClickInputSystem.InstanceExists())
                ClickInputSystem.instance.onCollisionEnter2D -= m_OnClickAnything;
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void PublishClick(Collider2D target)
        {
            if (target == null)
                return;

            if (target != m_Collider)
                return;

            HandleClick();

            if (m_Animator != null)
                m_Animator.Play(m_AnimationName, -1, 0f);

            if (OnClick == null)
                return;

            OnClick(this);
        }

        protected virtual void HandleClick()
        {
        }
    }
}
