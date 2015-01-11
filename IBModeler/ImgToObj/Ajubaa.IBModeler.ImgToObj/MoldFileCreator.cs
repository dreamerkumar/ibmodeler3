using System;
using System.IO;
using Ajubaa.IBModeler.Common;
namespace Ajubaa.IBModeler.ImgToObj
{
    public static class MoldFileCreator
    {
        public static Stream CreateNewMoldFile(float valueRangeX, float valueRangeY, float valueRangeZ)
        {
            return CreateNewMoldFile(Constants.UINT_DEFAULT_PT_DENSITY, valueRangeX, valueRangeY, valueRangeZ);
        }

        public static Stream CreateNewMoldFile(int uintPtDensity, float valueRangeX, float valueRangeY, float valueRangeZ) 
        {
            Stream fp = null;
            try
            {
                //open a memory stream for writing
                fp = new MemoryStream();
                fp.Seek(0, SeekOrigin.Begin);

                #region write header information
                //write point densities along the three axes on the target cuboid
                int uintPtDensityX, uintPtDensityY, uintPtDensityZ;
                uintPtDensityX = uintPtDensityY = uintPtDensityZ = uintPtDensity;
                fp.Write(BitConverter.GetBytes(uintPtDensityX), 0, 4);
                fp.Write(BitConverter.GetBytes(uintPtDensityY), 0, 4);
                fp.Write(BitConverter.GetBytes(uintPtDensityZ), 0, 4);

                //write cuboid ranges along the three axes
                WriteCuboidRanges(fp, valueRangeX, valueRangeY, valueRangeZ);

                //whether snaps or slides option is chosen. slides option is redundant and 1 means snaps option
                byte btOption = 1;
                fp.Write(BitConverter.GetBytes(btOption), 0, 1);
                //number of files processed
                int intFilesProcessed = 0;
                fp.Write(BitConverter.GetBytes(intFilesProcessed), 0, 4);
                #endregion write header information

                #region store cuboid data in the file
                // a one at a particular position signifies the point is valid
                // a zero at a particular position signifies the point is invalid
                // so we need to set all the bits to 1(valid) initially

                //get size of the mold data in bits in bits
                var uintTotalBits = uintPtDensityX * uintPtDensityY * uintPtDensityZ; 

                //increase the value to make it divisible by 8
                var uintRem = uintTotalBits % 8;
                if (uintRem != 0)
                    uintTotalBits = uintTotalBits + (8 - uintRem);

                //find the total number of bytes required to store the points
                var uintTotalBytes = uintTotalBits / 8;
                var uintBytesToWrite = uintTotalBytes + Constants.MOLD_HEADER_EXTRA_BYTES;
                uintTotalBytes = uintBytesToWrite;

                var btData = new byte[uintTotalBytes];//allocate bytes
                for (var uintCtr = 0; uintCtr < uintTotalBytes; uintCtr++)
                    btData[uintCtr] = 255; //set all bit positions to 1
                //write bytes
                if (uintTotalBytes > Int32.MaxValue)
                    throw new Exception("Cannot create mold file. The value of total bytes is greater than max int value.");
                fp.Write(btData, 0, uintTotalBytes);
                #endregion

                return fp;                
            }
            catch (Exception e)
            {
                if (fp != null)
                {
                    try { fp.Close(); } catch { }
                }
                throw new Exception("The following error occured while creating mold data :" + e.Source + "::" + e.Message);
            }
        }

        private static void WriteCuboidRanges(Stream fp, float valueRangeX, float valueRangeY, float valueRangeZ)
        {
            var halfOfValueRangeX = valueRangeX / 2.0f;
            var fltMinX = -halfOfValueRangeX;
            var fltMaxX = halfOfValueRangeX;

            var halfOfValueRangeY = valueRangeY / 2.0f;
            var fltMinY = -halfOfValueRangeY;
            var fltMaxY = halfOfValueRangeY;

            var halfOfValueRangeZ = valueRangeZ / 2.0f;
            var fltMinZ = -halfOfValueRangeZ;
            var fltMaxZ = halfOfValueRangeZ;

            fp.Write(BitConverter.GetBytes(fltMinX), 0, 4);
            fp.Write(BitConverter.GetBytes(fltMaxX), 0, 4);
            fp.Write(BitConverter.GetBytes(fltMinY), 0, 4);
            fp.Write(BitConverter.GetBytes(fltMaxY), 0, 4);
            fp.Write(BitConverter.GetBytes(fltMinZ), 0, 4);
            fp.Write(BitConverter.GetBytes(fltMaxZ), 0, 4);
        }
    }
}
