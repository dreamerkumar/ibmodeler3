using System;
using System.Drawing;
using System.Linq;
using Ajubaa.Common;
using System.Collections.Generic;

namespace Ajubaa.TextureGenerator
{
    public class ImgWidthOptimizer
    {
        public static string DebugFolderLocation;

        public static OptimizedImgParams[] OptimizeTexImageWidths(IEnumerable<TexCoodAndImgIndex> texCoods, AddTexImageInfo[] imgSpecifics)
        {
            var noOfImages = imgSpecifics.Length;
            var height = imgSpecifics[0].ImageBitmap.Height;
            if (imgSpecifics.Any(x => x.Height != height)) throw new Exception("OptimizeTexImageWidths: Height should be same for all images.");

            var indices = GetInitializedXLimitsAtYIndices(height, noOfImages);
            
            for (var imgIndex = 0; imgIndex < noOfImages; imgIndex++)
            {
                var img = imgSpecifics[imgIndex];
                var width = img.ImageBitmap.Width;

                var orderedDistinctYs = GetOrderedDistinctYs(texCoods, imgIndex);
                var totalDistinctYCt = orderedDistinctYs.Count();

                SetOverallMinAndMaxXIndices(imgIndex, texCoods, indices, width);

                if (totalDistinctYCt < 2)
                {
                    //cannot optimize this image
                    SetUnOptimizedValues(imgIndex, img, indices);
                    continue;
                }

                var yRangesCt = totalDistinctYCt - 1;
                for (var ctr = 0; ctr < yRangesCt; ctr++)
                {
                    SetOptimizedIndicesForYRange(height, imgIndex, texCoods, orderedDistinctYs, indices, ctr, width);
                }

                HolePlugger.PlugSideHoles(indices[imgIndex], 30);

                if (!string.IsNullOrWhiteSpace(DebugFolderLocation))
                {
                    DebugSaveIndicesForImageFile(indices[imgIndex], img);
                }

            }
            return indices;
        }
        
        private static void SetOverallMinAndMaxXIndices(int imgIndex, IEnumerable<TexCoodAndImgIndex> texCoods, OptimizedImgParams[] indices, int width)
        {
            var xValues = (from texCood in texCoods
                           where texCood.ImgIndex == imgIndex
                           select texCood.TexCood.X);
            var min = xValues.Min();
            var max = xValues.Max();
            indices[imgIndex].OptimizedImgMin = GetPixelForTexCood(min, width);
            indices[imgIndex].OptimizedImgMax = GetPixelForTexCood(max, width);
        }

        private static void SetOptimizedIndicesForYRange(int height, int imgIndex, IEnumerable<TexCoodAndImgIndex> texCoods, 
            double[] orderedDistinctYs, OptimizedImgParams[] indices, int ctr, int width)
        {
            //get a set of lower and upper value of texture coordinate
            var yMin = orderedDistinctYs[ctr];
            var yMax = orderedDistinctYs[ctr + 1];

            //get the index of applicable x pixels for these values
            var minMaxXForYPair = GetMinMaxXCoodsForTwoYs(texCoods, yMin, yMax, imgIndex);
            var minMaxXIndex = new MinAndMaxIndices
            {
                Min = GetPixelForTexCood(minMaxXForYPair.Min, width), Max = GetPixelForTexCood(minMaxXForYPair.Max, width)
            };

            //get the applicable y pixel values for these lower and upper y limits
            var minMaxYIndex = new MinAndMaxIndices
            {
                Min = GetPixelForTexCood(yMin, height), Max = GetPixelForTexCood(yMax, height)
            };

            //set their values based on the min and max x. take into account overlapping values between two different ranges
            for (var ctr1 = minMaxYIndex.Min; ctr1 <= minMaxYIndex.Max; ctr1++)
            {
                if (indices[imgIndex].XLimitsAtYIndices[ctr1].InitializedToZeroZero)
                    indices[imgIndex].XLimitsAtYIndices[ctr1] = new MinAndMaxIndices { Min = minMaxXIndex.Min, Max = minMaxXIndex.Max };
                else
                {
                    if (indices[imgIndex].XLimitsAtYIndices[ctr1].Min > minMaxXIndex.Min)
                    {
                        indices[imgIndex].XLimitsAtYIndices[ctr1].Min = minMaxXIndex.Min;
                    }
                    if (indices[imgIndex].XLimitsAtYIndices[ctr1].Max < minMaxXIndex.Max)
                    {
                        indices[imgIndex].XLimitsAtYIndices[ctr1].Max = minMaxXIndex.Max;
                    }
                }
            }
        }

