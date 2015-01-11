using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.TextureGenerator.BestImageSelectors
{
    internal class PolyNormalBasedImageSelector : IBestImageSelector
    {
        private readonly Vector3DCollection _meshNormals;
        private readonly ImageSpecifics[] _imageParameters;
        private readonly Point3DCollection _meshPositions;
        readonly List<ImageIndexAndDirection> _imageIndexAndDirections;
        readonly SpaceTransformationHandler[] _transformers;
        readonly CameraRatio _cameraRatio;

        public PolyNormalBasedImageSelector(Point3DCollection meshPositions, Vector3DCollection meshNormals, ImageSpecifics[] imageParameters, CameraRatio cameraRatio)
        {
            _meshNormals = meshNormals;
            _imageParameters = imageParameters;
            _cameraRatio = cameraRatio;
            _meshPositions = meshPositions;

            if (_meshNormals == null || _meshNormals.Count != meshPositions.Count)
                throw new Exception("GetTextureCoordinates cannot be called without setting the normals first");

            //calculate and keep ready all the required directions
            _imageIndexAndDirections = new List<ImageIndexAndDirection>();
            for (var ctr = 0; ctr < imageParameters.Count(); ctr++)
            {
                var imageIndexAndDirection = new ImageIndexAndDirection {Index = ctr, Direction = imageParameters[ctr].GetCameraDirection()};
                _imageIndexAndDirections.Add(imageIndexAndDirection);
            }

            //Have the transformation objects ready that will be used to transform the selected coordinates to a new coordinate system 
            //in which the z direction is the direction towards the camera
            _transformers = imageParameters.Select(x => new SpaceTransformationHandler(x.LookingAt, x.CameraLocation)).ToArray();
        }


        public BestImageParams GetBestImageParams(int ctr)
        {
            //Get the scalar angles between this vertex normal and the direction vectors
            var anglesAndImageIndices = _imageIndexAndDirections.Select(direction =>
                new ImageIndexAndAngle { Index = direction.Index, Angle = GetPositiveAngleOnXzPlane(_meshNormals[ctr], direction.Direction) }).ToList();

            var texCoodInAllowedRange = false;
            var selection = new BestImageParams();
            while (!texCoodInAllowedRange)
            {
                //get the min angle
                var minAngle = (from angle in anglesAndImageIndices select angle.Angle).Min();

                //get the indices which have this angle
                var indicesOfImagesToUse = (from t in anglesAndImageIndices where t.Angle == minAngle select t.Index).ToArray();

                var p = _meshPositions[ctr];

                if (indicesOfImagesToUse.Count() == 1)
                {
                    selection.IndexOfImageToUse = indicesOfImagesToUse[0];
                    var transformedPt = _transformers[selection.IndexOfImageToUse].GetTransformedPoint(p);
                    var zDistance = _imageIndexAndDirections.First(x => x.Index == selection.IndexOfImageToUse).Direction.Length - transformedPt.Z;
                    selection.TexCood = TexCoodCalculator.GetTexCood(_cameraRatio, transformedPt, zDistance);
                }
                else
                {
                    //angle is the same for a list of images
                    //choose the image to which this point is the closest
                    var zDistances = indicesOfImagesToUse.ToDictionary(index => index, index => GetZDistanceForImageIndex(_imageIndexAndDirections, _transformers, p, index));

                    var minIndex = GetIndexWithMinDistance(zDistances);
                    selection.IndexOfImageToUse = minIndex;
                    var transformedPt = _transformers[minIndex].GetTransformedPoint(p);
                    var zDistance = zDistances[minIndex];
                    selection.TexCood = TexCoodCalculator.GetTexCood(_cameraRatio, transformedPt, zDistance);
                }
                texCoodInAllowedRange = true;
                var selectedIndex = selection.IndexOfImageToUse;

                if (anglesAndImageIndices.Count <= 1 || _imageParameters[selectedIndex].AllowedXLimits == null ||
                    _imageParameters[selectedIndex].AllowedXLimits.Min < 0 || _imageParameters[selectedIndex].AllowedXLimits.Max <= 0) break;
                
                //if not in allowed range
                if (selection.TexCood.X < _imageParameters[selectedIndex].AllowedXLimits.Min || selection.TexCood.X > _imageParameters[selectedIndex].AllowedXLimits.Max)
                {
                    //remove this image from the list and try again
                    texCoodInAllowedRange = false;
                    anglesAndImageIndices.Remove(anglesAndImageIndices.First(x => x.Index == selection.IndexOfImageToUse));
                }
            }
            return selection;
        }

        private static int GetIndexWithMinDistance(Dictionary<int, double> zDistances)
        {
            var first = true;
            var minDistance = 0d;
            var minIndex = 0;
            foreach (var entry in zDistances)
            {
                if (first)
                {
                    minDistance = entry.Value;
                    minIndex = entry.Key;
                    first = false;
                }
                else
                {
                    var distance = entry.Value;
                    if (distance < minDistance)
                    {
                        minIndex = entry.Key;
                        minDistance = distance;
                    }
                }
            }
            return minIndex;
        }

        private static double GetZDistanceForImageIndex(IEnumerable<ImageIndexAndDirection> imageIndexAndDirections, SpaceTransformationHandler[] transformers, Point3D p, int index)
        {
            var trPt = transformers[index].GetTransformedPoint(p);
            var currZDistance = imageIndexAndDirections.First(x => x.Index == index).Direction.Length - trPt.Z;
            if (currZDistance < 0.0f)
                throw new Exception(
                    "Error: Incorrect camera orientation encountered. A camera position was found to be not in front of the model. It was either within the model or behind it");
            return currZDistance;
        }

        private static double GetPositiveAngleOnXzPlane(Vector3D v1, Vector3D v2)
        {
            //To get the angle along the x,z plane, we can ignore the components along the y axis
            v1.Y = 0.0f;
            v2.Y = 0.0f;

            //If the polygon is on the x,z plane, it will always return an angle of zero
            if (v1.Length == 0.0f || v2.Length == 0.0f)
                return 0.0f;

            //Get the cos angle value
            var cosOfAngle = Vector3D.DotProduct(v1, v2) / (v1.Length * v2.Length);

            //The range of acos is between 0 to pi
            return Math.Acos(cosOfAngle);
        }
    }
}
