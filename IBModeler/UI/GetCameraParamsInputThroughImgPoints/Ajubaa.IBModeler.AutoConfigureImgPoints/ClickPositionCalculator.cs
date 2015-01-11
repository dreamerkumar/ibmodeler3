using System;
using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class ClickPositionCalculator
    {
        private readonly AutoConfigureImgPointsParams _inputParams;
        private readonly Bitmap _image;

        public ClickPositionCalculator(Bitmap image, AutoConfigureImgPointsParams inputParams)
        {
            _image = image;
            _inputParams = inputParams;
        }

        /// <summary>
        /// returns disc top left end. Eats up any exception thrown during calculation
        /// </summary>
        /// <returns></returns>
        public Point? GetDiscTopLeftEnd()
        {
            try
            {
                var distanceProcessor = new LeftDistanceProcessor(_inputParams.BackgroundColor, _image, _inputParams.MinPixelsForBaseDisc);
                var y = GetDiscTopEndY(distanceProcessor);
                if (y.HasValue)
                {
                    var x = LeftEdgePtsProcessor.GetFirstValidPixelOnLeft(y.Value, _image, _inputParams.BackgroundColor);
                    return new Point(x, y.Value);
                }
            }
            catch {}

            return null;
        }

        /// <summary>
        /// returns disc top left end. Eats up any exception thrown during calculation
        /// </summary>
        /// <returns></returns>
        public Point? GetDiscTopRightEnd()
        {
            try
            {
                var distanceProcessor = new RightDistanceProcessor(_inputParams.BackgroundColor, _image, _inputParams.MinPixelsForBaseDisc);
                var y = GetDiscTopEndY(distanceProcessor);
                if (y.HasValue)
                {
                    var x = RightEdgePtsProcessor.GetFirstValidPixelOnRight(y.Value, _image, _inputParams.BackgroundColor);
                    return new Point(x, y.Value);
                }
            }
            catch {}

            return null;
        }

        /// <summary>
        /// gets point where the edge positions start to move away from the edge line
        /// </summary>
        /// <param name="distanceProcessor"></param>
        /// <returns></returns>
        private int? GetDiscTopEndY(IDistanceProcessor distanceProcessor)
        {
            var maxDistanceVarianceOnLine = GetMaxDistanceVarianceOnLine(distanceProcessor);

            var movingAwaySequentially = false;
            var yMovingAwayStartPt = 0;

            var startIndex = _image.Height - _inputParams.MinPixelsForBaseDisc -1;
            for (var y = startIndex; y >= 0; y--)
            {
                var distance = distanceProcessor.GetDistance(y);
                
                if (distance > maxDistanceVarianceOnLine)
                {
                    if(!movingAwaySequentially)
                    {
                        movingAwaySequentially = true;
                        yMovingAwayStartPt = y;
                    }

                    if (distance >= _inputParams.MinPixelsForBaseDiscEndOfEdge)
                    {
                        return yMovingAwayStartPt + 1 < _image.Height? yMovingAwayStartPt + 1 : (int?) null;
                    }
                }
                else
                {
                    movingAwaySequentially = false;
                }
            }
            
            return null;
        }

        private double GetMaxDistanceVarianceOnLine(IDistanceProcessor distanceProcessor)
        {
            var maxDistance = 0.0;
            for (int y = _image.Height-1, ctr =1; y >= 0 && ctr <= _inputParams.MinPixelsForBaseDisc; y--, ctr++)
            {
                var distance = Math.Abs(distanceProcessor.GetDistance(y));
                if (distance > maxDistance)
                    maxDistance = distance;
            }
            return maxDistance;
        }
    }
}
