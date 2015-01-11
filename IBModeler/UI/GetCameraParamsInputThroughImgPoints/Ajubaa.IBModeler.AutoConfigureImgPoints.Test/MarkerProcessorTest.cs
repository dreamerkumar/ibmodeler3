using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture]
    public class MarkerProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void LeftAndRightMarkerPositionsTest()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedTestImageForPerfectlyVerticalBase(backgroundColor);
            const string outputFile = @"\LeftAndRightMarkerPositionsTest.png";

            var xPositions = MarkerProcessor.GetLeftAndRightMarkerPositions(image, 229, 581, 500, new MarkerProcessingParams());

            var g = Graphics.FromImage(image);

            foreach (var xPosition in xPositions)
            {
                g.DrawLine(Pens.Red, (int)xPosition, 0, (int)xPosition, image.Height - 1);
            }
            g.Dispose();

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + outputFile);
        }

        [Test]
        public void TestMarkerListForStrippedMarioImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedFirstMarioImage(backgroundColor);
            const string outputFile = @"\TestMarkerListForStrippedMarioImage.png";
            const int targetYVal = 20;

            var variations = ColorVariationCalculator.GetColorVariationList(image, 0, image.Width - 1, image.Height - targetYVal);

            var xPositions = MarkerProcessor.GetAllMarkerPositions(variations, new MarkerProcessingParams(), 0);

            var g = Graphics.FromImage(image);
            
            foreach (var xPosition in xPositions)
            {
                g.DrawLine(Pens.Red, (int)xPosition, 0, (int)xPosition, image.Height - 1);
            }
            g.Dispose();

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + outputFile);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
