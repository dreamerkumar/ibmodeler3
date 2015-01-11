using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Ajubaa.TextureGenerator
{
    public static class NormalCalculator
    {
        public static void SetNormalsForModel(MeshGeometry3D model)
        {
            model.Normals = new Vector3DCollection();

            var positionWiseTrgIndexCollection = GetPositionWiseTrgIndexCollection(model);

            for (var index = 0; index < positionWiseTrgIndexCollection.Length; index++)
            {
                var triangleIndices = positionWiseTrgIndexCollection[index];
                if(triangleIndices == null || triangleIndices.Count <= 0 || triangleIndices.Count%3 != 0)
                    throw new Exception(string.Format("Invalid number of triangle indices ({0}) found for the position at {1}", triangleIndices == null? 0: triangleIndices.Count, index));

                var normalCollection = new List<Vector3D>();
                for (var i = 0; i < triangleIndices.Count; i += 3)
                {
                    var p0 = model.Positions[triangleIndices[i]];
                    var p1 = model.Positions[triangleIndices[i+1]];
                    var p2 = model.Positions[triangleIndices[i+2]];

                    var v1P = p1 - p0;
                    var v1 = new Vector3D(v1P.X, v1P.Y, v1P.Z);

                    var v2P = p2 - p0;
                    var v2 = new Vector3D(v2P.X, v2P.Y, v2P.Z);

                    var normal = Vector3D.CrossProduct(v1, v2);
                    if(normal.Length == 0)
                    {
                        //string.Format("Cannot form a valid triangle with the positions. The positions are {0} === {1} === {2}", p0, p1, p2));
                    }
                    else
                    {
                        var unitNormal = normal/normal.Length;
                        normalCollection.Add(unitNormal);
                    }
                }
                var normalVector = GetAverage(normalCollection);
                model.Normals.Add(normalVector);
            }
        }

        private static Vector3D GetAverage(IEnumerable<Vector3D> vectorList)
        {
            var totalVector = new Vector3D();
            totalVector = vectorList.Aggregate(totalVector, (current, vector) => current + vector);
            if(totalVector.Length > 0)
                return totalVector / totalVector.Length;
            return new Vector3D(0,0,1.0); //default for bad value
        }

        private static List<int>[] GetPositionWiseTrgIndexCollection(MeshGeometry3D model)
        {
            var positionWiseTrgIndexCollection = new List<int>[model.Positions.Count];
            for (var i = 0; i < model.TriangleIndices.Count; i+= 3)
            {
                var indexCollection = new List<int> {model.TriangleIndices[i], model.TriangleIndices[i+1], model.TriangleIndices[i+2]};
                foreach (var index in indexCollection)
                {
                    if (positionWiseTrgIndexCollection[index] == null)
                    {
                        positionWiseTrgIndexCollection[index] = indexCollection;
                    }
                    else
                    {
                        positionWiseTrgIndexCollection[index].AddRange(indexCollection);
                    }
                }
            }
            return positionWiseTrgIndexCollection;
        }
    }
}
