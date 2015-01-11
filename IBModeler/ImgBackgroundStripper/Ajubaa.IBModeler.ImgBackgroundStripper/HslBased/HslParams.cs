using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    /// <summary>
    ///Structure used to pass all the information for converting specified
    ///colors in an image to the background color
    /// </summary>
    public class HslParams
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public HslSelectionType HslSelectionType { get; set; }
        public byte LumSatVal { get; set; }
        public Color BackgroundColor { get;set;}
    }
}
