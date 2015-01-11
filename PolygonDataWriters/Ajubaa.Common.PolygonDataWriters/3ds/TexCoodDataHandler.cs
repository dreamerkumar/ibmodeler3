using System;
using System.IO;

namespace Ajubaa.Common.PolygonDataWriters._3ds
{
    internal class TexCoodDataHandler
    {
        //Max length of the buffer used when copying the file
        private const int MaxBuffLenForTexCood = 20000; 
        
        private bool _fileCreated;
        private BinaryWriter _writer;

        public ushort TextureCoodCount { get; private set; }

        internal void AddTexCoordinates(float u, float v)
        {
            if (TextureCoodCount >= _3DSWriter.MaxValForUnsignedShort)
                throw new Exception(string.Format("Max value ({0}) reached for texture coordinates.", _3DSWriter.MaxValForUnsignedShort));

            TextureCoodCount++;

            //Create and open the file for writing if not already done
            if (!_fileCreated)
            {
                _writer = new BinaryWriter(new MemoryStream());
                _fileCreated = true;
            }

            //Add the texture coordinate data to the file
            _writer.Write(u);

            //Reversing the direction of v
            v = 1.0f - v; 
            _writer.Write(v);
        }

        internal void CopyTexCoodData(ref BinaryWriter copyTo, ushort noOfFaces)
        {
            using (var reader = new BinaryReader(_writer.BaseStream))
            {
                copyTo.Seek(0, SeekOrigin.End); //We have to append our data to the end of the file

                //Step 1: Write the chunk header for texture coordinates
                const ushort idMappingCoodList = 0x4140;
                copyTo.Write(idMappingCoodList);
                var chunkLength = MappingCoodChunkLength();
                copyTo.Write(chunkLength);

                //Step 2: Write the number of vertices for which the u, v values exist
                copyTo.Write(TextureCoodCount);

                //Step 3: Copy the texture data
                var unwrittenBufferLength = (int) _writer.BaseStream.Length;

                _writer.Seek(0, SeekOrigin.Begin); //We have to copy all the data
                //Copy data from one file to another in a loop
                do
                {
                    int currentBufferLength;
                    if (unwrittenBufferLength > MaxBuffLenForTexCood)
                    {
                        currentBufferLength = MaxBuffLenForTexCood;
                        unwrittenBufferLength -= MaxBuffLenForTexCood;
                    }
                    else
                    {
                        currentBufferLength = unwrittenBufferLength;
                        unwrittenBufferLength = 0;
                    }
                    var buffer = reader.ReadBytes(currentBufferLength);

                    copyTo.Write(buffer);

                    copyTo.Seek(0, SeekOrigin.End); //We have to append our data to the end of the file

                } while (unwrittenBufferLength > 0);

                //We can close the temporary file now
                _writer.Close();
            
                _fileCreated = false;
                TextureCoodCount = 0;
                _writer.Close();
            }
        }

        internal bool DoesTextureValueExist()
        {
            return _fileCreated; //file is created only when values are added
        }

        internal int MappingCoodChunkLength()
        {
            return (2 + 4) + (sizeof (ushort)) + (TextureCoodCount*2*sizeof (float));
        }
    }
}