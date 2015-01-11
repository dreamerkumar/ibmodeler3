using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.Processor
{
    public class ImageCorners
    {
        public double Left { get; set; }

        public double Right { get; set; }

        public double Top { get; set; }

        public double Bottom { get; set; }

        public double Width { get { return Right + 1.0 - Left; } }

        public double Height { get { return Bottom + 1.0 - Top; } }


        public static ImageCorners GetImageCornersFromClickInputs(ImageClickInputDetails clickInputDetails, double imageHeightRatio, double bottomPaddingPercent)
    {
            var clickPositions = clickInputDetails.ClickPositionListForImages;
            var left = ImageClickInputDetails.GetXClickPosition(clickPositions, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
            var right = ImageClickInputDetails.GetXClickPosition(clickPositions, ClickPositionOnImageTypes.RightEndOfRotatingDisc);
            var bottom = ImageClickInputDetails.GetBottomMarkerPos(clickPositions, bottomPaddingPercent);

            var requiredImageHeight = imageHeightRatio * clickPositions[0].AllowedHeight;
            var top = bottom - requiredImageHeight + 1; //zero based index
            if (top < 0 || top >= clickPositions[0].AllowedHeight)
                throw new Exception("Calculations gave invalid value for top of the image.");

            return new ImageCorners { Left = left, Right = right, Bottom = bottom, Top = top };
        }
        
        public static ImageCorners GetActualImageCorners(ImageCorners clickImgCorners, Image image, IList<ClickPositionOnImage> clickInput)
        {
            return new ImageCorners
            {
                Left = clickImgCorners.Left * image.Width / clickInput[0].AllowedWidth,
                Right = clickImgCorners.Right * image.Width / clickInput[0].AllowedWidth,
                Bottom = clickImgCorners.Bottom * image.Height / clickInput[0].AllowedHeight,
                Top = clickImgCorners.Top * image.Height / clickInput[0].AllowedHeight
            };
        }

    }
}
