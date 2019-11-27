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

        /// <summary>
        /// Forbids the move made.
        ///
        /// Also forbids a move by an opponent that would be suicide.
        /// The only possible suicides would be adjacent to the move.
        /// The current move robs a liberty of any neighboring group.
        /// </summary>
        /// <remarks>
        /// Processing array of groups:
        /// On a move, find if move is adjacent to a group of the player.
        /// If not, create a new group liberty mask.
        /// For each neighbor of the move, find liberties.
        /// If empty and no liberties and no pieces adjacent, then the move is illegal.
        /// </remarks>
        public void Move(uint moveMask)
        {
            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;

            m_TurnIndex = m_TurnIndex == 0 ? 1 : 0;
            m_IllegalMoveMasks[m_TurnIndex] |= moveMask;
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
