using System.Drawing;

namespace Ajubaa.IBModeler.ImageAlterations
{
    public static class ExtraPadding
    {
        /// <summary>
        /// extra_padding.pdf
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageCorners"></param>
        /// <param name="invalidColor"></param>
        /// <returns></returns>
        public static double GetExtraPaddingPercent(Bitmap image, ImageCorners imageCorners, Color invalidColor)
        {
            var diffPercentLeft = GetDiffPercentOnLeft(imageCorners, image, invalidColor);

            var diffPercentRight = GetDiffPercentOnRight(imageCorners, image, invalidColor);

            //return the max between the two
            var max = diffPercentLeft > diffPercentRight? diffPercentLeft : diffPercentRight;

            return max + 5; //take a little extra to be on the safe side
        }

        private static double GetDiffPercentOnLeft(ImageCorners imageCorners, Bitmap image, Color invalidColor)
        {
            double x;
            var noOfModelPixelsAlongX = 0;
            for (x = imageCorners.Left-1; x >= 0; x--)
            {
                if (!CheckForModelPixelInYDirection(imageCorners, x, image, invalidColor))
                    break;
                noOfModelPixelsAlongX++;
            }
            return noOfModelPixelsAlongX * 100.0 / imageCorners.Width;
        }

        private static double GetDiffPercentOnRight(ImageCorners imageCorners, Bitmap image, Color invalidColor)
        {
            double x;
            var noOfModelPixelsAlongX = 0;
            for (x = imageCorners.Right + 1; x < image.Width; x++)
            {
                if (!CheckForModelPixelInYDirection(imageCorners, x, image, invalidColor))
                    break;
                noOfModelPixelsAlongX++;
            }
            return noOfModelPixelsAlongX * 100.0 / imageCorners.Width;
        }

        private static bool CheckForModelPixelInYDirection(ImageCorners imageCorners, double x, Bitmap image, Color invalidColor)
        {
            for (var y = imageCorners.Top; y <= imageCorners.Bottom; y++)
            {
                var pixel = image.GetPixel((int) x, (int) y);
                if (pixel.R != invalidColor.R || pixel.G != invalidColor.G || pixel.B != invalidColor.B)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
