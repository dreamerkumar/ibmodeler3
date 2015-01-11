using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture] 
    public class EdgePtsProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestEdgePtProcessors()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedFirstMarioImage(backgroundColor);

            var leftEdgeProcessor = new LeftEdgePtsProcessor(image, backgroundColor);
            var rightEdgeProcessor = new RightEdgePtsProcessor(image, backgroundColor);

            var leftMarkingColor = Color.DarkViolet;
            var rightMarkingColor = Color.Gold;
            for(var y = 0; y < image.Height; y++)
            {
                try
                {
                    var leftPt = leftEdgeProcessor.GetEdgePt(y);
                    image.SetPixel(leftPt.X, y, leftMarkingColor);
                }
                catch{}

                try
                {
                    var rightPt = rightEdgeProcessor.GetEdgePt(y);
                    image.SetPixel(rightPt.X, y, rightMarkingColor);
                }
                catch{}
                
            }

            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + @"\TestEdgePtProcessors.bmp");

        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
