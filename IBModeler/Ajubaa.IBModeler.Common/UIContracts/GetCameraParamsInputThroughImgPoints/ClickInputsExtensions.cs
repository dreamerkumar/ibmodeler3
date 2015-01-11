using System;

namespace Ajubaa.IBModeler.Common
{
    public static class ClickInputsExtensions
    {
        /// <summary>
        /// distance between the bottom most portion of model and the base disc
        /// as a percentage of the distance between the left and right edges of the disc
        /// </summary>
        public static double GetBottomPaddingPercent(this ClickInputs clickInputs)
        {
            if (clickInputs.ImageClickInputDetailsList != null && clickInputs.ImageClickInputDetailsList.Count > 0)
            {
                //only the first image has the bottom most part defined so pick the first image
                var imageClickDetails = clickInputs.ImageClickInputDetailsList[0];

                var bottomMostPartYPos = ImageClickInputDetails.GetYClickPosition(imageClickDetails.ClickPositionListForImages,
                    ClickPositionOnImageTypes.BottomMostPartOfModel);

                var leftPos = ImageClickInputDetails.GetSpecificClickInput(imageClickDetails.ClickPositionListForImages, 
                    ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
                var rightX = ImageClickInputDetails.GetXClickPosition(imageClickDetails.ClickPositionListForImages, 
                    ClickPositionOnImageTypes.RightEndOfRotatingDisc);

                var width = rightX + 1 - leftPos.ClickXPos;

                if (bottomMostPartYPos < leftPos.ClickYPos)
                {
                    return (leftPos.ClickYPos - bottomMostPartYPos) * 100.0 / width;
                }

                return 0.0;
            }

            throw new Exception("bottom padding percent cannot be calculated before the first image details are added");
        }

        /// <summary>
        /// different images can have different bottom portions for the model
        /// this function returns the minimum height as a fraction of total height that will be applied to
        /// all the images to keep the height same throughout
        /// </summary>
        public static double GetMinImageHeightRatio(this ClickInputs clickInputs)
        {
            var minHeightFraction = 1.0;
            var bottomPaddingPercent = clickInputs.GetBottomPaddingPercent();
            foreach (var imageClickInputDetails in clickInputs.ImageClickInputDetailsList)
            {
                var leftPos = ImageClickInputDetails.GetSpecificClickInput(
                    imageClickInputDetails.ClickPositionListForImages, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);

                var pos = ImageClickInputDetails.GetBottomMarkerPos(imageClickInputDetails.ClickPositionListForImages,
                                                                    bottomPaddingPercent);
                var fraction = (pos + 1) / leftPos.AllowedHeight; //+1 because it is zero based index
                if (fraction < minHeightFraction)
                    minHeightFraction = fraction;
            }
            return minHeightFraction;
        }
    }
}
