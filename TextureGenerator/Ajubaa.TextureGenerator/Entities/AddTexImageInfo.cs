using System.Drawing;

namespace Ajubaa.TextureGenerator
{
    public class AddTexImageInfo : ImageSpecifics
    {
        public Bitmap ImageBitmap { get; set; }

        public override int Height
        {
            get { return ImageBitmap.Height; }
        }

        public override int Width
        {
            get { return ImageBitmap.Width; }
        }
    };
}