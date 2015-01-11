using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class LeftDistanceProcessor : DistanceProcessor
    {
        public LeftDistanceProcessor(Color backgroundColor, Bitmap image, int pixelsToFeed) 
        {
            EdgeProcessor = new LeftEdgePtsProcessor(image, backgroundColor);
            ConfigureEdgeLine(pixelsToFeed);
        }

        #region Overrides of DistanceProcessor

        public override double GetDistance(int y)
        {
            var xLine = EdgeLine.GetXValueForY(y);
            var xActual = (double)EdgeProcessor.GetEdgePt(y).X;
            var distance = xActual - xLine;
            return distance;
        }

        #endregion
    }
}