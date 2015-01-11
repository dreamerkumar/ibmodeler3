using System;
using System.Drawing;

namespace Ajubaa.TextureGenerator
{
    public static class TexImageOptimizer
    {
        public static void ExtendTextureToAllSidesOfImage(Bitmap img, int consecutiveValidSidePixelsToOverlay)
        {
            for (var y = 0; y < img.Height; y++)
            {
                ExtendTextureAtYValue(y, img, consecutiveValidSidePixelsToOverlay);
            }
        }

        private static void ExtendTextureAtYValue(int y, Bitmap img, int consecutiveValidSidePixelsToOverlay)
        {
            TexImageLeftSideOptimizer.ExtendLeftTextureAtYValue(img, y, consecutiveValidSidePixelsToOverlay);
            TexImageRightSideOptimizer.ExtendRightTextureAtYValue(img, y, consecutiveValidSidePixelsToOverlay);
        }

        internal static void ColorPixels(int startIndex, int endIndex, Color colorToSet, Bitmap img, int y)
        {
            if (startIndex > endIndex)
            {
                var temp = startIndex;
                startIndex = endIndex;
                endIndex = temp;
            }
            if(startIndex < 0 || endIndex > img.Width -1)
                throw new Exception(String.Format("The startIndex:{0} and endIndex:{1} are invalid for image of width:{2}", startIndex, endIndex, img.Width));
                
            for (var ctr = startIndex; ctr <= endIndex; ctr++)
            {
                img.SetPixel(ctr, y, colorToSet);
            }
        }

        internal static bool IsValidPixel(int x, int y, Bitmap img)
        {
            return !IsInvalidPixel(x, y, img);
        }

        internal static bool IsInvalidPixel(int x, int y, Bitmap img)
        {
            var pixelColor = img.GetPixel(x, y);
            return IsInvalidPixel(pixelColor);
        }

        internal static bool IsInvalidPixel(Color pixelColor)
        {
            return (pixelColor.R == 200 && pixelColor.G == 200 && pixelColor.B == 200);
        }
    }
}
