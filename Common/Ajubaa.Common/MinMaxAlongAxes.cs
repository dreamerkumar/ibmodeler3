using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common
{
    public class MinMaxAlongAxes
    {
        public double? MinX { get; private set; }
        public double? MaxX { get; private set; }
        public double? MinY { get; private set; }
        public double? MaxY { get; private set; }
        public double? MinZ { get; private set; }
        public double? MaxZ { get; private set; }

        public MinMaxAlongAxes(double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
            MinZ = minZ;
            MaxZ = maxZ;
        }
        public MinMaxAlongAxes(IList<Triangle> triangles, IList<Point3D> pts)
        {
            Point3D firstPt;
            if (triangles != null && triangles.Count > 0)
                firstPt = triangles[0].V1;
            else if (pts != null && pts.Count > 0)
                firstPt = pts[0];
            else
                throw new Exception("Either some triangles, or some points have to be passed to calculate min max values.");

            //assign first point values as default
            MinX = firstPt.X;
            MaxX = firstPt.X;
            MinY = firstPt.Y;
            MaxY = firstPt.Y;
            MinZ = firstPt.Z;
            MaxZ = firstPt.Z;

            //readjust for each triangle
            if (triangles != null)
            {
                foreach (var t in triangles)
                {
                    for (var ctr = 1; ctr <= 3; ctr++)
                    {
                        Point3D pt;
                        switch (ctr)
                        {
                            case 1:
                                pt = t.V1;
                                break;
                            case 2:
                                pt = t.V2;
                                break;
                            default:
                                pt = t.V3;
                                break;
                        }
                        Readjust(pt);
                    }
                }
            }

            if (pts == null) return;

            //readjust for each point
            foreach (var pt in pts)
            {
                Readjust(pt);
            }
        }

        public MinMaxAlongAxes(Rect3D rect)
        {
            MinX = rect.X;
            MaxX = rect.X + rect.SizeX;
            MinY = rect.Y;
            MaxY = rect.Y + rect.SizeY;
            MinZ = rect.Z;
            MaxZ = rect.Z + rect.SizeZ;
        }

        public void Readjust(Rect3D bounds)
        {
            Readjust(new Point3D(bounds.X, bounds.Y, bounds.Z));
            Readjust(new Point3D(bounds.X + bounds.SizeX, bounds.Y + bounds.SizeY, bounds.Z + bounds.SizeZ));
        }
        
        private void Readjust(Point3D pt)
        {
            if (pt.X < MinX)
                MinX = pt.X;
            if (pt.X > MaxX)
                MaxX = pt.X;
            if (pt.Y < MinY)
                MinY = pt.Y;
            if (pt.Y > MaxY)
                MaxY = pt.Y;
            if (pt.Z < MinZ)
                MinZ = pt.Z;
            if (pt.Z > MaxZ)
                MaxZ = pt.Z;
        }

        /// <summary>
        /// whether a triangle is within the min max range
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="allowedRange"></param>
        /// <returns></returns>
        public static bool WithinRange(Triangle triangle, MinMaxAlongAxes allowedRange)
        {
            if (!WithinRange(triangle.V1, allowedRange)) return false;
            if (!WithinRange(triangle.V2, allowedRange)) return false;
            return WithinRange(triangle.V3, allowedRange);
        }

        //whether a point is within the min max range
        public static bool WithinRange(Point3D p, MinMaxAlongAxes allowedRange)
        {
            return p.X >= allowedRange.MinX && p.X <= allowedRange.MaxX
                   && p.Y >= allowedRange.MinY && p.Y <= allowedRange.MaxY
                   && p.Z >= allowedRange.MinZ && p.Z <= allowedRange.MaxZ;
        }

        public static Point3D GetCenterPosition(IList<Point3D> existingLoop)
        {
            var minMax = new MinMaxAlongAxes(null, existingLoop);
            return GetCenterOfMinMaxRange(minMax);
        }

        public static Point3D GetCenterOfMinMaxRange(MinMaxAlongAxes minMax)
        {
            var xRange = minMax.MaxX - minMax.MinX;
            var yRange = minMax.MaxY - minMax.MinY;
            var zRange = minMax.MaxZ - minMax.MinZ;
            return new Point3D((minMax.MinX + xRange/2.0).Value , (minMax.MinY + yRange/2.0).Value, (minMax.MinZ + zRange/2.0).Value);
        }
    }
}
