using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common
{
    public class PolygonsGetter
    {
        /// <summary>
        /// gets box polygons for min max values
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<Point3D> GetBoxPolygonsForMinMaxValues(MinMaxAlongAxes values)
        {
            if (!values.MinX.HasValue || !values.MaxX.HasValue || !values.MinY.HasValue ||
                !values.MaxY.HasValue || !values.MinZ.HasValue || !values.MaxZ.HasValue)
            {
                throw new Exception("Cannot create Box Polygon for the supplied min max values. Min Max Values not set.");
            }

            //front points
            var front = new Point3D[4];
            front[0] = new Point3D(values.MinX.Value, values.MinY.Value, values.MaxZ.Value);
            front[1] = new Point3D(values.MaxX.Value, values.MinY.Value, values.MaxZ.Value);
            front[2] = new Point3D(values.MaxX.Value, values.MaxY.Value, values.MaxZ.Value);
            front[3] = new Point3D(values.MinX.Value, values.MaxY.Value, values.MaxZ.Value);

            //back points
            var back = new Point3D[4];
            back[0] = new Point3D(front[0].X, front[0].Y, values.MinZ.Value);
            back[1] = new Point3D(front[1].X, front[1].Y, values.MinZ.Value);
            back[2] = new Point3D(front[2].X, front[2].Y, values.MinZ.Value);
            back[3] = new Point3D(front[3].X, front[3].Y, values.MinZ.Value);

            return GetBoxPolygonsForFrontAndBackFace(front, back);
        }

        /// <summary>
        /// gets box polygons around a point
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="sideLength"></param>
        /// <returns></returns>
        public static List<Point3D> GetBoxPolygonsAroundAPoint(Point3D pt, double sideLength)
        {
            var s = sideLength / 2;

            //front points
            var front = new Point3D[4];
            front[0] = new Point3D(pt.X - s, pt.Y - s, pt.Z + s);
            front[1] = new Point3D(pt.X + s, pt.Y - s, pt.Z + s);
            front[2] = new Point3D(pt.X + s, pt.Y + s, pt.Z + s);
            front[3] = new Point3D(pt.X - s, pt.Y + s, pt.Z + s);

            //back points
            var back = new Point3D[4];
            back[0] = new Point3D(front[0].X, front[0].Y, pt.Z - s);
            back[1] = new Point3D(front[1].X, front[1].Y, pt.Z - s);
            back[2] = new Point3D(front[2].X, front[2].Y, pt.Z - s);
            back[3] = new Point3D(front[3].X, front[3].Y, pt.Z - s);

            return GetBoxPolygonsForFrontAndBackFace(front, back);
        }

        /// <summary>
        /// gets box polygons for front and back face
        /// </summary>
        /// <param name="front"></param>
        /// <param name="back"></param>
        /// <returns></returns>
        private static List<Point3D> GetBoxPolygonsForFrontAndBackFace(Point3D[] front, Point3D[] back)
        {
            if (front == null) throw new ArgumentNullException("front");
            if (back == null) throw new ArgumentNullException("back");

            var pts = new List<Point3D>
            {
              //front face
              front[0],
              front[1],
              front[3],

              front[2],
              front[3],
              front[1],

              //back face
              back[0],
              back[2],
              back[1],

              back[0],
              back[3],
              back[2],

              //right face
              front[1],
              back[1],
              front[2],

              front[2],
              back[1],
              back[2],

              //left face
              front[0],
              front[3],
              back[0],

              front[3],
              back[3],
              back[0],

              //top face
              front[3],
              front[2],
              back[3],

              back[3],
              front[2],
              back[2],

              //bottom face
              front[0],
              back[0],
              front[1],

              front[1],
              back[0],
              back[1]
            };

            return pts;
        }

        /// <summary>
        /// gets a box for a triangle
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        public static GeometryModel3D GetBoxForATriange(Triangle triangle)
        {
            //Get the equation of the triangle plane
            var trianglePlane = new PlaneEquation(triangle.V1, triangle.V2, triangle.V3);

            //todo: logic to decide how far the second plane should be

            //make a parallel plane 
            var parallelPlane = new PlaneEquation(trianglePlane.A, trianglePlane.B, trianglePlane.C, trianglePlane.D + .02);

            var p1 = PlaneEquation.GetPtOnPlanePassingThroughAPtOnItsNormal(parallelPlane, triangle.V1);
            var p2 = PlaneEquation.GetPtOnPlanePassingThroughAPtOnItsNormal(parallelPlane, triangle.V2);
            var p3 = PlaneEquation.GetPtOnPlanePassingThroughAPtOnItsNormal(parallelPlane, triangle.V3);

            var mesh = new MeshGeometry3D();

            //add the triangle positions
            mesh.Positions.Add(triangle.V1);
            mesh.Positions.Add(triangle.V2);
            mesh.Positions.Add(triangle.V3);

            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);

            //add the triangle indices for the first triangle
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);

            //add the indices for the second triangle
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);

            //first side 031 341
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(1);

            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(1);

            //second side 142 452
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);

            //third side 502 530
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            //Add a default material
            var material = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));

            return new GeometryModel3D(mesh, material);
        }

        /// <summary>
        /// creates a double sided pyramid using a tip point and the base
        /// </summary>
        /// <param name="tipPt"></param>
        /// <param name="baseTrg"></param>
        /// <returns></returns>
        public static GeometryModel3D CreateDoubleSidedPyramid(Point3D tipPt, Triangle baseTrg)
        {
            var mesh = new MeshGeometry3D();

            //add the positions
            mesh.Positions.Add(tipPt);
            mesh.Positions.Add(baseTrg.V1);
            mesh.Positions.Add(baseTrg.V2);
            mesh.Positions.Add(baseTrg.V3);

            //add the indices
            var indices = new[] { 0, 1, 2, 0, 2, 1, 0, 1, 3, 0, 3, 1, 0, 2, 3, 0, 3, 2, 1, 2, 3, 1, 3, 2 };
            foreach (var index in indices)
                mesh.TriangleIndices.Add(index);

            //Add a default material
            var material = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));

            return new GeometryModel3D(mesh, material);
        }

        public static GeometryModel3D CreateDoubleSidedTriangleModel(Triangle triangle)
        {
            var mesh = new MeshGeometry3D();

            //add the triangle positions
            mesh.Positions.Add(triangle.V1);
            mesh.Positions.Add(triangle.V2);
            mesh.Positions.Add(triangle.V3);

            //add the indices
            var indices = new[] { 0, 1, 2, 0, 2, 1 };
            foreach (var index in indices)
                mesh.TriangleIndices.Add(index);

            //Add a default material
            var material = new DiffuseMaterial(new SolidColorBrush(Colors.SteelBlue));

            return new GeometryModel3D(mesh, material);
        }
    }
}
