using Ajubaa.Common;
using Ajubaa.Common.PolygonDataWriters;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using NUnit.Framework;

namespace Ajubaa.IBModeler.Processor.Test
{
    [TestFixture]
    public class MainProcessorTest
    {
        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void TestModelCreationForNativeAmericanModel()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "nativeamericanpictures");
            var outputFolder = ExecutionDirInfoHelper.CreateUniqueOutputPath();

            var contract = new CreateModelContract
            {
                VariationIn3DCoordinates = 5.0f,
                //todo: this wont work AllImgProcessedEventArgs1 uses an old class which will fail deserialization
                ClickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\AllImgProcessedEventArgs1.xml"),
                InvalidColor = System.Drawing.Color.FromArgb(1, 200, 200, 200),
                ImageFolder = imageFolder,
                BackgroundStrippingParams = new BackgroundStrippingParams 
                {
                    ScreenBasedParams = new ScreenBasedParams
                    {
                        MaxDiffPercent = 99,
                        MinColorOffset = 25,
                        ScreenColorTypes = ScreenColorTypes.GreenScreen
                    }
                },
                SmoothingIterationCount = 2,
                LogFilePath = outputFolder + @"\Log.txt",
                PtDensity = 250
            };
            var modelMeshAndTexture = MainProcessor.CreateDefaultModel(contract);

            XamlWriter.SaveMeshGeometryModel(outputFolder + @"\model.xaml", modelMeshAndTexture.MeshGeometry, modelMeshAndTexture.TextureBitmap);
        }

        [Test]
        public void TestModelCreationForDinosaur()
        {
            var imageFolder = string.Format(@"{0}\{1}", ExecutionDirInfoHelper.GetInputDirPath(), "dinosaur");
            var outputFolder = ExecutionDirInfoHelper.CreateUniqueOutputPath();

            var contract = new CreateModelContract
            {
                VariationIn3DCoordinates = 5.0f,
                //todo: this wont work AllImgProcessedEventArgs1 uses an old class which will fail deserialization
                ClickInputs = MainProcessor.GetClickInputsFromFile(imageFolder + @"\AllImgProcessedEventArgs.xml"),
                InvalidColor = System.Drawing.Color.FromArgb(1, 200, 200, 200),
                ImageFolder = imageFolder,
                BackgroundStrippingParams = new BackgroundStrippingParams
                {
                    ScreenBasedParams = new ScreenBasedParams
                    {
                        MaxDiffPercent = 99,
                        MinColorOffset = 25,
                        ScreenColorTypes = ScreenColorTypes.BlueScreen
                    }
                },
                SmoothingIterationCount = 2,
                LogFilePath = outputFolder + @"\Log.txt",
                PtDensity = 200
            };
            var modelMeshAndTexture = MainProcessor.CreateDefaultModel(contract);

            XamlWriter.SaveMeshGeometryModel(outputFolder + @"\model.xaml", modelMeshAndTexture.MeshGeometry, modelMeshAndTexture.TextureBitmap);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
