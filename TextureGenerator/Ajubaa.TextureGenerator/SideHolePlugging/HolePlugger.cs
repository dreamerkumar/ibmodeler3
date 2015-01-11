using System;
using System.Windows;
using System.Collections.Generic;
using Ajubaa.Common;

namespace Ajubaa.TextureGenerator
{
    public class HolePlugger
    {
        public static void PlugSideHoles(OptimizedImgParams indices, double allowedHoleHeight)
        {
            PlugOneSideHoles(indices, allowedHoleHeight, MinXOrMaxXEnum.MinX);
            PlugOneSideHoles(indices, allowedHoleHeight, MinXOrMaxXEnum.MaxX);
        }

        public static void PlugOneSideHoles(OptimizedImgParams indices, double allowedHoleHeight, MinXOrMaxXEnum minXOrMaxXEnum)
        {
            //create a new set of points that will be modified using the plug hole logic
            var points = new List<Point>();
            var xLimitsAtYIndices = indices.XLimitsAtYIndices;
            for (var y = 0; y < xLimitsAtYIndices.Length; y++)
            {
                var xLimits = xLimitsAtYIndices[y];
                if (xLimits.Width <= 0) continue;

                int x;
                switch (minXOrMaxXEnum)
                {
                    case MinXOrMaxXEnum.MinX:
                        x = xLimits.Min;
                        break;
                    case MinXOrMaxXEnum.MaxX:
                    default:
                        x = xLimits.Max;
                        break;
                }
                var point = new Point(x, y);
                points.Add(point);
            }

            PlugHolesRepeatedly(points, allowedHoleHeight, minXOrMaxXEnum);
            
            ModifyXValuesBasedOnNewPoints(xLimitsAtYIndices, points, minXOrMaxXEnum);
        }

        public static void ModifyXValuesBasedOnNewPoints(MinAndMaxIndices[] xLimitsAtYIndices, List<Point> points, MinXOrMaxXEnum minXOrMaxXEnum)
        {
            if (points.Count <= 1) return;
            var outerPtIndex = 0;
            var oneLessThanPtCount = points.Count - 1;

            for (var y = 0; y < xLimitsAtYIndices.Length; y++)
            {
                var xLimits = xLimitsAtYIndices[y];
                if (xLimits.Width <= 0) continue;

                //find the first index which has the points.y value greater than or equal to the current y
                var outerY = points[outerPtIndex].GetIntY();
                
                while (outerPtIndex < oneLessThanPtCount && outerY < y)
                {
                    outerPtIndex++;
                    outerY = points[outerPtIndex].GetIntY();
                }
                var newX = GetNewX(points, y, outerPtIndex, outerY);
                
                switch (minXOrMaxXEnum)
                {
                    case MinXOrMaxXEnum.MaxX:
                        xLimits.Max = newX;
                        break;
                    case MinXOrMaxXEnum.MinX:
                        xLimits.Min = newX;
                        break;
                }
            }
        }

        private static int GetNewX(IList<Point> points, int y, int outerPtIndex, int outerY)
        {
            int newX;
            if(outerY <= y || outerPtIndex == 0) 
            {
                newX = points[outerPtIndex].GetIntX();
            }
            else
            {
                //if we are here we have found the first outerY where outerY > y and there has to be an inner point as well

                var innerPtIndex = outerPtIndex -1;
                var innerY = points[innerPtIndex].GetIntY();

                if (innerY == y)
                {
                    newX = points[innerPtIndex].GetIntX();
                }
                else
                {
                    var innerX = points[innerPtIndex].GetIntX();
                    var outerX = points[outerPtIndex].GetIntX();

                    //this check is important otherwise line creation may fail
                    if (innerX == outerX)
                    {
                        newX = innerX;
                    }
                    else
                    {
                        var lineEquation = LineEquation2D.Get2DLineEquation(innerX, innerY, outerX, outerY);
                        newX = Convert.ToInt32(lineEquation.GetXValueForY(y));
                    }
                }
            }
            return newX;
        }

        /// <summary>
        /// keeps removing holes till no more holes can be removed
        /// </summary>
        /// <param name="points"></param>
        /// <param name="allowedHoleHeight"></param>
        /// <param name="minXOrMaxXEnum"></param>
        public static void PlugHolesRepeatedly(List<Point> points, double allowedHoleHeight, MinXOrMaxXEnum minXOrMaxXEnum)
        {
            bool holesRemoved;
            IHoleStatusHelper holeStatusHelper;
            switch (minXOrMaxXEnum)
            {
                case MinXOrMaxXEnum.MinX:
                    holeStatusHelper = new LeftHoleStatusHelper();
                    break;
                case MinXOrMaxXEnum.MaxX:
                default:
                    holeStatusHelper = new RightHoleStatusHelper();
                    break;
            }
            do { holesRemoved = PlugSideHoles(points, holeStatusHelper, allowedHoleHeight); } while (holesRemoved);
        }

        /// <summary>
        /// Hole: for the right side, it is a hole if the x index decreases and then increases again
        /// for left side, it will be just the opposite
        /// </summary>
        /// <param name="points"></param>
        /// <param name="holeStatusHelper"></param>
        /// <param name="allowedHoleHeight"></param>
        /// <returns>whether a hole was removed</returns>
        private static bool PlugSideHoles(List<Point> points, IHoleStatusHelper holeStatusHelper, double allowedHoleHeight)
        {
            if (points.Count < 2) return false;
            var holesRemoved = false;
            var holeStatus = holeStatusHelper.GetHoleStatus(points[0].X, points[1].X);
            var holeInProgress = holeStatus == HoleStatusEnum.Decreasing;
            var holeStartIndex = holeInProgress? 1 : int.MaxValue;
            
            for(var index = 2; index < points.Count; index++)
            {
                holeStatus = holeStatusHelper.GetHoleStatus(points[index].X, points[index - 1].X);
                if(holeInProgress)
                {
                    var holeEndsNow = holeStatus == HoleStatusEnum.Increasing;
                    if (holeEndsNow)
                    {
                        holeInProgress = false;

                        var holeHeight = points[index].Y - points[holeStartIndex -1].Y;
                        if (holeHeight < allowedHoleHeight)
                        {
                            //indicesToRemove is sum of all indices between current and last non hole pixel
                            var indicesToRemove = index - holeStartIndex;
                            points.RemoveRange(holeStartIndex, indicesToRemove);
                            holesRemoved = true;
                            index = index - indicesToRemove;
                        }
                    }
                }
                else
                {
                    //check if a new hole is starting
                    if (holeStatus == HoleStatusEnum.Decreasing)
                    {
                        holeInProgress = true;
                        holeStartIndex = index;
                    }
                }
            }
            return holesRemoved;
        }
    }
}
