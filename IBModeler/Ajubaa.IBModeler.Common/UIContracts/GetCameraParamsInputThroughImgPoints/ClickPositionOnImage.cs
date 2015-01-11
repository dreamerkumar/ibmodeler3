using System;

namespace Ajubaa.IBModeler.Common
{
    [Serializable]
    public class ClickPositionOnImage
    {
        public ClickPositionOnImageTypes PositionType { get; set; }

        public double ClickXPos { get; set; }

        public double ClickYPos { get; set; }

        public double AllowedWidth { get; set; }

        public double AllowedHeight { get; set; }

        public bool Processed { get; set; }
    }
}
