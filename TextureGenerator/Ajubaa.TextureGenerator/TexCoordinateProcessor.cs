using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Media3D;
using Ajubaa.TextureGenerator.BestImageSelectors;

namespace Ajubaa.TextureGenerator
{
    public static class TexCoordinateProcessor
    {
        public static IEnumerable<TexCoodAndImgIndex> GetTextureCoordinates(ImageSpecifics[] imageParameters, CameraRatio cameraRatio, Vector3DCollection meshNormals, Point3DCollection meshPositions)
        {
            var imageSelector = new PolyNormalBasedImageSelector(meshPositions, meshNormals, imageParameters, cameraRatio);

            var unmergedCoordinates = new List<TexCoodAndImgIndex>();
            
            for (var ctr = 0; ctr < meshPositions.Count; ctr++)
            {
                var selection = imageSelector.GetBestImageParams(ctr);

                //Assign the u,v values
                unmergedCoordinates.Add(new TexCoodAndImgIndex { ImgIndex = selection.IndexOfImageToUse, TexCood = new Point(selection.TexCood.X, selection.TexCood.Y) });
            }

            return unmergedCoordinates;
        }
    }
}
