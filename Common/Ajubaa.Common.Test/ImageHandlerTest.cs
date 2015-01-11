using System.Diagnostics;
using NUnit.Framework;

namespace Ajubaa.Common.Test
{
    [TestFixture]
    public class ImageHandlerTest
    {
        private string _inputPath;
        private string _outputPath;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputPath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
        }
        
        [Test]
        public void TestJpgToBitmapConversionOnLoad()
        {
            var jellyfishjpg = _inputPath + @"\Jellyfish.jpg";
            var imageHandler = new ImageHandler(jellyfishjpg);
            Assert.AreEqual(768, imageHandler.Height);
        }

        [Test]
        public void TestSaveBitmapImageAsBitmap()
        {
            var jellyfishjpg = _inputPath + @"\Jellyfish.jpg";
            var imageHandler = new ImageHandler(jellyfishjpg);
            Assert.AreEqual(768, imageHandler.Height);
            var outputImgPath = _outputPath + @"\jellyfish.bmp";
            imageHandler.SaveBitmap(outputImgPath);
            Process.Start("mspaint", outputImgPath);
        }

        [Test]
        public void TestSaveBitmapImageAsJpg()
        {
            var jellyfishjpg = _inputPath + @"\Jellyfish.jpg";
            var imageHandler = new ImageHandler(jellyfishjpg);
            Assert.AreEqual(768, imageHandler.Height);
            var outputImgPath = _outputPath + @"\jellyfish.jpg";
            imageHandler.SaveBitmap(outputImgPath);
            Process.Start("mspaint", outputImgPath);
        }

        [Test]
        public void TestSaveBitmapImageAsPng()
        {
            var jellyfishjpg = _inputPath + @"\Jellyfish.jpg";
            var imageHandler = new ImageHandler(jellyfishjpg);
            Assert.AreEqual(768, imageHandler.Height);
            var outputImgPath = _outputPath + @"\jellyfish.png";
            imageHandler.SaveBitmap(outputImgPath);
            Process.Start("mspaint", outputImgPath);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }
}
