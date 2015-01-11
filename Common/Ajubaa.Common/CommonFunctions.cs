using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Color = System.Drawing.Color;

namespace Ajubaa.Common
{
    public class CommonFunctions
    {
        /// <summary>
        /// Returns a value without rounding
        /// </summary>
        /// <param name="dblValue"></param>
        /// <returns></returns>
        public static uint GetUIntFromDouble(double dblValue)
        {
            return (uint)GetIntFromDouble(dblValue);
        }
        /// <summary>
        /// Returns a value without rounding
        /// </summary>
        /// <param name="dblValue"></param>
        /// <returns></returns>
        public static int GetIntFromDouble(double dblValue)
        {
            var decVal = Convert.ToDecimal(dblValue);
            decVal = Decimal.Truncate(decVal);
            return Convert.ToInt32(decVal);
        }
        public static Point3D Mutiply(Point3D inPt, double dblVal)
        {
            return new Point3D(inPt.X * dblVal, inPt.Y * dblVal, inPt.Z * dblVal);
        }
        public static Point3D Divide(Point3D inPt, double dblVal)
        {
            return new Point3D(inPt.X / dblVal, inPt.Y / dblVal, inPt.Z / dblVal);
        }
        public static Point3D Add(Point3D inPt1, Point3D inPt2)
        {
            return new Point3D(inPt1.X + inPt2.X, inPt1.Y + inPt2.Y, inPt1.Z + inPt2.Z);
        }
        public static bool ArePointsSame(Point3D p1, Point3D p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }
        public static Vector3D GetVector(Point3D pt)
        {
            return new Vector3D(pt.X, pt.Y, pt.Z);
        }

        public static double ModValue(double inVal)
        {
            return inVal < 0 ? -inVal : inVal;
        }
        public static bool HasSameSigns(double inVal1, double inVal2)
        {
            if (inVal1 < 0 && inVal2 < 0)
                return true;
            return inVal1 >= 0 && inVal2 >= 0;
        }
        public static bool IsSameRgbColor(Color clr1, Color clr2)
        {
            return clr1.R == clr2.R && clr1.G == clr2.G && clr1.B == clr2.B;
        }
        public static double GetMin(double dbl1, double dbl2, double dbl3)
        {
            var dblMin = dbl1;
            if (dbl2 < dblMin)
                dblMin = dbl2;
            if (dbl3 < dblMin)
                dblMin = dbl3;
            return dblMin;
        }
        public static Point3D GetMiddlePoint(Point3D p1, Point3D p2, double m, double mPlusN)
        {
            if (m == mPlusN) return p2;
            var denominator = (mPlusN - m);
            return GetMiddlePoint(p1, p2, m / denominator);
        }
        public static Point3D GetMiddlePoint(Point3D p1, Point3D p2, double d1ByD2)
        {
            var m = d1ByD2;
            const double n = 1.0f;
            return Divide(Add(Mutiply(p2, m), Mutiply(p1, n)), (m + n));
        }
        public static double Min(params double[] values)
        {
            var outVal = values[0];
            foreach (var value in values)
            {
                if (outVal > value)
                    outVal = value;
            }
            return outVal;
        }
        public static double Max(params double[] values)
        {
            var outVal = values[0];
            foreach (var value in values)
            {
                if (outVal < value)
                    outVal = value;
            }
            return outVal;
        }
        /// <summary>
        /// pi = 180 degrees, x = 180*x/PI
        /// </summary>
        /// <param name="radianValue"></param>
        /// <param name="roundTo"></param>
        /// <returns></returns>
        public static double RadianToDegrees(double radianValue, int roundTo)
        {
            return Math.Round(180.0*radianValue/Math.PI, roundTo);
        }
    }
}
