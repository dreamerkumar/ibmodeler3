using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common.MatrixManipulations
{
    /// <summary>
    /// ref: http://www.euclideanspace.com/maths/algebra/matrix/orthogonal/rotation/index.htm
    /// </summary>
    public static class RotateAlongXYOrZAxis
    {
        public static Point3D[] GetRotatedPts(Axis axis, double angleInRadian, params Point3D[] inputPts)
        {
            return GetRotatedPtList(axis, angleInRadian, inputPts).ToArray();
        }
        public static List<Point3D> GetRotatedPtList(Axis axis, double angleInRadian, params Point3D[] inputPts)
        {
            double[,] multiplicationMatrix = null;
            switch (axis)
            {
                case Axis.X:
                    multiplicationMatrix = GetMatrixToRotateByXAxis(angleInRadian);
                    break;
                case Axis.Y:
                    multiplicationMatrix = GetMatrixToRotateByYAxis(angleInRadian);
                    break;
                case Axis.Z:
                    multiplicationMatrix = GetMatrixToRotateByZAxis(angleInRadian);
                    break;
            }
            var ptList = new List<Point3D>();
            foreach (var inputPt in inputPts)
            {
                var inputMatrix = new double[3, 1] { { inputPt.X }, { inputPt.Y }, { inputPt.Z } };
                var outputMatrix = MatrixDataProcessor.MultiplyMatrices(multiplicationMatrix, inputMatrix);
                ptList.Add(new Point3D(outputMatrix[0, 0], outputMatrix[1, 0], outputMatrix[2, 0]));
            }
            return ptList;
        }
        public static double[,] GetMatrixToRotateByXAxis(double angleInRadian)
        {
            return new double[3, 3]
                       {
                            {1.0, 0.0, 0.0},    
                            { 0.0, Math.Cos(angleInRadian), -Math.Sin(angleInRadian)}, 
                            {  0.0,  Math.Sin(angleInRadian),Math.Cos(angleInRadian)} 
                           
                       };
        }
        public static double[,] GetMatrixToRotateByYAxis(double angleInRadian)
        {
            return new double[3, 3]
                       {
                           { Math.Cos(angleInRadian), 0.0, Math.Sin(angleInRadian) }, 
                           { 0.0, 1.0, 0.0 }, 
                           { -Math.Sin(angleInRadian),  0.0,  Math.Cos(angleInRadian) }
                       };
        }
        public static double[,] GetMatrixToRotateByZAxis(double angleInRadian)
        {
            return new double[3, 3]
                       {
                           {Math.Cos(angleInRadian), -Math.Sin(angleInRadian), 0.0},
                           {Math.Sin(angleInRadian), Math.Cos(angleInRadian), 0.0},
                           {0.0, 0.0, 1.0}
                       };
        }
    }
}
