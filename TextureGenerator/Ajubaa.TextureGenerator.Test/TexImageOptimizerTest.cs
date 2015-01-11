using System.Drawing;
using Ajubaa.Common;
using NUnit.Framework;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class TexImageOptimizerTest
    {
        private string _inputPath;
        private string _outputPath;

        [SetUp]
        public void SetUp()
        {
            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputPath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
        }


        [Test]
        public void TestTexImageOptimizer()
        {
            var img = (Bitmap)Image.FromFile(string.Format(@"{0}\model.xaml.bmp", _inputPath));
            TexImageOptimizer.ExtendTextureToAllSidesOfImage(img,5);
            img.Save(string.Format(@"{0}\model.xaml.bmp", _outputPath));
        }

        [TearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }
}
