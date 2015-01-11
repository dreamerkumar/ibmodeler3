using System.Windows.Media.Media3D;

namespace Ajubaa.Common._3DModelView
{
    public class ModelViewInfo
    {
        public int Index { get; set; }
        public bool Show { get; set; }
        public string DisplayName { get; set; }
        public string FilePath { get; set; }
        public GeometryModel3D GeometryModel3D { get; set; }
    }
}
