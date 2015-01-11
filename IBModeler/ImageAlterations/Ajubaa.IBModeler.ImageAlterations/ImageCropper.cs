using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.ImageAlterations
{
    public static class ImageCropper
    {
        public static Bitmap GetCroppedImage(ImageClickInputDetails clickInputDetails, Bitmap image, ImageAlterationParams imageAlterationParams)
        {
            var imgCornersExtra = GetImgCornersWithExtraPadding(clickInputDetails, image, imageAlterationParams);

            if (imgCornersExtra.Left >= 0 && imgCornersExtra.Right < image.Width)
            {
                //crop image from all side
                var rectangle = new Rectangle((int)imgCornersExtra.Left, (int)imgCornersExtra.Top, (int)imgCornersExtra.Width, (int)imgCornersExtra.Height);
                return image.Clone(rectangle, image.PixelFormat);
            }
            else
            {
                //prepare a new image
                var newImg = new Bitmap(Convert.ToInt32(imgCornersExtra.Width), Convert.ToInt32(imgCornersExtra.Height), image.PixelFormat);
                var g = Graphics.FromImage(newImg);
                g.Clear(imageAlterationParams.InvalidColor);
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;

                //when overlaying the image, give the top coordinate in the negative direction so that it gets cropped to the top pixel
                var topOfImageForOverlay = -imgCornersExtra.Top;

                if (imgCornersExtra.Left > 0)
                {
                    //needs trimming from the left
                    var rectangle = new Rectangle((int)imgCornersExtra.Left, 0, (int)(image.Width - imgCornersExtra.Left - 1), image.Height);
                    image = image.Clone(rectangle, image.PixelFormat);
                    g.DrawImage(image, 0, (int)topOfImageForOverlay, image.Width, image.Height);
                }
                else
                {
                    var moveForwardBy = -imgCornersExtra.Left;
                    g.DrawImage(image, (int)moveForwardBy, (int)topOfImageForOverlay, image.Width, image.Height);
                }

                g.Dispose();
                return newImg;
            }
        }

        private static ImageCorners GetImgCornersWithExtraPadding(ImageClickInputDetails clickInputs, Bitmap image, ImageAlterationParams imageAlterationParams)
        {
            //image corners with respect to the dimensions of click area
            var clickImgCorners = ImageCorners.GetImageCornersFromClickInputs(clickInputs, imageAlterationParams.MinImageHeightRatio, imageAlterationParams.BottomPaddingPercent);

            //image corners in actual image dimension
            var actualImgCorners = ImageCorners.GetActualImageCorners(clickImgCorners, image, clickInputs.ClickPositionListForImages);

            //image corners with extra padding on on each side of disc
            return GetModifiedImgCornersForExtraWidth(actualImgCorners, imageAlterationParams.PercentExtraWidth);
        }

        private static ImageCorners GetModifiedImgCornersForExtraWidth(ImageCorners origCorners, double percentExtraWidth)
        {
            var extraOnEachSide = percentExtraWidth * origCorners.Width / 100.0;

            //return modified left and right based on extraOnEachSide
            return new ImageCorners
            {
                Left = origCorners.Left - extraOnEachSide,
                Right = origCorners.Right + extraOnEachSide,
                Top = origCorners.Top,
                Bottom = origCorners.Bottom
            };
        }
    }
}
