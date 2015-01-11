using System;
using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class RightEdgePtsProcessor : EdgePtsProcessorBase
    {
        public RightEdgePtsProcessor(Bitmap processedImg, Color backgroundColor)
            : base(processedImg, backgroundColor)
        {

        }

        #region Implementation of IEdgePtsProcessor

        public override Point GetEdgePt(int y)
        {
            if (XValuesForY[y] == null)
                XValuesForY[y] = GetFirstValidPixelOnRight(y, ProcessedImg, BackgroundColor);

            return new Point(XValuesForY[y].Value, y); 
        }

        #endregion

        public static int GetFirstValidPixelOnRight(int yIndex, Bitmap image, Color backgroundColor)
        {
            var lastXIndex = image.Width - 1;
            if (image.GetPixel(lastXIndex, yIndex) == backgroundColor)
            {
                for (var x = lastXIndex - 1; x > 0; x--)
                {
                    if (image.GetPixel(x, yIndex) != backgroundColor)
                        return x;//return right edge
                }
            }
            throw new Exception("Cannot determine edge point. Make sure image background is stripped properly.");
        }
    }
}