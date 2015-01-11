using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Ajubaa.Common.PolygonDataWriters;
using Ajubaa.Common.PolygonDataWriters._3ds;

namespace Ajubaa.IBModeler.Processor.Save
{
    public static class SaveProcessor
    {
        public static void AddADefaultSetOfTextureCoordinatesIfNone(MeshGeometry3D modelData)
        {
            if (modelData != null && (modelData.TextureCoordinates == null || modelData.TextureCoordinates.Count <= 0))
            {
                modelData.TextureCoordinates = new PointCollection();
                var vertexCount = modelData.Positions.Count;
                for(var ctr = 1; ctr <= vertexCount; ctr++)
                {
                    var pt = new System.Windows.Point(.5, .5);
                    modelData.TextureCoordinates.Add(pt);
                }

            }
        }
        
        public static void SaveAsXaml(string filePath, MeshGeometry3D modelData, Bitmap bitmapTextureImg)
        {
            AddADefaultSetOfTextureCoordinatesIfNone(modelData);

            XamlWriter.SaveMeshGeometryModel(filePath, modelData, bitmapTextureImg);
        }

        public static void SaveAs3DS(string filePath, GeometryModel3D modelData, Image textureFileData)
        {
            AddADefaultSetOfTextureCoordinatesIfNone(modelData.Geometry as MeshGeometry3D);

            _3DSWriter.ExportTo3DS(filePath, modelData, textureFileData);
        }
    }
}
