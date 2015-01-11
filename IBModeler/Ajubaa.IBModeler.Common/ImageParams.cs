using System.Windows.Media.Media3D;
using System.Drawing;

namespace Ajubaa.IBModeler.Common
{
    public class ImageParams
    {
        public Color InvalidColor = Color.FromArgb(200,200,200);

        public Point3D cameraLocation = new Point3D(0.0f, 0.0f, 0.0f);

        public Point3D lookingAt = new Point3D(0.0f, 0.0f, 0.0f); 

        public bool CameraAtInfinity = true;

        public Bitmap Image { get; set; }

        public CameraRatio CameraRatio { get; set; }

        public void Dispose()
        {
            if(Image != null)
            {
                Image.Dispose();
            }
        }
    }
}
