using System.Linq;
using System.Windows;
using Ajubaa.Common;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Ajubaa.Common.MatrixManipulations;


namespace Ajubaa.LineFromPtCollection
{
    public static class StraightLineProcessor
    {
        //gets the straight line equation using the data points and the logic in Linear Regression.pdf
        public static LineEquationUsingVectorAndPoint GetStraightLine(Axis axisOfMaxVariation, List<Point3D> linearDataPoints)
        {
            //see line vector from two 2d equations.pdf
            switch(axisOfMaxVariation)
            {
                case Axis.X:
                    var pointsxY = (linearDataPoints.Select(pt => new Point { X = pt.X, Y = pt.Y })).ToList();
                    var _2DEqnxY = Get2DStraightLineFromDataConstants(pointsxY);
                    var pointsxZ = (linearDataPoints.Select(pt => new Point { X = pt.X, Y = pt.Z })).ToList();
                    var _2DEqnxZ = Get2DStraightLineFromDataConstants(pointsxZ);
                    var vectorX = new Vector3D(1, _2DEqnxY.M, _2DEqnxZ.M);
                    var pointX = new Point3D(0, _2DEqnxY.C, _2DEqnxZ.C);
                    return new LineEquationUsingVectorAndPoint { Vector = vectorX, Point = pointX };
                case Axis.Y:
                    var pointsyX = (linearDataPoints.Select(pt => new Point { X = pt.Y, Y = pt.X })).ToList();
                    var _2DEqnyX = Get2DStraightLineFromDataConstants(pointsyX);
                    var pointsyZ = (linearDataPoints.Select(pt => new Point { X = pt.Y, Y = pt.Z })).ToList();
                    var _2DEqnyZ = Get2DStraightLineFromDataConstants(pointsyZ);
                    var vectorY = new Vector3D(_2DEqnyX.M, 1, _2DEqnyZ.M);
                    var pointY = new Point3D(_2DEqnyX.C, 0, _2DEqnyZ.C);
                    return new LineEquationUsingVectorAndPoint { Vector = vectorY, Point = pointY };
                case Axis.Z:
                default:
                    var pointszX = (linearDataPoints.Select(pt => new Point { X = pt.Z, Y = pt.X })).ToList();
                    var _2DEqnzX = Get2DStraightLineFromDataConstants(pointszX);
                    var pointszY = (linearDataPoints.Select(pt => new Point { X = pt.Z, Y = pt.Y })).ToList();
                    var _2DEqnzY = Get2DStraightLineFromDataConstants(pointszY);
                    var vectorZ = new Vector3D(_2DEqnzX.M, _2DEqnzY.M, 1);
                    var pointZ = new Point3D(_2DEqnzX.C, _2DEqnzY.C, 0);
                    return new LineEquationUsingVectorAndPoint { Vector = vectorZ, Point = pointZ };
            }
        }

        /// <summary>
        /// uses logic in Linear Regression.pdf to give a straight line based on the passed data points
        /// see "calculate line equation using linear regression.pdf" for calculation parameters
        /// </summary>
        /// <param name="xYPts"></param>
        /// <returns></returns>
        public static LineEquation2D Get2DStraightLineFromDataConstants(List<Point> xYPts)
        {
            //eqn form y = a + bx
            var n = xYPts.Count;
            var sumX = (xYPts.Select(pt => pt.X)).Sum();
            var sumY = (xYPts.Select(pt => pt.Y)).Sum();
            var sumXx = (xYPts.Select(pt => pt.X * pt.X)).Sum();
            var sumXy = (xYPts.Select(pt => pt.X*pt.Y)).Sum();

            //n a    + sumX  b = sumY
            //sumX a + sumXX b = sumXY
            var coefficientMatrix = new[,] {{n,sumX}, {sumX,sumXx}};
            var eqnConstants = new[] {sumY, sumXy};
            var result = MatrixDataProcessor.SolveEquation(coefficientMatrix, eqnConstants);

            return new LineEquation2D {M = result[1], C = result[0]};

        }
    }
}
