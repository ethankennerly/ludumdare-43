using Go;
using System;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class BoardLayout : MonoBehaviour
    {
        public static event Action<Board.PositionContent> OnContentChanged;

        [SerializeField]
        private Cell m_CellPrefab;

        [SerializeField]
        private Vector3 m_HorizontalOffset;

        [SerializeField]
        private Vector3 m_VerticalOffset;

        [SerializeField]
        private Vector3 m_Center;

        [SerializeField]
        private Referee m_Referee;

        private Action<Board> m_OnSetup;

        private Cell[] m_Cells;

        private Board m_PreviousBoard;

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

        public void Setup(Board nextBoard)
        {
            Debug.Log(nextBoard);

            if (nextBoard == null)
            {
                DestroyCells();
                return;
            }

            if (!EqualSize(m_PreviousBoard, nextBoard))
            {
                CreateCells(nextBoard);
            }
            ChangeContents(m_PreviousBoard, nextBoard);
            m_PreviousBoard = nextBoard;
        }

        private bool EqualSize(Board previousBoard, Board nextBoard)
        {
            if (previousBoard == null && nextBoard == null)
                return true;

            return previousBoard != null && nextBoard != null &&
                nextBoard.SizeX == previousBoard.SizeX &&
                nextBoard.SizeY == previousBoard.SizeY;
        }

        private void DestroyCells()
        {
            m_PreviousBoard = null;
            foreach (Cell cell in m_Cells)
                Destroy(cell.gameObject);
            m_Cells = null;
        }

        private void CreateCells(Board board)
        {
            if (m_Cells != null)
                foreach (Cell cell in m_Cells)
                    Destroy(cell.gameObject);

            int numCells = board.SizeX * board.SizeY;
            m_Cells = new Cell[numCells];
            float halfWidth = 0.5f * board.SizeX - 0.5f;
            float halfHeight = 0.5f * board.SizeY - 0.5f;
            Vector3 origin = transform.position + m_Center -
                (m_HorizontalOffset * halfWidth) -
                (m_VerticalOffset * halfHeight);
            for (int cellIndex = 0; cellIndex < numCells; ++cellIndex)
            {
                int x = cellIndex % board.SizeX;
                int y = cellIndex / board.SizeY;
                Vector3 position = origin +
                    (m_HorizontalOffset * x) +
                    (m_VerticalOffset * y);
                Cell cell = Instantiate(m_CellPrefab, position, Quaternion.identity, transform);
                cell.Point = new Point(x, y);
                m_Cells[cellIndex] = cell;
            }
        }

        private void ChangeContents(Board previousBoard, Board nextBoard)
        {
            if (nextBoard == null)
                return;

            if (OnContentChanged == null)
                return;

            bool equalSize = EqualSize(previousBoard, nextBoard);
            foreach (Board.PositionContent cell in nextBoard.AllCells)
            {
                if (equalSize &&
                    previousBoard.GetContentAt(cell.Position) == cell.Content)
                        continue;

                OnContentChanged(cell);
            }
        }
    }
}