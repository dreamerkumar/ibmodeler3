using System.Drawing;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    internal class AddClickPositionHelper
    {
        private readonly double _imgWidth;
        private readonly double _imgHeight;

        internal AddClickPositionHelper(double imgWidth, double imgHeight)
        {
            _imgWidth = imgWidth;
            _imgHeight = imgHeight;
        }

        internal ClickPositionOnImage GetClickPositionObject(Point pt, ClickPositionOnImageTypes positionType)
        {
            return new ClickPositionOnImage
            {
                ClickYPos = pt.Y,
                ClickXPos = pt.X,
                AllowedHeight = _imgHeight,
                AllowedWidth = _imgWidth,
                PositionType = positionType,
                Processed = true
            };
        }

        internal ClickPositionOnImage GetClickPositionObject(double x, double y, ClickPositionOnImageTypes positionType)
        {
            return new ClickPositionOnImage
            {
                ClickYPos = y,
                ClickXPos = x,
                AllowedHeight = _imgHeight,
                AllowedWidth = _imgWidth,
                PositionType = positionType,
                Processed = true
            };
        }
    }
}
