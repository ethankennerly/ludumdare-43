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
    }

    /// <summary>
    /// Varies throughout play.
    ///
    /// Public variables for speedy access.
    /// </summary>
    public sealed class GoGameState5x5
    {
        public GoConfig5x5 Config = new GoConfig5x5();

        public uint IllegalMoveMask;

        public uint CoordinateToMask(BoardPosition pos)
        {
            return (uint)((1 << (Config.SizeY * pos.y)) + pos.x);
        }

        public void Move(uint moveMask)
        {
            IllegalMoveMask |= moveMask;
        }
    }
}
