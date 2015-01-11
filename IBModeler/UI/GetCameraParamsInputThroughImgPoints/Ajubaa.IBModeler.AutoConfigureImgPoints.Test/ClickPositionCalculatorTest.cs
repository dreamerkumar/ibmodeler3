using System;
using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Processor;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture] 
    public class ClickPositionCalculatorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestClickPositionCalculatorWithPerfectVerticalDisc()
        {
            var filePath = String.Format(@"{0}\TestImage1.png", ExecutionDirInfoHelper.GetInputDirPath());

            const int minPixelsForBaseDisc = 100;
            const string outputFileName = @"\TestClickPositionCalculatorWithPerfectVerticalDisc.png";

            RunTest(minPixelsForBaseDisc, filePath, outputFileName);
        }

        [Test]
        public void TestClickPositionCalculatorWithTestImage()
        {
            var filePath = String.Format(@"{0}\TestImage.png", ExecutionDirInfoHelper.GetInputDirPath());

            const int minPixelsForBaseDisc = 100;
            const string outputFileName = @"\TestClickPositionCalculatorWithTestImage.png";

            RunTest(minPixelsForBaseDisc, filePath, outputFileName);
        }

        [Test]
        public void TestClickPositionCalculatorWithMarioImage()
        {
            var filePath = String.Format(@"{0}\Mario\IMG_0944.JPG", ExecutionDirInfoHelper.GetInputDirPath());

            const int minPixelsForBaseDisc = 25;
            const string outputFileName = @"\TestClickPositionCalculatorWithMarioImage.png";

            RunTest(minPixelsForBaseDisc, filePath, outputFileName);
        }

        private static void RunTest(int minPixelsForBaseDisc, string filePath, string outputFileName)
        {
            var inputParams = new AutoConfigureImgPointsParams
            {
                BackgroundColor = TestHelper.GetBackgroundColor(),
                MinPixelsForBaseDisc = minPixelsForBaseDisc,
                MinPixelsForBaseDiscEndOfEdge = 10,
                ResizeHeight = null,
                ResizeWidth = null,
                BackgroundStrippingParams = TestHelper.GetScreenBasedParamsForFirstMarioImage()
            };

            Bitmap image;
            if (inputParams.ResizeWidth.HasValue && inputParams.ResizeHeight.HasValue)
            {
                image = MainProcessor.ResizeJpg(filePath, null, inputParams.ResizeWidth.Value, inputParams.ResizeHeight.Value);
            }
            else
            {
                image = (Bitmap)Image.FromFile(filePath);
            }
            MainProcessor.StripBackground(image, inputParams.BackgroundStrippingParams);
            
            var calculator = new ClickPositionCalculator(image, inputParams);
            var topLeftEnd = calculator.GetDiscTopLeftEnd();
            Assert.IsNotNull(topLeftEnd);
            var topRightEnd = calculator.GetDiscTopRightEnd();
            Assert.IsNotNull(topRightEnd);

            var g = Graphics.FromImage(image);
            //g.FillEllipse(Brushes.Coral, topLeftEnd.Value.X - 8, topLeftEnd.Value.Y - 8, 16, 16);
            //g.FillEllipse(Brushes.Cyan, topRightEnd.Value.X - 8, topRightEnd.Value.Y - 8, 16, 16);
            g.DrawLine(Pens.White, topLeftEnd.Value.X, topLeftEnd.Value.Y, topRightEnd.Value.X, topRightEnd.Value.Y);

            g.Dispose();
            
            image.Save(ExecutionDirInfoHelper.GetOutputDirPath() + outputFileName);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
