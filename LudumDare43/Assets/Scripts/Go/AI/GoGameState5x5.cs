using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FineGameDesign.Go
{
    /// <summary>
    /// Constant throughout play.
    ///
    /// Public variables for speedy access.
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
        private void AppendBoardDiagram(StringBuilder sb)
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
                    string cell = kEmptyCell;
                    for (int playerIndex = 0; playerIndex < 2 && cell == kEmptyCell; ++playerIndex)
                    {
                        List<uint> occupiedMasks = m_GroupOccupiedMasks[playerIndex];
                        foreach (uint occupiedMask in occupiedMasks)
                        {
                            if ((occupiedMask & posMask) == 0)
                            {
                                continue;
                            }

                            cell = playerIndex == 0 ? kBlackCell : kWhiteCell;
                            break;
                        }
                    }

                    sb.Append(cell);
                }
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

            m_EmptyMask = (uint)((1 << (numCells + 1)) - 1);
        }

        /// <summary>
        /// Forbids the move made.
        ///
        /// Also forbids a move by an opponent that would be suicide.
        /// The only possible suicides would be adjacent to the move.
        /// The current move robs a liberty of any neighboring group.
        /// </summary>
        public void Move(uint moveMask)
        {
            Debug.Assert((m_EmptyMask & moveMask) > 0,
                "Expected move was in an empty position. move mask: " + moveMask);
            
            m_EmptyMask &= ~moveMask;

            RemoveLiberties(moveMask);

            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            m_TurnIndex = m_TurnIndex == 0 ? 1 : 0;
            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            ForbidAdjacentEmptySuicides(m_TurnIndex, moveMask);
        }

        public void MoveAtPosition(BoardPosition pos)
        {
            uint moveMask = CoordinateToMask(pos);
            Move(moveMask);
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
        /// TODO: Shares liberties if adjacent to group of equal player.
        /// </summary>
        private void TryForbidSuicideAtIndex(int positionIndex, int turnIndex)
        {
            uint positionMask = (uint)(1 << positionIndex);
            if ((m_EmptyMask & positionMask) == 0)
            {
                return;
            }

            uint positionLibertyMask = CreateLibertyMaskFromIndex(positionIndex);
            if (positionLibertyMask != 0)
            {
                return;
            }

            if (WouldShareAnyLiberty(positionMask, turnIndex))
            {
                return;
            }

            if (WouldCaptureAny(positionMask, turnIndex))
            {
                return;
            }

            m_IllegalMoveMasks[turnIndex] |= positionMask;
        }

        /// <remarks>
        /// Loops through opponent's groups' liberty masks.
        /// If the position equals the liberty mask,
        /// then the opposing group would be captured.
        /// </remarks>
        private bool WouldCaptureAny(uint positionMask, int turnIndex)
        {
            int opponentIndex = turnIndex == 0 ? 1 : 0;
            List<uint> opponentLibertyMasks = m_GroupLibertyMasks[opponentIndex];
            foreach (uint opponentLibertyMask in opponentLibertyMasks)
            {
                if (opponentLibertyMask == positionMask)
                {
                    return true;
                }
            }

            return false;
        }

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
