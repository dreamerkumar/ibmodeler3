using System.Drawing;
using Ajubaa.Common.PolygonDataReaders;
using Ajubaa.Common.PolygonDataWriters._3ds;
using NUnit.Framework;

namespace Ajubaa.Common.PolygonDataWriters.Test
{
    [TestFixture]
    public class _3DSWriterTest
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
        public void TestXamlTo3DsConversion()
        {
            var inputModelPath = _inputPath + @"\_3ds\model.xaml";
            var models = XamlFormatModelReader.GetModelsFromFile(inputModelPath);
            _3DSWriter.ExportTo3DS(filePath: _outputPath + @"\output.3ds",
                modelData: models[0],
                textureFileData: Image.FromFile(inputModelPath + ".bmp"));
        }

        [Test]
        public void TestXamlTo3DsConversionForDinosaurModel()
        {
            var inputModelPath = _inputPath + @"\_3ds\dinosaur\model.xaml";
            var models = XamlFormatModelReader.GetModelsFromFile(inputModelPath);
            _3DSWriter.ExportTo3DS(filePath: _outputPath + @"\output.3ds",
                modelData: models[0],
                textureFileData: Image.FromFile(inputModelPath + ".bmp"));
        }

        [Test]
        public void TestXamlTo3DsConversionForNativeAmericanModel()
        {
            var inputModelPath = _inputPath + @"\_3ds\nativeamerican\model.xaml";
            var models = XamlFormatModelReader.GetModelsFromFile(inputModelPath);
            _3DSWriter.ExportTo3DS(filePath: _outputPath + @"\output.3ds",
               modelData: models[0],
               textureFileData: Image.FromFile(inputModelPath + ".bmp"));
        }

        [TearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }

}
