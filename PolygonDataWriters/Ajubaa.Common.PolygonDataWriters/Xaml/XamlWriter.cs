using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml;
using Color = System.Windows.Media.Color;


namespace Ajubaa.Common.PolygonDataWriters
{
    public class XamlWriter
    {
        public static void WritePolygonsToXamlFile(string strTemplateLocation, string strFilePath, List<Triangle> lstTriangles, bool writeTextures)
        {
            //convert triangles to points
            var lstPolygonPts = new List<Point3D>();
            foreach (var triangle in lstTriangles)
                lstPolygonPts.AddRange(new List<Point3D> { triangle.V1, triangle.V2, triangle.V3 });

            WritePolygonsToXamlFile(strTemplateLocation, strFilePath, lstPolygonPts, writeTextures);
        }

        public static void WritePolygonsToXamlFile(string strTemplateLocation, string strFilePath, IEnumerable<Point3D> lstPolygonPts, bool writeTextures)
        {
            if (lstPolygonPts.Count() < 3 || lstPolygonPts.Count() % 3 > 0)
                throw new Exception("Invalid number of points supplied for the polygons. Each polygon should be a triangle consisting of three vertices.");

            //if no template is found, use the default template
            if (string.IsNullOrEmpty(strTemplateLocation))
                strTemplateLocation = string.Format(@"{0}\{1}\{2}", ExecutionDirInfoHelper.GetExecutionPath(),
                                             "Xaml", "ModelTemplateWithNoTexture.xaml");

            //Load the template
            var objXamlDoc = new XmlDocument();
            objXamlDoc.Load(strTemplateLocation);

            //Access the required node positions
            if (objXamlDoc.DocumentElement == null)
                throw new Exception("The xml document could not be loaded.");

            var objCoordinatesNode = objXamlDoc.DocumentElement.GetElementsByTagName("MeshGeometry3D.Positions");
            var objTexturesNode =
                objXamlDoc.DocumentElement.GetElementsByTagName("MeshGeometry3D.TextureCoordinates");
            var objIndicesNode = objXamlDoc.DocumentElement.GetElementsByTagName("MeshGeometry3D.TriangleIndices");

            if (objCoordinatesNode[0] == null || objIndicesNode[0] == null)
                throw new Exception("Could not find the nodes to write the model data in the xaml template.");
            if (writeTextures && objTexturesNode[0] == null)
                throw new Exception(
                    "Could not find the nodes to write the texture information in the xaml template.");

            //Write all the coordinates
            var objCoordinates = new StringBuilder("");
            var objTextures = new StringBuilder("");
            var objIndices = new StringBuilder("");
            var intIndexPos = 0;

            //Initialize values used for calculating texture coordinates
            var dblMinX = 0d;
            var dblMaxX = 0d;
            var dblMinY = 0d;
            var dblMaxY = 0d;
            if (writeTextures)
                GetMinMaxXyValues(lstPolygonPts, out dblMinX, out dblMaxX, out dblMinY, out dblMaxY);

            foreach (var objPt in lstPolygonPts)
            {
                if (objCoordinates.Length > 0)
                {
                    //Attach a comma
                    objCoordinates.Append(",");
                    objTextures.Append(",");
                    objIndices.Append(",");
                }
                objCoordinates.Append(String.Format("{0},{1},{2}", objPt.X, objPt.Y, objPt.Z));

                objIndices.Append(String.Format("{0}", intIndexPos));
                intIndexPos += 1;

                if (writeTextures)
                {
                    //Calculate texture values
                    double dblTexX;
                    double dblTexY;
                    GetTextureCoordinates(out dblTexX, out dblTexY, objPt, dblMinX, dblMaxX, dblMinY, dblMaxY);
                    objTextures.Append(String.Format("{0},{1}", dblTexX, dblTexY));
                }

                //TODO: Write the normals too
            }

            objCoordinatesNode[0].InnerText = objCoordinates.ToString();
            if (writeTextures)
                objTexturesNode[0].InnerText = objTextures.ToString();
            objIndicesNode[0].InnerText = objIndices.ToString();

            objXamlDoc.Save(strFilePath);


        }

        private static void GetMinMaxXyValues(IEnumerable<Point3D> lstPolygonPts, out double dblMinX, out double dblMaxX, out double dblMinY, out double dblMaxY)
        {
            if (lstPolygonPts == null || lstPolygonPts.Count() < 1)
                throw new Exception("WritePolygonsToXamlFile.GetMinMaxXYValues: Error occured while calculating the min/max X,Y values. No polygon points passed to the function.");
            dblMinX = lstPolygonPts.First().X;
            dblMaxX = lstPolygonPts.First().X;
            dblMinY = lstPolygonPts.First().Y;
            dblMaxY = lstPolygonPts.First().Y;
            foreach (var pt in lstPolygonPts)
            {
                if (dblMinX > pt.X)
                    dblMinX = pt.X;
                if (dblMaxX < pt.X)
                    dblMaxX = pt.X;
                if (dblMinY > pt.Y)
                    dblMinY = pt.Y;
                if (dblMaxY < pt.Y)
                    dblMaxY = pt.Y;
            }
            if (dblMinX == dblMaxX)
                throw new Exception("WritePolygonsToXamlFile.GetMinMaxXYValues: Cannot get Min, Max values to write texture as there is no variation along the X axis.");
            if (dblMinY == dblMaxY)
                throw new Exception("WritePolygonsToXamlFile.GetMinMaxXYValues: Cannot get Min, Max values to write texture as there is no variation along the Y axis.");

        }

