using System;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common
{
    public class LineEquationUsingVectorAndPoint
    {
        public Vector3D Vector { get; set; }
        public Point3D Point { get; set; }

        public LineEquationUsingVectorAndPoint(Point3D pt1, Point3D pt2)
        {
            Point = pt1;
            Vector = new Vector3D(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z);
        }

        public LineEquationUsingVectorAndPoint()
        {
            
        }

        /// <summary>
        /// not tested
        /// </summary>
        /// <param name="axisAlongWhichValueIsKnown"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Point3D GetPointIfOneValueIsKnown(Axis axisAlongWhichValueIsKnown, double value)
        {
            //X = x1 + ai, Y = y1 + aj, Z = z1 + ak
            double a;
            switch (axisAlongWhichValueIsKnown)
            {
                case Axis.X:
                    a = (value - Point.X) / Vector.X;
                    return new Point3D(value, Point.Y + a * Vector.Y, Point.Z + a * Vector.Z);
                case Axis.Y:
                    a = (value - Point.Y) / Vector.Y;
                    return new Point3D(Point.X + a * Vector.X, value, Point.Z + a * Vector.Z);
                default:
                case Axis.Z:
                    a = (value - Point.Z) / Vector.Z;
                    return new Point3D(Point.X + a * Vector.X, Point.Y + a * Vector.Y, value);
            }
        }

    }
}
