using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Ajubaa.Common;
using Point = System.Windows.Point;

namespace Ajubaa.TextureGenerator
{
    public class TextureImageHandler
    {
        /// <summary>
        /// does some sanity checks and then calls the underlying method to get the merged bitmap
        /// </summary>
        /// <param name="images"></param>
        /// <param name="xLimitsAtYIndices"></param>
        /// <param name="logFilePath"></param>
        /// <returns></returns>
        public static Bitmap GetMergedImage(AddTexImageInfo[] images, OptimizedImgParams[] xLimitsAtYIndices, string logFilePath)
        {
            if (images.Any(img => img.ImageBitmap.Height < 4 || img.ImageBitmap.Width < 4))
            {
                throw new Exception("Image dimensions cannot be less than 4 pixels in width and height.");
            }

            //Merge the front and back texture images into one
            return GetMergedImagePrivate(images, xLimitsAtYIndices,logFilePath);
        }

        private static Bitmap GetMergedImagePrivate(AddTexImageInfo[] images, OptimizedImgParams[] xLimitsAtYIndices, string logFilePath)
        {
            var logger = new Logger(logFilePath);

            #region some previous notes
            //The pixel data of the two images has to be merged into one
            //First decide if we want to lay them side by side or one below the other
            //If the heights are more than the widths, then we will place them side by side
            //Else we will place them one below the other

            //	When the two texture images (front and back) are one above the other in the combined texture file
            //	for the model, the sides of the model gets screwed up, at the point where the front texture image
            //	ends and the bottom one begins. Especially prominent when the vertices of the model are more torn
            //	apart. To address this, we should always go for the front and back images lying side by side for the
            //	combined texture.
            //	if(maxHeight > maxWidth)
            //		mImageDataLayout = ImgDataLayout.HorizontalLayout;
            //	else
            //		mImageDataLayout = ImgDataLayout.VerticalLayout;
            #endregion

            var mergedImageDimensions = GetMergedImageDimensions(xLimitsAtYIndices);
       
            //logger.Log("TextureImageHandler: Start extending texture for all images");
            //foreach(var frontImg in images)
            //    TexImageOptimizer.ExtendTextureToAllSidesOfImage(frontImg.ImageBitmap, 3);
            //logger.Log("TextureImageHandler: End extending texture for all images");

            return GetMergedBitmap(images, xLimitsAtYIndices, mergedImageDimensions.Width, mergedImageDimensions.Height);
        }

        /// <summary>
        /// todo
        /// There is a thin layer of a different color at the border, and extending that does not give a great look, 
        /// although we are doing slightly better in terms of texture output because of this code
        /// </summary>
        /// <returns></returns>
        private static ImageDimensions GetMergedImageDimensions(OptimizedImgParams[] xLimitsAtYIndices)
        {
            var imageCount = xLimitsAtYIndices.GetLength(0);
            var height = xLimitsAtYIndices[0].XLimitsAtYIndices.Count();

            var totalWidth = 0;
            for(var imgIndex = 0; imgIndex < imageCount; imgIndex++)
            {
                //get the max width for each image
                var maxWidthForCurrImg = xLimitsAtYIndices[imgIndex].OptimizedImgWidth;
                totalWidth = totalWidth + maxWidthForCurrImg;
            }

            var dimensions = new ImageDimensions
            {
                Height = height, Width = totalWidth
            };

            #region make sure that the dimensions are a power of 2 (2,4,8,16,..256,512, 1024..)
            //dont know why but this is required. I confirmed by testing and getting bad results without this
            var powerOfTwoValues = 2;
            while (powerOfTwoValues < dimensions.Height)
            {
                powerOfTwoValues *= 2;
            }
            dimensions.Height = powerOfTwoValues;
            powerOfTwoValues = 2;
            while (powerOfTwoValues < dimensions.Width)
            {
                powerOfTwoValues *= 2;
            }
            dimensions.Width = powerOfTwoValues;
            #endregion

            return dimensions;
        }

        public static Bitmap GetMergedBitmap(AddTexImageInfo[] images, OptimizedImgParams[] xLimits, 
            int mergedImgWidth, int mergedImgHeight)
        {
            var heightOfEachImage = images[0].ImageBitmap.Height;
            if (images.Any(image => image.ImageBitmap.Height != heightOfEachImage))
            {
                throw new Exception("GetMergedBitmap: All images should be of the same height");
            }
            
            var mergedBitmap = new Bitmap(mergedImgWidth, mergedImgHeight);
            var g = Graphics.FromImage(mergedBitmap);
            g.Clear(Color.FromArgb(200, 200, 200));
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.Dispose();

            var previousImgWidths = 0;
            for (var index = 0; index < images.Length; index++) 
            {
                for (var y = 0; y < heightOfEachImage; y++)
                {
                    var image = images[index];

                    //in the merged image, the original bitmaps are stacked one next to the other
                    //the widths of the stacked images are cropped to their optimizedWidth

                    //move forward by the previous image width
                    var mergedImageXPos = previousImgWidths;

                    if (xLimits[index].XLimitsAtYIndices[y].Min >= xLimits[index].OptimizedImgMin)
                    {
                        //move forward by the cropped min
                        mergedImageXPos += xLimits[index].XLimitsAtYIndices[y].Min - xLimits[index].OptimizedImgMin;

                        //Copy the pixel data from the image
                        for (int x = xLimits[index].XLimitsAtYIndices[y].Min, noOfPixels = 1;
                             noOfPixels <= xLimits[index].XLimitsAtYIndices[y].Width;
                             noOfPixels++, x++)
                        {
                            var pixel = image.ImageBitmap.GetPixel(x, y);
                            mergedBitmap.SetPixel(mergedImageXPos, y, pixel);
                            mergedImageXPos++;
                        }
                    }
                }
                previousImgWidths += xLimits[index].OptimizedImgWidth;
            }

            //fill up blank spaces by extending edges
            TexImageOptimizer.ExtendTextureToAllSidesOfImage(mergedBitmap, 0);

            return mergedBitmap;
        }

        public static Point GetMergedTexCoordinate(int indexOfImageToUse, Point currentTexCood, 
            ImageDimensions[] imgDimensions, OptimizedImgParams[] xLimitsAtYIndices, ImageDimensions mergedImgDimensions)
        {
            var netPixelWidth = GetNetPixelWidth(currentTexCood, imgDimensions, indexOfImageToUse, xLimitsAtYIndices);

            var mergedImgTexCood = new Point
            {
                X = netPixelWidth / mergedImgDimensions.Width,
                Y = (imgDimensions[indexOfImageToUse].Height * currentTexCood.Y) /
                        mergedImgDimensions.Height
            };

            return mergedImgTexCood;
        }

        private static double GetNetPixelWidth(Point currentTexCood, ImageDimensions[] imgDimensions, int indexOfImageToUse, OptimizedImgParams[] xLimitsAtYIndices)
        {
            var prevImagesPixelWidths = 0.0;
            for (var ctr = 0; ctr < indexOfImageToUse; ctr++ )
            {
                prevImagesPixelWidths += xLimitsAtYIndices[ctr].OptimizedImgWidth;
            }

            var origCurrImgPixelWidth = imgDimensions[indexOfImageToUse].Width*currentTexCood.X;

            //subtract the pixels removed from the left for this image
            var newCurrImgPixelWidth = origCurrImgPixelWidth - xLimitsAtYIndices[indexOfImageToUse].OptimizedImgMin;

            return prevImagesPixelWidths + newCurrImgPixelWidth;
        }
    }
}