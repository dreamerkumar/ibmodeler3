using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgBackgroundStripper.Test
{
    [TestFixture]
    public class ComparisonWithImgWithoutModelBasedStripperTest
    {
        private const string Comparisonwithimgwithoutmodel = @"\ComparisonWithImgWithoutModel";
        private string _sourceDirPath;
        private string _outputDirPath;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _sourceDirPath = ExecutionDirInfoHelper.GetInputDirPath() +  Comparisonwithimgwithoutmodel;
            _outputDirPath = ExecutionDirInfoHelper.GetOutputDirPath();
        }

        [Test]
        public void TestStripBackgroundForMini()
        {
            const int allowedVariation = 15;
            const string imageName = "mini.jpg";

            TestStripBackground(allowedVariation, imageName, "base.jpg", imageName);
        }

        [Test]
        public void TestStripBackgroundForTeddy()
        {
            const string imageName = "teddy.jpg";

            for (var allowedVariation = 15; allowedVariation <= 15; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "base1.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe()
        {
            const string imageName = "me.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe1()
        {
            const string imageName = "me1.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me1_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe2()
        {
            const string imageName = "me2.jpg";

            for (var allowedVariation = 20; allowedVariation <= 20; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me2_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe3()
        {
            const string imageName = "me3.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me3_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe4()
        {
            const string imageName = "me4.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me4_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe5()
        {
            const string imageName = "me5.jpg";

            for (var allowedVariation = 20; allowedVariation <= 20; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me5_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForMe6()
        {
            const string imageName = "me6.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "me6_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForNoopur()
        {
            const string imageName = "noopur.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "noopur_background.jpg", allowedVariation + "_" + imageName);
            }
        }


        [Test]
        public void TestStripBackgroundForBooklet()
        {
            const string imageName = "booklet.jpg";

            for (var allowedVariation = 30; allowedVariation <= 30; allowedVariation++)
            {
                TestStripBackground(allowedVariation, imageName, "booklet_background.jpg", allowedVariation + "_" + imageName);
            }
        }

        [Test]
        public void TestStripBackgroundForDoggy()
        {
            const int allowedVariation = 15;
            const string imageName = "doggy.jpg";

            TestStripBackground(allowedVariation, imageName, "base1.jpg", imageName);
        }

        [Test]
        public void TestStripBackgroundForMario()
        {
            const int allowedVariation = 15;
            const string imageName = "mario.jpg";

            TestStripBackground(allowedVariation, imageName, "base1.jpg", imageName);
        }

        [Test]
        public void TestStripBackgroundForToyCatterpillar()
        {
            const int allowedVariation = 15;
            const string imageName = "toy_catterpillar.jpg";

            TestStripBackground(allowedVariation, imageName, "base.jpg", imageName);
        }

        private void TestStripBackground(int allowedVariation, string imageName, string imgWithoutModel, string outputImageName)
        {
            var imgPath = _sourceDirPath + @"\" + imageName;
            var imgWithoutModelPath = _sourceDirPath + @"\" + imgWithoutModel;

            var imgBitmap = (Bitmap) Image.FromFile(imgPath);
            var imgWithoutModelBitmap = (Bitmap) Image.FromFile(imgWithoutModelPath);

            var backgroundColor = Color.FromArgb(1, 200, 200, 200);

            var comparisonWithImgWithoutModelParams = new ComparisonWithImgWithoutModelParams
                                                          {AllowedVariationInRgb = allowedVariation};
            var imgStripStatus = ComparisonWithImgWithoutModelBasedStripper.Process(imgWithoutModelBitmap, imgBitmap,
                                                               comparisonWithImgWithoutModelParams, backgroundColor);

            var targetPath = _outputDirPath + Comparisonwithimgwithoutmodel + "_" + outputImageName;
            imgStripStatus.SaveImgWithInvalidPixels(imgBitmap, targetPath, backgroundColor);

            var blockSizeStripper = new BlockSizeBasedStripper(imgStripStatus, true);
            blockSizeStripper.RemoveAllBlocksExceptForLargest();

            targetPath = targetPath + ".blocksremoved.png";
            imgStripStatus.SaveImgWithInvalidPixels(imgBitmap, targetPath, backgroundColor);

            var blockSizeStripper1 = new BlockSizeBasedStripper(imgStripStatus, false);
            blockSizeStripper1.RemoveAllBlocksExceptForLargest();

            targetPath = targetPath + ".filledupholes.png";
            imgStripStatus.SaveImgWithInvalidPixels(imgBitmap, targetPath, backgroundColor);

        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _sourceDirPath = null;
            _outputDirPath = null;
        }
    }

}
