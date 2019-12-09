using FineGameDesign.Go.AI;
using NUnit.Framework;
using System.Diagnostics;
using UnityEditor;

using Debug = UnityEngine.Debug;

namespace FineGameDesign.Go.UnitTests
{
    public sealed class GoSearcher5x5Tests
    {
        [MenuItem("Tools/Go/Make Move On 3x1 For Profiling")]
        public static void MakeMoveOn3x1ForProfiling()
        {
            var tester = new GoSearcher5x5Tests();
            tester.MakeMoveOn3x1();
        }

        [MenuItem("Tools/Go/Make Move On 3x3 For Profiling")]
        public static void MakeMoveOn3x3ForProfiling()
        {
            var tester = new GoSearcher5x5Tests();
            tester.MakeMoveOn3x3();
        }

        private static void AssertTinyAndOdd(int size, string dimension)
        {
            Assert.IsTrue(size == 1 || size == 3 || size == 5,
                "Only small odd size is simple to calculate optimal move in center. " +
                "size" + dimension + "=" + size
            );
        }

        public void MakeMove(int sizeX, int sizeY)
        {
            AssertTinyAndOdd(sizeX, "X");
            AssertTinyAndOdd(sizeY, "Y");

            Stopwatch timePerMove = new Stopwatch();

            GoSearcher5x5 searcher = new GoSearcher5x5();
            searcher.Game.SetSize(sizeX, sizeY);

            timePerMove.Start();
            searcher.MakeMove();
            timePerMove.Stop();
            long millisecondsPerMove = timePerMove.ElapsedMilliseconds;

            uint centerMask = searcher.Game.CoordinateToMask(
                new BoardPosition(){x = sizeX / 2, y = sizeY / 2}
            );
            Assert.AreEqual(searcher.Game.MaskToBitString(centerMask), 
                searcher.Game.MaskToBitString(~searcher.Game.EmptyMask),
                searcher.Game.Audit());

            Debug.Log("MakeMoveOn" + sizeX + "x" + sizeY + ": " + millisecondsPerMove + "ms");
        }

        [Test]
        public void MakeMoveOn3x1()
        {
            MakeMove(3, 1);
        }

        [Test]
        public void MakeMoveOn3x3()
        {
            MakeMove(3, 3);
        }

        [Test]
        public void MakeMoveOn5x3()
        {
            MakeMove(5, 3);
        }

        [Test]
        public void MakeMoveOn5x5()
        {
            MakeMove(5, 5);
        }
    }
}
