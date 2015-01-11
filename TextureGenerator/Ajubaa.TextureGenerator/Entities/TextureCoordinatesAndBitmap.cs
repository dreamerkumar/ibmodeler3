using System.Drawing;
using System.Windows.Media;

namespace Ajubaa.TextureGenerator
{
    public class TextureCoordinatesAndBitmap
    {
        public Bitmap Bitmap { get; set; }

        public PointCollection TextureCoordinates { get; set; }

        /// <summary>
        /// it gives the min and max values of x tex cood value that was used to create the final merged texture
        /// these limits give the upper limits while determining a smaller x tex cood limits that can be specified for each image
        /// </summary>
        public MinAndMaxTexCoodValueLimits[] XCoodRangesForEachImage { get; set; }
    }
}
