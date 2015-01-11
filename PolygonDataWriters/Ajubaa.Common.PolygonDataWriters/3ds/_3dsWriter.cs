using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Color = System.Windows.Media.Color;

namespace Ajubaa.Common.PolygonDataWriters._3ds
{
    public static class _3DSWriter
    {
        internal const ushort MaxValForUnsignedShort = 65535;

        public static void ExportTo3DS(string filePath, GeometryModel3D modelData, Image textureFileData)
        {
            //sanity checks
            if(modelData == null)
                throw new ArgumentException("Geometry model info not supplied.");
            if (!(modelData.Geometry != null && modelData.Geometry is MeshGeometry3D))
                throw new ArgumentException("Passed model is not of type mesh geometry 3d");

            //read specific values from xaml file
            var modelColor = GetModelColor(modelData);
            var mesh = ((MeshGeometry3D)modelData.Geometry);
            var containsTextureInfo = ContainsTextureInfo(mesh);
            var positionCount = mesh.Positions.Count;
            var points = mesh.Positions;
            var textures = mesh.TextureCoordinates;
            var polygons = mesh.TriangleIndices;

            //Open the 3DS file for writing
            var writer = InitializeFile(filePath);
            
            //Write Material Info
            var textureFileName = MakeTextureFileName(filePath);
            MaterialChunkWriter.WriteMaterialChunk(modelColor, containsTextureInfo, textureFileName, writer);

            var objectCount = 0;
            _3DSObjectDataHandler objHandler = null;
            ushort[] setIndicesForObj = null;
            bool[] isIndicesSetForObj = null;
            ushort positionIndexInObj = 0;

            for (var ctr = 0; ctr < polygons.Count; ctr += 3)
            {
                var createNewObject = objHandler == null 
                    || objHandler.PolygonCount >= MaxValForUnsignedShort
                    || objHandler.TextureCoodCount >= MaxValForUnsignedShort
                    || objHandler.VertexCount >= MaxValForUnsignedShort;

                if (createNewObject)
                {
                    if (objHandler != null)
                        objHandler.MakeFinalUpdates();

                    objectCount++;
                    if (objectCount > 9999)
                        throw new Exception("Error: Cannot create 3ds file.\n The number of objects into which the model can be broken down exceeds 9999");

                    objHandler = new _3DSObjectDataHandler(writer, objectCount);
                    setIndicesForObj = new ushort[positionCount];
                    isIndicesSetForObj = new bool[positionCount];
                    positionIndexInObj = 0;     
                }
                
                var origIndices = new[] { (ushort) polygons[ctr], (ushort) polygons[ctr+1], (ushort) polygons[ctr+2] };
                var newIndices = new List<ushort>();
                foreach (var origIndex in origIndices)
                {
                    if (isIndicesSetForObj[origIndex])
                        newIndices.Add(setIndicesForObj[origIndex]);
                    else
                    {//Add the new position

                        //Interchange y and z coordinates and change the sign of y to accomodate 
                        //the difference in the axes orientation between the 3ds and xaml format
                        var origPoint = points[origIndex];
                        var point = new Point3D(origPoint.X, -origPoint.Z, origPoint.Y);
                        objHandler.AddPosition(point);

                        if (textures != null && textures.Count > origIndex)
                        {
                            var texCood = mesh.TextureCoordinates[origIndex];
                            objHandler.AddTextureIndices((float) texCood.X, (float) texCood.Y);
                        }

                        setIndicesForObj[origIndex] = positionIndexInObj;
                        isIndicesSetForObj[origIndex] = true;

                        newIndices.Add(positionIndexInObj);

                        positionIndexInObj++;
                    }
                }
                objHandler.AddPolygonIndices(newIndices[0], newIndices[1], newIndices[2]);
            }
            
            //Make the final updation call on object data handler 
            if (objHandler != null) 
                objHandler.MakeFinalUpdates();

            //Close the 3ds file (after final updations to the header chunks)
            UpdateAndCloseFile(writer);

            //save the texture image data
            SaveTextureImageData(filePath, textureFileData, textureFileName);
        }

