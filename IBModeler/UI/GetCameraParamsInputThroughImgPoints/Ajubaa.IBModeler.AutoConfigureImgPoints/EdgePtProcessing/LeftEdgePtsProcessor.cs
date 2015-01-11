using System;
using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class LeftEdgePtsProcessor : EdgePtsProcessorBase 
    {
        public LeftEdgePtsProcessor(Bitmap processedImg, Color backgroundColor)
            : base(processedImg, backgroundColor)
        {
        }

        #region Implementation of IEdgePtsProcessor

        public override Point GetEdgePt(int y)
        {
            if (XValuesForY[y] == null)
                XValuesForY[y] = GetFirstValidPixelOnLeft(y, ProcessedImg, BackgroundColor);

            return new Point(XValuesForY[y].Value, y);
        }

        #endregion

        public static int GetFirstValidPixelOnLeft(int yIndex, Bitmap image, Color backgroundColor)
        {
            if (image.GetPixel(0, yIndex) == backgroundColor)
            {
                var lastXIndex = image.Width - 1;
                for (var x = 1; x < lastXIndex; x++)
                {
                    if (image.GetPixel(x, yIndex) != backgroundColor)
                        return x - 1;//return left edge
                }
            }
            throw new Exception("Cannot determine edge point. Make sure image background is stripped properly.");
        }
    }
}