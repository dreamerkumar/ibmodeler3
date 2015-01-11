using System;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    internal static class ColorVariationPercentBasedStripper
    {
        public static int Process(Bitmap image, double variationPercent, Color backgroundColor, Point imgLocationOfBaseColor = new Point())
        {
            var colorToCompare = image.GetPixel(imgLocationOfBaseColor.X, imgLocationOfBaseColor.Y);

            return Process(image, variationPercent, backgroundColor, colorToCompare);
        }

        public static int Process(Bitmap image, double variationPercent, Color backgroundColor, Color colorToCompare)
        {
            var pixelsModified = 0;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var variation = GetVariationPercent(image, x, y, colorToCompare);

                    if(variation <= variationPercent)
                    {
                        image.SetPixel(x, y, backgroundColor);
                        pixelsModified++;
                    }
                }
            }
            return pixelsModified;
        }

        internal static double GetVariationPercent(Bitmap image, int x, int y, Color compareColor)
        {
            var currColor = image.GetPixel(x, y);

            var variationR = Math.Abs(currColor.R - compareColor.R) / 255.0;
            var variationG = Math.Abs(currColor.G - compareColor.G) / 255.0;
            var variationB = Math.Abs(currColor.B - compareColor.B) / 255.0;

            return (variationR + variationG + variationB) *100.0/ 3.0;
        }
    }
}
