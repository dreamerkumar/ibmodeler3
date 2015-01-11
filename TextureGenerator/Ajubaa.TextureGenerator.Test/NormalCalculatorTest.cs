using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.Common._3DGeometryHelpers;
using Ajubaa.Common.PolygonDataReaders;
using Ajubaa.Common.PolygonDataWriters;
using Ajubaa.SurfaceSmoother.FullModelSmoother;
using NUnit.Framework;
using System.Collections.Generic;

namespace Ajubaa.TextureGenerator.Test
{
    [TestFixture]
    public class NormalCalculatorTest
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
        public void TestNormals()
        {
            var mdlReader = new MdlFilePolygonDataReader(_inputPath + @"\flowerpetals.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlReader.Points);
            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            Assert.AreEqual(0, meshGeometryModel.Normals.Count);
            XamlWriter.SaveMeshGeometryModel(_outputPath + @"\ModelWithoutSetNormals.xaml", meshGeometryModel, Color.FromRgb(255, 255, 0));

            NormalCalculator.SetNormalsForModel(meshGeometryModel);

            Assert.AreEqual(meshGeometryModel.Positions.Count, meshGeometryModel.Normals.Count);

            XamlWriter.SaveMeshGeometryModel(_outputPath + @"\ModelWithSetNormals.xaml", meshGeometryModel, Color.FromRgb(255,255,0));

            var normalModelPts = new List<Point3D>();
            for (var index = 0; index < meshGeometryModel.Normals.Count; index++)
            {
                var normal = meshGeometryModel.Normals[index]; 
                var position = meshGeometryModel.Positions[index];

                var p2 = new Point3D(position.X + normal.X, position.Y + normal.Y, position.Z + normal.Z);

                var lineModel = LineDisplayModel.GetLineModel(position, p2, .03);
                normalModelPts.AddRange(lineModel);
            }
            XamlWriter.WritePolygonsToXamlFile("", _outputPath + @"\NormalsAsLines.xaml", normalModelPts, false);
        }

        [Test]
        public void TestCreateNormalWithTheDinosaurModel()
        {
            var mdlReader = new MdlFilePolygonDataReader(_inputPath + @"\dinosaur.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlReader.Points);
            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            Assert.AreEqual(0, meshGeometryModel.Normals.Count);
            NormalCalculator.SetNormalsForModel(meshGeometryModel);
            Assert.AreEqual(meshGeometryModel.Positions.Count, meshGeometryModel.Normals.Count);
            XamlWriter.SaveMeshGeometryModel(_outputPath + @"\ModelWithSetNormals.xaml", meshGeometryModel, Color.FromRgb(255, 255, 0));
        }

        [TearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }
}
