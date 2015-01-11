using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public abstract class EdgePtsProcessorBase : IEdgePtsProcessor
    {
        protected Bitmap ProcessedImg;
        protected Color BackgroundColor { get; set; }

        protected int?[] XValuesForY;

        public EdgePtsProcessorBase(Bitmap processedImg, Color backgroundColor)
        {
            ProcessedImg = processedImg;
            XValuesForY = new int?[processedImg.Height];
            BackgroundColor = backgroundColor;
        }

        #region Implementation of IEdgePtsProcessor

        public abstract Point GetEdgePt(int y);

        public Point[] GetEdgePtsAtImgBottom(int count)
        {
            var pts = new Point[count];
            for (int y = ProcessedImg.Height - 1, ctr = 0; y > 0 && ctr < count; y--, ctr++)
            {
                pts[ctr] = GetEdgePt(y);
            }
            return pts;
        }
        #endregion
    }
}
