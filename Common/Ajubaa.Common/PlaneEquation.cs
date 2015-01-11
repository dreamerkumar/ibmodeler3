using System.Windows.Media.Media3D;

namespace Ajubaa.Common
{
    /// <summary>
    /// Equation of a plane is in the format ax + by + cz + d = 0
    /// </summary>
    public class PlaneEquation
    {
        #region Properties
        public double A { get; private set; }

        public double B { get; private set; }

        public double C { get; private set; }

        public double D { get; private set; }

        public Vector3D Normal { get { return new Vector3D(A, B, C);}}

        public Point3D NormalCoordinates{get { return new Point3D(A, B, C); }}
        #endregion Properties

        public PlaneEquation(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }

        public PlaneEquation(Point3D inP1, Point3D inP2, Point3D inP3)
        {
            var normal = Triangle.GetNormal(inP1, inP2, inP3);
            A = normal.X;
            B = normal.Y;
            C = normal.Z;
            D = -(inP1.X * normal.X + inP1.Y * normal.Y + inP1.Z * normal.Z);
        }

        public static double ValueInPlaneEquation(PlaneEquation p, Point3D pt)
        {
            return pt.X * p.A + pt.Y * p.B + pt.Z * p.C + p.D;
        }
        public static double ValueInPlaneEquation(Point3D pt, double a, double b, double c, double d)
        {
            var p = new PlaneEquation(a, b, c, d);
            return pt.X * p.A + pt.Y * p.B + pt.Z * p.C + p.D;
        }

        /// <summary>
        /// tested on 30-Aug-10
        /// </summary>
        /// <param name="eq"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static Point3D GetPtOnPlanePassingThroughAPtOnItsNormal(PlaneEquation eq, Point3D p)
        {
            //x = x1 + Ak
            //y = y1 + Bk
            //z = z1 + Ck
            //Ax + By + Cz + D = 0
            //A(x1 + Ak) + B(y1 + Bk) + C(z1 + Ck) + D = 0
            //(A*A + B*B + C*C)k = -Ax1 - By1 - Cz1 - D

            var k = -(eq.A * p.X + eq.B * p.Y + eq.C * p.Z + eq.D) / (eq.A * eq.A + eq.B * eq.B + eq.C * eq.C);

            return new Point3D(p.X + eq.A * k, p.Y + eq.B * k, p.Z + eq.C * k);

        }
    }
}
