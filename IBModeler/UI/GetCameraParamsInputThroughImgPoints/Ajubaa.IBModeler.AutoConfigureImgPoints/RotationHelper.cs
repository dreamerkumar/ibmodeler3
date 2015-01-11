using System;
using System.Windows;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public static class RotationHelper
    {
        public static Point GetRotatedPosition(double angleInDegrees, Point position)
        {
            var angleInRadian = GetTotalRotatedAngleInRadian(angleInDegrees, position);

            var r = Math.Sqrt(position.X * position.X + position.Y * position.Y);

            //x = r cos theta, y = r sin theta
            return new Point(r * Math.Cos(angleInRadian), r * Math.Sin(angleInRadian));
        }

        private static double GetTotalRotatedAngleInRadian(double rotatedAngleInDegrees, Point position)
        {
            var origAngle = GetOrigRotationAngle(position.X, position.Y);

            var newAngle = origAngle + DegreesToRadians(rotatedAngleInDegrees);

            if (newAngle < 0 || newAngle > Math.PI / 2.0)
                throw new Exception("Cannot determine click points after realigning the image");

            return newAngle;
        }

        private static double GetOrigRotationAngle(double positiveX, double positiveY)
        {
            if (positiveX == 0 && positiveY == 0)
                return 0;

            if (positiveX == 0)
                return Math.PI / 2.0;

            var tanTheta = positiveY / positiveX;

            var theta = Math.Atan(tanTheta);

            return theta;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}