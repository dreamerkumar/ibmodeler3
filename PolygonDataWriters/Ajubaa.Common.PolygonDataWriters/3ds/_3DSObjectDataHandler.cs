using System;
using System.IO;
using System.Windows.Media.Media3D;

namespace Ajubaa.Common.PolygonDataWriters._3ds
{
    /// <summary>
    /// PlgnDataHandler will store the polygon data separately in a temporary file and
    /// add it to the main file after we have finished writing data for each object
    /// TexCoodHandler will store the texture coordinate data (if the model contains textures)
    /// separately in a temporary file and add it to the main file
    /// after we have finished writing data for each object
    /// </summary>
    internal class _3DSObjectDataHandler
    {
        private const ushort IdObjectBlock = 0x4000;
        private const ushort IdTriangularMesh = 0x4100;
        private const ushort IdVerticesList = 0x4110;

        private const int ObjNameLen = 8;
        
        private PlgnDataHandler _polygonDataHandler = new PlgnDataHandler();
        private TexCoodDataHandler _texCoordinatesHandler = new TexCoodDataHandler();

        private BinaryWriter _writer;

        public ushort VertexCount { get; private set; }

        public ushort PolygonCount { get { return _polygonDataHandler.PolygonCount; } }

        public ushort TextureCoodCount { get { return _texCoordinatesHandler.TextureCoodCount; } }

        /// <summary>
        ///does the required initialization when a new object is to be added to the file
        /// </summary>
        public _3DSObjectDataHandler(BinaryWriter writer, int objectCount)
        {
            _writer = writer;
 
            //Formulate the name of the object block
            var objectName = GetObjectName(objectCount);

            if (objectName.Length != ObjNameLen - 1) //-1 for null character 
                throw new Exception("Invalid object name length");

            _writer.Seek(0, SeekOrigin.End); //Move to the end of the file to write

            //Write the object block chunk
            _writer.Write(IdObjectBlock);
            _writer.Write((int)0);
            _writer.Write(objectName.ToCharArray()); //Writer(string) writes a length-prefixed string
            _writer.Write('\0');

            //Write the triangular mesh chunk
            _writer.Write(IdTriangularMesh);
            _writer.Write((int)0);

            //Write the vertices chunk
            _writer.Write(IdVerticesList);
            _writer.Write((int)0);
            _writer.Write(VertexCount);
        }

        private static string GetObjectName(int count)
        {
            string objectName;

            if (count < 10)
                objectName = String.Format("obj000{0:D}", count);
            else if (count < 100)
                objectName = String.Format("obj00{0:D}", count);
            else if (count < 1000)
                objectName = String.Format("obj0{0:D}", count);
            else
                objectName = String.Format("obj{0:D}", count);

            return objectName;
        }
        
        public void AddPosition(Point3D point)
        {
            if (VertexCount >= _3DSWriter.MaxValForUnsignedShort)
                throw new Exception(string.Format("Vertex count for this object exceeded the maximum limit : {0}", _3DSWriter.MaxValForUnsignedShort));

            VertexCount++;

            _writer.Write((float)point.X);
            _writer.Write((float)point.Y);
            _writer.Write((float)point.Z);
        }

        internal void AddPolygonIndices(ushort p, ushort p_2, ushort p_3)
        {
            _polygonDataHandler.AddPolygon(p, p_2, p_3);
        }

        internal void AddTextureIndices(float u, float v)
        {
            _texCoordinatesHandler.AddTexCoordinates(u, v);
        }

        /// <summary>
        ///This function should be called after all the vertices and polygons for each object
        ///have been written to the file. The function updates the following information: 
        ///The main chunk length
        ///The 3D editor chunk length
        ///The object chunk length
        ///The triangular mesh chunk length
        ///The vertices chunk length
        ///The number of vertices
        ///The polygons chunk length
        ///The number of polygons
        ///Texture information 
        /// </summary>
        public void MakeFinalUpdates()
        {
            //Copy the polygon data from the temp file to the main file
            _polygonDataHandler.CopyPlgnData(ref _writer);

            //Write the material information for all the objects
            var faceMaterialChunkLength = MaterialChunkWriter.GetMaterialListChunkLength(_polygonDataHandler.PolygonCount);
            MaterialChunkWriter.WriteMatInformationForObjFaces(_polygonDataHandler.PolygonCount, faceMaterialChunkLength, _writer);

            //Copy the texture data (u,v) values from the temp file to the main file
            //Store the length of the texture chunk in a variable before it is reset in the below call
            var texCoodChunkLength = 0;
            if (_texCoordinatesHandler.DoesTextureValueExist())
            {
                texCoodChunkLength = _texCoordinatesHandler.MappingCoodChunkLength();
                _texCoordinatesHandler.CopyTexCoodData(ref _writer, _polygonDataHandler.PolygonCount);
            }
        
            //Calculate the length of the vertices list chunk
            var vertChnkLen = (2 + (VertexCount*3*sizeof (float)) + 6); //6 for the usual header length
            //The first two bytes are for the quantity value written after the vertices header

            //Calculate the length of the polygons chunk (faces chunk)
            var polsChnkLen = (6 + 2 + (_polygonDataHandler.PolygonCount*4*2) + faceMaterialChunkLength);
            //6 for the usual header length

            //Calculate the length of the triangular mesh chunk
            var meshChnkLen = 6 + vertChnkLen + polsChnkLen + texCoodChunkLength; //Header + sub-chunks

            //Calculate the length of the object block chunk
            var objChnkLen = 6 + ObjNameLen + meshChnkLen; //6 for header
        
            var prevLen = _writer.BaseStream.Length - objChnkLen;

            //Update the object chunk length
            var offset = (int) (prevLen + 2); //2 because the chunk length is stored after the chunk id
            _writer.Seek(offset, SeekOrigin.Begin);
            _writer.Write(objChnkLen);

            //Object name is stored at this position in the object chunk

            //Update the triangular mesh chunk length
            offset += 6 + ObjNameLen;
            _writer.Seek(offset, SeekOrigin.Begin);
            _writer.Write(meshChnkLen);

            //Update the vertices chunk length
            offset += 6;
            _writer.Seek(offset, SeekOrigin.Begin);
            _writer.Write(vertChnkLen);
            //Update the number of vertices
            _writer.Write(VertexCount);

            //Update the polygon chunk length
            offset += vertChnkLen;
            _writer.Seek(offset, SeekOrigin.Begin);
            _writer.Write(polsChnkLen);

            //Update the number of polygons
            _writer.Write(_polygonDataHandler.PolygonCount);

            _polygonDataHandler = null;
            _texCoordinatesHandler = null;
        }
    }
}