        /// <summary>
        /// gets the pixel that represents a specific texture coordinate value
        /// </summary>
        /// <param name="texCoodValue"></param>
        /// <param name="imgPixelCt"></param>
        /// <returns></returns>
        public static int GetPixelForTexCood(double texCoodValue, int imgPixelCt)
        {
            var dblPixelValue = texCoodValue*imgPixelCt;
            
            //this call ensures that only the integer part of a double value is returned
            var intValue = CommonFunctions.GetIntFromDouble(dblPixelValue);

            //a value of 2.0 is pixel index 1
            if(intValue > 0 && (double)intValue == dblPixelValue)
                intValue--;

            return intValue;
        }

        /// <summary>
        /// gets the min and max x coordinate values for a pair of y values
        /// </summary>
        /// <param name="texCoods"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="imgIndex"></param>
        /// <returns></returns>
        public static MinAndMaxTexCoodValueLimits GetMinMaxXCoodsForTwoYs(IEnumerable<TexCoodAndImgIndex> texCoods, double value1, double value2, int imgIndex)
        {
            var xValues = GetFilteredXCoodsForTwoYs(texCoods, value1, value2, imgIndex);
            return new MinAndMaxTexCoodValueLimits {Min = xValues.Min(), Max = xValues.Max()};
        }

        /// <summary>
        /// gets the list applicable x values for corresponding pair of y values
        /// </summary>
        /// <param name="texCoods"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="imgIndex"></param>
        /// <returns></returns>
        public static double[] GetFilteredXCoodsForTwoYs(IEnumerable<TexCoodAndImgIndex> texCoods, double value1, double value2, int imgIndex)
        {
            return (from cood in texCoods
                    where cood.ImgIndex == imgIndex &&
                    (cood.TexCood.Y == value1 || cood.TexCood.Y == value2)
                    select cood.TexCood.X).ToArray();
        }

        /// <summary>
        /// gets ordered list of y values from the texture coordinates
        /// </summary>
        /// <param name="texCoods"></param>
        /// <param name="imgIndex"></param>
        /// <returns></returns>
        public static double[] GetOrderedDistinctYs(IEnumerable<TexCoodAndImgIndex> texCoods, int imgIndex)
        {
            return (from cood in texCoods
                    where cood.ImgIndex == imgIndex
                    orderby cood.TexCood.Y
                    select cood.TexCood.Y).Distinct().ToArray();
        }

        /// <summary>
        /// initializes all indices to min max 0
        /// </summary>
        /// <param name="height"></param>
        /// <param name="noOfImages"></param>
        /// <returns></returns>
        private static OptimizedImgParams[] GetInitializedXLimitsAtYIndices(int height, int noOfImages)
        {
            var xLimitsAtYIndices = new OptimizedImgParams[noOfImages];
            
            for (var imgIndex = 0; imgIndex < noOfImages; imgIndex++)
            {
                xLimitsAtYIndices[imgIndex] = new OptimizedImgParams
                {
                    XLimitsAtYIndices = new MinAndMaxIndices[height]
                };
                //initialize to zero
                for (var y = 0; y < height; y++)
                    xLimitsAtYIndices[imgIndex].XLimitsAtYIndices[y] = new MinAndMaxIndices {Min = 0, Max = 0, InitializedToZeroZero = true};
            }
            return xLimitsAtYIndices;
        }

        /// <summary>
        /// set indices in such a way that no pixels are removed from either side
        /// </summary>
        /// <param name="imgIndex"></param>
        /// <param name="img"></param>
        /// <param name="indices"></param>
        public static void SetUnOptimizedValues(int imgIndex, AddTexImageInfo img, OptimizedImgParams[] indices)
        {
            for (var y = 0; y < img.Height; y++)
                indices[imgIndex].XLimitsAtYIndices[y] = new MinAndMaxIndices { Min = 0, Max = img.Width - 1 };
        }

        /// <summary>
        /// saves modified individual textures to separate files in the provided folder location
        /// helps in debugging
        /// </summary>
        /// <param name="optimizedImgParams"></param>
        /// <param name="img"></param>
        private static void DebugSaveIndicesForImageFile(OptimizedImgParams optimizedImgParams, AddTexImageInfo img)
        {
            if (string.IsNullOrWhiteSpace(DebugFolderLocation)) return;

            var sourceBitmap = img.ImageBitmap;
            var debugImage = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);
            for (var y = 0; y < optimizedImgParams.XLimitsAtYIndices.Length; y++)
            {
                for (var x = optimizedImgParams.XLimitsAtYIndices[y].Min; x <= optimizedImgParams.XLimitsAtYIndices[y].Max; x++)
                {
                    var pixelColor = sourceBitmap.GetPixel(x, y);
                    debugImage.SetPixel(x, y, pixelColor);
                }
            }
            debugImage.Save(string.Format(@"{0}\ImgWidthOptimizer_{1}.png", DebugFolderLocation, DateTime.Now.Ticks));
        }

    }
}
