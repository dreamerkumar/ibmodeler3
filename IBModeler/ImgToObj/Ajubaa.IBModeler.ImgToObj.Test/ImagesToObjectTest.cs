using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;
using NUnit.Framework;

namespace Ajubaa.IBModeler.ImgToObj.Test
{
    [TestFixture]
    public class ImagesToObjectTest
    {
        private string _inputFilePath;
        private string _outputFilePath;

        [SetUp]
        public void SetUp()
        {
            _inputFilePath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputFilePath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
        }

        //[Test]
        //public void TestCreateMoldFile()
        //{
        //    var inputFilePath = _outputFilePath + @"\processingmold.mld";
        //    MoldFileCreator.CreateNewMoldFile(inputFilePath, 100);

        //    var imageToObj = new ImagesToObject(new ProcessMoldParams
        //    {
        //        cameraRatio = new CameraRatio { xRangeAtInfinity = 20, yRangeAtInfinity = 20, xRatio = 0, yRatio = 0},
        //        g_strInputFile = inputFilePath,
        //        imgParams = new ImageParams
        //        {
        //            cameraLocation = new Point3D(0,0,20),
        //            lookingAt = new Point3D(0,0,0),
        //            InvalidColor = System.Drawing.Color.FromArgb(200, 200, 200),
        //            mblnCameraAtInfinity = true,
        //            mstrImageFilePath = _inputFilePath + @"\v.bmp"
        //        }
        //    },
        //    _outputFilePath + @"\log.txt");
        //    Assert.IsTrue(imageToObj.ProcessImage());
        //}

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
