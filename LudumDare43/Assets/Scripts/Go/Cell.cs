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
            set { m_Point = value; }
        }

        private Action<Collider2D> m_OnClickAnything;

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

            RemoveListeners();

            ClickInputSystem.instance.onCollisionEnter2D += m_OnClickAnything;
        }

        private void RemoveListeners()
        {
            if (ClickInputSystem.InstanceExists())
                ClickInputSystem.instance.onCollisionEnter2D -= m_OnClickAnything;
        }

        private void PublishClick(Collider2D target)
        {
            if (target == null)
                return;

            Cell cell = (Cell)target.GetComponent(typeof(Cell));
            if (cell == null)
                return;

            if (OnClick == null)
                return;

            OnClick(cell.Point.x, cell.Point.y);
        }
    }
}
