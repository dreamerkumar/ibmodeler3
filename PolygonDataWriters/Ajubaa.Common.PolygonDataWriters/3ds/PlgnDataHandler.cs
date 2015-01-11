using System;
using System.IO;

namespace Ajubaa.Common.PolygonDataWriters._3ds
{
    internal class PlgnDataHandler
    {
        private const short PolygonFlag = 0xF; // 0x7; //All the polygons will be stored with this flag
        private const int MaxBuffLen = 20000; //Max length of the buffer used when copying the file
        private const ushort IdFacesDesc = 0x4120; //polygon chunk header

        private bool _fileCreated;
        private BinaryWriter _writer;

        internal ushort PolygonCount { get; private set; }

        internal void AddPolygon(ushort index1, ushort index2, ushort index3)
        {
            if (PolygonCount >= _3DSWriter.MaxValForUnsignedShort)
                throw new Exception(string.Format("Max value ({0}) reached for polygons.", _3DSWriter.MaxValForUnsignedShort));

            PolygonCount++;

            if (!_fileCreated)
            {
                _writer = new BinaryWriter(new MemoryStream());

                //Write the polygon chunk header
                _writer.Write(IdFacesDesc);

                //This data will be updated later
                _writer.Write((int)0);
                _writer.Write((ushort)0); 
                
                _fileCreated = true;
            }

            //Add the polygon data to the file
            _writer.Write(index1);
            _writer.Write(index2);
            _writer.Write(index3);
            _writer.Write(PolygonFlag);
        }

        internal void CopyPlgnData(ref BinaryWriter copyTo)
        {
            using (var reader = new BinaryReader(_writer.BaseStream))
            {
                var totalLength = (int) _writer.BaseStream.Length;

                _writer.Seek(0, SeekOrigin.Begin); //We have to copy all the data
                copyTo.Seek(0, SeekOrigin.End); //We have to append our data to the end of the file

                //Copy data from one file to another in a loop
                do
                {
                    int length;
                    if (totalLength > MaxBuffLen)
                    {
                        length = MaxBuffLen;
                        totalLength -= MaxBuffLen;
                    }
                    else
                    {
                        length = totalLength;
                        totalLength = 0;
                    }

                    var buffer = reader.ReadBytes(length);

                    copyTo.Write(buffer);

                    copyTo.Seek(0, SeekOrigin.End); //We have to append our data to the end of the file

                } while (totalLength > 0);

                //We can close the temporary file now
                _writer.Close();
            
                _fileCreated = false;
            }
        }
    }
}