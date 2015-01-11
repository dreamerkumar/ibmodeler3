using System.Drawing;

namespace Ajubaa.TextureGenerator
{
    public class TextureInformation
    {
        public ImageSpecifics[] ImageSpecifics { get; set; }

        public Bitmap BitmapTextureImg { get; set; }

        public ImageSpecifics MergedImageSpecifics { get; set; }

        public CameraRatio CameraRatio { get; set; }
    }
}
