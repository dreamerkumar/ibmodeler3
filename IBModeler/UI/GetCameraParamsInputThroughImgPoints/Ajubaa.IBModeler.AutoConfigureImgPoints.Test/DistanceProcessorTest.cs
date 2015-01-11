using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture]
    public class DistanceProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestDistanceProcessorForTestImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedTestImage();

            const string outputFileLeft = @"\TestDistanceProcessorForTestImageLeft.bmp";
            const string outputFileRight = @"\TestDistanceProcessorForTestImageRight.bmp";

            const int pixelsToFeed = 100;

            OutputDistances(backgroundColor, image, pixelsToFeed, outputFileLeft, outputFileRight);
        }

        [Test]
        public void TestDistanceProcessorForMarioImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedFirstMarioImage(backgroundColor);

            const string outputFileLeft = @"\TestDistanceProcessorForMarioImageLeft.bmp";
            const string outputFileRight = @"\TestDistanceProcessorForMarioImageRight.bmp";

            const int pixelsToFeed = 25;

            OutputDistances(backgroundColor, image, pixelsToFeed, outputFileLeft, outputFileRight);
        }

        private static void OutputDistances(Color backgroundColor, Bitmap image, int pixelsToFeed, string outputFileLeft, string outputFileRight)
        {
            var leftDistanceProcessor = new LeftDistanceProcessor(backgroundColor, image, pixelsToFeed);
            var rightDistanceProcessor = new RightDistanceProcessor(backgroundColor, image, pixelsToFeed);

            var outputBitmapLeft = new Bitmap(image.Width*2, image.Height);
            var outputBitmapRight = new Bitmap(image.Width*2, image.Height);
            for(var y = 0; y < image.Height; y++)
            {
                try
                {
                    var leftDistance = leftDistanceProcessor.GetDistance(y);
                    outputBitmapLeft.SetPixel(image.Width + (int)leftDistance, y, Color.Green);

                    var rightDistance = rightDistanceProcessor.GetDistance(y);
                    outputBitmapRight.SetPixel(image.Width + (int)rightDistance, y, Color.Green);
                }
                catch {}
            }
            outputBitmapLeft.Save(ExecutionDirInfoHelper.GetOutputDirPath() + outputFileLeft);
            outputBitmapRight.Save(ExecutionDirInfoHelper.GetOutputDirPath() + outputFileRight);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
