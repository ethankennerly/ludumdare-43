using System.Collections.Generic;

namespace FineGameDesign.Go
{
    /// <summary>
    /// Constant throughout play.
    ///
    /// Public variables for speedy access.
    /// </summary>
    public sealed class GoConfig5x5
    {
        public int SizeX = 5;
        public int SizeY = 5;
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

        public GoConfig5x5 Config = new GoConfig5x5();

        public uint IllegalMoveMask
        {
            get { return m_IllegalMoveMasks[m_TurnIndex]; }
        }

        private uint[] m_IllegalMoveMasks = new uint[2];
        private int m_TurnIndex;

        private List<uint>[] m_GroupLibertyMasks = new List<uint>[]
        {
            new List<uint>(16),
            new List<uint>(16)
        };

        public int GetNumGroups()
        {
            return m_GroupLibertyMasks[m_TurnIndex].Count;
        }

        public uint GetGroupLibertyMask(int groupIndex)
        {
            return m_GroupLibertyMasks[m_TurnIndex][groupIndex];
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
            RemoveLiberties(moveMask);

            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            m_TurnIndex = m_TurnIndex == 0 ? 1 : 0;
            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;
        }

        /// <remarks>
        /// Processing array of groups:
        /// On a move, find if the move is adjacent to a group of the player.
        /// If not, create a new group liberty mask.
        /// For each neighbor of the move, find liberties.
        /// If empty and no liberties and no pieces adjacent, then the move is illegal.
        /// The empty positions are explored.
        /// </remarks>
        public void RemoveLiberties(uint moveMask)
        {
            for (int playerIndex = 0; playerIndex < kNumPlayers; ++playerIndex)
            {
                bool moveEditsGroup = false;
                List<uint> libertyMasks = m_GroupLibertyMasks[playerIndex];
                for (int groupIndex = 0, numGroups = libertyMasks.Count; groupIndex < numGroups; ++numGroups)
                {
                    uint libertyMask = libertyMasks[groupIndex];
                    if ((libertyMask & moveMask) == 0)
                    {
                        continue;
                    }

                    moveEditsGroup = true;
                    libertyMasks[groupIndex] = libertyMask ^ moveMask;
                }

                if (moveEditsGroup)
                {
                    continue;
                }

                if (m_TurnIndex != playerIndex)
                {
                    continue;
                }

                int positionIndex = MaskToIndex(moveMask);
                uint newLibertyMask = CreateLibertyMaskFromIndex(moveMask);
                libertyMasks.Add(newLibertyMask);
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
        public uint CreateLibertyMaskFromIndex(uint moveMask)
        {
            return (uint)0;
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
    }
}
