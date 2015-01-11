using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.ImgToObj
{
    public class MoldDataHandler
    {
    	byte[] mArrPointData;
        uint cuboidWidth;
        uint cuboidHeight;
        uint cuboidThickness;
        uint muintTotalPtBytes;
        Stream fp;
        
        /// <summary>
        /// default constructor
        /// </summary>
        public MoldDataHandler() 
        { 
	        cuboidWidth = cuboidHeight = cuboidThickness = 0;
	        mArrPointData = null;
        }

        /// <summary>
        /// main constructor
        /// </summary>
        /// <param name="moldData"></param>
        public MoldDataHandler(Stream moldData) 
        {
        	mArrPointData = null;

	        //open the existing file
            fp = moldData;// new FileStream(inStrCuboidLocatn, System.IO.FileMode.Open, FileAccess.ReadWrite);
            //if(!fp.Open(inStrCuboidLocatn, CFile::modeReadWrite | CFile::shareExclusive))		        
		      //  throw new Exception("Error occured while accessing the mold file.");
		 	
	        if(fp.Length <= (uint)(Constants.MOLD_HEADER_SIZE) ) 
            {
                fp.Close();
                throw new Exception("Invalid or corrupt mold data. The data size is invalid.");
	        }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inT"></param>
        public void RetrieveValues(ref TargetCuboid inT, bool blnCloseAfterRetrieving)
        {

            /////////////////////////Variable Declaration////////////////////////////////////////
            uint totalBits, rem;
            ////////////////////////////////////////////////////////////////////////////////////

            //Read the values from the file 
            //fp.Seek(0, SeekOrigin.Begin);

            //inT.uintXPoints = readInt();
            //inT.uintYPoints = readInt();
            //inT.uintZPoints = readInt();

            //inT.fltMinx = readFloat();	
            //inT.fltMaxx = readFloat();
            //inT.fltMiny = readFloat();
            //inT.fltMaxy = readFloat();
            //inT.fltMinz = readFloat();
            //inT.fltMaxz = readFloat();
            byte[] arrValues = new byte[4];
            int intOffset = 0;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.uintXPoints = BitConverter.ToUInt32(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.uintYPoints = BitConverter.ToUInt32(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.uintZPoints = BitConverter.ToUInt32(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMinx = BitConverter.ToSingle(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMaxx = BitConverter.ToSingle(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMiny = BitConverter.ToSingle(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMaxy = BitConverter.ToSingle(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMinz = BitConverter.ToSingle(arrValues, 0);
            intOffset += 4;
            fp.Position = intOffset;
            if (fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
            inT.fltMaxz = BitConverter.ToSingle(arrValues, 0);


            //byte snapSlideOptn = read();
            intOffset += 4;
            fp.Position = intOffset;
            fp.Read(arrValues, 0, 1);
            byte snapSlideOptn = arrValues[0];

            if (snapSlideOptn != 1)
            {
                fp.Close();
                throw new Exception("This mold type is not for using pictures to create the model");
            }

            /*~~~~~~~~~~~~set the cuboid values~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            cuboidWidth = inT.uintXPoints; cuboidHeight = inT.uintYPoints; cuboidThickness = inT.uintZPoints;

            //find the size of the file in bits
            totalBits = cuboidWidth * cuboidHeight * cuboidThickness;

            //increase the value to make it divisible by 8
            rem = totalBits % 8;
            if (rem != 0)
                totalBits = totalBits + (8 - rem);

            //find the total number of bytes required to store the points
            muintTotalPtBytes = totalBits / 8;

            if (fp.Length != (uint)(Constants.MOLD_HEADER_SIZE + muintTotalPtBytes))
            {
                fp.Close();
                throw new Exception("Invalid or corrupt mold file. The file size is invalid");
            }

            //27-Feb-2008 Changed logic to keep all the point information in the memory
            //uint uintBytesAlongX = (inT.uintXPoints - inT.uintXPoints%8)/8 + 3;	
            //
            //try {
            //	//Allocate memory for storing points along X axis temporarily		
            //	g_btMldPointData = (byte *)malloc(uintBytesAlongX); 
            //	if(g_btMldPointData == NULL) {
            //			error = FILE_POINTS_RETRIEVE_VALUES + MEMORY_ALLOCATION_FAILURE;
            //			fp.Close();
            //	}
            try
            {
                //Allocate memory for storing points along X axis temporarily		
                mArrPointData = new byte[muintTotalPtBytes];
                if (mArrPointData == null)
                {
                    fp.Close();
                    throw new Exception("Could not allocate memory to store the mold data");
                }
                //End of changes done on 27-Feb-2008

            }
            catch (Exception e)
            {
                mArrPointData = null;
                fp.Close();
                throw new Exception("Could not allocate memory to store the mold data. The following error occured: " + e.Message);
            }

            //Change done 27-Feb-2008
            //Read all the mold point data and keep it in memory
            //fp.Seek(Constants.MOLD_HEADER_SIZE, SeekOrigin.Begin);
            //if(fp.Read(g_btMldPointData, totalBytes) != totalBytes)
            //{
            //    fp.Close();
            //    throw new Exception("Error occured while accessing the file");
            //}
            //End of change done on 27-Feb-2008
            fp.Position = Constants.MOLD_HEADER_SIZE;
            if (fp.Read(mArrPointData, 0, (int)muintTotalPtBytes) != (int)muintTotalPtBytes)
                throw new Exception("An error occured while trying to read the mold file data.");
            
            if (blnCloseAfterRetrieving)
                fp.Close();
        }
        
        #region functions for setting invalid points in the mold file        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        /// <param name="inZ"></param>
        /// <returns></returns>
        bool SetPoint(uint inX, uint inY, uint inZ) 
        {
	        /*~~~~~~~~~Variable declaration~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	        uint bytePos =0;
            uint zeroBitPos =0;
	        byte readByte =0;
            byte byteToWrite =0;
	        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

            if (!GetZeroIndexedBitBytePos(inX, inY, inZ, out zeroBitPos, out bytePos))
                return false;
        	
	        //Change on 27-Feb-07 Don't read from file all the time
	        //fp.Seek((bytePos - 1), SeekOrigin.Begin);//If we want to read the nth byte then we
							        //need to bypass n-1 bytes
	        //get the byte from the file
	        //readByte = read();
	        readByte = mArrPointData[bytePos];
	        //End of change done on 27-Feb-2008

	        //set the appropriate bit to zero to make it invalid 
	        switch(zeroBitPos) {
	        case 8 :
		        byteToWrite = 254;
		        break;
	        case 7 :
		        byteToWrite = 253;
		        break;
	        case 6 :
		        byteToWrite = 251;
		        break;
	        case 5 :
		        byteToWrite = 247;
		        break;
	        case 4 :
		        byteToWrite = 239;
		        break;
	        case 3 :
		        byteToWrite = 223;
		        break;
	        case 2 :
		        byteToWrite = 191;
		        break;
	        case 1 :
		        byteToWrite = 127;
		        break;
	        }

	        byteToWrite = (byte) (readByte & byteToWrite) ; //the particular bit pos will be set to zero
										           // while the other bits will remain as they are	
	        if(readByte != byteToWrite) 
	        {
		        //Change on 27-Feb-2008 just update the memory. We will write all file data at the end
		        //move back to the previous position
		        //fp.Seek(-1, SeekOrigin.Current);
		        //write to the file
		        //write(byteToWrite);
		        mArrPointData[bytePos] = byteToWrite;
		        //End of change done 27-Feb-07
	        }
        	
	        return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inUintX1"></param>
        /// <param name="inUintX2"></param>
        /// <param name="inUintY"></param>
        /// <param name="inUintZ"></param>
        /// <returns></returns>
        public bool SetPointRanges(uint inUintX1, uint inUintX2, uint inUintY, uint inUintZ)  
        {

	        if(inUintX1 == inUintX2) 
		        return SetPoint(inUintX1, inUintY, inUintZ);

	        //Temporary code :~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	        //for(uint uintCtr = inUintX1; uintCtr <= inUintX2; uintCtr++) 		
	        //	setPoint(uintCtr, inUintY, inUintZ);
	        //return SUCCESS;
	        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

	        uint uintBitPos1, uintBytePos1, uintBitPos2, uintBytePos2;
        	
	        //Get the position of the bits and the bytes
	        GetZeroIndexedBitBytePos(inUintX1, inUintY, inUintZ, out uintBitPos1, out uintBytePos1);
        	
       	
	        GetZeroIndexedBitBytePos(inUintX2, inUintY, inUintZ, out uintBitPos2, out uintBytePos2);
        	
    
	        uint uintTotBytes = uintBytePos2 + 1 - uintBytePos1; //The total number of bytes traversed
        	
	        //Change on 27-Feb-08. No need to read the bytes from the file as we are going to retain them in memory
	        //fp.Seek((uintBytePos1 -1), SeekOrigin.Begin);//if we want to read the nth byte then we
							        //need to bypass n-1 bytes
	        //if(fp.Read(g_btMldPointData, uintTotBytes) != uintTotBytes)
	        //	throw new ExceptionWithMessage("The following error occured while reading the mold file. Invalid file size.");
	        //End of change done on 27-Feb-2008

	        if(uintBytePos1 == uintBytePos2) {//All bits lie in a single byte

		        //Set the appropriate bits in the single byte
                if (uintBitPos1 == 1 && uintBitPos2 == 8)
                    //*g_btMldPointData = 0;
                    mArrPointData[uintBytePos1] = 0;
                else
                    //setInvalidBits(*(g_btMldPointData + uintBytePos1), uintBitPos1, uintBitPos2);
                    SetInvalidBits(ref mArrPointData[uintBytePos1], (byte)uintBitPos1, (byte)uintBitPos2);
	        } 
            else 
            {        		
		        //Set the bits of the first byte
		        if(uintBitPos1 == 1)
			        //*g_btMldPointData = 0;
                    mArrPointData[uintBytePos1] = 0;
		        else
			        //setInvalidBits(*(g_btMldPointData + uintBytePos1), uintBitPos1, 8);		
                    SetInvalidBits(ref mArrPointData[uintBytePos1], (byte)uintBitPos1, 8);		
        		
		        //Set all the bytes that lie in between the first and the last byte to zero
                if (uintTotBytes > 2)
                //memset((g_btMldPointData+ 1 + uintBytePos1), 0, uintTotBytes -2);
                {
                    uint uintMaxPos = uintBytePos2 - 1;
                    for (uint uintBytePos = uintBytePos1 + 1; uintBytePos <= uintMaxPos; uintBytePos++)
                    {
                        mArrPointData[uintBytePos] = 0;
                    }
                }
        		
		        //Set the bits of the last byte
		        if(uintBitPos2 == 8)
			        //*(g_btMldPointData + uintTotBytes + uintBytePos1 - 1) = 0;
                    mArrPointData[uintBytePos2] = 0;
		        else
			        //setInvalidBits(*(g_btMldPointData + uintTotBytes  + uintBytePos1 - 1), 1, uintBitPos2);				
                    SetInvalidBits(ref mArrPointData[uintBytePos2], 1, (byte)uintBitPos2);				
	        }

	        //Change on 27-Feb-2008 just update the memory. We will write all file data at the end
	        //fp.Seek(-(int)uintTotBytes, SeekOrigin.Current);//Move back to the original position to write
												        //the byte				
	        //Write the bits back to the file
	        //fp.Write(g_btMldPointData, uintTotBytes);
	        //End of change done 27-Feb-07

	        return true;
        }

        /// <summary>
        /// Sets the particular bits in the bytes to zero. The start and the end positions are defined from left to right
        /// </summary>
        /// <param name="inOutBtTarget"></param>
        /// <param name="btStartBit"></param>
        /// <param name="btEndBit"></param>
        void SetInvalidBits(ref byte inOutBtTarget, byte btStartBit, byte btEndBit) 
        {
        	
	        if(!(btStartBit > 0 && btStartBit < 9 && btEndBit > 0 && btEndBit < 9 
		        && btStartBit <= btEndBit))
                throw new Exception("Set Invalid bits called with invalid parameters");

	        byte btMyByte = 255;
        	
	        byte[] btSubtract = new byte[] {128, 64, 32, 16, 8, 4, 2, 1};

	        for(byte btCtr = btStartBit; btCtr <= btEndBit; btCtr++) 
		        btMyByte-= btSubtract[btCtr-1];	

	        inOutBtTarget = (byte) (inOutBtTarget & btMyByte);

        }

        /// <summary>
        ///Returns the corresponding byte for a particular bit. 
        /// </summary>
        /// <param name="inUintX"></param>
        /// <param name="inUintY"></param>
        /// <param name="inUintZ"></param>
        /// <param name="outUintBitPos"></param>
        /// <param name="outUintBytePos"></param>
        /// <returns></returns>
        bool GetZeroIndexedBitBytePos(uint inUintX, uint inUintY, uint inUintZ, out uint outUintBitPos, out uint outUintBytePos) 
        {        	
        	
	        /*~~~~~~~~~~~~~~~~~~~~~~~~~~DESCRIPTION OF FILE FORMAT~~~~~~~~~~~~~~~~~~~~~*/
	        /*The way points are stored in the file:
	        The points are stored the way the cuboid exists in the 
	        coordinate system. First the point (1,1,1) is stored. Then we
	        increase x and store points till (cuboidWidth, 1, 1).We repeat
	        this series from y index 1 to cuboidHeight. So the first plane
	        at z =1 is stored. We then store bits for z = 2 and so on till
	        cuboidThickness.
	        ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

           if (inUintX <= 0 || inUintY <= 0 || inUintZ <= 0)
               throw new Exception("The number of points along the three axes is not allocated.");
        	
	        //Find the position in terms of bits
        	
	        uint uintBits	= ( (inUintZ - 1) * cuboidWidth * cuboidHeight ) 
			        + ( (inUintY - 1) * cuboidWidth  )
			        + inUintX; 
        	
	        outUintBitPos = uintBits%8; 
        	
	        if(outUintBitPos == 0 )
		        outUintBitPos = 8;
	        else
		        uintBits = uintBits + 8 - outUintBitPos ;//if it is not divisible by 8 then 
		        //we make it so, by suitable addition 
        	
            //Find the byte pos
	        outUintBytePos = uintBits/8;

            if (outUintBytePos > muintTotalPtBytes)
                throw new Exception("Byte position exceeded");

            //Added logic on 2-Mar-08. My array index is now zero based so we need to make this position also zero based
            outUintBytePos--;
        	
	        //Commented out the below on 27-Feb-08 as we are not reading from the file again and again but only the mold point data from the memory
	        //outUintBytePos += MOLD_HEADER_SIZE ; //add MOLD_HEADER_SIZE to bypass the cuboid 
					        //information stored at the beginning of the file
	        //End of commenting

	        return true;	
        }
 
        /// <summary>
        /// Increases the stored count of images processed by mold file by 1. Also saves all the modified point data
        /// </summary>
        public void UpdateMoldFileWithNewImg() 
        {
            //uint imgCount;
            //fp.Seek(Constants.MOLD_POS_FOR_IMAGE_COUNT, SeekOrigin.Begin);
            //imgCount = readInt();
            //imgCount++; //Increment the count by 1
            int imgCount;
            byte[] arrIntVal = new byte[4];
            fp.Position = Constants.MOLD_POS_FOR_IMAGE_COUNT;
            fp.Read(arrIntVal, 0, 4);
            imgCount = BitConverter.ToInt32(arrIntVal, 0);
            imgCount++; //Increment the count by 1

	        //move back to the previous position
	        //fp.Seek(-4, SeekOrigin.Current);
	        //write to the file
	        // write(imgCount);
            fp.Position = Constants.MOLD_POS_FOR_IMAGE_COUNT;
            fp.Write(BitConverter.GetBytes(imgCount), 0, BitConverter.GetBytes(imgCount).Length); 

	        //This would also mean that the mold points have been successfully altered, so this would be the right place to write back the altered points
	        //fp.Seek(Constants.MOLD_HEADER_SIZE, SeekOrigin.Begin);
	        //fp.Write(g_btMldPointData, totalBytes);
            fp.Position = Constants.MOLD_HEADER_SIZE;
            fp.Write(mArrPointData, 0, (int)muintTotalPtBytes);
            
            //fp.Close();

        }
        #endregion

        #region functions for analyzing mold file for lost data
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inX"></param>
        /// <param name="inY"></param>
        /// <param name="inZ"></param>
        /// <returns></returns>
        bool CheckForAValidPoint(uint inX, uint inY, uint inZ, ref bool blnAValidPtFound)
        {
            //Variable declaration
            uint bytePos = 0;
            uint uintBitPos = 0;
            byte checkByte = 0;

            if (!Get0IndexedByteAnd1IndexedBitPos(inX, inY, inZ, out uintBitPos, out bytePos))
                return false;

            //set the appropriate bit to zero to make it invalid 
            switch (uintBitPos)
            {
                case 1:
                    checkByte = 128;
                    break;
                case 2:
                    checkByte = 64;
                    break;
                case 3:
                    checkByte = 32;
                    break;
                case 4:
                    checkByte = 16;
                    break;
                case 5:
                    checkByte = 8;
                    break;
                case 6:
                    checkByte = 4;
                    break;
                case 7:
                    checkByte = 2;
                    break;
                case 8:
                    checkByte = 1;
                    break;

            }
            if ((mArrPointData[bytePos] & checkByte) == checkByte)
                blnAValidPtFound = true;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inUintX1"></param>
        /// <param name="inUintX2"></param>
        /// <param name="inUintY"></param>
        /// <param name="inUintZ"></param>
        /// <returns></returns>
        public bool CheckPointRangesForValidity(uint inUintX1, uint inUintX2, uint inUintY, uint inUintZ, ref bool blnAValidPtFound)
        {
            if (inUintX1 == inUintX2)
                return CheckForAValidPoint(inUintX1, inUintY, inUintZ, ref blnAValidPtFound);

            //Temporary code :~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //for(uint uintCtr = inUintX1; uintCtr <= inUintX2; uintCtr++) 		
            //	setPoint(uintCtr, inUintY, inUintZ);
            //return SUCCESS;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            uint uintBitPos1, uintBytePos1, uintBitPos2, uintBytePos2;

            //Get the position of the bits and the bytes
            Get0IndexedByteAnd1IndexedBitPos(inUintX1, inUintY, inUintZ, out uintBitPos1, out uintBytePos1);

            Get0IndexedByteAnd1IndexedBitPos(inUintX2, inUintY, inUintZ, out uintBitPos2, out uintBytePos2);

            uint uintTotBytes = uintBytePos2 + 1 - uintBytePos1; //The total number of bytes traversed       	

            if (uintBytePos1 == uintBytePos2)
            {//All bits lie in a single byte
                blnAValidPtFound = CheckBitsForAValidPt(mArrPointData[uintBytePos1], (byte)uintBitPos1, (byte)uintBitPos2);
                if (blnAValidPtFound)
                    return true;
            }
            else
            {
                //Check the bits in the first byte
                blnAValidPtFound = CheckBitsForAValidPt(mArrPointData[uintBytePos1], (byte)uintBitPos1, 8);
                if (blnAValidPtFound)
                    return true;

                //Set all the bytes that lie in between the first and the last byte to zero
                if (uintTotBytes > 2)
                {
                    uint uintMaxPos = uintBytePos2 - 1;
                    for (uint uintBytePos = uintBytePos1 + 1; uintBytePos <= uintMaxPos; uintBytePos++)
                    {
                        if (mArrPointData[uintBytePos] > 0) //At least one bit is valid
                        {
                            blnAValidPtFound = true;
                            return true;
                        }
                    }
                }

                //Check the bits in the last byte
                blnAValidPtFound = CheckBitsForAValidPt(mArrPointData[uintBytePos2], 1, (byte)uintBitPos2);
                if (blnAValidPtFound)
                    return true;
            }

            return true;
        }

        /// <summary>
        /// Sets the particular bits in the bytes to zero. The start and the end positions are defined from left to right
        /// </summary>
        /// <param name="inOutBtTarget"></param>
        /// <param name="btStartBit"></param>
        /// <param name="btEndBit"></param>
        bool CheckBitsForAValidPt(byte btCurrByte, byte btStartBit, byte btEndBit)
        {
            if (!(btStartBit > 0 && btStartBit < 9 && btEndBit > 0 && btEndBit < 9 && btStartBit <= btEndBit))
                throw new Exception("CheckBitsForAValidPt called with invalid parameters");

            byte[] arrValuesForDiffBitPos = new byte[] { 111, 128, 64, 32, 16, 8, 4, 2, 1, }; //first value is a filler while the rest of the values are the value of the byte for different bit position from 1 to 8
            for (byte btCtr = btStartBit; btCtr <= btEndBit; btCtr++)
            {
                if ((arrValuesForDiffBitPos[btCtr] & btCurrByte) == arrValuesForDiffBitPos[btCtr]) //a one was encountered
                    return true;
            }
            return false;
        }

        /// <summary>
        ///Returns the corresponding byte for a particular bit. 
        /// </summary>
        /// <param name="inUintX"></param>
        /// <param name="inUintY"></param>
        /// <param name="inUintZ"></param>
        /// <param name="outUintBitPos"></param>
        /// <param name="outUintBytePos"></param>
        /// <returns></returns>
        bool Get0IndexedByteAnd1IndexedBitPos(uint inUintX, uint inUintY, uint inUintZ, out uint outUintBitPos, out uint outUintBytePos)
        {

            /*~~~~~~~~~~~~~~~~~~~~~~~~~~DESCRIPTION OF FILE FORMAT~~~~~~~~~~~~~~~~~~~~~*/
            /*The way points are stored in the file:
            The points are stored the way the cuboid exists in the 
            coordinate system. First the point (1,1,1) is stored. Then we
            increase x and store points till (cuboidWidth, 1, 1).We repeat
            this series from y index 1 to cuboidHeight. So the first plane
            at z =1 is stored. We then store bits for z = 2 and so on till
            cuboidThickness.
            ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

            if (inUintX <= 0 || inUintY <= 0 || inUintZ <= 0)
                throw new Exception("The number of points along the three axes is not allocated.");

            //Find the position in terms of bits

            uint uintBits = ((inUintZ - 1) * cuboidWidth * cuboidHeight)
                    + ((inUintY - 1) * cuboidWidth)
                    + inUintX;

            outUintBitPos = uintBits % 8;

            if (outUintBitPos == 0)
                outUintBitPos = 8;
            else
                uintBits = uintBits + 8 - outUintBitPos;//if it is not divisible by 8 then 
            //we make it so, by suitable addition 

            //Find the byte pos
            outUintBytePos = uintBits / 8;

            if (outUintBytePos > muintTotalPtBytes)
                throw new Exception("Byte position exceeded");

            //Added logic on 2-Mar-08. My array index is now zero based so we need to make this position also zero based
            outUintBytePos--;

            return true;
        }
        #endregion
    }
}
