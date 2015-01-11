using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.SurfaceSmoother.FullModelSmoother;
using Ajubaa.TextureGenerator;

namespace Ajubaa.IBModeler.Processor.Smoothen
{
    /// <summary>
    /// wrapper around paul bourke smoother
    /// </summary>
    public static class SmoothenProcessor
    {
        public static MeshGeometry3D SmoothenMesh(int count, HashSet<int>[] positionNeighbors, MeshGeometry3D mesh)
        {
            mesh.Positions = GetSmoothenedPositions(mesh.Positions, mesh.TriangleIndices, positionNeighbors, count);

            //important to set normals to avoid distorted display
            NormalCalculator.SetNormalsForModel(mesh);

            return mesh;
        }

        private static Point3DCollection GetSmoothenedPositions(Point3DCollection originalPositions, 
                Int32Collection triangleIndexCollection, HashSet<int>[] positionNeighbors, int smootheningIterationCount)
        {
            var newPositions = originalPositions;
            for (var ctr = 1; ctr <= smootheningIterationCount; ctr++)
            {
                newPositions = PaulBourkeSmoother.GetSmoothenedPositions(newPositions, triangleIndexCollection, positionNeighbors);
            }

            return newPositions;
        }
    }
}
