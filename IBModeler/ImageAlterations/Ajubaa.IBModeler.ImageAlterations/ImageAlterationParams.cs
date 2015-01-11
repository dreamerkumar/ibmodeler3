using System;

namespace Ajubaa.IBModeler.ImageAlterations
{
    public class ImageAlterationParams
    {
        public double MinImageHeightRatio { get; set; }

        public double PercentExtraWidth { get; set; }

        public ResizeType ResizeType { get; set; }

        public int MoldPtDensity { get; set; }

        public int SpecificResizeWidth { get; set; }

        public int SpecificResizeHeight { get; set; }

        public System.Drawing.Color InvalidColor { get; set; }

        public double BottomPaddingPercent { get; set; }

        public string ImageFolder { get; set; }
    }
}