        #region helper functions for writing
        /// <summary>
        /// This function does the necessary initialization for creating a 3ds file 
        /// </summary>
        /// <param name="filePath"></param>
        private static BinaryWriter InitializeFile(string filePath)
        {
            const ushort idMainChunk = 0x4D4D;
            const ushort id_3DEditorChunk = 0x3D3D;

            var _3DSFileWriter = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write));
            
            //Write the main chunk
            _3DSFileWriter.Write(idMainChunk);
            const int mainChnkLen = 12;
            _3DSFileWriter.Write(mainChnkLen);

            //Write the 3D editor chunk
            _3DSFileWriter.Write(id_3DEditorChunk);
            const int _3DChnkLen = 6;
            _3DSFileWriter.Write(_3DChnkLen);

            return _3DSFileWriter;
        }

        /// <summary>
        /// Close the 3ds file writer (after final updations to the header chunks)
        /// </summary>
        /// <param name="writer"></param>
        private static void UpdateAndCloseFile(BinaryWriter writer)
        {
            //Update the main chunk length
            var lngOffset = 2;
            writer.Seek(lngOffset, SeekOrigin.Begin);
            var intChunkLength = (int)writer.BaseStream.Length;
            writer.Write(intChunkLength);

            //Update the 3D editor chunk length
            lngOffset += 6;
            intChunkLength -= 6;
            writer.Seek(lngOffset, SeekOrigin.Begin);
            writer.Write(intChunkLength);

            writer.Close();
        }

        /// <summary>
        /// Saves the texture image data at the location specified in the 3ds file
        /// </summary>
        /// <param name="_3DSModelFilePath"></param>
        /// <param name="textureFileData"></param>
        /// <param name="textureFileName"></param>
        private static void SaveTextureImageData(string _3DSModelFilePath, Image textureFileData, string textureFileName)
        {
            if (textureFileData == null)
                return;
            try
            {
                var fileInfo = new FileInfo(_3DSModelFilePath);
                var textureFilePath = string.Format(@"{0}\{1}", fileInfo.DirectoryName, textureFileName);
                textureFileData.Save(textureFilePath);
            }
            catch (Exception exception)
            {
                throw new Exception(
                    string.Format("The following error occured while trying to save the texture file: {0}",
                                  exception.Message));
            }
        }
        #endregion

        #region helper functions to read xaml data
        private static Color GetModelColor(GeometryModel3D mptrModelData)
        {
            Material material;

            if (mptrModelData.Material is MaterialGroup)
                material = ((MaterialGroup) mptrModelData.Material).Children.FirstOrDefault(x => x is DiffuseMaterial);
            else
                material = mptrModelData.Material;

            if (material != null && material is DiffuseMaterial)
            {
                var brush = ((DiffuseMaterial) material).Brush;
                if (brush is SolidColorBrush)
                {
                    var color = ((SolidColorBrush) brush).Color;
                    return new Color {R = color.R, G = color.G, B = color.B};
                }
            }
            return new Color {R = 200, G = 200, B = 200};
        }

        private static bool ContainsTextureInfo(MeshGeometry3D mesh)
        {
            return mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count > 0;
        }

        private static string MakeTextureFileName(string _3DSFilePath)
        {
            var fileInfo = new FileInfo(_3DSFilePath);
            var fileName = fileInfo.Name;

            //Strip the extension
            fileName = fileName.Substring(0, fileName.LastIndexOf('.'));

            //Check the length (If it is more than 8 characters, change the filename
            if (fileName.Length > 8)
            {
                //just keep the first seven characters and a ~ symbol
                fileName = fileName.Substring(0, 7) + "~";

                //Make it uppercase
                fileName = fileName.ToUpper();
            }

            //Add a .jpg extension
            fileName = fileName + ".JPG";

            return fileName;
        }
        #endregion
    }
}