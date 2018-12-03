using Go;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FineGameDesign.Go
{
    public sealed class BoardLayout : MonoBehaviour
    {
        public static event Action<Board.PositionContent> OnContentChanged;
        public static event Action<Board.PositionContent> OnTerritoryChanged;

        [SerializeField]
        private Cell m_CellPrefab;

        [SerializeField]
        private Vector3 m_HorizontalOffset;

        [SerializeField]
        private Vector3 m_VerticalOffset;

        [SerializeField]
        private Vector3 m_Center;

        [Header("Logs diagram of board.")]
        [SerializeField]
        private bool m_Verbose;

        private Action<Board> m_OnSet;

        private Cell[] m_Cells;

        private Board m_PreviousBoard;

        private void OnEnable()
        {
            if (m_OnSet == null)
                m_OnSet = SetBoard;
            Referee.OnBoardSet -= m_OnSet;
            Referee.OnBoardSet += m_OnSet;
            if (Referee.InstanceExists() &&
                Referee.instance.Game != null)
                SetBoard(Referee.instance.Game.Board);
        }

        private void OnDisable()
        {
            Referee.OnBoardSet -= m_OnSet;
        }

        private void SetBoard(Board nextBoard)
        {
            if (m_Verbose)
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
            ChangeTerritory(m_PreviousBoard, nextBoard);
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
            if (m_Cells == null)
                return;

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

        private void ChangeTerritory(Board previousBoard, Board nextBoard)
        {
            if (nextBoard == null)
                return;

            if (OnTerritoryChanged == null)
                return;

            if (!nextBoard.IsScoring)
                return;

            nextBoard.UpdateScoring();

            List<Board.PositionContent> territories = nextBoard.AllTerritory;
            foreach (Board.PositionContent cell in territories)
                OnTerritoryChanged(cell);

            var game = Referee.instance.Game;
            FillInCaptures(
                game.BlackCaptures,
                game.WhiteCaptures,
                territories);
        }

        /// <summary>
        /// Signals capture by two events, to distinguish from neutral.
        /// </summary>
        private void FillInCaptures(int blackCaptures, int whiteCaptures,
            List<Board.PositionContent> territories)
        {
            if (whiteCaptures <= 0 && blackCaptures <= 0)
                return;

            foreach (Board.PositionContent cell in territories)
            {
                if (cell.Content == Content.Empty)
                    continue;

                if (cell.Content == Content.White && blackCaptures <= 0)
                    continue;

                if (cell.Content == Content.Black && whiteCaptures <= 0)
                    continue;

                if (cell.Content == Content.White)
                    blackCaptures--;
                else if (cell.Content == Content.Black)
                    whiteCaptures--;

                Board.PositionContent capture;
                capture.Content = Content.Empty;
                capture.Position = cell.Position;
                OnTerritoryChanged(capture);

                if (whiteCaptures <= 0 && blackCaptures <= 0)
                    return;
            }
        }
    }
}
