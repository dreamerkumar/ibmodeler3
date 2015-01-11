using System;
using System.Collections.Generic;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints
{
    internal class IndividualImgProcessedEventArgs : EventArgs
    {
        public double RotateImageBy { get; set; }

        public string ImageName { get; set; }
        
        public List<ClickPositionOnImage> ClickPositions { get; set; }

        public double[] Angles { get; set; }
    }
}
