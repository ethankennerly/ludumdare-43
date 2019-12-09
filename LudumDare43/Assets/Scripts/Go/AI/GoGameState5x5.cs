using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Debug = UnityEngine.Debug;

namespace FineGameDesign.Go
{
    /// <summary>
    /// Constant throughout play.
    ///
    /// Public variables for speedy access.
    ///
    /// Traditionally in the game of go, the first player plays the black stones.
    /// The second player plays the white stones.
    /// So descriptors of black and white, are interchangeable with first and second player, respectively.
    /// </summary>
    internal sealed class GoConfig5x5
    {
        internal int SizeX = 5;
        internal int SizeY = 5;

        internal int NumCells;
    }

    public struct BoardPosition
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }

    /// <summary>
    /// Varies throughout play.
    ///
    /// Public variables for speedy access.
    /// </summary>
    public sealed class GoGameState5x5
    {
        private const int kNumPlayers = 2;

        private GoConfig5x5 Config = new GoConfig5x5();

        public uint IllegalMoveMask
        {
            get { return m_IllegalMoveMasks[m_TurnIndex]; }
        }

        private uint[] m_IllegalMoveMasks = new uint[2];

        private uint m_EmptyMask;
        public uint EmptyMask
        {
            get { return m_EmptyMask; }
        }

        private int m_TurnIndex;
        public int TurnIndex
        {
            get { return m_TurnIndex; }
        }

        public int PointsForPlayer1;

        private List<uint>[] m_GroupLibertyMasks = new List<uint>[]
        {
            new List<uint>(16),
            new List<uint>(16)
        };

        private List<uint>[] m_GroupOccupiedMasks = new List<uint>[]
        {
            new List<uint>(16),
            new List<uint>(16)
        };

        public GoGameState5x5 Clone()
        {
            return (GoGameState5x5)MemberwiseClone();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendBoardDiagram(sb);
            return sb.ToString();
        }

        public string Audit()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Audit:\n");
            sb.Append("Board Diagram: \n");
            AppendBoardDiagram(sb);
            sb.Append("\n");
            sb.Append("Illegal Move Masks: ").Append(MaskToBitString(m_IllegalMoveMasks[0]));
            sb.Append(", ").Append(MaskToBitString(m_IllegalMoveMasks[1])).Append("\n");
            sb.Append("Turn Index: ").Append(m_TurnIndex).Append("\n");
            for (int playerIndex = 0, numPlayers = m_GroupLibertyMasks.Length; playerIndex < numPlayers; ++playerIndex)
            {
                sb.Append("Player Index ").Append(playerIndex).Append(":\n");
                List<uint> libertyMasks = m_GroupLibertyMasks[playerIndex];
                List<uint> occupiedMasks = m_GroupOccupiedMasks[playerIndex];
                for (int groupIndex = 0, numGroups = libertyMasks.Count; groupIndex < numGroups; ++groupIndex)
                {
                    sb.Append("Group Liberty Mask: ").Append(MaskToBitString(libertyMasks[groupIndex]));
                    sb.Append(", Group Occupied Mask: ").Append(MaskToBitString(occupiedMasks[groupIndex]));
                    sb.Append("\n");
                }
            }

            if (m_BoardHistory.Count > 0)
            {
                sb.Append("Board History:");
            }
            foreach (UniqueBoard previousBoard in m_BoardHistory)
            {
                sb.Append("\n\n");
                AppendBoardDiagram(sb, previousBoard, mayAppendBitString: true);
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Example:
        /// 3x2 board: mask (2 + 8) --> "010/100".
        /// See tests.
        /// </summary>
        public string MaskToBitString(uint boardMask)
        {
            StringBuilder sb = new StringBuilder();

            int numRows = Config.SizeY;
            int numCols = Config.SizeX;
            BoardPosition pos;
            for (int rowIndex = 0; rowIndex < numRows; ++rowIndex)
            {
                if (rowIndex > 0)
                {
                    sb.Append("/");
                }

                for (int colIndex = 0; colIndex < numCols; ++colIndex)
                {
                    pos.x = colIndex;
                    pos.y = rowIndex;
                    uint posMask = CoordinateToMask(pos);
                    string bit = (boardMask & posMask) == 0 ? "0" : "1";
                    sb.Append(bit);
                }
            }

            return sb.ToString();
        }

        private const string kEmptyCell = ".";
        private const string kBlackCell = "x";
        private const string kWhiteCell = "o";

        /// <summary>
        /// Appends 2D grid of cells.
        /// Conventional cell format:
        ///     .   Empty
        ///     x   Black (first player)
        ///     o   White (second player)
        /// Lower case "o" reduces ambiguity with character "0" (zero).
        /// </summary>
        private void AppendBoardDiagram(StringBuilder sb, bool mayAppendBitString = false)
        {
            int numBoards = m_BoardHistory.Count;
            UniqueBoard currentBoard = numBoards == 0 ? new UniqueBoard() : m_BoardHistory[numBoards - 1];
            AppendBoardDiagram(sb, currentBoard, mayAppendBitString);
        }
        
        /// <summary>
        /// Infers white position as nonempty and nonblack.
        /// </summary>
        private void AppendBoardDiagram(StringBuilder sb, UniqueBoard board, bool mayAppendBitString = false)
        {
            int numRows = Config.SizeY;
            int numCols = Config.SizeX;
            BoardPosition pos;
            for (int rowIndex = 0; rowIndex < numRows; ++rowIndex)
            {
                if (rowIndex > 0)
                {
                    sb.Append("\n");
                }

                for (int colIndex = 0; colIndex < numCols; ++colIndex)
                {
                    pos.x = colIndex;
                    pos.y = rowIndex;
                    uint posMask = CoordinateToMask(pos);
                    bool empty = (board.emptyMask & posMask) > 0;
                    bool black = (board.player0Mask & posMask) > 0;
                    string cell = empty ? kEmptyCell :
                        (black ? kBlackCell : kWhiteCell);
                    sb.Append(cell);
                }
            }

            if (mayAppendBitString)
            {
                AppendBoardBitString(sb, board);
            }
        }

        public int GetNumGroups()
        {
            return GetNumGroupsForPlayer(m_TurnIndex);
        }
        
        public int GetNumGroupsForPlayer(int turnIndex)
        {
            return m_GroupLibertyMasks[turnIndex].Count;
        }

        public uint GetGroupLibertyMask(int groupIndex)
        {
            return m_GroupLibertyMasks[m_TurnIndex][groupIndex];
        }

        public void SetSize(int sizeX, int sizeY)
        {
            Config.SizeX = sizeX;
            Config.SizeY = sizeY;
            int numCells = sizeX * sizeY;
            Config.NumCells = numCells;

            m_EmptyMask = (uint)((1 << numCells) - 1);
        }

        /// <summary>
        /// Forbids the move made.
        ///
        /// Also forbids a move by an opponent that would be suicide.
        /// The only possible suicides would be adjacent to the move.
        /// The current move robs a liberty of any neighboring group.
        ///
        /// If forbidden due to repeating board, recheck if would repeat now.
        /// If group's liberty was taken, reevaluate suicide at each remaining liberty.
        /// TODO: If a group was captured, reevaluate suicide at each liberty the captured group had taken.
        /// TODO: Reevaluates legality at each empty position.
        /// </summary>
        public void Move(uint moveMask)
        {
            Debug.Assert((m_EmptyMask & moveMask) > 0,
                "Expected move was in an empty position. move mask: " + moveMask);
            
            m_EmptyMask &= ~moveMask;

            RemoveLiberties(moveMask);

            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            AddBoardToHistory();

            m_TurnIndex = m_TurnIndex == 0 ? 1 : 0;
            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            RemovePositionsThatNoLongerRepeat(m_TurnIndex);
            ForbidAdjacentEmptySuicides(m_TurnIndex, moveMask);
        }

        public void MoveAtPosition(BoardPosition pos)
        {
            uint moveMask = CoordinateToMask(pos);
            Move(moveMask);
        }

        /// <returns>
        /// 0.5 if each still can move.
        /// Otherwise 0 for player 0 (black) or 1 for player 1 (white)
        /// Player 1 breaks ties.
        /// </returns>
        public float CalculateWinner()
        {
            bool blackCanMove = CanMove(0);
            bool whiteCanMove = CanMove(1);
            if (blackCanMove && whiteCanMove)
            {
                return 0.5f;
            }

            if (PointsForPlayer1 == 0)
            {
                return blackCanMove ? 0f : 1f;
            }

            int movesForPlayer0 = CountBits(CreateLegalMoveMask(0));
            int movesForPlayer1 = CountBits(CreateLegalMoveMask(1));
            movesForPlayer1 += PointsForPlayer1;
            return movesForPlayer0 > movesForPlayer1 ? 0f : 1f;
        }

        public float CalculateResultForPlayer(int turnIndex)
        {
            float result = CalculateWinner();
            if (turnIndex == 1)
            {
                result = 1f - result;
            }

            return result;
        }

        public bool CanMove(int turnIndex)
        {
            return CreateLegalMoveMask(turnIndex) > 0;
        }
        
        public uint CreateLegalMoveMask()
        {
            return CreateLegalMoveMask(m_TurnIndex);
        }

        public uint CreateLegalMoveMask(int turnIndex)
        {
            uint illegalMoveMask = m_IllegalMoveMasks[turnIndex];
            uint legalMoveMask = m_EmptyMask & ~illegalMoveMask;
            return legalMoveMask;
        }

        /// <remarks>
        /// Copied from:
        /// <a href="https://stackoverflow.com/a/12171691/1417849">
        /// Aug 29 '12 at 5:56 Jon Skeet
        /// </a>
        /// </remarks>
        private static int CountBits(uint value)
        {
            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }
            return count;
        }

        /// <summary>
        /// If the move joins a group of the same player,
        /// then also merge liberty mask from adjacents.
        /// </summary>
        /// <remarks>
        /// Processing array of groups:
        /// On a move, find if the move is adjacent to a group of the player.
        /// If not, create a new group liberty mask.
        /// For each neighbor of the move, find liberties.
        /// If empty and no liberties and no pieces adjacent, then the move is illegal.
        /// The empty positions are explored.
        ///
        /// Capture permits move at each captured stone,
        /// except suicide or ko.
        ///
        /// Capture removes captured group.
        /// </remarks>
        public void RemoveLiberties(uint moveMask)
        {
            for (int playerIndex = 0; playerIndex < kNumPlayers; ++playerIndex)
            {
                bool moveEditsGroup = false;
                int groupIndexToMerge = -1;
                List<uint> libertyMasks = m_GroupLibertyMasks[playerIndex];
                for (int groupIndex = libertyMasks.Count - 1; groupIndex >= 0; --groupIndex)
                {
                    uint libertyMask = libertyMasks[groupIndex];
                    if ((libertyMask & moveMask) == 0)
                    {
                        continue;
                    }

                    moveEditsGroup = true;
                    uint nextLibertyMask = libertyMask & (~moveMask);
                    libertyMasks[groupIndex] = nextLibertyMask;
                    List<uint> expandingOccupiedMasks = m_GroupOccupiedMasks[playerIndex];

                    if (m_TurnIndex != playerIndex)
                    {
                        if (nextLibertyMask == 0)
                        {
                            uint capturedGroupMask = expandingOccupiedMasks[groupIndex];
                            int capturedPoints = CountBits(capturedGroupMask);
                            if (m_TurnIndex == 0)
                            {
                                PointsForPlayer1 -= capturedPoints;
                            }
                            else
                            {
                                PointsForPlayer1 += capturedPoints;
                            }

                            // Capturing removes the group.
                            expandingOccupiedMasks.RemoveAt(groupIndex);
                            libertyMasks.RemoveAt(groupIndex);
                            m_EmptyMask |= capturedGroupMask;

                            for (int anyPlayerIndex = 0, numPlayers = m_IllegalMoveMasks.Length;
                                anyPlayerIndex < numPlayers;
                                ++anyPlayerIndex)
                            {
                                m_IllegalMoveMasks[anyPlayerIndex] &= ~capturedGroupMask;
                                List<uint> anyPlayerLibertyMasks = m_GroupLibertyMasks[anyPlayerIndex];

                                // Capturing liberates each captured cell.
                                for (int anyPlayerGroupIndex = anyPlayerLibertyMasks.Count - 1;
                                    anyPlayerGroupIndex >= 0;
                                    --anyPlayerGroupIndex)
                                {
                                    // Capturing only liberates cells adjacent to the group.
                                    for (int capturableIndex = 0; capturableIndex < Config.NumCells; ++capturableIndex)
                                    {
                                        uint capturablePositionMask = (uint)1 << capturableIndex;
                                        if ((capturedGroupMask & capturablePositionMask) == 0)
                                        {
                                            continue;
                                        }
                                        
                                        uint capturedAdjacencyMask = CreateAdjacencyMaskFromIndex(capturableIndex);
                                        uint anyPlayerGroupMask = m_GroupOccupiedMasks[anyPlayerIndex][anyPlayerGroupIndex];
                                        if ((capturedAdjacencyMask & anyPlayerGroupMask) == 0)
                                        {
                                            continue;
                                        }
                                        anyPlayerLibertyMasks[anyPlayerGroupIndex] |= capturablePositionMask;
                                    }
                                }
                            }
                        }

                        continue;
                    }

                    int expandingPositionIndex = MaskToIndex(moveMask);
                    uint expandingLibertyMask = CreateLibertyMaskFromIndex(expandingPositionIndex);
                    libertyMasks[groupIndex] |= expandingLibertyMask;

                    expandingOccupiedMasks[groupIndex] |= moveMask;

                    if (groupIndexToMerge < 0)
                    {
                        groupIndexToMerge = groupIndex;
                        continue;
                    }
                    
                    MergeGroups(playerIndex, groupIndex, groupIndexToMerge);
                    groupIndexToMerge = groupIndex;
                }

                if (moveEditsGroup)
                {
                    continue;
                }

                if (m_TurnIndex != playerIndex)
                {
                    continue;
                }

                int newPositionIndex = MaskToIndex(moveMask);
                uint newLibertyMask = CreateLibertyMaskFromIndex(newPositionIndex);
                libertyMasks.Add(newLibertyMask);
                List<uint> newOccupiedMasks = m_GroupOccupiedMasks[playerIndex];
                newOccupiedMasks.Add(moveMask);
            }
        }

        /// <remarks>
        /// Merges two groups that are joined by this move.
        /// Merges occupied by including either group.
        /// Merges liberties by keeping where empty and removing where occupied.
        /// Removes the later occurring group.
        /// </remarks>
        private void MergeGroups(int playerIndex, int groupIndex, int groupIndexToMerge)
        {
            List<uint> occupiedMasks = m_GroupOccupiedMasks[playerIndex];
            List<uint> libertyMasks = m_GroupLibertyMasks[playerIndex];

            occupiedMasks[groupIndex] |= occupiedMasks[groupIndexToMerge];
            
            libertyMasks[groupIndex] |= occupiedMasks[groupIndexToMerge];
            libertyMasks[groupIndex] &= m_EmptyMask;
            
            occupiedMasks.RemoveAt(groupIndexToMerge);
            libertyMasks.RemoveAt(groupIndexToMerge);
        }

        private const int kMaxBits = 32;
        private const int kNoBits = -1;

        /// <summary>
        /// Expects exactly only one bit.
        /// Equivalent to logarithm.
        /// If no bits, returns -1.
        /// </summary>
        public static int MaskToIndex(uint moveMask)
        {
            for (int index = 0; index < kMaxBits; ++index)
            {
                if ((moveMask & (1 << index)) > 0)
                {
                    return index;
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates a liberty mask around the move.
        /// Only supports one position in the mask.
        /// </summary>
        public uint CreateLibertyMaskFromIndex(int positionIndex)
        {
            uint adjacencyMask = CreateAdjacencyMaskFromIndex(positionIndex);
            return adjacencyMask & m_EmptyMask;
        }

        /// <returns>
        /// TODO: Horizontal is adjacent only if the preceding or subsequent index is in the same row.
        /// </returns>
        public uint CreateAdjacencyMaskFromIndex(int positionIndex)
        {
            int sizeX = Config.SizeX;
            int numCells = Config.NumCells;

            uint adjacencyMask = 0;

            if (positionIndex >= sizeX)
            {
                adjacencyMask |= (uint)(1 << (positionIndex - sizeX));
            }

            if (positionIndex < (numCells - sizeX))
            {
                adjacencyMask |= (uint)(1 << (positionIndex + sizeX));
            }

            if (positionIndex > 0 &&
                (positionIndex % sizeX) > 0)
            {
                adjacencyMask |= (uint)(1 << (positionIndex - 1));
            }

            if (positionIndex < (numCells - 1) &&
                (positionIndex % sizeX) < (sizeX - 1))
            {
                adjacencyMask |= (uint)(1 << (positionIndex + 1));
            }

            return adjacencyMask;
        }

        /// <remarks>
        /// Example with 3x2:
        ///
        ///   x:012
        ///
        ///     000     y:0
        ///     000     y:1
        ///
        ///     000
        ///     001
        ///
        /// 100000
        /// 64
        /// </remarks>
        public uint CoordinateToMask(BoardPosition pos)
        {
            return (uint)(1 << (Config.SizeX * pos.y + pos.x));
        }

        /// <summary>
        /// Adds suicides to illegal move masks.
        /// Suicides would be empty spaces adjacent that have no liberties.
        /// </summary>
        private void ForbidAdjacentEmptySuicides(int turnIndex, uint moveMask)
        {
            int sizeX = Config.SizeX;

            int positionIndex = MaskToIndex(moveMask);
            if (positionIndex >= sizeX)
            {
                TryForbidSuicideAtIndex(positionIndex - sizeX, turnIndex);
            }

            if (positionIndex < (Config.NumCells - sizeX))
            {
                TryForbidSuicideAtIndex(positionIndex + sizeX, turnIndex);
            }

            if (positionIndex > 0)
            {
                TryForbidSuicideAtIndex(positionIndex - 1, turnIndex);
            }

            if (positionIndex < (Config.NumCells - 1))
            {
                TryForbidSuicideAtIndex(positionIndex + 1, turnIndex);
            }
        }

        /// <summary>
        /// If would capture, then permit move.
        ///
        /// Shares liberties if adjacent to group of equal player.
        ///
        /// TODO: If occupied by a stone of the same player,
        /// then also check for suicide at the last liberty of that stone.
        /// </summary>
        private void TryForbidSuicideAtIndex(int positionIndex, int turnIndex)
        {
            uint positionMask = (uint) (1 << positionIndex);
            if ((m_EmptyMask & positionMask) == 0)
            {
                int numBoards = m_BoardHistory.Count;
                UniqueBoard currentBoard = m_BoardHistory[numBoards - 1];
                bool isPlayer0Position = (currentBoard.player0Mask & positionMask) > 0;
                if (isPlayer0Position != (turnIndex == 0))
                {
                    return;
                }

                List<uint> libertyMasks = m_GroupLibertyMasks[turnIndex];
                for (int groupIndex = 0, numMasks = libertyMasks.Count; groupIndex < numMasks; ++groupIndex)
                {
                    uint libertyMask = libertyMasks[groupIndex];
                    int lastLibertyIndex = 0;
                    uint lastLibertyMask = 0;
                    bool exactlyOneLiberty = false;
                    for (; lastLibertyIndex < Config.NumCells; ++lastLibertyIndex)
                    {
                        lastLibertyMask = (uint) (1 << lastLibertyIndex);
                        if ((libertyMask & lastLibertyMask) == 0)
                        {
                            continue;
                        }

                        if (exactlyOneLiberty)
                        {
                            return;
                        }

                        exactlyOneLiberty = true;
                    }

                    if (!exactlyOneLiberty)
                    {
                        continue;
                    }
                    
                    TryForbidPositionMask(lastLibertyIndex, lastLibertyMask, turnIndex);
                    return;
                }

                return;
            }

            TryForbidPositionMask(positionIndex, positionMask, turnIndex);
        }
        
        private void TryForbidPositionMask(int positionIndex, uint positionMask, int turnIndex)
        {
            uint positionLibertyMask = CreateLibertyMaskFromIndex(positionIndex);
            if (positionLibertyMask != 0)
            {
                return;
            }

            if (WouldShareAnyLiberty(positionMask, turnIndex))
            {
                return;
            }

            if (WouldCaptureAny(positionMask, turnIndex, true))
            {
                return;
            }

            m_IllegalMoveMasks[turnIndex] |= positionMask;
        }

        /// <remarks>
        /// Loops through opponent's groups' liberty masks.
        /// If the position equals the liberty mask,
        /// then the opposing group would be captured.
        ///
        /// Side effect: Prevents ko.
        /// Ko is a Japanese word for eternity.
        /// In Go, ko represents repeating a board position.
        /// Practically this occurs when capturing.
        /// If the captured pieces being removed from the board would result in a previous board, then this move is illegal.
        /// An illegal move may not capture.
        /// A naive and robust check for all repeating boards on all potentially legal positions would be more expensive.
        /// </remarks>
        private bool WouldCaptureAny(uint positionMask, int turnIndex, bool rememberRepetition = false)
        {
            int opponentIndex = turnIndex == 0 ? 1 : 0;
            List<uint> opponentLibertyMasks = m_GroupLibertyMasks[opponentIndex];
            for (int groupIndex = 0, numGroups = opponentLibertyMasks.Count; groupIndex < numGroups; ++groupIndex)
            {
                uint opponentLibertyMask = opponentLibertyMasks[groupIndex];
                if (opponentLibertyMask != positionMask)
                {
                    continue;
                }

                uint occupiedMask = m_GroupOccupiedMasks[opponentIndex][groupIndex];
                if (!WouldRepeatBoardAfterCapturing(turnIndex, occupiedMask, positionMask))
                {
                    return true;
                }

                if (rememberRepetition)
                {
                    m_PositionMasksThatWouldRepeat[turnIndex].Add(positionMask);
                }
                return false;
            }

            return false;
        }
        
        #region BoardHistory

        /// <summary>
        /// No hash code defined so slow for equality and dictionary comparison.
        /// A board is unique if the empty cells or player 0 (black) cells are unique.
        /// THe player 1 (white) cells are the non-empty and non-black cells.
        /// </summary>
        private struct UniqueBoard
        {
            public uint emptyMask;
            public uint player0Mask;

            public bool Equals(UniqueBoard other)
            {
                return emptyMask == other.emptyMask &&
                    player0Mask == other.player0Mask;
            }
        }
        
        private List<UniqueBoard> m_BoardHistory = new List<UniqueBoard>(16);

        private List<uint>[] m_PositionMasksThatWouldRepeat = new List<uint>[]
        {
            new List<uint>(),
            new List<uint>()
        };

        private void AddBoardToHistory()
        {
            UniqueBoard nextBoard = new UniqueBoard();
            nextBoard.emptyMask = m_EmptyMask;
            nextBoard.player0Mask = MergePlayerOccupiedMasks(0);
            m_BoardHistory.Add(nextBoard);
        }

        private uint MergePlayerOccupiedMasks(int turnIndex)
        {
            List<uint> occupiedMasks = m_GroupOccupiedMasks[turnIndex];
            uint mergedMask = 0;
            foreach (uint occupiedMask in occupiedMasks)
            {
                mergedMask |= occupiedMask;
            }

            return mergedMask;
        }

        /// <summary>
        /// Removes the stones from the current board
        /// and checks if that next board equals any previous board.
        /// </summary>
        private bool WouldRepeatBoardAfterCapturing(int capturerTurnIndex, uint prisonerMask, uint capturerMask)
        {
            int numBoards = m_BoardHistory.Count;
            if (numBoards == 0)
            {
                return false;
            }
            
            UniqueBoard currentBoard = m_BoardHistory[numBoards - 1];
            UniqueBoard nextBoard;
            uint noncapturerMask = ~capturerMask;
            nextBoard.emptyMask = (currentBoard.emptyMask | prisonerMask) & noncapturerMask;
            nextBoard.player0Mask = currentBoard.player0Mask;
            if (capturerTurnIndex == 0)
            {
                nextBoard.player0Mask |= capturerMask;
            }
            else
            {
                uint survivorMask = ~prisonerMask;
                nextBoard.player0Mask &= survivorMask;
                nextBoard.player0Mask &= noncapturerMask;
            }

            LogBoard(nextBoard,
                "WouldRepeatBoardAfterCapturing: " + MaskToBitString(prisonerMask) + ":" +
                " on capturer's turn: " + capturerTurnIndex + ", number of boards: " + numBoards);
            
            for (int boardIndex = numBoards - 2; boardIndex >= 0; --boardIndex)
            {
                UniqueBoard previousBoard = m_BoardHistory[boardIndex];
                if (previousBoard.Equals(nextBoard))
                {
                    Log("WouldRepeatBoardAfterCapturing: true");
                    return true;
                }
            }

            return false;
        }

        private void RemovePositionsThatNoLongerRepeat(int turnIndex)
        {
            List<uint> positionMasksThatWouldRepeat = m_PositionMasksThatWouldRepeat[turnIndex];
            int numRepetitions = positionMasksThatWouldRepeat.Count;
            if (numRepetitions == 0)
            {
                return;
            }

            for (int repetitionIndex = numRepetitions - 1; repetitionIndex >= 0; --repetitionIndex)
            {
                uint positionMaskThatWouldRepeat = positionMasksThatWouldRepeat[repetitionIndex];
                if (!WouldCaptureAny(positionMaskThatWouldRepeat, turnIndex, false))
                {
                    continue;
                }
                positionMasksThatWouldRepeat.RemoveAt(repetitionIndex);
                m_IllegalMoveMasks[turnIndex] &= ~positionMaskThatWouldRepeat;
            }
        }

        [Conditional("LOG_GO_GAME_STATE")]
        private void AppendBoardBitString(StringBuilder sb, UniqueBoard board)
        {
            sb.Append("\nEmpty Mask: ");
            sb.Append(MaskToBitString(board.emptyMask));
            sb.Append(": ");
            sb.Append(board.emptyMask);
            sb.Append("\nBlack Mask: ");
            sb.Append(MaskToBitString(board.player0Mask));
            sb.Append(": ");
            sb.Append(board.player0Mask);
        }

        [Conditional("LOG_GO_GAME_STATE")]
        private void LogBoard(UniqueBoard board, string prefix)
        {
            StringBuilder sb = new StringBuilder();
            AppendBoardDiagram(sb, board, mayAppendBitString: true);
            string boardDiagram = sb.ToString();
            string currentDiagram = ToString();
            Debug.Log(prefix + "\n" + boardDiagram +
                "\n    compared to current diagram:\n" + currentDiagram);
        }
        
        [Conditional("LOG_GO_GAME_STATE")]
        private void Log(string message)
        {
            Debug.Log(message);
        }
        
        #endregion BoardHistory

        /// <returns>
        /// If any group's liberty masks of the player overlap position,
        /// and the liberty mask minus the position still has another liberty.
        /// </returns>
        private bool WouldShareAnyLiberty(uint positionMask, int turnIndex)
        {
            List<uint> libertyMasks = m_GroupLibertyMasks[turnIndex];
            foreach (uint libertyMask in libertyMasks)
            {
                if ((libertyMask & positionMask) == 0)
                {
                    continue;
                }

                if (libertyMask == positionMask)
                {
                    // This position would be suicide.
                    // Seems like this position is already known to be illegal.
                    continue;
                }

                return true;
            }
            
            return false;
        }
    }
}
