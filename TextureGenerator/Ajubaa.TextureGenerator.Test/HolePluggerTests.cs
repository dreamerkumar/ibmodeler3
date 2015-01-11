using System.Collections.Generic;
using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;
using Point = System.Windows.Point;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class HolePluggerTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void TestStraightLine()
        {
            const string outputFileName = "TestStraightLine";

            var points = new List<Point>
                             {
                new Point(50, 0),
                new Point(50, 10),
                new Point(50, 20),
                new Point(50, 30),
                new Point(50, 40),
                new Point(50, 50),
                new Point(50, 60),
                new Point(50, 70),
                new Point(50, 80),
                new Point(50, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestOneDip()
        {
            const string outputFileName = "TestOneDip";

            var points = new List<Point>
                             {
                new Point(50, 0),
                new Point(25, 40),
                new Point(50, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestTwoDips()
        {
            const string outputFileName = "TestTwoDips";

            var points = new List<Point>
            {
                new Point(50, 0),
                new Point(50, 10),
                new Point(25, 20),
                new Point(50, 30),
                new Point(50, 40),
                new Point(50, 50),
                new Point(50, 60),
                new Point(35, 70),
                new Point(50, 80),
                new Point(50, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestDoubleDip()
        {
            const string outputFileName = "TestDoubleDip";

            var points = new List<Point>
                             {
                new Point(50, 0),
                new Point(50, 10),
                new Point(25, 20),
                new Point(40, 30),
                new Point(25, 40),
                new Point(50, 50),
                new Point(50, 60),
                new Point(50, 70),
                new Point(50, 80),
                new Point(50, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestDoubleDipOnASlant()
        {
            const string outputFileName = "TestDoubleDipOnASlant";

            var points = new List<Point>
            {
                new Point(50, 0),
                new Point(60, 10),
                new Point(35, 20),
                new Point(50, 30),
                new Point(35, 40),
                new Point(60, 50),
                new Point(70, 60),
                new Point(80, 70),
                new Point(90, 80),
                new Point(99, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestDoubleDipOnASlantWithUnfinishedHole()
        {
            const string outputFileName = "TestDoubleDipOnASlantWithUnfinishedHole";

            var points = new List<Point>
            {
                new Point(50, 0),
                new Point(60, 10),
                new Point(35, 20),
                new Point(50, 30),
                new Point(35, 40),
                new Point(60, 50),
                new Point(70, 60),
                new Point(40, 70),
                new Point(40, 80),
                new Point(30, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 50);
        }

        [Test]
        public void TestHoleHeightsCheck()
        {
            const string outputFileName = "TestHoleHeightsCheck";

            var points = new List<Point>
            {
                new Point(50, 0),
                new Point(60, 10),
                new Point(35, 20),
                new Point(50, 30),
                new Point(35, 40),
                new Point(60, 50),
                new Point(70, 60),
                new Point(80, 70),
                new Point(90, 80),
                new Point(99, 90),
            };

            RunTestAndSaveResults(points, outputFileName, 30);
        }

        [Test]
        public void TestModifyMaxXValuesBasedOnNewPoints()
        {
            const string outputFileName = "TestModifyMaxXValuesBasedOnNewPoints";

            var points = new List<Point>
            {
                new Point(50, 0),
                new Point(60, 10),
                new Point(35, 20),
                new Point(50, 30),
                new Point(35, 40),
                new Point(60, 50),
                new Point(70, 60),
                new Point(80, 70),
                new Point(90, 80),
                new Point(99, 90),
            };

            var bitmap = GetWhiteBitmap();
            DrawOutPoints(bitmap, Color.Green, points);

            HolePlugger.PlugHolesRepeatedly(points, 30, MinXOrMaxXEnum.MaxX);

            var minMaxXIndices = new MinAndMaxIndices[89];
            //if index values are not initialized, everything will be bypassed
            for (var yIndex = 0; yIndex < minMaxXIndices.Length; yIndex++)
            {
                minMaxXIndices[yIndex] = new MinAndMaxIndices {Max = 1, InitializedToZeroZero = false};
            }

            HolePlugger.ModifyXValuesBasedOnNewPoints(minMaxXIndices, points, MinXOrMaxXEnum.MaxX);

            DrawOutIndices(bitmap, minMaxXIndices, Color.Red);
            SaveResults(bitmap, outputFileName); 
   
        }

        private static void DrawOutIndices(Bitmap bitmap, MinAndMaxIndices[] minMaxXIndices, Color color)
        {
            using (var g = Graphics.FromImage(bitmap))
            {
                var sourcePen = new Pen(color);
                for (var index = 0; index < minMaxXIndices.Length - 1; index++)
                {
                    var p1Draw = new System.Drawing.Point(minMaxXIndices[index].Max, index);
                    var p2Draw = new System.Drawing.Point(minMaxXIndices[index + 1].Max, (index + 1));
                    g.DrawLine(sourcePen, p1Draw, p2Draw);
                }
            }
        }

        private static void RunTestAndSaveResults(List<Point> points, string outputFileName, double allowedHoleHeight)
        {
            var bitmap = GetWhiteBitmap();
            DrawOutPoints(bitmap, Color.Green, points);

            HolePlugger.PlugHolesRepeatedly(points, allowedHoleHeight, MinXOrMaxXEnum.MaxX);

            DrawOutPoints(bitmap, Color.Red, points);
            SaveResults(bitmap, outputFileName);
        }

        private static void SaveResults(Image bitmap, string outputFileName)
        {
            var outputPath = ExecutionDirInfoHelper.GetOutputDirPath();
            var outputFilePath = string.Format(@"{0}\HolePluggerTests_{1}.png", outputPath, outputFileName);
            bitmap.Save(outputFilePath);
        }

        private static Bitmap GetWhiteBitmap()
        {
            var bitmap = new Bitmap(100, 100);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
            }
            return bitmap;
        }

        private static void DrawOutPoints(Image bitmap, Color color, IList<Point> points)
        {
            using (var g = Graphics.FromImage(bitmap))
            {
                var sourcePen = new Pen(color);
                for (var index = 0; index < points.Count - 1; index++)
                {
                    var p1 = points[index];
                    var p2 = points[index + 1];
                    var p1Draw = new System.Drawing.Point((int)p1.X, (int)p1.Y);
                    var p2Draw = new System.Drawing.Point((int)p2.X, (int)p2.Y);
                    g.DrawLine(sourcePen, p1Draw, p2Draw);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
