using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class AutoConfigureImgPointsParams
    {
        public BackgroundStrippingParams BackgroundStrippingParams { get; set; }

        public System.Drawing.Color BackgroundColor { get; set; }

        public int MinPixelsForBaseDisc { get; set; }

        public AutoConfigureImgPointsParams()
        {
            MinPixelsForBaseDisc = 10;
            MinPixelsForBaseDiscEndOfEdge = 10;
            ResizeWidth = 500;
            ResizeHeight = 500;
            MarkerProcessingParams = new MarkerProcessingParams();
        }

        public double MinPixelsForBaseDiscEndOfEdge { get; set; }

        public int? ResizeWidth { get; set; }

        public int? ResizeHeight { get; set; }

        public MarkerProcessingParams MarkerProcessingParams { get; set; }
    }
}