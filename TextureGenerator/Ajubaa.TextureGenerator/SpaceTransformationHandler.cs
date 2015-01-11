using System.Windows.Media.Media3D;
using Ajubaa.Common.MatrixManipulations;

namespace Ajubaa.TextureGenerator
{
    /// <summary>
    /// Handles the calculations required to transform points in 3D coordinate space from one coordinate system to the other
    /// </summary>
    public class SpaceTransformationHandler
    {
        private readonly double[,] _transformationMatrix;

        public SpaceTransformationHandler(Point3D newCenter, Point3D newPtOnZAxis)
        {
            //Calculate the unit normals along the three direction for the new system
            var n = new Vector3D
            {
                X = newPtOnZAxis.X - newCenter.X,
                Y = newPtOnZAxis.Y - newCenter.Y,
                Z = newPtOnZAxis.Z - newCenter.Z
            };
            n = n / n.Length;

            var v = new Vector3D { X = n.X, Y = n.Y + 10.0f, Z = n.Z }; //A vector pointing a little higher than the first vector

            var u = Vector3D.CrossProduct(v, n);
            u = u / u.Length;

            v = Vector3D.CrossProduct(n , u);
            v = v / v.Length;

            double[,] rotatXonMatrXx = { { u.X, u.Y, u.Z, 0.0f }, { v.X, v.Y, v.Z, 0.0f }, { n.X, n.Y, n.Z, 0.0f }, { 0.0f, 0.0f, 0.0f, 1.0f } };

            double[,] translationMatrix = { { 1.0f, 0.0f, 0.0f, -newCenter.X }, { 0.0f, 1.0f, 0.0f, -newCenter.Y }, { 0.0f, 0.0f, 1.0f, -newCenter.Z }, { 0.0f, 0.0f, 0.0f, 1.0f } };

            _transformationMatrix = MatrixDataProcessor.MultiplyMatrices(rotatXonMatrXx, translationMatrix);
        }
        
        public Point3D GetTransformedPoint(Point3D inP)
        {
            double[,] inputMatrix = { {inP.X}, {inP.Y}, {inP.Z}, {1.0f} };

            var resultantMatrix = MatrixDataProcessor.MultiplyMatrices(_transformationMatrix, inputMatrix);
            var result = new Point3D
            {
                X = resultantMatrix[0,0],
                Y = resultantMatrix[1,0],
                Z = resultantMatrix[2,0]
            };

            return result;
        }
    }
}