using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgBackgroundStripper.Test
{
    [TestFixture]
    public class ImageStripStatusTest
    {
        private string _sourceDirPath;
        private string _outputDirPath;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _sourceDirPath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputDirPath = ExecutionDirInfoHelper.GetOutputDirPath();
        }

        [Test]
        public void TestStripBackground()
        {
            const string dinosaur = @"\dinosaur.jpg";
            var sourceImagePath = _sourceDirPath + dinosaur;
            var bitmap = (Bitmap)Image.FromFile(sourceImagePath);
            
            var stripStatus = new ImgStripStatus(bitmap.Width, bitmap.Height);

            for (var y = 0; y < bitmap.Height/2; y++)
            {
                for (var x = 0; x < bitmap.Width/2; x++)
                {
                    stripStatus.SetInvalid(x,y);
                }
            }

            var targetPath = _outputDirPath + dinosaur + ".testStripStatus.jpg";
            stripStatus.SaveImgWithInvalidPixels(bitmap, targetPath, Color.FromArgb(1,200,200,200));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _sourceDirPath = null;
            _outputDirPath = null;
        }
    }
}
