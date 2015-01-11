using System;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common._3DModelView
{
    public class TriangleWithIndex : Triangle
    {
        public int? Index { get; set; }
        public int? IndexFromTheParentIndexFile { get; set; }
        public TriangleWithIndex(Point3D v1, Point3D v2, Point3D v3) : base(v1, v2, v3)
        {
            throw new Exception("Wrong type of constructor used.");
        }

        public TriangleWithIndex(Point3D v1, Point3D v2, Point3D v3, int index)
            : base(v1, v2, v3)
        {
            Index = index;
        }

        public TriangleWithIndex(Point3D v1, Point3D v2, Point3D v3, int index, int? indexFromTheParentIndexFile)
            : base(v1, v2, v3)
        {
            Index = index;
            IndexFromTheParentIndexFile = indexFromTheParentIndexFile;
        }
    }
}
