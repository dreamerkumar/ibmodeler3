using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.Processor.Test
{
    [TestFixture]
    public class ExtraPaddingTest
    {
        private readonly Color _bkColor = Color.FromArgb(200, 200, 200);
        private readonly string _inputFolderPath = ExecutionDirInfoHelper.GetInputDirPath() + @"\ExtraPadding\";
        private readonly ImageCorners _imageCorners = new ImageCorners { Left = 50, Bottom = 90, Right = 59, Top = 30 };

        [SetUp]
        public void SetUp()
        {
                
        }

        [Test]
        public void TestBlankImage()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "blank.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(0.0, result); 
        }

        [Test]
        public void TestOutsideTopBottom()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "painted_outside_top_bottom.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(0.0, result);
        }

        [Test]
        public void TestWithinTopBottom()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "painted_within_top_bottom.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(500.0, result);
        }

        [Test]
        public void TestDetachedAndOutsideLeftAndRight()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "painted_detached_outside_left_right.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(0.0, result);
        }

        [Test]
        public void TestWithinButNotBeyond()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "within_but_not_beyond.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(0.0, result);
        }

        [Test]
        public void Test20PercentOnLeft()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "20_percent_on_left.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(20.0, result);
        }

        [Test]
        public void Test40PercentOnRight()
        {
            var img = (Bitmap)Image.FromFile(_inputFolderPath + "40_percent_on_right.png");
            var result = ExtraPadding.GetExtraPaddingPercent(img, _imageCorners, _bkColor);
            Assert.AreEqual(40.0, result);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
