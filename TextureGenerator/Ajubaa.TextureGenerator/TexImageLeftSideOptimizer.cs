using System.Drawing;

namespace Ajubaa.TextureGenerator
{
    internal class TexImageLeftSideOptimizer
    {
        internal static void ExtendLeftTextureAtYValue(Bitmap img, int y, int consecutiveValidSidePixelsToOverlay)
        {
            var nextRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(-1, img, y, validInvalidFlag: false);

            while (nextRange != null)
            {
                if (consecutiveValidSidePixelsToOverlay > 0)
                {
                    var validRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(nextRange.MaxIndex, img, y, validInvalidFlag: true);
                    if (validRange != null && (validRange.MaxIndex + 1 - validRange.MinIndex) > consecutiveValidSidePixelsToOverlay)
                    {
                        nextRange.MaxIndex = nextRange.MaxIndex + consecutiveValidSidePixelsToOverlay;
                    }
                }
                var indexOfPixelToOverlay = nextRange.MaxIndex + 1;
                if (0 <= indexOfPixelToOverlay && indexOfPixelToOverlay < img.Width)
                {
                    var colorForOverlay = img.GetPixel(indexOfPixelToOverlay, y);
                    TexImageOptimizer.ColorPixels(nextRange.MinIndex, nextRange.MaxIndex, colorForOverlay, img, y);
                }
                nextRange = IndexRangeProcessor.GetNextIndexRangeFromLeft(nextRange.MaxIndex, img, y, validInvalidFlag: false);
            }
        }
    }
}
