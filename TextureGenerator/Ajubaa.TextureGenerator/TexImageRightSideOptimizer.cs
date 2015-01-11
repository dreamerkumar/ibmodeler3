using System.Drawing;

namespace Ajubaa.TextureGenerator
{
    internal class TexImageRightSideOptimizer
    {
        internal static void ExtendRightTextureAtYValue(Bitmap img, int y, int consecutiveValidSidePixelsToOverlay)
        {
            var nextRange = IndexRangeProcessor.GetNextIndexRangeFromRight(img.Width, img, y, validInvalidFlag: false);

            while (nextRange != null)
            {
                if (consecutiveValidSidePixelsToOverlay > 0)
                {
                    var validRange = IndexRangeProcessor.GetNextIndexRangeFromRight(nextRange.MinIndex, img, y, validInvalidFlag: true);
                    if (validRange != null && (validRange.MaxIndex + 1 - validRange.MinIndex) > consecutiveValidSidePixelsToOverlay)
                    {
                        nextRange.MinIndex = nextRange.MinIndex - consecutiveValidSidePixelsToOverlay;
                    }
                }
                var indexOfPixelToOverlay = nextRange.MinIndex - 1;
                if (0 <= indexOfPixelToOverlay && indexOfPixelToOverlay < img.Width)
                {
                    var colorForOverlay = img.GetPixel(indexOfPixelToOverlay, y);
                    TexImageOptimizer.ColorPixels(nextRange.MinIndex, nextRange.MaxIndex, colorForOverlay, img, y);
                }
                nextRange = IndexRangeProcessor.GetNextIndexRangeFromRight(nextRange.MinIndex, img, y, validInvalidFlag: false);
            }
        }

    }
}
