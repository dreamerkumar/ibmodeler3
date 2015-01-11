using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Ajubaa.TextureGenerator
{
    public static class TexCoodCalculator
    {
        public static Point GetTexCood(CameraRatio cameraRatio, Point3D transformedPt, double zDistance)
        {
            if (zDistance < 0.0f)
                throw new Exception("Error: Incorrect camera orientation encountered. A camera position was found to be not in front of the model. It was either within the model or behind it");

            var maxXRange = 0.0;
            var maxYRange = 0.0;

            //If the camera is at infinity then max values along x and y will remain the same
            //Confirm if xRangeAtInfinity is the total range or it is the range in either direction
            if (cameraRatio.XRangeAtInfinity > 0.0f)
            {
                maxXRange = cameraRatio.XRangeAtInfinity;
                maxYRange = cameraRatio.YRangeAtInfinity;
            }
            if (cameraRatio.XRatio > 0.0f) //The ranges have to be set for each vertex depending on their distance from the camera location
            {
                maxXRange = cameraRatio.XRatio * zDistance;
                maxYRange = cameraRatio.YRatio * zDistance;
            }

            return GetTexCood(transformedPt, maxXRange, maxYRange);
        }

        private static Point GetTexCood(Point3D transformedPt, double maxXRange, double maxYRange)
        {
            //We have x and y values within  -xmax/2 and +xmax/2. We can convert it to zero to xmax by always adding xmax/2
            var texCood = new Point
            {
                X = transformedPt.X + (maxXRange / 2.0f),
                Y = transformedPt.Y + (maxYRange / 2.0f)
            };

            //Now the range is from zero to xmax. Convert it to the range zero to 1
            texCood.X /= maxXRange;
            texCood.Y /= maxYRange;

            //Consolidate out of range points
            if (texCood.X < 0.0f)
                texCood.X = 0.0f;
            else if (texCood.X > 1.0f)
                texCood.X = 1.0f;

            if (texCood.Y < 0.0f)
                texCood.Y = 0.0f;
            else if (texCood.Y > 1.0f)
                texCood.Y = 1.0f;

            //Vertical correction for OpenGL: OpenGL assumes 0 to be top of the image and 1 to be bottom
            texCood.Y = 1.0f - texCood.Y;

            return texCood;
        }
    }
}
