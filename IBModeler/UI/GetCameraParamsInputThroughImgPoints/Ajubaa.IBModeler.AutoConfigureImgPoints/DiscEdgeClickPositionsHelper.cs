using System.Drawing;
using Ajubaa.IBModeler.Common;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public static class DiscEdgeClickPositionsHelper
    {
        internal static List<ClickPositionOnImage> GetDiscEdgeClickPositions(Bitmap image, AutoConfigureImgPointsParams inputParams)
        {
            var positionList = new List<ClickPositionOnImage>();

            var calculator = new ClickPositionCalculator(image, inputParams);
            var helper = new AddClickPositionHelper(image.Width, image.Height);

            var discTopLeftEnd = calculator.GetDiscTopLeftEnd();
            if (discTopLeftEnd.HasValue)
            {
                var position = helper.GetClickPositionObject(discTopLeftEnd.Value, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
                positionList.Add(position);
            }

            var discTopRightEnd = calculator.GetDiscTopRightEnd();
            if(discTopRightEnd.HasValue)
            {
                var position = helper.GetClickPositionObject(discTopRightEnd.Value, ClickPositionOnImageTypes.RightEndOfRotatingDisc);
                positionList.Add(position);
            }

            return positionList;
        }

 
    }
}
