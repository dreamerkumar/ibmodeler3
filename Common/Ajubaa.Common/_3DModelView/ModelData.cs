using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common._3DModelView
{
    public class ModelData
    {
        #region properties
        public MinMaxAlongAxes MinMax { get; set; }
        public Model3DGroup Model3DGroupDataForDisplay { get; set; }
        public Point3D CenterPt { get; set; }
        public double MaximumSideLength { get; set; }
        public int MaxModelIndex { get; set; }
        public List<ModelViewInfo> ModelViewInfoList { get; set; }
        #endregion

        public ModelData()
        {
            Model3DGroupDataForDisplay = new Model3DGroup();
            ModelViewInfoList = new List<ModelViewInfo>();
            MaxModelIndex = 0;
        }
    }
}