using System;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    [Serializable]
    public class ScreenBasedParams
    {
        public ScreenColorTypes ScreenColorTypes { get; set; }
        public byte MinColorOffset { get; set; }
        public byte MaxDiffPercent { get; set; }
    }
}
