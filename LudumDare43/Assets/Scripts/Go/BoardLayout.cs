using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class BoardLayout : MonoBehaviour
    {
        [SerializeField]
        private Cell m_CellPrefab;

        [SerializeField]
        private Vector3 m_CellOffset;

        [SerializeField]
        private Vector3 m_Center;

        [SerializeField]
        private Referee m_Referee;

        private Action<Board> m_OnSetup;

        private void OnEnable()
        {
            if (m_OnSetup == null)
                m_OnSetup = Setup;
            m_Referee.OnBoardSetup -= m_OnSetup;
            m_Referee.OnBoardSetup += m_OnSetup;
        }

        private void OnDisable()
        {
            m_Referee.OnBoardSetup -= m_OnSetup;
        }

        public void Setup(Board board)
        {
            Debug.Log(board);
        }
    }
}
