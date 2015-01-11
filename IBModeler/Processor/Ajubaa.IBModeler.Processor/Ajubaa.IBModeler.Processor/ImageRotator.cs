using System;
using System.Drawing;
using Ajubaa.IBModeler.Common;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.Processor
{
    public static class ImageRotator
    {
        /// <summary>
        /// derived from the below link
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/a1e62e03-d732-444d-bb3d-6e7907fd5e16/
        /// </summary>
        /// <param name="origImg"></param>
        /// <param name="angle"></param>
        /// <param name="bkColor"></param>
        /// <returns></returns>
        public static Bitmap RotateImg(Bitmap origImg, float angle, Color bkColor)
        {
            var newImg = new Bitmap(origImg.Width, origImg.Height, origImg.PixelFormat);
            var g = Graphics.FromImage(newImg);
            g.Clear(bkColor);

            g.RotateTransform(angle);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            
            //very important to add these last two parameters of width and height
            //otherwise we will get unpredictable results
            g.DrawImage(origImg, 0, 0, origImg.Width, origImg.Height);
            
            g.Dispose();
            
            return newImg;
        }

        /// <summary>
        /// Gets the angle by which the image should be rotated (using function RotateImg) 
        /// to make the disc edge horizontal
        /// </summary>
        /// <param name="clickInput"></param>
        /// <returns></returns>
        public static float GetRotationAngleToRealign(IList<ClickPositionOnImage> clickInput)
        {
            var leftOfDisc = ImageClickInputDetails.GetSpecificClickInput(clickInput, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);

            var rightOfDisc = ImageClickInputDetails.GetSpecificClickInput(clickInput, ClickPositionOnImageTypes.RightEndOfRotatingDisc);

            var xDiff = Math.Abs(rightOfDisc.ClickXPos - leftOfDisc.ClickXPos);

            var yDiff = Math.Abs(rightOfDisc.ClickYPos - leftOfDisc.ClickYPos);

            if (xDiff == 0 || yDiff == 0)
                return 0;

            var degrees = GetAngleInDegrees(xDiff, yDiff);

            //positive direction is clockwise from top left
            return rightOfDisc.ClickYPos < leftOfDisc.ClickYPos ? degrees : -degrees;
        }

        private static float GetAngleInDegrees(double positiveX, double positiveY)
        {
            var tanTheta = positiveY/positiveX;

            var theta = Math.Atan(tanTheta);

            return (float)(180.0*theta/Math.PI);
        }
    }
}
