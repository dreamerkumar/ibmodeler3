using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common
{
    public class Triangle
    {
        public Triangle(Point3D v1, Point3D v2, Point3D v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }

        public Point3D V1 { get; private set; }
        public Point3D V2 { get; private set; }
        public Point3D V3 { get; private set; }

        public static List<Triangle> GetTrianglesFromPts(List<Point3D> points)
        {
            var triangles = new List<Triangle>();
            for (var index = 0; index < points.Count; index += 3)
                triangles.Add(new Triangle(points[index], points[index + 1], points[index + 2]));
            return triangles;
        }

        public static List<Triangle> GetTrianglesFromPts(Point3DCollection points)
        {
            var triangles = new List<Triangle>();
            for (var index = 0; index < points.Count; index += 3)
                triangles.Add(new Triangle(points[index], points[index + 1], points[index + 2]));
            return triangles;
        }

        /// <summary>
        /// Gets the normal for the given triangle
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3D GetNormal(Triangle t)
        {
            return GetNormal(t.V1, t.V2, t.V3);
        }

        public static Vector3D GetNormal(Point3D inP1, Point3D inP2, Point3D inP3)
        {
            return Vector3D.CrossProduct(CommonFunctions.GetVector(inP2) - CommonFunctions.GetVector(inP1), 
                (CommonFunctions.GetVector(inP3) - CommonFunctions.GetVector(inP1)));
        }
    }
}
