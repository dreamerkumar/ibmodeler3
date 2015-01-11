using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common;

namespace Ajubaa.SurfaceSmoother.FullModelSmoother
{
    public static class PaulBourkeSmoother
    {
        /// <summary>
        /// smoothens the model iteratively
        /// </summary>
        /// <param name="originalPositions"></param>
        /// <param name="triangleIndexCollection"></param>
        /// <param name="smoothingIterationCount"></param>
        /// <returns></returns>
        public static Point3DCollection GetSmoothenedPositions(Point3DCollection originalPositions, Int32Collection triangleIndexCollection, int smoothingIterationCount)
        {
            if(smoothingIterationCount <= 0) throw new ArgumentException(string.Format("Iteration count ({0}) should be greater than zero", smoothingIterationCount));

            var positionNeighbors = GetPositionNeighbors(originalPositions.Count, triangleIndexCollection);

            var currentPositions = originalPositions;
            for (var ctr = 1; ctr <= smoothingIterationCount; ctr++)
                currentPositions = GetSmoothenedPositions(currentPositions, triangleIndexCollection, positionNeighbors);

            return currentPositions;
        }

        /// <summary>
        /// see Surface_Simplification_Paul_Bourke.pdf
        /// </summary>
        public static Point3DCollection GetSmoothenedPositions(Point3DCollection originalPositions, Int32Collection triangleIndexCollection, HashSet<int>[] positionNeighbors)
        {
            var smoothPositions = new Point3DCollection();
            for (var ctr = 0; ctr < originalPositions.Count; ctr++)
            {
                var neighborIndices = positionNeighbors[ctr];
                var neighboringPts = new HashSet<Point3D>();
                foreach (var neighborIndex in neighborIndices)
                    neighboringPts.Add(originalPositions[neighborIndex]);

                var smoothenedPosition = GetSmoothenedPt(originalPositions[ctr], neighboringPts);
                smoothPositions.Add(smoothenedPosition);
            }
            return smoothPositions;
        }

        /// <summary>
        /// Smoothened positions to have new values as defined in Surface_Simplification_Paul_Bourke.pdf
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="neighboringPts"></param>
        /// <returns></returns>
        public static Point3D GetSmoothenedPt(Point3D pi, HashSet<Point3D> neighboringPts)
        {
            if (neighboringPts.Count < 3)
            {
                //throw new Exception(string.Format("Invalid surface. Neighboring points for a point ({0}), was less than 3.", neighboringPts.Count));
                return pi;
            }

            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            foreach (var pj in neighboringPts)
            {
                x += pj.X - pi.X;
                y += pj.Y - pi.Y;
                z += pj.Z - pi.Z;
            }
            x = x / neighboringPts.Count;
            y = y / neighboringPts.Count;
            z = z / neighboringPts.Count;

            return new Point3D(pi.X + x, pi.Y + y, pi.Z + z);
        }

        /// <summary>
        /// Sets the neighbors for each position based on the triangle indices supplied
        /// Any vertex of a triangle has the other two vertices as neighbors
        /// </summary>
        /// <param name="positionCount"></param>
        /// <param name="indices"></param>
        /// <returns></returns>
        public static HashSet<int>[] GetPositionNeighbors(int positionCount, Int32Collection indices)
        {
            var neighbors = new HashSet<int>[positionCount];
            for (var ctr = 0; ctr < indices.Count; ctr += 3)
            {
                var vertexIndices = new[] { indices[ctr], indices[ctr + 1], indices[ctr + 2] };

                for (var i = 0; i < 3; i++)
                {
                    var otherVertexIndices = new List<int> { 0, 1, 2 }.Except(new List<int> { i }).ToArray();

                    if (neighbors[vertexIndices[i]] == null)
                        neighbors[vertexIndices[i]] = new HashSet<int>();

                    neighbors[vertexIndices[i]].Add(vertexIndices[otherVertexIndices[0]]);
                    neighbors[vertexIndices[i]].Add(vertexIndices[otherVertexIndices[1]]);
                }
            }
            return neighbors;
        }

        /// <summary>
        /// If the model exists as a list of triangles only, it has to be converted to positions and indices
        /// before we can smoothen it using Paul Bourke's logic (see included pdf). This function does a straight
        /// conversion for the same. There is scope to improve it's performance, but I am expecting the models to 
        /// be more in the indices format so expecting this function to be used less.
        /// </summary>
        /// <param name="triangles"></param>
        /// <returns></returns>
        public static MeshGeometry3D CreateMeshGeometry3DFromTriangles(List<Triangle> triangles)
        {
            if (triangles == null || triangles.Count <= 0)
                throw new ArgumentException("CreateMeshGeometry3DFromTriangles: No triangles supplied.");

            var positions = new Point3DCollection();
            var triangleIndexCollection = new Int32Collection();
            
            var zeroesToRoundTo = GetZeroesToRoundTo(triangles[0]);

            foreach (var triangle in triangles)
            {
                var triangleIndex = new int[3];

                var existingVertices = new List<Point3D> { triangle.V1, triangle.V2, triangle.V3 };
                for (var ctr = 0; ctr < existingVertices.Count; ctr++)
                {
                    var p0 = existingVertices[ctr];
                    //the same positions could not be recognized as unique due to minute fractional differences, so rounding off positions.
                    //If this is not done, the algorithm to find all neighboring point fails.
                    var p = new Point3D(Math.Round(p0.X, zeroesToRoundTo), Math.Round(p0.Y, zeroesToRoundTo),Math.Round(p0.Z, zeroesToRoundTo));

                    var existingIndex = positions.IndexOf(p);
                    if (existingIndex >= 0)
                    {
                        triangleIndex[ctr] = existingIndex;
                    }
                    else
                    {
                        positions.Add(p);
                        triangleIndex[ctr] = positions.Count - 1;
                    }
                }

                triangleIndexCollection.Add(triangleIndex[0]);
                triangleIndexCollection.Add(triangleIndex[1]);
                triangleIndexCollection.Add(triangleIndex[2]);
            }

            return new MeshGeometry3D {Positions = positions, TriangleIndices = triangleIndexCollection};
        }

        private static int GetZeroesToRoundTo(Triangle triangle)
        {
            var minMax = new MinMaxAlongAxes(new List<Triangle> {triangle}, null);
            var diff = minMax.MaxX - minMax.MinX;
            var counter = 0;
            while (diff < 1)
            {
                counter++;
                diff = diff * 10;
            }
            return counter + 2;
        }
    }
}
