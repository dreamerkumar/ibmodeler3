using System.Windows.Media.Media3D;

namespace Ajubaa.TextureGenerator
{
    public class ImageSpecifics : ImageDimensions
    {
        public Point3D CameraLocation { get; set; }

        public Point3D LookingAt { get; set; }

        /// <summary>
        /// if specified, controls the miniumum and max value of x texture coordinates
        /// if the calculated value exceeds this limit then another image should be chosen
        /// </summary>
        public MinAndMaxTexCoodValueLimits AllowedXLimits { get; set; }

        /// <summary>
        /// The direction will be from the point at which the camera was looking at 
        /// to the point at which the camera was located
        /// </summary>
        /// <returns></returns>
        public Vector3D GetCameraDirection()
        {
            return new Vector3D
            {
                X = (CameraLocation.X - LookingAt.X),
                Y = (CameraLocation.Y - LookingAt.Y),
                Z = (CameraLocation.Z - LookingAt.Z)
            };
        }
    }
}
