using System.Windows;

namespace Ajubaa.TextureGenerator
{
    /// <summary>
    /// entity that defines the parameters of the photo that should be used as
    /// texture for a given 3d position
    /// </summary>
    internal class BestImageParams
    {
        public int IndexOfImageToUse { get; set; }

        public Point TexCood { get; set; }
    }
}
