using System;
using System.Collections.Generic;
using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    [TestFixture]
    public class ColorVariationCalculatorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestColorVariationCalculatorForTestImage()
        {
            var image = TestHelper.GetStrippedTestImage();
            const string outputFile = @"\TestColorVariationCalculatorForTestImage.png";
            const int targetYVal = 100;

            PrintVariations(image, targetYVal, outputFile);
        }

        [Test]
        public void TestColorVariationCalculatorForStrippedMarioImage()
        {
            var backgroundColor = TestHelper.GetBackgroundColor();
            var image = TestHelper.GetStrippedFirstMarioImage(backgroundColor);
            const string outputFile = @"\TestColorVariationCalculatorForStrippedMarioImage.png";
            const int targetYVal = 20;

            PrintVariations(image, targetYVal, outputFile);
        }

        [Test]
        public void TestColorVariationCalculatorForUnStrippedMarioImage()
        {
            
            var filePath = String.Format(@"{0}\mario\IMG_0944.JPG", ExecutionDirInfoHelper.GetInputDirPath());;
            var image = (Bitmap) Image.FromFile(filePath);
            const string outputFile = @"\TestColorVariationCalculatorForUnStrippedMarioImage.png";
            const int targetYVal = 340;

            PrintVariations(image, targetYVal, outputFile);
        }

        [Test]
        public void TestColorVariationFromASinglePtListForMarioImage()
        {

            var filePath = String.Format(@"{0}\mario\IMG_0944.JPG", ExecutionDirInfoHelper.GetInputDirPath()); ;
            var image = (Bitmap)Image.FromFile(filePath);
            const string outputFile = @"\TestColorVariationFromASinglePtListForMarioImage.png";
            const int targetYVal = 340;

            var variations = ColorVariationCalculator.GetColorVariationFromASinglePtList(image, 0, image.Width - 1, image.Height - targetYVal, 0);
            PrintProvidedVariations(image, targetYVal, variations, outputFile);
        }

        private static void PrintVariations(Bitmap image, int targetYVal, string outputFile)
        {
            var variations = ColorVariationCalculator.GetColorVariationList(image, 0, image.Width - 1, image.Height - targetYVal);

            PrintProvidedVariations(image, targetYVal, variations, outputFile);
        }

        private static void PrintProvidedVariations(Image image, int targetYVal, IEnumerable<double> variations, string outputFile)
        {
            var g = Graphics.FromImage(image);
            var x = 0;
            foreach(var variation in variations)
            {
                var y = variation*300;
                g.DrawLine(Pens.Red, x, image.Height - targetYVal, x, image.Height - targetYVal - (int)y);
                x++;
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
