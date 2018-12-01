using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Cell : MonoBehaviour
    {
        public static event Action<int, int> OnClick;

        [SerializeField]
        private Collider2D m_Collider;

        private Point m_Point;
        public Point Point
        {
            get { return m_Point; }
            set
            {
                name = "Cell_" + value.x + "_" + value.y;

                m_Point = value;
            }
        }

        private Content m_Content;

        [Serializable]
        public struct AnimatedPlayerTile
        {
            public Animator animator;
            public AnimatedPlayerTileSet tileSet;

            public void Update(Content previous, Content next)
            {
                string animationName = tileSet.GetAnimationName(previous, next);
                if (string.IsNullOrEmpty(animationName))
                    return;

                animator.Play(animationName, -1, 0f);
            }
        }

        [SerializeField]
        private AnimatedPlayerTile[] m_AnimatedPlayerTiles;

        private Action<Collider2D> m_OnClickAnything;

        private Action<Board.PositionContent> m_OnContentChanged;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            if (m_OnClickAnything == null)
                m_OnClickAnything = PublishClick;

            if (m_OnContentChanged == null)
                m_OnContentChanged = SetContent;

            RemoveListeners();

            ClickInputSystem.instance.onCollisionEnter2D += m_OnClickAnything;
            BoardLayout.OnContentChanged += m_OnContentChanged;
        }

        private void RemoveListeners()
        {
            if (ClickInputSystem.InstanceExists())
                ClickInputSystem.instance.onCollisionEnter2D -= m_OnClickAnything;
            
            BoardLayout.OnContentChanged -= m_OnContentChanged;
        }

        private void PublishClick(Collider2D target)
        {
            if (target == null)
                return;

            if (target != m_Collider)
                return;

            if (OnClick == null)
                return;

            OnClick(Point.x, Point.y);
        }

        private void SetContent(Board.PositionContent next)
        {
            if (next.Position.x != m_Point.x ||
                next.Position.y != m_Point.y)
                return;

            foreach (AnimatedPlayerTile tile in m_AnimatedPlayerTiles)
                tile.Update(m_Content, next.Content);

            m_Content = next.Content;
        }
    }
}
