using System;
using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.Processor.Test
{
    [TestFixture]
    public class ImageRotatorTest
    {
        private readonly Color _bkColor = Color.FromArgb(200, 200, 200);
        private readonly string _inputFolderPath = ExecutionDirInfoHelper.GetInputDirPath() + @"\ImageRotator\";
        private readonly string _outputFolder = ExecutionDirInfoHelper.GetOutputDirPath();

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestClockwiseDir()
        {
            var inputImg = (Bitmap) Image.FromFile(_inputFolderPath + @"\1.png");
            var rotatedBy45 = ImageRotator.RotateImg(inputImg, 45, _bkColor);
            rotatedBy45.Save(_outputFolder + @"\1_rotated_by_45_clockwise.jpg");
        }

        [Test]
        public void TestAntiClockwiseDir()
        {
            var inputImg = (Bitmap)Image.FromFile(_inputFolderPath + @"\1.png");
            var rotatedBy45 = ImageRotator.RotateImg(inputImg, -45, _bkColor);
            rotatedBy45.Save(_outputFolder + @"\1_rotated_by_45_anti_clockwise.jpg");
        }

        [Test]
        public void TestGetRotationAngleToRealignForPositiveAngle()
        {
            const string leftEndLower = @"\disc_first_end_lower.";
            var clickInputs = MainProcessor.GetClickInputsFromFile(_inputFolderPath + leftEndLower + "xml");

            var clickPositions = clickInputs.ImageClickInputDetailsList[0].ClickPositionListForImages;

            var result = ImageRotator.GetRotationAngleToRealign(clickPositions);

            Assert.IsTrue(result > 0);
            Assert.IsTrue(Math.Abs(Math.Abs(result) - 45) < 5);

            var inputImg = (Bitmap)Image.FromFile(_inputFolderPath + leftEndLower + "jpg");
            var rotated = ImageRotator.RotateImg(inputImg, result, _bkColor);
            rotated.Save(_outputFolder + leftEndLower + "rotated.jpg");

        }

        [Test]
        public void TestGetRotationAngleToRealignForNegativeAngle()
        {
            const string secondEndLower = @"\disc_second_end_lower.";
            var clickInputs = MainProcessor.GetClickInputsFromFile(_inputFolderPath + secondEndLower + "xml");

            var clickPositions = clickInputs.ImageClickInputDetailsList[0].ClickPositionListForImages;

            var result = ImageRotator.GetRotationAngleToRealign(clickPositions);

            Assert.IsTrue(result < 0);
            Assert.IsTrue(Math.Abs(Math.Abs(result) - 45) < 5);

            var inputImg = (Bitmap)Image.FromFile(_inputFolderPath + secondEndLower + "jpg");
            var rotated = ImageRotator.RotateImg(inputImg, result, _bkColor);
            rotated.Save(_outputFolder + secondEndLower + "rotated.jpg");
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
