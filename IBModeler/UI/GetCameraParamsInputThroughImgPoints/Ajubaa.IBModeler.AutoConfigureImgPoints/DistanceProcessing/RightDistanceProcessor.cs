using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class RightDistanceProcessor : DistanceProcessor
    {
        public RightDistanceProcessor(Color backgroundColor, Bitmap image, int pixelsToFeed) 
        {
            EdgeProcessor = new RightEdgePtsProcessor(image, backgroundColor);
            ConfigureEdgeLine(pixelsToFeed);
        }

        #region Overrides of DistanceProcessor

        public override double GetDistance(int y)
        {
            var xLine = EdgeLine.GetXValueForY(y);
            var xActual = (double)EdgeProcessor.GetEdgePt(y).X;
            var distance = xLine - xActual;
            return distance;
        }

        #endregion
    }
}