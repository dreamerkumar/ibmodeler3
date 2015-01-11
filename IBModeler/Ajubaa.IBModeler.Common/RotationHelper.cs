using System;
using System.Windows;

namespace Ajubaa.IBModeler.Common
{
    /// <summary>
    /// helper to get a rotated point on an image
    /// </summary>
    public static class RotationHelper
    {
        /// <summary>
        /// gets the rotated position on an image
        /// </summary>
        /// <param name="angleInDegrees"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point? GetRotatedPosition(double angleInDegrees, Point position)
        {
            var angleInRadian = GetTotalRotatedAngleInRadian(angleInDegrees, position);
            if (!angleInRadian.HasValue)
                return null;

            var r = Math.Sqrt(position.X * position.X + position.Y * position.Y);

            //x = r cos theta, y = r sin theta
            return new Point(r * Math.Cos(angleInRadian.Value), r * Math.Sin(angleInRadian.Value));
        }

        private static double? GetTotalRotatedAngleInRadian(double rotatedAngleInDegrees, Point position)
        {
            var origAngle = GetOrigRotationAngle(position.X, position.Y);

            var newAngle = origAngle + DegreesToRadians(rotatedAngleInDegrees);

            if (newAngle < 0 || newAngle > Math.PI / 2.0)
                return null;

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
