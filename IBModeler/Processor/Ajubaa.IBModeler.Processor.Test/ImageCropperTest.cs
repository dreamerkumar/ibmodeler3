using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.Processor.Test
{
    [TestFixture]
    public class ImageCropperTest
    {
        private Bitmap _image;
        private readonly Color _bkColor = Color.FromArgb(200, 200, 200);
        private readonly string _outputFolder = ExecutionDirInfoHelper.GetOutputDirPath();

        [TestFixtureSetUp]
        public void SetUp()
        {
        }

        [Test]
        public void TestWithNoExtraWidth()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "testcropping_orig");

            var clickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\ClickInputs.xml");

            var imagePath = string.Format(@"{0}\{1}", imageFolder, clickInputs.ImageClickInputDetailsList[0].ImageName);
            _image = (Bitmap)Image.FromFile(imagePath);

            var imageAlterationParams = new ImageAlterationParams
            {
                MinImageHeightRatio = 0.7,
                PercentExtraWidth = 0,
                InvalidColor = _bkColor,
                BottomPaddingPercent = 0
            };
            var croppedImg = ImageCropper.GetCroppedImage(clickInputs.ImageClickInputDetailsList[0], _image, imageAlterationParams);
            
            var outputPath = string.Format(@"{0}\cropped.jpg", _outputFolder);
            croppedImg.Save(outputPath);
        }

        [Test]
        public void TestWithExtraWidthThatCanBeAccomodated()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "testcropping_orig");

            var clickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\ClickInputs.xml");

            var imagePath = string.Format(@"{0}\{1}", imageFolder, clickInputs.ImageClickInputDetailsList[0].ImageName);
            _image = (Bitmap)Image.FromFile(imagePath);

            var imageAlterationParams = new ImageAlterationParams
            {
                MinImageHeightRatio = 0.7,
                PercentExtraWidth = 2,
                InvalidColor = _bkColor,
                BottomPaddingPercent = 0
            };
            var croppedImg = ImageCropper.GetCroppedImage(clickInputs.ImageClickInputDetailsList[0], _image, imageAlterationParams);
            
            var outputPath = string.Format(@"{0}\cropped_with_allowed_extra.jpg", _outputFolder);
            croppedImg.Save(outputPath);
        }

        [Test]
        public void TestWithExtraWidthThatIsWayBeyond()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "testcropping_orig");

            var clickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\ClickInputs.xml");

            var imagePath = string.Format(@"{0}\{1}", imageFolder, clickInputs.ImageClickInputDetailsList[0].ImageName);
            _image = (Bitmap)Image.FromFile(imagePath);

            var imageAlterationParams = new ImageAlterationParams
            {
                MinImageHeightRatio = 0.7,
                PercentExtraWidth = 100,
                InvalidColor = _bkColor,
                BottomPaddingPercent = 0
            };
            var croppedImg = ImageCropper.GetCroppedImage(clickInputs.ImageClickInputDetailsList[0], _image, imageAlterationParams);

            var outputPath = string.Format(@"{0}\cropped_with_allowed_extra_on_both_sides.jpg", _outputFolder);
            croppedImg.Save(outputPath);
        }

        [Test]
        public void TestWithExtraWidthThatMakesLeftNegative()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "testcropping_disc_very_left_of_image");

            var clickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\ClickInputs.xml");

            var imagePath = string.Format(@"{0}\{1}", imageFolder, clickInputs.ImageClickInputDetailsList[0].ImageName);
            _image = (Bitmap)Image.FromFile(imagePath);

            var imageAlterationParams = new ImageAlterationParams
            {
                MinImageHeightRatio = 0.7,
                PercentExtraWidth = 10,
                InvalidColor = _bkColor,
                BottomPaddingPercent = 0
            };
            var croppedImg = ImageCropper.GetCroppedImage(clickInputs.ImageClickInputDetailsList[0], _image, imageAlterationParams);

            var outputPath = string.Format(@"{0}\cropped_with_extra_beyond_left.jpg", _outputFolder);
            croppedImg.Save(outputPath);
        }

        [Test]
        public void TestWithExtraWidthThatMakesLeftPositiveButNoSufficientSpaceOnRight()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "testcropping_disc_very_right_of_image");

            var clickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\ClickInputs.xml");

            var imagePath = string.Format(@"{0}\{1}", imageFolder, clickInputs.ImageClickInputDetailsList[0].ImageName);
            _image = (Bitmap)Image.FromFile(imagePath);

            var imageAlterationParams = new ImageAlterationParams
            {
                MinImageHeightRatio = 0.7,
                PercentExtraWidth = 10,
                InvalidColor = _bkColor,
                BottomPaddingPercent = 0
            };
            var croppedImg = ImageCropper.GetCroppedImage(clickInputs.ImageClickInputDetailsList[0], _image, imageAlterationParams);

            var outputPath = string.Format(@"{0}\cropped_with_extra_beyond_right.jpg", _outputFolder);
            croppedImg.Save(outputPath);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _image = null;
        }
    }
}
