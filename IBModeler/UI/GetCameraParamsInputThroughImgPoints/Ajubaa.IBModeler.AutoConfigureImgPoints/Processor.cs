using System.IO;
using System.Linq;
using System.Drawing;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Processor;
using System.Collections.Generic;
using Point = System.Windows.Point;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    /// <summary>
    /// tries to find out the click points in the images without the user click inputs
    /// </summary>
    public class Processor
    {
        public List<ImageClickInputDetails> GetClickPositionsForImgFolder(string imageDirPath, AutoConfigureImgPointsParams inputParams)
        {
            var filePaths = GetImageFilesHelper.GetImageFilesFromLocation(imageDirPath);

            var list = new List<ImageClickInputDetails>();
            for (var ctr = 0; ctr < filePaths.Count(); ctr++ )
            {
                var details = GetImgClickInputDetails(filePaths[ctr], inputParams, ctr == 0);
                list.Add(details);
            }
            return list;
        }

        public static ImageClickInputDetails GetImgClickInputDetails(string filePath, AutoConfigureImgPointsParams inputParams, bool firstImage)
        {
            var imageName = Path.GetFileName(filePath);

            var image = GetProcessedImage(filePath, inputParams);

            var clickPositionList = DiscEdgeClickPositionsHelper.GetDiscEdgeClickPositions(image, inputParams);

            var rotateImageBy = 0.0f;
            
            //if left and right ends of the disc were found, rotate the image and try to identify the markers
            var leftEdge = clickPositionList.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
            var rightEdge = clickPositionList.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.RightEndOfRotatingDisc);

            if(leftEdge != null && rightEdge == null)
            {
                return new ImageClickInputDetails
                {
                    ImageName = imageName,
                    RotateImageBy = 0.0,
                    ClickPositionListForImages = clickPositionList
                };
            }

            if(leftEdge != null && rightEdge != null)
            {
                rotateImageBy = MainProcessor.GetRotationAngleToRealign(leftEdge, rightEdge);
                
                //rotate the positions before trying to find the markers
                var origLeftPt = new Point(leftEdge.ClickXPos, leftEdge.ClickYPos);
                var newLeftEdgePosition = RotationHelper.GetRotatedPosition(rotateImageBy, origLeftPt);
                if (newLeftEdgePosition == null || !(0 <= newLeftEdgePosition.Value.X && newLeftEdgePosition.Value.X < image.Width)
                    || !(0 <= newLeftEdgePosition.Value.Y && newLeftEdgePosition.Value.Y < image.Height))
                {
                    return new ImageClickInputDetails
                    {
                        ImageName = imageName,
                        RotateImageBy = 0.0,
                        ClickPositionListForImages = new List<ClickPositionOnImage>()
                    };
                }

                var origRightPt = new Point(rightEdge.ClickXPos, rightEdge.ClickYPos);
                var newRightEdgePosition = RotationHelper.GetRotatedPosition(rotateImageBy, origRightPt);
                if (newRightEdgePosition == null || !(0 <= newRightEdgePosition.Value.X && newRightEdgePosition.Value.X < image.Width)
                    || !(0 <= newRightEdgePosition.Value.Y && newRightEdgePosition.Value.Y < image.Height))
                {
                    return new ImageClickInputDetails
                    {
                        ImageName = imageName,
                        RotateImageBy = 0.0,
                        ClickPositionListForImages = new List<ClickPositionOnImage>()
                    };
                }
                
                //update values in the original collection
                rightEdge.ClickXPos = newRightEdgePosition.Value.X;
                rightEdge.ClickYPos = newRightEdgePosition.Value.Y;

                leftEdge.ClickXPos = newLeftEdgePosition.Value.X;
                leftEdge.ClickYPos = newLeftEdgePosition.Value.Y;
                
                image = MainProcessor.RotateImg(image, rotateImageBy, inputParams.BackgroundColor);
                
                var helper = new AddClickPositionHelper(image.Width, image.Height);
                
                //add marker positions
                var y = MarkerProcessor.GetMiddlePosition((int)leftEdge.ClickYPos, image.Height - 1); 
                var markerPositions = MarkerProcessor.GetLeftAndRightMarkerPositions(image, (int)leftEdge.ClickXPos, 
                                                (int)rightEdge.ClickXPos, (int)y, inputParams.MarkerProcessingParams);
                if(markerPositions != null && markerPositions.Count == 2)
                {
                    //add left marker
                    var leftX = markerPositions[0];
                    var leftPosition = helper.GetClickPositionObject(leftX, y, ClickPositionOnImageTypes.MarkerLeftFromCenter);
                    clickPositionList.Add(leftPosition);

                    //add right marker
                    var rightX = markerPositions[1];
                    var rightPosition = helper.GetClickPositionObject(rightX, y, ClickPositionOnImageTypes.MarkerRightFromCenter);
                    clickPositionList.Add(rightPosition);

                    if (firstImage)
                    {
                        //add bottommost part
                        var middleXPt = MarkerProcessor.GetMiddlePosition((int) leftEdge.ClickXPos, (int) rightEdge.ClickXPos);
                        var bottomMostPart = helper.GetClickPositionObject(middleXPt, leftEdge.ClickYPos, ClickPositionOnImageTypes.BottomMostPartOfModel);
                        clickPositionList.Add(bottomMostPart);
                    }
                }
            }

            return new ImageClickInputDetails
            {
                ImageName = imageName,
                RotateImageBy = rotateImageBy,
                ClickPositionListForImages = clickPositionList
            };
        }

        /// <summary>
        /// load the image with necessary resizing and background stripping
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="inputParams"></param>
        /// <returns></returns>
        private static Bitmap GetProcessedImage(string filePath, AutoConfigureImgPointsParams inputParams)
        {
            Bitmap image;
            if (inputParams.ResizeWidth.HasValue && inputParams.ResizeHeight.HasValue)
            {
                image = MainProcessor.ResizeJpg(filePath, null, inputParams.ResizeWidth.Value, inputParams.ResizeHeight.Value);
            }
            else
            {
                image = (Bitmap)Image.FromFile(filePath);
            }
            MainProcessor.StripBackground(image, inputParams.BackgroundStrippingParams);
            return image;
        }

    }
}
