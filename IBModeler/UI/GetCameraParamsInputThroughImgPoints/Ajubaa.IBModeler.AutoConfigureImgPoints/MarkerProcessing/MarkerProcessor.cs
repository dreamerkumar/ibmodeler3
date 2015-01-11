using System.Drawing;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class MarkerProcessor
    {
        /// <summary>
        /// returns the first markers to the left and right of the center of disc
        /// </summary>
        /// <param name="image"></param>
        /// <param name="discLeftEdgeX"></param>
        /// <param name="discRightEdgeX"></param>
        /// <param name="y"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static List<double> GetLeftAndRightMarkerPositions(Bitmap image, int discLeftEdgeX, int discRightEdgeX, int y, MarkerProcessingParams @params)
        {
            var colorVariations = ColorVariationCalculator.GetColorVariationList(image, discLeftEdgeX, discRightEdgeX, y);

            var markerPositions = GetAllMarkerPositions(colorVariations, @params, discLeftEdgeX);

            var center = GetMiddlePosition(discLeftEdgeX, discRightEdgeX);

            for (var index = 0; index < markerPositions.Count; index++)
            {
                var potentialRightMarkerPosition = markerPositions[index];
                if (potentialRightMarkerPosition >= center)
                {
                    if(index > 0)
                    {
                        var leftMarkerPosition = markerPositions[index - 1];
                        return new List<double> { leftMarkerPosition, potentialRightMarkerPosition };
                    }

                    //left index could not be found. break and return null
                    break;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the list of marker positions that satisfy the supplied conditions
        /// </summary>
        /// <param name="colorVariations"></param>
        /// <param name="params"></param>
        /// <param name="extraLeftPixels"></param>
        /// <returns></returns>
        public static List<double> GetAllMarkerPositions(List<double> colorVariations, MarkerProcessingParams @params, int extraLeftPixels)
        {
            var maxMarkerWidth = colorVariations.Count * @params.MarkerWidthPercent / 100.0;

            var validPositions = GetValidPositions(colorVariations, @params.MarkerColorVariationPercent, maxMarkerWidth);

            var positions = new List<double>();
            for (var ctr = 0; ctr < validPositions.Count; ctr++)
            {
                var endValidPositionIndex = GetEndValidIndex(ctr, validPositions, maxMarkerWidth);
                if(endValidPositionIndex != null)
                {
                    var startValidPosition = validPositions[ctr];
                    var endValidPosition = validPositions[endValidPositionIndex.Value];

                    var position = GetMiddlePosition(startValidPosition, endValidPosition);
                    positions.Add(extraLeftPixels + position);
                    
                    //move the counter forward to avoid repetition
                    ctr = endValidPositionIndex.Value;
                }
            }

            return positions;
        }

        /// <summary>
        /// gets the middle position for a given start and end position
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        public static double GetMiddlePosition(int startPosition, int endPosition)
        {
            var width = endPosition + 1 - startPosition;
            return startPosition - 1 + (width) / 2.0;
        }

        /// <summary>
        /// returns list of positions that satisfy the minimum color variation criteria
        /// </summary>
        /// <param name="colorVariations"></param>
        /// <param name="markerColorVariationPercent"></param>
        /// <param name="maxMarkerWidth"></param>
        /// <returns></returns>
        private static List<int> GetValidPositions(IList<double> colorVariations, double markerColorVariationPercent, double maxMarkerWidth)
        {
            var count = colorVariations.Count;
            var startIndex = (int) maxMarkerWidth;
            var endIndex = count - 1 - (int) maxMarkerWidth;

            var validPositions = new List<int>();
            for(var index = startIndex; index <= endIndex && index < count; index++)
            {
                var variation = colorVariations[index];
                if(variation*100 >= markerColorVariationPercent)
                {
                    validPositions.Add(index);
                }
            }
            return validPositions;
        }

        /// <summary>
        /// returns the index of the max position which is in the vicinity of the start position
        /// keeps looping as long as the condition is satisfied to find the furthest end position.
        /// returns null if there are are no positions in the vicinity
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="validPositions"></param>
        /// <param name="maxMarkerWidth"></param>
        /// <returns></returns>
        private static int? GetEndValidIndex(int startIndex, IList<int> validPositions, double maxMarkerWidth)
        {
            int? endIndex = null;
            var startPosition = validPositions[startIndex];
            for(var ctr = startIndex + 1; ctr < validPositions.Count; ctr++)
            {
                var potentialEndPosition = validPositions[ctr];
                if(potentialEndPosition +1 - startPosition <= maxMarkerWidth)
                {
                    endIndex = ctr;
                }
                else
                {
                    break;
                }
            }
            return endIndex;
        }
    }
}
