using System.Drawing;
using System.Linq;
using Ajubaa.Common;
using System.Collections.Generic;
using Ajubaa.LineFromPtCollection;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class LinePtsProcessor
    {
        private readonly LineEquation2D _lineEqn;

        public LinePtsProcessor(IEnumerable<Point> initializingPts)
        {
            var xYPts = GetReOrderedPoints(initializingPts);
            _lineEqn = StraightLineProcessor.Get2DStraightLineFromDataConstants(xYPts);
        }

        private static List<System.Windows.Point> GetReOrderedPoints(IEnumerable<Point> initializingPts)
        {
            //we are expecting the slope to be vertical or close to vertical 
            //reverse x with y so that there is less chance of slope being infinity in the eqn y = mx + C
            return (from pt in initializingPts
                    select new System.Windows.Point {X = (double) pt.Y, Y = (double) pt.X}).ToList();
        }

        public int GetXValueForY(int y)
        {
            //x,y value were reversed while calculating the line eqn
            return (int)_lineEqn.GetYValueForX((double)y);
        }
    }
}
