using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common;

namespace Ajubaa.TextureGenerator
{
    public static class TextureProcessor
    {
        public static TextureCoordinatesAndBitmap GenerateTexture(AddTextureInfo? texInfo, MeshGeometry3D mesh, string logFilePath)
        {
            if (texInfo == null || texInfo.Value.ImageInfos == null || texInfo.Value.ImageInfos.Length < 1
                || texInfo.Value.ImageInfos.Any(imageInfo => imageInfo.ImageBitmap == null) 
                || mesh == null || mesh.Positions == null || mesh.Positions.Count < 3)
                    return null;

            var addTextureInfo = texInfo.Value;

            var logger = new Logger(logFilePath);
            
            //if the normals are not present, add them
            if (mesh.Normals == null || mesh.Normals.Count != mesh.Positions.Count)
            {
                logger.Log("TextureProcessor: start setting normals for model.");
                NormalCalculator.SetNormalsForModel(mesh);
                logger.Log("TextureProcessor: End setting normals for model.");
            }

            var imageSpecifics = addTextureInfo.ImageInfos;

            var cameraRatio = addTextureInfo.CameraRatio;

            var unmergedCoordinates = TexCoordinateProcessor.GetTextureCoordinates(imageSpecifics, cameraRatio, mesh.Normals, mesh.Positions);

            var xCoodRangesForEachImage = new MinAndMaxTexCoodValueLimits[imageSpecifics.Count()];
            for (var ctr = 0; ctr < imageSpecifics.Count(); ctr++)
            {
                var valRange = from texCood in unmergedCoordinates where texCood.ImgIndex == ctr select texCood.TexCood.X;
                xCoodRangesForEachImage[ctr] = new MinAndMaxTexCoodValueLimits { Min = valRange.Min(), Max = valRange.Max() };
            }

            var xLimitsAtYIndices = ImgWidthOptimizer.OptimizeTexImageWidths(unmergedCoordinates, imageSpecifics);

            var mergedImage = TextureImageHandler.GetMergedImage(addTextureInfo.ImageInfos, xLimitsAtYIndices, logFilePath);

            var mergedImageSpecifics = new ImageSpecifics { Width = mergedImage.Width, Height = mergedImage.Height};

            //remap these coordinates and return
            var textureCoordinates = GetRemappedTextureCoordinates(unmergedCoordinates, imageSpecifics, xLimitsAtYIndices, mergedImageSpecifics);

            logger.Log("Texture generation complete.");
            return new TextureCoordinatesAndBitmap
            {
                TextureCoordinates = textureCoordinates,
                Bitmap = mergedImage,
                XCoodRangesForEachImage = xCoodRangesForEachImage
            };
        }

        /// <summary>
        /// Remap the texture coordinates based on the merged image
        /// </summary>
        /// <param name="unmappedCoods"></param>
        /// <param name="allImageSpecifics"></param>
        /// <param name="xLimitsAtYIndices"></param>
        /// <param name="mergedImageSpecifics"></param>
        /// <returns></returns>
        private static PointCollection GetRemappedTextureCoordinates(IEnumerable<TexCoodAndImgIndex> unmappedCoods, 
            ImageDimensions[] allImageSpecifics, OptimizedImgParams[] xLimitsAtYIndices, ImageSpecifics mergedImageSpecifics)
        {
            var remappedTexCoods = new PointCollection();
            foreach (var texCood in
                unmappedCoods.Select(unmappedCood => TextureImageHandler.GetMergedTexCoordinate(unmappedCood.ImgIndex, 
                    unmappedCood.TexCood, allImageSpecifics, xLimitsAtYIndices, mergedImageSpecifics)))
            {
                remappedTexCoods.Add(texCood);
            }
            return remappedTexCoods;
        }
    }
}