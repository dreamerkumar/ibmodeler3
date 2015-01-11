using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgBackgroundStripper.Test
{
    [TestFixture]
    public class StripsBasedOnImgWithoutModelTest
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
            var sourceImagePath = _sourceDirPath + @"\dinosaur.jpg";
            byte allowedVariationInR = 0;
            byte allowedVariationInG = 0;
            byte allowedVariationInB = 0;
            var invalidColor = Color.FromArgb(1, 200, 200, 200);
            var result = StripsBasedOnImgWithoutModel.StripBackground(sourceImagePath, allowedVariationInR,
                                                                      allowedVariationInG, allowedVariationInB,
                                                                      invalidColor);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _sourceDirPath = null;
            _outputDirPath = null;
        }
    }
}
