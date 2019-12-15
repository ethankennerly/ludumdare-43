using FineGameDesign.Go.AI;
using Go;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class BoardLayout5x5 : MonoBehaviour
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

        private Action<Board5x5> m_OnSet;
        private Action<Board5x5> OnSet
        {
            get
            {
                if (m_OnSet == null)
                {
                    m_OnSet = SetBoard;
                }
                return m_OnSet;
            }
        }

        private Cell[] m_Cells;

        private Board5x5 m_PreviousBoard = new Board5x5();

        private void OnEnable()
        {
            Referee5x5.OnBoardSet += OnSet;
        }

        private void OnDisable()
        {
            Referee5x5.OnBoardSet -= OnSet;
        }

        /// <summary>
        /// Copies contents to previous board.
        /// Otherwise change was not detected.
        /// </summary>
        private void SetBoard(Board5x5 nextBoard)
        {
            if (nextBoard == null)
            {
                m_PreviousBoard.AllContents.Clear();
                DestroyCells();
                return;
            }

            if (!EqualSize(m_PreviousBoard, nextBoard))
            {
                CreateCells(nextBoard);
            }
            ChangeContents(m_PreviousBoard, nextBoard);
            m_PreviousBoard.Config = nextBoard.Config;
            m_PreviousBoard.AllContents = new List<Board.PositionContent>(nextBoard.AllContents);
        }

        private bool EqualSize(Board5x5 previousBoard, Board5x5 nextBoard)
        {
            if (previousBoard == null && nextBoard == null)
            {
                return true;
            }

            return previousBoard != null && nextBoard != null &&
                nextBoard.Config != null && previousBoard.Config != null &&
                nextBoard.Config.SizeX == previousBoard.Config.SizeX &&
                nextBoard.Config.SizeY == previousBoard.Config.SizeY;
        }

        private void DestroyCells()
        {
            if (m_Cells == null)
            {
                return;
            }

            foreach (Cell cell in m_Cells)
            {
                Destroy(cell.gameObject);
            }
            m_Cells = null;
        }

        private void CreateCells(Board5x5 board)
        {
            DestroyCells();

            int numCells = board.Config.NumCells;
            int sizeX = board.Config.SizeX;
            int sizeY = board.Config.SizeY;
            m_Cells = new Cell[numCells];
            float halfWidth = 0.5f * sizeX - 0.5f;
            float halfHeight = 0.5f * sizeY - 0.5f;
            Vector3 origin = transform.position + m_Center -
                (m_HorizontalOffset * halfWidth) -
                (m_VerticalOffset * halfHeight);
            for (int cellIndex = 0; cellIndex < numCells; ++cellIndex)
            {
                int x = cellIndex % sizeX;
                int y = cellIndex / sizeY;
                Vector3 position = origin +
                    (m_HorizontalOffset * x) +
                    (m_VerticalOffset * y);
                Cell cell = Instantiate(m_CellPrefab, position, Quaternion.identity, transform);
                cell.Point = new Point(x, y);
                cell.SetSpritesByIndex(cellIndex);
                m_Cells[cellIndex] = cell;
            }
        }

        private void ChangeContents(Board5x5 previousBoard, Board5x5 nextBoard)
        {
            if (nextBoard == null)
            {
                return;
            }

            if (OnContentChanged == null)
            {
                return;
            }

            bool equalSize = EqualSize(previousBoard, nextBoard);
            for (int cellIndex = 0, numCells = nextBoard.Config.NumCells; cellIndex < numCells; ++cellIndex)
            {
                Board.PositionContent cell = nextBoard.AllContents[cellIndex];
                if (equalSize &&
                    previousBoard.AllContents[cellIndex].Content == cell.Content)
                {
                    continue;
                }

                OnContentChanged(cell);
            }
        }
    }
}
