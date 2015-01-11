using System;
using System.Drawing;
using System.Net.Mime;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.SurfaceSmoother.FullModelSmoother;
using Ajubaa.TextureGenerator;
using NUnit.Framework;
using Ajubaa.Common.PolygonDataWriters;
using Color = System.Windows.Media.Color;

namespace Ajubaa.IBModeler.PtsToPolygons.Test
{
    [TestFixture]
    public class ModelCreationTest
    {
        private string _inputFolderPath;
        private string _outputFolderPath;

        [SetUp]
        public void SetUp()
        {
            _inputFolderPath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputFolderPath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
        }

        [Test]
        public void TestModelCreation()
        {
            const string moldFilename = "nativeamerican100ptmold.mld";
            var moldFilePath = _inputFolderPath + @"\" + moldFilename;
            var createModelInfo = new CreateModelInfo
            {
                FilePath = moldFilePath,
                Minx = 0,
                Maxx = 0,
                Miny = 0,
                Maxy = 0,
                Minz = 0,
                Maxz = 0
            };
            var testobj = new PointsToPolygons(createModelInfo);
            var result = testobj.Process();
            XamlWriter.SaveMeshGeometryModel(_outputFolderPath + @"\nativeamericanfullmodel.xaml", result, Color.FromRgb(100, 100, 100));
            Assert.AreEqual(29516*3, result.TriangleIndices.Count);
        }

        [Test]
        public void TestPartModelCreation()
        {
            const string moldFilename = "nativeamerican100ptmold.mld";
            var moldFilePath = _inputFolderPath + @"\" + moldFilename;
            var createModelInfo = new CreateModelInfo
            {
                FilePath = moldFilePath,
                Minx = 1,
                Maxx = 100,
                Miny = 13,
                Maxy = 100,
                Minz = 1,
                Maxz = 100
            };
            var testobj = new PointsToPolygons(createModelInfo);
            var result = testobj.Process();
            XamlWriter.SaveMeshGeometryModel(_outputFolderPath + @"\nativeamericanpartmodel.xaml", result, Color.FromRgb(100, 100, 100));
        }

        [Test]
        public void TestDinosaurModelCreation()
        {
            const string moldFilename = "dinosaur.mld";
            var moldFilePath = _inputFolderPath + @"\" + moldFilename;
            var createModelInfo = new CreateModelInfo
            {
                FilePath = moldFilePath,
                Minx = 1,
                Maxx = 200,
                Miny = 24,
                Maxy = 200,
                Minz = 1,
                Maxz = 200
            };
            var testobj = new PointsToPolygons(createModelInfo);
            var result = testobj.Process();
            XamlWriter.SaveMeshGeometryModel(_outputFolderPath + @"\dinosaur.xaml", result, Color.FromRgb(100, 100, 100));
            Assert.AreEqual(42850*3, result.TriangleIndices.Count);
        }

        [Test]
        public void TestBatmanModelCreationWithSmootheningAndFrontBackTexture()
        {
            const string moldFilename = "batman.mld";
            var moldFilePath = _inputFolderPath + @"\" + moldFilename;
            var createModelInfo = new CreateModelInfo
            {
                FilePath = moldFilePath,
                Minx = 1,
                Maxx = 296,
                Miny = 109,
                Maxy = 296,
                Minz = 1,
                Maxz = 296
            };
            var testobj = new PointsToPolygons(createModelInfo);
            var meshGeometry3D = testobj.Process();

            Assert.AreEqual(42422 * 3, meshGeometry3D.TriangleIndices.Count);
            
            //smoothen the model five times
            meshGeometry3D.Positions = PaulBourkeSmoother.GetSmoothenedPositions(meshGeometry3D.Positions, meshGeometry3D.TriangleIndices, 5);

            //Add textures
            //SETCAMERAPARAMS_INFINITY  20.000000, 26.661116
            //ADDFRONTBACKTEXTURE "C:\Documents and Settings\Vishal Kumar\Desktop\batman\capture_00003.jpg", 0.000000, 0.000000, 40.000000, -0.549542, 0.000000, 0.000000, 
            //"C:\Documents and Settings\Vishal Kumar\Desktop\batman\capture_00031.jpg", 0.476956, 0.000000, -39.997158, 0.549503, 0.000000, 0.006553, "C:\Documents and Settings\Vishal Kumar\Desktop\batman\IBModelerFiles\modelfull.mdl"

            var camPos1 = new Point3D(0.000000, 0.000000, 40.000);
            var lookingAtPt1 = new Point3D(-0.549542, 0.000000, 0.000000);
            var image1 = (Bitmap)Image.FromFile(_inputFolderPath + @"\batmanfront.jpg");
            var frontTexImageInfo = new AddTexImageInfo { CameraLocation = camPos1, ImageBitmap = image1, LookingAt = lookingAtPt1 };

            var camPos2 = new Point3D(0.476956, 0.000000, -39.997158);
            var lookingAtPt2 = new Point3D(0.549503, 0.000000, 0.006553);
            var image2 = (Bitmap)Image.FromFile(_inputFolderPath + @"\batmanback.jpg");
            var backTexImageInfo = new AddTexImageInfo { CameraLocation = camPos2, ImageBitmap = image2, LookingAt = lookingAtPt2 };

            var cameraRatio = new CameraRatio
            {
                XRangeAtInfinity = 26.661116,
                YRangeAtInfinity = 20.000000
            };

            var addTextureInfo = new AddTextureInfo { BackImageInfo = backTexImageInfo, CameraRatio = cameraRatio, FrontImageInfo = frontTexImageInfo };
            var result = TextureProcessor.GetModelWithFrontAndBackTexture(addTextureInfo, meshGeometry3D);

            var textureImageName = _outputFolderPath + @"\" + "batman.bmp";
            result.BitmapTextureImg.Save(textureImageName);
            var geometryModel3D = new GeometryModel3D
            {
                Geometry = result.MeshGeometry,
                Material = new DiffuseMaterial { Brush = new ImageBrush { ImageSource = new BitmapImage(new Uri(textureImageName, UriKind.Relative)), ViewportUnits = BrushMappingMode.Absolute } }
            };

            XamlWriter.SaveGeometryModel3D(_outputFolderPath + @"\batman.xaml", geometryModel3D);
        }
        [TearDown]
        public void TearDown()
        {
            
        }
    }
}
