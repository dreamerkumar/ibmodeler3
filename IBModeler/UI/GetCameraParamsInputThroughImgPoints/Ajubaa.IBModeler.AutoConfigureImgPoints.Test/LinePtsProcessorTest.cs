using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture] 
    public class LinePtsProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestLinePtProcessorsForMarioImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedFirstMarioImage(backgroundColor);
            const string testlineptprocessorsBmp = @"\TestLinePtProcessorsForMarioImage.bmp";
            
            ProcessLineValues(backgroundColor, image, 25);

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + testlineptprocessorsBmp);
        }

        [Test]
        public void TestLinePtProcessorsForTestImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedTestImage();
            const string testlineptprocessorsBmp = @"\TestLinePtProcessorsForTestImage.bmp";

            ProcessLineValues(backgroundColor, image, 100);

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + testlineptprocessorsBmp);
        }

        [Test]
        public void TestLinePtProcessorsForPerfectlyVerticalBase()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedTestImageForPerfectlyVerticalBase(backgroundColor);
            const string testlineptprocessorsBmp = @"\TestLinePtProcessorsForPerfectlyVerticalBase.bmp";

            ProcessLineValues(backgroundColor, image, 100);

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + testlineptprocessorsBmp);
        }

        private static void ProcessLineValues(Color backgroundColor, Bitmap image, int pixelsToFeed)
        {
            var leftEdgeProcessor = new LeftEdgePtsProcessor(image, backgroundColor);
            var rightEdgeProcessor = new RightEdgePtsProcessor(image, backgroundColor);

            var leftPts = new Point[pixelsToFeed];
            var rightPts = new Point[pixelsToFeed];
            for(int y = image.Height-2, ctr = 1; y > 0 && ctr <= pixelsToFeed; y--, ctr++)
            {
                leftPts[ctr-1] = leftEdgeProcessor.GetEdgePt(y);
                rightPts[ctr-1] = rightEdgeProcessor.GetEdgePt(y);
            }

            var leftEdgeLine = new LinePtsProcessor(leftPts);
            var rightEdgeLine = new LinePtsProcessor(rightPts);

            var leftMarkingColor = Color.DarkViolet;
            var rightMarkingColor = Color.Gold;
            for (var y = 0; y < image.Height; y++ )
            {
                var xLeft = leftEdgeLine.GetXValueForY(y);
                if(xLeft >= 0 && xLeft < image.Width)
                {
                    image.SetPixel(xLeft, y, leftMarkingColor);
                }
                var xRight = rightEdgeLine.GetXValueForY(y);
                if(xRight >= 0 && xRight < image.Width)
                {
                    image.SetPixel(xRight, y, rightMarkingColor);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
