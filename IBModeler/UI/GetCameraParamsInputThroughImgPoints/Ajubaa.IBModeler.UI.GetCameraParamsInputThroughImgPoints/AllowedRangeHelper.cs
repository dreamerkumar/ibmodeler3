using System;
using System.Linq;
using System.Windows;
using Ajubaa.IBModeler.Common;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints
{
    public static class AllowedRangeHelper
    {
        public static bool IsInAllowedRange(Point position, double allowedWidth, double allowedHeight, ClickPositionOnImageTypes positionType, List<ClickPositionOnImage> currentClickSequences)
        {
            switch (positionType)
            {
                case ClickPositionOnImageTypes.None:
                    break;
                case ClickPositionOnImageTypes.LeftEndOfRotatingDisc:
                    break;
                case ClickPositionOnImageTypes.RightEndOfRotatingDisc:
                    {
                        var leftEnd = GetLeftEndOfRotatingDisc(currentClickSequences);
                        return (leftEnd < position.X);
                    }
                case ClickPositionOnImageTypes.BottomMostPartOfModel:
                    {
                        var leftEndPos = ImageClickInputDetails.GetSpecificClickInput(currentClickSequences,  ClickPositionOnImageTypes.LeftEndOfRotatingDisc  );
                        return (position.Y <= leftEndPos.ClickYPos);
                    }
                    break;
                case ClickPositionOnImageTypes.MarkerLeftFromCenter:
                    {
                        var leftEnd = GetLeftEndOfRotatingDisc(currentClickSequences);
                        var centerLine = GetCenterLine(currentClickSequences);
                        return (leftEnd <= position.X && position.X <= centerLine);
                    }
                case ClickPositionOnImageTypes.MarkerRightFromCenter:
                    {
                        var rightEnd = GetRightEndOfRotatingDisc(currentClickSequences);
                        var centerLine = GetCenterLine(currentClickSequences);
                        return (centerLine <= position.X && position.X <= rightEnd);
                    }
                default:
                    throw new ArgumentOutOfRangeException("positionType");
            }
            return true;
        }

        public static double GetBottomMostPartOfModel(List<ClickPositionOnImage> clickSequences)
        {
            var matchedItem =
                clickSequences.FirstOrDefault(
                    x => x.PositionType == ClickPositionOnImageTypes.BottomMostPartOfModel && x.Processed);

            if (matchedItem == null)
                throw new Exception("BottomMostPartOfModel not set.");

            return matchedItem.ClickYPos;
        }

        public static double GetLeftEndOfRotatingDisc(List<ClickPositionOnImage> clickSequences)
        {
            var matchedItem =
                clickSequences.FirstOrDefault(
                    x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc && x.Processed);

            if (matchedItem == null)
                throw new Exception("LeftEndOfRotatingDisc not set.");

            return matchedItem.ClickXPos;
        }

        public static double GetRightEndOfRotatingDisc(List<ClickPositionOnImage> clickSequences)
        {
            var matchedItem =
                clickSequences.FirstOrDefault(
                    x => x.PositionType == ClickPositionOnImageTypes.RightEndOfRotatingDisc && x.Processed);

            if (matchedItem == null)
                throw new Exception("RightEndOfRotatingDisc not set.");

            return matchedItem.ClickXPos;
        }

        public static double GetCenterLine(List<ClickPositionOnImage> clickSequences)
        {
            var left = GetLeftEndOfRotatingDisc(clickSequences);

            var right = GetRightEndOfRotatingDisc(clickSequences);

            return left + (right - left) / 2.0;
        }

    }
}
