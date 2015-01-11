using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
//using Ajubaa.Common._3DGeometryHelpers;

namespace Ajubaa.TextureGenerator.BestImageSelectors
{
    internal class PositionAngleBasedImageSelector : IBestImageSelector
    {
        private readonly Point3DCollection _meshPositions;
        readonly Vector3D[] _directions;
        readonly SpaceTransformationHandler[] _transformers;

        public PositionAngleBasedImageSelector(Point3DCollection meshPositions, IEnumerable<ImageSpecifics> imageParameters)
        {
            _meshPositions = meshPositions;

            _directions = imageParameters.Select(imageParameter => imageParameter.GetCameraDirection()).ToArray();

            //Have the transformation objects ready that will be used to transform the selected coordinates to a new coordinate system 
            //in which the z direction is the direction towards the camera
            _transformers = imageParameters.Select(x => new SpaceTransformationHandler(x.LookingAt, x.CameraLocation)).ToArray();
        }

        public BestImageParams GetBestImageParams(int ptIndex)
        {
            var meshPosition = _meshPositions[ptIndex];
            //var sphericalPos = SphericalCoordinateSystem.GetSphericalFromCartesian(meshPosition);
            //var angleInDegrees = 180*sphericalPos.Φ/Math.PI;

            var selection = new BestImageParams();
            //if((0 <= angleInDegrees && angleInDegrees <= 90 ) || (270 <= angleInDegrees && angleInDegrees <= 360))
            //{
            //    selection.IndexOfImageToUse = 0;
            //}
            //else
            //{
            //    selection.IndexOfImageToUse = 1;
            //}
            //selection.TransformedPt = _transformers[selection.IndexOfImageToUse].GetTransformedPoint(meshPosition);
            //selection.ZDistance = _directions[selection.IndexOfImageToUse].Length - selection.TransformedPt.Z;
            return selection;
        }
    }
}