        private static void GetTextureCoordinates(out double dblX, out double dblY, Point3D pt, double dblMinX, double dblMaxX, double dblMinY, double dblMaxY)
        {
            //minx should have the coordinate position of 0.0
            //maxx should have the coordinate position of 1.0
            //the coordinate points hence change from 0 to 1 from minx to maxx
            //at any given point x, the value should be x-minx/(maxx - minx)

            dblX = (pt.X - dblMinX) / (dblMaxX - dblMinX);

            //Similarly for Y
            dblY = (pt.Y - dblMinY) / (dblMaxY - dblMinY);
        }

        /// <summary>
        /// saves a geometry model 3d object to file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="geometry"></param>
        public static void SaveGeometryModel3D(string filePath, Model3D geometry)
        {
            try
            {
                var textWriter = new StreamWriter(filePath);

                var viewBox = new Viewbox();
                var canvas = new Canvas { Width = 321, Height = 201 };
                viewBox.Child = canvas;
                // The Viewport3D provides a rendering surface for 3-D visual content. --
                var viewport3D = new Viewport3D
                {
                    ClipToBounds = true,
                    Width = 150,
                    Height = 150,
                    Camera = new PerspectiveCamera
                    {
                        Position = new Point3D(0, 0, 8),
                        LookDirection = new Vector3D(0, 0, -1),
                        FieldOfView = 60
                    }
                };

                //This ModelVisual3D defines the light cast in the scene. Without light, the 3D 
                //object cannot be seen. Also, the direction of the lights affect shadowing. If desired,
                //you can create multiple lights with different colors that shine from different directions. --
                var modelVisual3D = new ModelVisual3D
                {
                    Content = new DirectionalLight
                    {
                        Color = Color.FromRgb(255, 255, 255),
                        Direction = new Vector3D(-0.612372, -0.5, -0.612372)
                    }
                };

                viewport3D.Children.Add(modelVisual3D);

                var modelGeometryContainer = new ModelVisual3D { Content = geometry };
                viewport3D.Children.Add(modelGeometryContainer);
                canvas.Children.Add(viewport3D);

                System.Windows.Markup.XamlWriter.Save(viewBox, textWriter);
                textWriter.Flush();
                textWriter.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(String.Format("Following error occured while trying to save: {0}", ex));
            }
        }

        public static void SavePositionsAndTriangleIndicesAsModel(string filePath, Point3DCollection positions, Int32Collection triangleIndexCollection, Color color)
        {
            var modelGeometry = new MeshGeometry3D { Positions = positions, TriangleIndices = triangleIndexCollection };
            SaveMeshGeometryModel(filePath, modelGeometry, color);
        }

        public static void SaveMeshGeometryModel(string filePath, MeshGeometry3D meshGeometryModel, Color color)
        {
            var geometryModel3D = new GeometryModel3D { Geometry = meshGeometryModel };
            var solidColorBrush = new SolidColorBrush { Color = color };
            var material = new DiffuseMaterial(solidColorBrush);
            geometryModel3D.Material = material;
            SaveGeometryModel3D(filePath, geometryModel3D);
        }

        public static List<Triangle> GetTrianglesFromPositionsAndTriangleIndices(Point3DCollection positions, Int32Collection triangleIndexCollection)
        {
            var triangles = new List<Triangle>();
            for (var ctr = 0; ctr < triangleIndexCollection.Count; ctr += 3)
            {
                triangles.Add(new Triangle(positions[triangleIndexCollection[ctr]],
                                           positions[triangleIndexCollection[ctr + 1]],
                                           positions[triangleIndexCollection[ctr + 2]]));
            }
            return triangles;
        }

        public static void SaveMeshGeometryModel(string filePath, MeshGeometry3D meshGeometryModel, Bitmap bitmapTextureImg)
        {
            var textureImagePath = filePath + ".bmp";
            bitmapTextureImg.Save(textureImagePath);
            var geometryModel3D = new GeometryModel3D
            {
                Geometry = meshGeometryModel,
                Material = new DiffuseMaterial
                {
                    Brush = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(textureImagePath, UriKind.Relative)),
                        ViewportUnits = BrushMappingMode.Absolute
                    }
                }
            };
            SaveGeometryModel3D(filePath, geometryModel3D);
        }

        public static T SerializeDeserializeXamlObject<T>(T t) where T : class
        {
            var memory = new MemoryStream();
            System.Windows.Markup.XamlWriter.Save(t, memory);
            memory.Flush();
            memory.Seek(0, SeekOrigin.Begin);
            return XamlReader.Load(memory) as T;
        }
    }
}
