using System.Collections.Generic;
using System.Text;

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
            sb.Append("BoardDiagram: ");
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
            return m_GroupLibertyMasks[m_TurnIndex].Count;
        }

        public uint GetGroupLibertyMask(int groupIndex)
        {
            return m_GroupLibertyMasks[m_TurnIndex][groupIndex];
        }

        public void SetSize(int sizeX, int sizeY)
        {
            Config.SizeX = sizeX;
            Config.SizeY = sizeY;
            Config.NumCells = sizeX * sizeY;

            m_EmptyMask = (uint)((1 << (sizeX + sizeY)) - 1);
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
        /// TODO: Capture permits move at each captured stone,
        /// except suicide or ko.
        ///
        /// TODO: Capture removes captured group.
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
                    uint nextLibertyMask = libertyMask ^ moveMask;
                    libertyMasks[groupIndex] = nextLibertyMask;
                    List<uint> expandingOccupiedMasks = m_GroupOccupiedMasks[playerIndex];

                    if (m_TurnIndex != playerIndex)
                    {
                        if (nextLibertyMask == 0)
                        {
                            m_IllegalMoveMasks[playerIndex] ^= expandingOccupiedMasks[groupIndex];
                            expandingOccupiedMasks.RemoveAt(groupIndex);
                            libertyMasks.RemoveAt(groupIndex);
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
            int SizeX = Config.SizeX;
            int SizeY = Config.SizeY;

            uint libertyMask = (uint)0;

            if (positionIndex >= SizeX)
            {
                libertyMask |= (uint)(1 << (positionIndex - SizeX));
            }

            if (positionIndex < (SizeX * SizeY - SizeX))
            {
                libertyMask |= (uint)(1 << (positionIndex + SizeX));
            }

            if (positionIndex > 0)
            {
                libertyMask |= (uint)(1 << (positionIndex - 1));
            }

            if (positionIndex < (SizeX * SizeY - 1))
            {
                libertyMask |= (uint)(1 << (positionIndex + 1));
            }

            return libertyMask & m_EmptyMask;
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
            int SizeX = Config.SizeX;

            int positionIndex = MaskToIndex(moveMask);
            if (positionIndex >= SizeX)
            {
                TryForbidSuicideAtIndex(positionIndex - SizeX, turnIndex);
            }

            if (positionIndex < (Config.NumCells - SizeX))
            {
                TryForbidSuicideAtIndex(positionIndex + SizeX, turnIndex);
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
        /// </summary>
        /// <remarks>
        /// Loops through opponent's groups' liberty masks.
        /// If the position equals the liberty mask,
        /// then the opposing group would be captured.
        /// </remarks>
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

            int opponentIndex = turnIndex == 0 ? 1 : 0;
            List<uint> opponentLibertyMasks = m_GroupLibertyMasks[opponentIndex];
            foreach (uint opponentLibertyMask in opponentLibertyMasks)
            {
                if (opponentLibertyMask == positionMask)
                {
                    return;
                }
            }

            m_IllegalMoveMasks[turnIndex] |= positionMask;
        }
    }
}
