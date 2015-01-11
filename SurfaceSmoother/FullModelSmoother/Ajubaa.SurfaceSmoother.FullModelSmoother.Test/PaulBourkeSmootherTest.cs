using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.Common.PolygonDataReaders;
using Ajubaa.Common.PolygonDataWriters;
using NUnit.Framework;

namespace Ajubaa.SurfaceSmoother.FullModelSmoother.Test
{
    [TestFixture]
    public class PaulBourkeSmootherTest
    {
        private string _inputPath;
        private string _outputPath;

        [SetUp]
        public void SetUp()
        {
            _outputPath = ExecutionDirInfoHelper.CreateUniqueOutputPath();
            _inputPath = ExecutionDirInfoHelper.GetInputDirPath();
        }

        [Test]
        public void SanityCheckForCreateMeshGeometry3DFromTriangles()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\flowerpetals.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            
            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);

            Assert.AreEqual(triangles.Count*3, meshGeometryModel.TriangleIndices.Count);

            //check each triangle for proper orientation
            for (var ctr = 0; ctr < meshGeometryModel.TriangleIndices.Count; ctr += 3)
            {
                var p0 = meshGeometryModel.Positions[meshGeometryModel.TriangleIndices[ctr]];
                var p1 = meshGeometryModel.Positions[meshGeometryModel.TriangleIndices[ctr + 1]];
                var p2 = meshGeometryModel.Positions[meshGeometryModel.TriangleIndices[ctr + 2]];

                var v1P = p1 - p0;
                var v2P = p2 - p0;
                var v1 = new Vector3D(v1P.X, v1P.Y, v1P.Z);
                var v2 = new Vector3D(v2P.X, v2P.Y, v2P.Z);

                var crossVector = Vector3D.CrossProduct(v1, v2);
                Assert.IsFalse(Math.Round(crossVector.Length, 6) == 0.0, string.Format("Proper triangle cannot be formed at startind index {0} with the positions {1} {2} {3}", ctr, p0, p1, p2));
            }
        }
        
        [Test]
        public void TestGetSmoothenedPositions()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\flowerpetals.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\InputModel.xaml", _outputPath), mdlFileReader.Points, false);

            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            XamlWriter.SaveMeshGeometryModel(string.Format(@"{0}\InputModelMeshGeometry.xaml", _outputPath), meshGeometryModel, Color.FromRgb(100, 100, 100));

            var currentPositions = meshGeometryModel.Positions;
            var positionNeighbors = PaulBourkeSmoother.GetPositionNeighbors(currentPositions.Count, meshGeometryModel.TriangleIndices);
            for (var ctr = 1; ctr <= 10; ctr++)
            {
                var newPositions = PaulBourkeSmoother.GetSmoothenedPositions(currentPositions, meshGeometryModel.TriangleIndices, positionNeighbors);
                XamlWriter.SavePositionsAndTriangleIndicesAsModel(string.Format(@"{0}\Smoothing_Iteration_{1}.xaml", _outputPath, ctr), 
                    newPositions, meshGeometryModel.TriangleIndices, Color.FromRgb(100, 100, 100));
                currentPositions = newPositions;
            }
        }

        [Test]
        public void TestIterativeSmoothening()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\block.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\InputModel.xaml", _outputPath), mdlFileReader.Points, false);

            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            XamlWriter.SaveMeshGeometryModel(string.Format(@"{0}\InputModelMeshGeometry.xaml", _outputPath), meshGeometryModel, Color.FromRgb(100, 100, 100));

            var smoothPositions = PaulBourkeSmoother.GetSmoothenedPositions(meshGeometryModel.Positions, meshGeometryModel.TriangleIndices, 5);
            
            XamlWriter.SavePositionsAndTriangleIndicesAsModel(string.Format(@"{0}\Smoothened_{1}_times.xaml", _outputPath, 5),
                    smoothPositions, meshGeometryModel.TriangleIndices, Color.FromRgb(100, 100, 100));
        }

        [Test]
        public void TestMediumSizedModel()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\v.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\InputModel.xaml", _outputPath), mdlFileReader.Points, false);

            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            XamlWriter.SaveMeshGeometryModel(string.Format(@"{0}\InputModelMeshGeometry.xaml", _outputPath), meshGeometryModel, Color.FromRgb(100, 100, 100));

            var smoothPositions = PaulBourkeSmoother.GetSmoothenedPositions(meshGeometryModel.Positions, meshGeometryModel.TriangleIndices, 6);

            XamlWriter.SavePositionsAndTriangleIndicesAsModel(string.Format(@"{0}\Smoothened_{1}_times.xaml", _outputPath, 6),
                    smoothPositions, meshGeometryModel.TriangleIndices, Color.FromRgb(100, 100, 100));
        }

        [Test]
        public void TestLargeSizedModel()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\dinosaur.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\InputModel.xaml", _outputPath), mdlFileReader.Points, false);

            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            XamlWriter.SaveMeshGeometryModel(string.Format(@"{0}\InputModelMeshGeometry.xaml", _outputPath), meshGeometryModel, Color.FromRgb(100, 100, 100));


            var currentPositions = meshGeometryModel.Positions;
            var positionNeighbors = PaulBourkeSmoother.GetPositionNeighbors(currentPositions.Count, meshGeometryModel.TriangleIndices);
            for (var ctr = 1; ctr <= 20; ctr++)
            {
                var newPositions = PaulBourkeSmoother.GetSmoothenedPositions(currentPositions, meshGeometryModel.TriangleIndices, positionNeighbors);
                XamlWriter.SavePositionsAndTriangleIndicesAsModel(string.Format(@"{0}\Smoothing_Iteration_{1}.xaml", _outputPath, ctr),
                    newPositions, meshGeometryModel.TriangleIndices, Color.FromRgb(100, 100, 100));
                currentPositions = newPositions;
            }
        }

        [Test]
        public void TestSmoothingOfBatman()
        {
            var mdlFileReader = new MdlFilePolygonDataReader(_inputPath + @"\batman.mdl");
            var triangles = Triangle.GetTrianglesFromPts(mdlFileReader.Points);
            XamlWriter.WritePolygonsToXamlFile("", string.Format(@"{0}\InputModel.xaml", _outputPath), mdlFileReader.Points, false);

            var meshGeometryModel = PaulBourkeSmoother.CreateMeshGeometry3DFromTriangles(triangles);
            XamlWriter.SaveMeshGeometryModel(string.Format(@"{0}\InputModelMeshGeometry.xaml", _outputPath), meshGeometryModel, Color.FromRgb(100, 100, 100));


            var currentPositions = meshGeometryModel.Positions;
            var positionNeighbors = PaulBourkeSmoother.GetPositionNeighbors(currentPositions.Count, meshGeometryModel.TriangleIndices);
            for (var ctr = 1; ctr <= 20; ctr++)
            {
                var newPositions = PaulBourkeSmoother.GetSmoothenedPositions(currentPositions, meshGeometryModel.TriangleIndices, positionNeighbors);
                XamlWriter.SavePositionsAndTriangleIndicesAsModel(string.Format(@"{0}\Smoothing_Iteration_{1}.xaml", _outputPath, ctr),
                    newPositions, meshGeometryModel.TriangleIndices, Color.FromRgb(100, 100, 100));
                currentPositions = newPositions;
            }
        }

        [TearDown]
        public void TearDown()
        {
            _inputPath = null;
            _outputPath = null;
        }
    }
}
