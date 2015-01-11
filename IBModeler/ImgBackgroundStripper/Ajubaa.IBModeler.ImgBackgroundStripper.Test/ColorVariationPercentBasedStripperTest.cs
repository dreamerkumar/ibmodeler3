using System;
using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgBackgroundStripper.Test
{
    [TestFixture]
    public class ColorVariationPercentBasedStripperTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestColorVariationPercentBasedStripperWithMarioImage()
        {
            const string inputFileName = @"\mario.jpg";
            const string outputFileName = @"\TestColorVariationPercentBasedStripperWithMarioImage.png";

            RunTest(inputFileName, outputFileName, 11, new Point());
        }

        [Test]
        public void TestColorVariationPercentBasedStripperWithToyPenguinImage()
        {
            const string inputFileName = @"\toypenguin.jpg";
            const string outputFileName = @"\TestColorVariationPercentBasedStripperWithToyPenguinImage.png";

            RunTest(inputFileName, outputFileName, 13, new Point(200,200));
        }

        [Test]
        public void TestColorVariationPercentBasedStripperWithNativeAmericanImage()
        {
            const string inputFileName = @"\nativeamerican.jpg";
            const string outputFileName = @"\TestColorVariationPercentBasedStripperWithNativeAmericanImage.png";

            RunTest(inputFileName, outputFileName, 10, new Point(200, 200));
        }

        [Test]
        public void TestColorVariationPercentBasedStripperWithMyPhotoImage()
        {
            const string inputFileName = @"\MyPhoto.jpg";
            const string outputFileName = @"\TestColorVariationPercentBasedStripperWithMyPhotoImage.png";

            var sourceImagePath = ExecutionDirInfoHelper.GetInputDirPath() + inputFileName;
            var bitmap = (Bitmap)Image.FromFile(sourceImagePath);
            var backgroundColor = new SerializableColor(200, 200, 200);

            var @params = new BackgroundStrippingParams
            {
                BackgroundColor = backgroundColor,
                ColorVariationBasedParams = new ColorVariationBasedParams
                {
                    VariationPercent = 10,
                    CompareColor = GetColor(bitmap.GetPixel(0, 0))
                }
            };
            BackgroundStripper.StripBackground(bitmap, @params);

            var @params1 = new BackgroundStrippingParams
            {
                BackgroundColor = backgroundColor,
                ColorVariationBasedParams = new ColorVariationBasedParams
                {
                    VariationPercent = 5,
                    CompareColor = GetColor(bitmap.GetPixel(626, 1802))
                }
            };
            BackgroundStripper.StripBackground(bitmap, @params1);

            var @params2 = new BackgroundStrippingParams
            {
                BackgroundColor = backgroundColor,
                ColorVariationBasedParams = new ColorVariationBasedParams
                {
                    VariationPercent = 5,
                    CompareColor = GetColor(bitmap.GetPixel(549, 1750))
                }
            };
            BackgroundStripper.StripBackground(bitmap, @params2);
            
            var targetPath = ExecutionDirInfoHelper.GetOutputDirPath() + outputFileName;
            bitmap.Save(targetPath);
        }

        private static SerializableColor GetColor(Color color)
        {
            return new SerializableColor(color.R, color.G, color.B);
        }

        private static void RunTest(string inputFileName, string outputFileName, int variationPercent, Point imgLocationOfBaseColor)
        {
            var sourceImagePath = ExecutionDirInfoHelper.GetInputDirPath() + inputFileName;
            var bitmap = (Bitmap)Image.FromFile(sourceImagePath);
            var backgroundColor = new SerializableColor(200, 200, 200);

            var @params = new BackgroundStrippingParams
            {
                BackgroundColor = backgroundColor,
                ColorVariationBasedParams = new ColorVariationBasedParams
                {
                    VariationPercent = variationPercent,
                    CompareColor = GetColor(bitmap.GetPixel(imgLocationOfBaseColor.X, imgLocationOfBaseColor.Y))
                }
            };

            BackgroundStripper.StripBackground(bitmap, @params);
            
            var targetPath = ExecutionDirInfoHelper.GetOutputDirPath() + outputFileName;
            bitmap.Save(targetPath);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
