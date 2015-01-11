using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajubaa.IBModeler.Common
{
    [Serializable]
    public class ImageClickInputDetails
    {
        /// <summary>
        /// name of the image
        /// not the full image path
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// angle by which the image should be rotated 
        /// in the clockwise direction from top left 
        /// to align the base disc horizontally
        /// </summary>
        public double RotateImageBy { get; set; }

        /// <summary>
        /// list of click inputs for this image
        /// </summary>
        public List<ClickPositionOnImage> ClickPositionListForImages { get; set; }


        #region static helper methods
        public static double GetXClickPosition(IEnumerable<ClickPositionOnImage> clickInput, ClickPositionOnImageTypes clickType)
        {
            var input = GetSpecificClickInput(clickInput, clickType);
            return input.ClickXPos;
        }

        public static double GetYClickPosition(IEnumerable<ClickPositionOnImage> clickInput, ClickPositionOnImageTypes clickType)
        {
            var input = GetSpecificClickInput(clickInput, clickType);
            return input.ClickYPos;
        }

        public static ClickPositionOnImage GetSpecificClickInput(IEnumerable<ClickPositionOnImage> clickInput, ClickPositionOnImageTypes clickType)
        {
            var input = clickInput.FirstOrDefault(x => x.PositionType == clickType);
            if (input == null)
                throw new Exception(string.Format("Required user click input '{0}' not specified", clickType));
            return input;
        }

        public static double GetBottomMarkerPos(IEnumerable<ClickPositionOnImage> clickPositions, double bottomPaddingPercent)
        {
            var leftPos = GetSpecificClickInput(clickPositions, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
            var left = leftPos.ClickXPos;
            var right = GetXClickPosition(clickPositions, ClickPositionOnImageTypes.RightEndOfRotatingDisc);

            var discYPos = leftPos.ClickYPos;
            var bottomPadding = bottomPaddingPercent * (right + 1.0 - left) / 100.0;
            var bottom = discYPos - bottomPadding;
            return bottom;
        }
        #endregion
    }
}
