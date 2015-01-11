using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.Common._3DGeometryHelpers;
using Ajubaa.Common.PolygonDataReaders;
using Ajubaa.Common.PolygonDataWriters;
using Ajubaa.SurfaceSmoother.FullModelSmoother;
using NUnit.Framework;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class TextureProcessorTest
    {
        private string _inputPath;
        private string _outputPath;

        [SetUp]
        public void SetUp()
        {
            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
            _outputPath = ExecutionDirInfoHelper.GetOutputDirPath();
        }

        [Test]
        public void TestWithABlankCube()
        {
            var camPos1 = new Point3D(0.000000, 0.000000, 1239.995361);
            var lookingAtPt1 = new Point3D(0.0, 0.000000, 0.000000);
            var image1 = (Bitmap)Image.FromFile(_inputPath + @"\native_american_front.jpg");
            var frontTexImageInfo = new AddTexImageInfo { CameraLocation = camPos1, ImageBitmap = image1, LookingAt = lookingAtPt1 };

            var camPos2 = new Point3D(0, 0.000000, -1238.818726);
            var lookingAtPt2 = new Point3D(0.0, 0.000000, 0);
            var image2 = (Bitmap)Image.FromFile(_inputPath + @"\native_american_back.jpg");
            var backTexImageInfo = new AddTexImageInfo { CameraLocation = camPos2, ImageBitmap = image2, LookingAt = lookingAtPt2 };

            var cameraRatio = new CameraRatio {XRangeAtInfinity = 2.0, YRangeAtInfinity = 2.0};
            var addTextureInfo = new AddTextureInfo{ CameraRatio = cameraRatio, ImageInfos = new[] { frontTexImageInfo, backTexImageInfo } };

            var points = PolygonsGetter.GetBoxPolygonsAroundAPoint(new Point3D(0, 0, 0), 20);
            var triangles = Triangle.GetTrianglesFromPts(points);
            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            Assert.AreEqual(0, meshGeometryModel.Normals.Count);

            var result = TextureProcessor.GenerateTexture(addTextureInfo, meshGeometryModel, _outputPath + @"\log.txt");

            var textureImageName = _outputPath + @"\" + "native_american.bmp";
            result.Bitmap.Save(textureImageName);
            meshGeometryModel.TextureCoordinates = result.TextureCoordinates;
            XamlWriter.SaveMeshGeometryModel(_outputPath + @"\blank_cube_model_with_native_american_texture.xaml", meshGeometryModel, result.Bitmap);
        }

        [Test]
        public void TestDinosaurModel()
        {
            var camPos1 = new Point3D(0.000000, 0.000000, 40.000000);
            var lookingAtPt1 = new Point3D(0.449719, 0.000000, 0.000000);
            var image1 = (Bitmap) Image.FromFile(_inputPath + @"\dinosaur_front.bmp");
            var frontTexImageInfo = new AddTexImageInfo { CameraLocation = camPos1, ImageBitmap = image1, LookingAt = lookingAtPt1 };

            var camPos2 = new Point3D(9.568636, 0.000000, -38.838657);
            var lookingAtPt2 = new Point3D(-0.436662, 0.000000, -0.107580);
            var image2 = (Bitmap)Image.FromFile(_inputPath + @"\dinosaur_back.bmp");
            var backTexImageInfo = new AddTexImageInfo { CameraLocation = camPos2, ImageBitmap = image2, LookingAt = lookingAtPt2 };

            var cameraRatio = new CameraRatio {XRangeAtInfinity =30.000000 , YRangeAtInfinity = 22.504684};
            var addTextureInfo = new AddTextureInfo { CameraRatio = cameraRatio, ImageInfos = new[] { frontTexImageInfo,  backTexImageInfo}};

            var models = XamlFormatModelReader.GetModelsFromFile(_inputPath + @"\dinosaur_with_normals.xaml");
            var meshGeometryModel = (MeshGeometry3D)models[0].Geometry;

            //make the model smoother
            var currentPositions = meshGeometryModel.Positions;
            var positionNeighbors = PaulBourkeSmoother.GetPositionNeighbors(currentPositions.Count, meshGeometryModel.TriangleIndices);
            for (var ctr = 1; ctr <= 21; ctr++)
            {
                var newPositions = PaulBourkeSmoother.GetSmoothenedPositions(currentPositions, meshGeometryModel.TriangleIndices, positionNeighbors);
                currentPositions = newPositions;
            }
            meshGeometryModel.Positions = currentPositions;

            var result = TextureProcessor.GenerateTexture(addTextureInfo, meshGeometryModel, _outputPath + @"\log.txt");

            var textureImageName = _outputPath + @"\" + "dinosaur_texture.bmp";
            result.Bitmap.Save(textureImageName);
            meshGeometryModel.TextureCoordinates = result.TextureCoordinates;
            var geometryModel3D = new GeometryModel3D
            {
                Geometry = meshGeometryModel,
                Material = new DiffuseMaterial { Brush = new ImageBrush { ImageSource = new BitmapImage(new Uri(textureImageName, UriKind.Relative)), ViewportUnits = BrushMappingMode.Absolute } }
            };

            XamlWriter.SaveGeometryModel3D(_outputPath + @"\ModelWithTexture.xaml", geometryModel3D);
            MdlToXamlConverter.SaveAsGeometryModel3D(_inputPath + @"\dinosaur.mdl", _outputPath + @"\Orig_dinosaur_Model_WithTexture.xaml");
        }

        [Test]
        public void TestNativeAmerican()
        {
            var camPos1 = new Point3D(0.000000, 0.000000, 1239.995361);
            var lookingAtPt1 = new Point3D(-0.810811, 0.000000, 0.000000);
            var image1 = (Bitmap)Image.FromFile(_inputPath + @"\native_american_front.jpg");
            var frontTexImageInfo = new AddTexImageInfo { CameraLocation = camPos1, ImageBitmap = image1, LookingAt = lookingAtPt1 };

            var camPos2 = new Point3D(-54.005825, 0.000000, -1238.818726);
            var lookingAtPt2 = new Point3D(0.810041, 0.000000, -0.035313);
            var image2 = (Bitmap)Image.FromFile(_inputPath + @"\native_american_back.jpg");
            var backTexImageInfo = new AddTexImageInfo { CameraLocation = camPos2, ImageBitmap = image2, LookingAt = lookingAtPt2 };

            var cameraRatio = Te();
            var addTextureInfo = new AddTextureInfo { CameraRatio = cameraRatio, ImageInfos = new[] { frontTexImageInfo, backTexImageInfo } }; 

            var mdlFilePath = _inputPath + @"\native_american.mdl";
            var mdlReader = new MdlFilePolygonDataReader(mdlFilePath);
            var triangles = Triangle.GetTrianglesFromPts(mdlReader.Points);;
            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            Assert.AreEqual(0, meshGeometryModel.Normals.Count);

            //make the model smoother
            var currentPositions = meshGeometryModel.Positions;
            var positionNeighbors = PaulBourkeSmoother.GetPositionNeighbors(currentPositions.Count, meshGeometryModel.TriangleIndices);
            for (var ctr = 1; ctr <= 4; ctr++)
            {
                var newPositions = PaulBourkeSmoother.GetSmoothenedPositions(currentPositions, meshGeometryModel.TriangleIndices, positionNeighbors);
                currentPositions = newPositions;
            }
            meshGeometryModel.Positions = currentPositions;

            var result = TextureProcessor.GenerateTexture(addTextureInfo, meshGeometryModel, _outputPath + @"\log.txt");

            var textureImageName = _outputPath + @"\" + "native_american.bmp";
            result.Bitmap.Save(textureImageName);
            meshGeometryModel.TextureCoordinates = result.TextureCoordinates;
            var geometryModel3D = new GeometryModel3D
            {
                Geometry = meshGeometryModel,
                Material = new DiffuseMaterial { Brush = new ImageBrush { ImageSource = new BitmapImage(new Uri(textureImageName, UriKind.Relative)), ViewportUnits = BrushMappingMode.Absolute } }
            };
            XamlWriter.SaveGeometryModel3D(_outputPath + @"\native_american_ModelWithTexture.xaml", geometryModel3D);
            MdlToXamlConverter.SaveAsGeometryModel3D(mdlFilePath, _outputPath + @"\Orig_native_american_Model_WithTexture.xaml");
        }
        
        
        public CameraRatio Te()
        {

            var cameraInfo = new CAMERA_INFO(0.041580, 2.577963, 0.041580, 2.577963, 0.055411, 2.577963, 0.055411, 2.577963, false);

            double x; //x here stands for an extra variable whose value has to be computed before
			//we get the value of the y ratio
			
			if(cameraInfo.fltHeight1 == cameraInfo.fltHeight2)
				x = 0.0f;
			else
				x = (cameraInfo.fltHeight2 * cameraInfo.fltHtDistance1 - 
					cameraInfo.fltHtDistance2 * cameraInfo.fltHeight1)/ 
					(cameraInfo.fltHeight1 - cameraInfo.fltHeight2);

            var cameraRatio = new CameraRatio
            {
                YRatio = cameraInfo.fltHeight1/(x + cameraInfo.fltHtDistance1)
            };

            //Calculate the x Ratio
            if (cameraInfo.blnWtSameAsHt)
                cameraRatio.XRatio = cameraRatio.YRatio;
            else
            {
                if (cameraInfo.fltWidth1 == cameraInfo.fltWidth2)
                    x = 0.0f;
                else
                    x = (cameraInfo.fltWidth2*cameraInfo.fltWtDistance1 -
                         cameraInfo.fltWtDistance2*cameraInfo.fltWidth1)/
                        (cameraInfo.fltWidth1 - cameraInfo.fltWidth2);

                cameraRatio.XRatio
                    = cameraInfo.fltWidth1/(x + cameraInfo.fltWtDistance1);
            }
            return cameraRatio;

        }


        [TearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }


    public class CAMERA_INFO
    {
        public double fltHeight1;
        public double fltHtDistance1;
        public double fltHeight2;
        public double fltHtDistance2;
        public double fltWidth1;
        public double fltWtDistance1;
        public double fltWidth2;
        public double fltWtDistance2;

        public bool blnWtSameAsHt;

        public CAMERA_INFO(double fltHeight1, double fltHtDistance1, double fltHeight2, double fltHtDistance2, double fltWidth1, double fltWtDistance1, double fltWidth2, double fltWtDistance2, bool blnWtSameAsHt)
        {
            this.fltHeight1 = fltHeight1;
            this.blnWtSameAsHt = blnWtSameAsHt;
            this.fltWtDistance2 = fltWtDistance2;
            this.fltWidth2 = fltWidth2;
            this.fltWtDistance1 = fltWtDistance1;
            this.fltWidth1 = fltWidth1;
            this.fltHtDistance2 = fltHtDistance2;
            this.fltHeight2 = fltHeight2;
            this.fltHtDistance1 = fltHtDistance1;
        }
    }

}

