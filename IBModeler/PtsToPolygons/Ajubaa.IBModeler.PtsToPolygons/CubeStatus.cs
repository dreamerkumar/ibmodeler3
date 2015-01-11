using System;
using System.Collections.Generic;
using Ajubaa.IBModeler.PtsToPolygons;

public class CubeStatus
{
    private int _cubesX;
	private int _cubesY;
	private byte[] _curCubes;
    private byte[] _prevCubes;
	private int _totalBytes;

    //initialize cube for x and y
	public void InitCubeStatus(int x, int y)
	{
	    if(!(x > 0 && x <= 1000 && y > 0 && y <= 1000))
            throw  new Exception("initCubeStatus: Invalid value of x and y");
	
		_cubesX = x;
		_cubesY = y;
	
		//find the size of the file in bits
		var totalBits = _cubesX * _cubesY;
	
		//increase the value to make it divisible by 8
		var rem = totalBits%8;
		if(rem != 0)
			totalBits = totalBits + (8 - rem);
	
		//find the total number of bytes required to store the points
		_totalBytes = totalBits / 8;
	
        _curCubes = new byte[_totalBytes];
	    _prevCubes = new byte[_totalBytes];
	
		// A one at a particular position signifies the cube is not valid
		// A zero at a particular position signifies the cube is valid.
		// So we need to set all the bits to 1(invalid).
        InitializeByteArrayToMaxByteValue(_curCubes);
	}

    /// <summary>
	///	Goes to specified index and sets the bit value to zero
	///	
	///	For a particular cube index:
	///	1.	If the bit value is zero then the cube is valid.
	///	2.	If the bit value is one then the cube is not valid.
	///
	///Order in which the cube indices are stored:
	///  	 
	///	  The cube info is always stored for two planes. The current plane and the
	///	  the previous plane. When the cubes for the first plane are accessed the 
	///	  previous plane has no significance. For each plane, we store all cubes 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
	public void SetStatusAsValid(int x, int y)
	{
	    //Variable declaration

        if(!(x > 0 && x <= _cubesX && y > 0 && y <= _cubesY))
            throw new ArgumentException("Invalid call to setStatusAsValid.");
	
		//find the position in terms of individual bits
	
		var bits = ((y - 1) * _cubesX) + x;
	
		var zeroBitPos = bits%8;
		//in the byte of 8 bits that is to be set to zero. For eg, if the value of
		//zeroBitPos is 0, then the bit to be altered is the 8th one in the
		//current byte; If its 7 then it means the first element in the next byte.
	
		//find the byte that have to be traversed
	
		if(zeroBitPos > 0)
			bits = bits + 8 - zeroBitPos; //if it is not divisible by 8 then
			//we make it so, by suitable addition
	
		var bytePos = bits/8;
	
		bytePos = bytePos - 1; //if we want to read the nth byte then we
								//need to bypass n-1 bytes
	
		var readByte = _curCubes[bytePos];
	
		//set the appropriate bit to zero to make it invalid
        byte byteToWrite;
		switch(zeroBitPos)
		{
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
		case 0 :
			byteToWrite = 254;
		        break;
		    default:
                throw new Exception("Invalid zeroBitPos");
			break;
		}
	
		var abyteToWrite = readByte & byteToWrite; //the particular bit pos will be set to zero
											   // while the other bits will remain as they are
		_curCubes[bytePos] = (byte)abyteToWrite;
	
	}
	
    public bool IsValidCube(int x, int y, PlaneType planeType)
	{
	    if(!(x > 0 && x <= _cubesX && y > 0 && y <= _cubesY))
            throw new ArgumentException("Invalid call to isValidCube");
	
		//find the position in terms of individual bits
		var bits = ((y - 1) * _cubesX) + x;
	
		var bitPos = bits%8;
		//in the byte of 8 bits. For eg, if the value of
		//bitPos is 0, then the bit to be accessed is the 8th one in the
		//current byte; If its 7 then it means the first element in the next byte.
	
		//find the byte that has to be accessed.
		if(bitPos > 0)
			bits = bits + 8 - bitPos; //if it is not divisible by 8 then we make it so, by suitable addition
		var bytePos = bits/8;
	
		bytePos = bytePos - 1; //if we want to read the nth byte then we need to bypass n-1 bytes

		var curByte = planeType == PlaneType.CurrentPlane ? _curCubes[bytePos] : _prevCubes[bytePos];
	    byte checkByte = 0;
		switch(bitPos)
		{
			case 1 :
				checkByte = 128;
				break;
			case 2 :
				checkByte = 64;
				break;
			case 3 :
				checkByte = 32;
				break;
			case 4 :
				checkByte = 16;
				break;
			case 5 :
				checkByte = 8;
				break;
			case 6 :
				checkByte = 4;
				break;
			case 7 :
				checkByte = 2;
				break;
			case 0 :
				checkByte = 1;
				break;
		}
	
		if((curByte & checkByte) == checkByte)
			return false; //The point is set to 1 which means it is not processed
	    return true; //The point is set to zero which means it is processed
	}

	/// <summary>
	///As we are moving to the next z plane, the status of cubes for the current z 
	///index which is stored in curCubes should now be stored as prevCubes. We also need a new
	///curCubes. 
	/// </summary>
	public void GoToNextPlane()
	{
	    //copy value to prev
		_curCubes.CopyTo(_prevCubes, 0);

		//initialize
		InitializeByteArrayToMaxByteValue(_curCubes);
	}

    private static void InitializeByteArrayToMaxByteValue(IList<byte> byteArray)
    {
        for (var ctr = 0; ctr < byteArray.Count - 1; ctr++)
            byteArray[ctr] = 255;
    }
}
