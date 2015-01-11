using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgBackgroundStripper.Test
{
    [TestFixture]
    public class ScreenBasedStripperTest
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
            var backgroundColor = new SerializableColor(200, 200, 200);

            var classicStripperParams = new BackgroundStrippingParams
            {
                ScreenBasedParams = new ScreenBasedParams
                {
                    ScreenColorTypes = ScreenColorTypes.BlueScreen,
                    MaxDiffPercent = 40,
                    MinColorOffset = 40
                },
                BackgroundColor = backgroundColor
            };
            BackgroundStripper.StripBackground(bitmap, classicStripperParams);
            
            var targetPath = _outputDirPath + dinosaur;
            bitmap.Save(targetPath);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _sourceDirPath = null;
            _outputDirPath = null;
        }
    }
}
