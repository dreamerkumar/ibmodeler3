using System;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    /// <summary>
    /// this one based on comparing the a photo taken without the model
    /// the idea is the finally save the pixels that are actually different in the second model
    /// because the background pixels will remain the same
    /// due to camera limitations found out that the pixel colors had changed for the background between the two images
    /// this is true for even areas beyond where shadows were created
    /// couple of tests are attached 
    /// there might be some room for improvement if we convert to colors to hsl and then compare maybe
    /// I am adding this on 4-Mar-2012
    /// </summary>
    public static class ComparisonWithImgWithoutModelBasedStripper
    {
        public static ImgStripStatus Process(Bitmap imgWithoutModel, Bitmap img, ComparisonWithImgWithoutModelParams @params, Color backgroundColor)
        {
            var imgStripStatus = new ImgStripStatus(img.Width, img.Height);

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var imgWithoutModelColor = imgWithoutModel.GetPixel(x, y);
                    var imgColor = img.GetPixel(x, y);

                    var variationR = Math.Abs(imgColor.R - imgWithoutModelColor.R);
                    var variationG = Math.Abs(imgColor.G - imgWithoutModelColor.G);
                    var variationB = Math.Abs(imgColor.B - imgWithoutModelColor.B);
                    
                    var similar = variationR <= @params.AllowedVariationInRgb && variationG <= @params.AllowedVariationInRgb && variationB <= @params.AllowedVariationInRgb;

                    //if the color is the same as the color on the image without model then it is the background 
                    if (similar)
                    {
                        imgStripStatus.SetInvalid(x, y);
                    }
                }
            }
            return imgStripStatus;
        }
    }
}