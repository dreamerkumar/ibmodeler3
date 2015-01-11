using System;

namespace Ajubaa.Common
{
    public class LineEquation2D
    {
        //Y = mX + c
        public double M { get; set; }
        public double C { get; set; }

        public double GetYValueForX(double x)
        {
            if(M == double.PositiveInfinity || M == double.NegativeInfinity)
                throw new Exception("Slope value is infinity");
            return M*x + C;
        }

        public double GetXValueForY(double y)
        {
            if(M == 0.0) throw new Exception("Slope value is zero");
            return (y - C)/M;
        }

        public static LineEquation2D Get2DLineEquation(double x1, double y1, double x2, double y2)
        {
            var dx = x2 - x1;
            if (dx == 0) throw new Exception("The form y = mx + c is not possible for vertical lines.");
            var dy = y2 - y1;
            var slope = dy / dx;

            // y = mx + c
            // intercept c = y - mx
            var intercept = y1 - slope * x1; // which is same as y2 - slope * x2

            return new LineEquation2D {M = slope, C = intercept};
        }
    }
}
