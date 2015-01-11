//using Ajubaa.Common;
//using NUnit.Framework;
//using System.Drawing;

//namespace Ajubaa.TextureGenerator.Test
//{
//    [TestFixture]
//    public class TextureImageHandlerTest
//    {
//        private string _inputPath;
//        private string _outputPath;
//        private string _logFilePath;

//        [SetUp]
//        public void SetUp()
//        {
//            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
//            _outputPath = ExecutionDirInfoHelper.GetOutputDirPath();
//            _logFilePath = _outputPath + @"\log.txt";
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            _inputPath = null;
//            _outputPath = null;
//            _logFilePath = null;
//        }

//        [Test]
//        public void TestImageMerging()
//        {
//            var image1 = _inputPath + @"\AcuraNSX203_2.jpg";
//            var image2 = _inputPath + @"\Audi_TT_Roadster.jpg";

//            //var handler = new TextureImageHandler((Bitmap)Image.FromFile(image1), (Bitmap)Image.FromFile(image2), _logFilePath);

//            //var mergedImage = handler.MergedBitmap;
//            //mergedImage.Save(_outputPath + @"\merged_acura_and_audi.bmp");
//        }

//        [Test]
//        public void TestDifferentSizedImageMerging()
//        {
//            var image1 = _inputPath + @"\AcuraNSX203_2.jpg";
//            var image2 = _inputPath + @"\Neptune.jpg";

//            //var handler = new TextureImageHandler((Bitmap)Image.FromFile(image1), (Bitmap)Image.FromFile(image2), _logFilePath);
//            //var mergedImage = handler.MergedBitmap;
//            //mergedImage.Save(_outputPath + @"\merged_acura_and_neptune.bmp");

//            //handler = new TextureImageHandler((Bitmap)Image.FromFile(image2), (Bitmap)Image.FromFile(image1), _logFilePath);
//            //mergedImage = handler.MergedBitmap;
//            //mergedImage.Save(_outputPath + @"\merged_neptune_and_acura.bmp");
//        }

//        [Test]
//        public void GetMergedBitmapTestWithOneImageForStaircaseDisplay()
//        {
//            var inputImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50red.bmp");
//            //provide indices for a staircase output
//            var indices = new MinAndMaxIndices[1,inputImage.Height];
//            for(var ctr = 0; ctr < inputImage.Height; ctr++)
//            {
//                indices[0,ctr] = new MinAndMaxIndices {Min = 0, Max = ctr};
//            }
//            var texImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = inputImage
//            };
//            var input = new[] {texImageInfo};
//            var result = TextureImageHandler.GetMergedBitmap(input, indices, inputImage.Width, inputImage.Height);

//            result.Save(_outputPath + @"\GetMergedBitmapTestWithOneImageForStaircaseDisplay.bmp");
//        }

//        [Test]
//        public void GetMergedBitmapTestWithOneImageForStaircaseDisplayUsingPixelsFromRight()
//        {
//            var inputImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50red.bmp");
//            //provide indices for a staircase output from right
//            var indices = new MinAndMaxIndices[1,inputImage.Height];
//            for (var ctr = 0; ctr < inputImage.Height; ctr++)
//            {
//                indices[0,ctr] = new MinAndMaxIndices { Min = inputImage.Width - ctr - 1, Max = inputImage.Width -1 };
//            }
//            var texImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = inputImage
//            };
//            var input = new[] { texImageInfo };
//            var result = TextureImageHandler.GetMergedBitmap(input, indices, inputImage.Width, inputImage.Height);

//            result.Save(_outputPath + @"\GetMergedBitmapTestWithOneImageForStaircaseDisplayUsingPixelsFromRight.bmp");
//        }

//        [Test]
//        public void GetMergedBitmapTestForStaircaseDisplayForRedOnLeftAndFullGreenOnRight()
//        {
//            var redImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50red.bmp");
//            //provide indices for a staircase output
//            var indices = new MinAndMaxIndices[2,redImage.Height];
//            for (var ctr = 0; ctr < redImage.Height; ctr++)
//            {
//                indices[0,ctr] = new MinAndMaxIndices { Min = 0, Max = ctr };
//            }
//            var redTexImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = redImage
//            };


//            var greenImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50green.bmp");
//            //cover entire width for each image
            
//            for (var i = 0; i < greenImage.Height; i++)
//            {
//                indices[1,i] = new MinAndMaxIndices {Min = 0, Max = greenImage.Width - 1};
//            }

//            var greenTexImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = greenImage
//            };

//            var input = new[] { redTexImageInfo, greenTexImageInfo };
//            var result = TextureImageHandler.GetMergedBitmap(input, indices, 100, redImage.Height);

//            result.Save(_outputPath + @"\GetMergedBitmapTestForStaircaseDisplayForRedOnLeftAndFullGreenOnRight.bmp");
//        }

//        [Test]
//        public void GetMergedBitmapTestForStaircaseDisplayForPartRedOnLeftAndPartGreenOnRight()
//        {
//            var redImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50red.bmp");
//            //provide indices for a staircase output
//            var indices = new MinAndMaxIndices[2,redImage.Height];
//            for (var ctr = 0; ctr < redImage.Height; ctr++)
//            {
//                indices[0,ctr] = new MinAndMaxIndices { Min = 0, Max = ctr };
//            }
//            var redTexImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = redImage
//            };

//            var greenImage = (Bitmap)Image.FromFile(_inputPath + @"\50by50green.bmp");
//            //cover entire width for each image
//            for (var ctr = 0; ctr < greenImage.Height; ctr++)
//            {
//                indices[1,ctr] = new MinAndMaxIndices { Min = greenImage.Width - ctr - 1, Max = greenImage.Width - 1 };
//            }
//            var greenTexImageInfo = new AddTexImageInfo
//            {
//                ImageBitmap = greenImage
//            };

//            var input = new[] { redTexImageInfo, greenTexImageInfo };
//            var result = TextureImageHandler.GetMergedBitmap(input, indices, 100, redImage.Height);

//            result.Save(_outputPath + @"\GetMergedBitmapTestForStaircaseDisplayForPartRedOnLeftAndPartGreenOnRight.bmp");
//        }
//    }
//}
