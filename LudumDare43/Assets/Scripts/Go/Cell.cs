using FineGameDesign.Utils;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class Cell : MonoBehaviour
    {
        private static readonly bool s_Verbose = false;

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

        private Content m_Territory;

        [Serializable]
        public struct AnimatedPlayerTile
        {
            public Animator animator;
            public AnimatedPlayerTileSet tileSet;

            public void Update(Content previous, Content next, bool doNotRestart = false)
            {
                string animationName = tileSet.GetAnimationName(previous, next);
                if (string.IsNullOrEmpty(animationName))
                    return;

                if (s_Verbose)
                    Debug.Log("Update: animationName=" + animationName +
                        " previous=" + previous + " next=" + next + " doNotRestart=" + doNotRestart,
                        animator);

                if (doNotRestart)
                    animator.Play(animationName);
                else
                    animator.Play(animationName, -1, 0f);
            }
        }

        [SerializeField]
        private AnimatedPlayerTile[] m_AnimatedPlayerTiles;

        [SerializeField]
        private AnimatedPlayerTile[] m_PlayerTerritories;

        private Action<Collider2D> m_OnClickAnything;

        private Action<Board.PositionContent> m_OnContentChanged;
        private Action<Board.PositionContent> m_OnTerritoryChanged;

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

            if (m_OnTerritoryChanged == null)
                m_OnTerritoryChanged = SetTerritory;

            RemoveListeners();

            ClickInputSystem.instance.onCollisionEnter2D += m_OnClickAnything;
            BoardLayout.OnContentChanged += m_OnContentChanged;
            BoardLayout.OnTerritoryChanged += m_OnTerritoryChanged;
        }

        private void RemoveListeners()
        {
            if (ClickInputSystem.InstanceExists())
                ClickInputSystem.instance.onCollisionEnter2D -= m_OnClickAnything;
            
            BoardLayout.OnContentChanged -= m_OnContentChanged;
            BoardLayout.OnTerritoryChanged -= m_OnTerritoryChanged;
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

        private void SetTerritory(Board.PositionContent next)
        {
            if (next.Position.x != m_Point.x ||
                next.Position.y != m_Point.y)
                return;

            foreach (AnimatedPlayerTile tile in m_PlayerTerritories)
                tile.Update(m_Territory, next.Content);

            m_Territory = next.Content;
        }
    }
}
