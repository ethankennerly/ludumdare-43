using FineGameDesign.Go.AI;
using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class ClickPlacer : MonoBehaviour
    {
        private Action<int, int> m_OnClickCell;

        private void OnEnable()
        {
            if (m_OnClickCell == null)
                m_OnClickCell = Referee5x5.instance.MakeMove;
            Cell.OnClick -= m_OnClickCell;
            Cell.OnClick += m_OnClickCell;
        }

        private void OnDisable()
        {
            Cell.OnClick -= m_OnClickCell;
        }
    }
}
