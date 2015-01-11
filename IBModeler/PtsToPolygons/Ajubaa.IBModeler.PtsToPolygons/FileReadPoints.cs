using System;
using System.IO;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.PtsToPolygons;

public class FileReadPoints
{
    private readonly Stream _fp;
    private int _curZ;
    private bool _firstCall;
    private readonly int _bytesPerPlane;
    private readonly long _fileSize;
    private byte _curBkOffsetBits;
    private byte _curFtOffsetBits;
    private byte _nextOffsetBits;
    private readonly CuboidFileInfo _moldFileInfo = new CuboidFileInfo();

    //Pointers to store planes along z axis
    private byte[] _curBkPlane;
    private byte[] _curFtPlane;
    private byte[] _nextPlane;
	
	public FileReadPoints(Stream moldData)
	{
	    //open the existing file
        try
        {
            _fp = moldData;// new FileStream(moldFilePath, FileMode.Open, FileAccess.ReadWrite);
        }
        catch(Exception e)
        {
            throw  new Exception(string.Format("The following error occured while trying to read the mold file: {0}", e.Message));
        }

	    _fileSize = _fp.Length;

	    if (_fileSize <= Constants.MOLD_HEADER_SIZE)
	        throw new Exception("File size does not match.");

	    //get the cuboid values
		_fp.Position = 0;
	
		//First get the point density (number of points per 10 cm) which is stored in the file
		_moldFileInfo.X = ReadInt();
		_moldFileInfo.Y = ReadInt();
		_moldFileInfo.Z = ReadInt();
	
		//Get the range in 3D space along which the points are defined.
		_moldFileInfo.MinX = ReadFloat();
		_moldFileInfo.MaxX = ReadFloat();
		_moldFileInfo.MinY = ReadFloat();
		_moldFileInfo.MaxY = ReadFloat();
		_moldFileInfo.MinZ = ReadFloat();
		_moldFileInfo.MaxZ = ReadFloat();
	
		if(_moldFileInfo.X < 2 || _moldFileInfo.Y < 2 || _moldFileInfo.Z < 2)
		{
		    throw new Exception("Insufficient number of points in the mold file.");
		}
	
		var totalBytes = _moldFileInfo.X * _moldFileInfo.Y * _moldFileInfo.Z;
		var rem = totalBytes % 8;
		if(rem != 0)
			totalBytes = (totalBytes + 8 - rem)/8;
		else
			totalBytes = totalBytes/8;

	    if (_fileSize != (Constants.MOLD_HEADER_SIZE + totalBytes))
	        throw new Exception("File sizes do not match.");

	    //Allocate memory to pointers
		_bytesPerPlane = _moldFileInfo.X * _moldFileInfo.Y;
	
		rem = _bytesPerPlane % 8;
	
		if(rem == 0)
			_bytesPerPlane = _bytesPerPlane / 8;
		else
			_bytesPerPlane = (_bytesPerPlane + 8 - rem) / 8;

        //Add extra byte to accomodate sliding of data due to shifting of the first bit on accessing consecutive planes.
        _bytesPerPlane++;

        _curBkPlane = new byte[_bytesPerPlane];
        _curFtPlane = new byte[_bytesPerPlane];
        _nextPlane = new byte[_bytesPerPlane];
        
		_curZ = 1;
		_firstCall = true;
	}
	public CuboidFileInfo GetCuboidValues()
	{
	    return new CuboidFileInfo
	    {
            MaxX = _moldFileInfo.MaxX,
            MaxY = _moldFileInfo.MaxY,
            MaxZ = _moldFileInfo.MaxZ,
            MinX = _moldFileInfo.MinX,
            MinY = _moldFileInfo.MinY,
            MinZ = _moldFileInfo.MinZ,
            X = _moldFileInfo.X,
            Y = _moldFileInfo.Y,
            Z = _moldFileInfo.Z
	    };
	}
	public CubeCorners GetNextXFace(int inX, int inY, int inZ)
	{
	    var outCb = new CubeCorners();
	
		if(! (inX > 0 && inX < _moldFileInfo.X && inY > 0 && inY < _moldFileInfo.Y && inZ > 0 && inZ < _moldFileInfo.Z)) 
            throw new ArgumentException("Invalid call to GetNextFace./ Cube indexes should be one less than total points");
	
		outCb.BackBottomRight = IsValidPoint(inX+1, inY, inZ);
		outCb.BackTopRight = IsValidPoint(inX+1, inY+1, inZ);
		outCb.FrontBottomRight = IsValidPoint(inX+1, inY, inZ+1);
		outCb.FrontTopRight = IsValidPoint(inX+1, inY+1, inZ+1);
	
		return outCb;
	}
	public CubeCorners GetCube(int inX, int inY, int inZ)
	{
	    var outCb = new CubeCorners();

        if(! (inX > 0 && inX < _moldFileInfo.X && inY > 0 && inY < _moldFileInfo.Y && inZ > 0 && inZ < _moldFileInfo.Z))
            throw new Exception("Cube indexes should be one less than total points");
	
		outCb.BackTopLeft = IsValidPoint(inX, inY+1, inZ);
		outCb.BackBottomLeft = IsValidPoint(inX, inY, inZ);
		outCb.BackBottomRight = IsValidPoint(inX+1, inY, inZ);
		outCb.BackTopRight = IsValidPoint(inX+1, inY+1, inZ);
	
		outCb.FrontTopLeft = IsValidPoint(inX, inY+1, inZ+1);
		outCb.FrontBottomLeft = IsValidPoint(inX, inY, inZ+1);
		outCb.FrontBottomRight = IsValidPoint(inX+1, inY, inZ+1);
		outCb.FrontTopRight = IsValidPoint(inX+1, inY+1, inZ+1);
	
		return outCb;
	}
	private int ReadInt()
	{
        var arrValues = new byte[4];
        if (_fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
        return BitConverter.ToInt32(arrValues, 0);
	}
	private float ReadFloat()
	{
        var arrValues = new byte[4];
        if (_fp.Read(arrValues, 0, 4) != 4) throw new Exception("File Access Error");
        return BitConverter.ToSingle(arrValues, 0);
	}
    /// <summary>
    /// Returns true if it gets even one valid point on the specified face, else returns false
    /// </summary>
    /// <param name="inX"></param>
    /// <param name="inY"></param>
    /// <param name="inZ"></param>
    /// <param name="inFace"></param>
    /// <returns></returns>
    public bool IsFaceWithValidPt(int inX, int inY, int inZ, CubeFaceType inFace)
    {
        if(!(inX > 0 && inX < _moldFileInfo.X && inY > 0 && inY < _moldFileInfo.Y && inZ > 0 && inZ < _moldFileInfo.Z))
            throw new Exception("cube indexes should be one less than total points");

        if(!(inFace == CubeFaceType.MaxX || inFace == CubeFaceType.MaxY || inFace == CubeFaceType.MaxZ))
            throw new Exception("Face is not a right value.");

        if (inFace == CubeFaceType.MaxX)
        {
            //frontTopRight - frontBottomRight - backTopRight - backBottomRight
            return IsValidPoint(inX + 1, inY, inZ) || IsValidPoint(inX + 1, inY + 1, inZ) || IsValidPoint(inX + 1, inY, inZ + 1) || IsValidPoint(inX + 1, inY + 1, inZ + 1);

        }
        if (inFace == CubeFaceType.MaxY)
        {
            //frontTopRight - frontTopLeft - backTopRight - backTopLeft
            return IsValidPoint(inX, inY + 1, inZ) || IsValidPoint(inX + 1, inY + 1, inZ) || IsValidPoint(inX, inY + 1, inZ + 1) || IsValidPoint(inX + 1, inY + 1, inZ + 1);

        }
        if (inFace != CubeFaceType.MaxZ)
            throw new Exception("Cube face type is not one of the expected values.");
        
        //frontTopRight - frontBottomRight - frontBottomLeft - frontTopLeft
        return IsValidPoint(inX, inY + 1, inZ + 1) || IsValidPoint(inX, inY, inZ + 1) || IsValidPoint(inX + 1, inY, inZ + 1) || IsValidPoint(inX + 1, inY + 1, inZ + 1);
    }
    
    /// <summary>
	///	The way points are stored in the file:
	///	The points are stored the way the cuboid exists in the
	///	coordinate system. First the point (1,1,1) is stored. Then we
	///	increase x and store points till (cuboidWidth, 1, 1).We repeat
	///	this series from y index 1 to cuboidHeight. So the first plane
	///	at z =1 is stored. We then store bits for z = 2 and so on till
	///	cuboidThickness.
	///	To minimize the file read and write operations, we read and
	///	store the data for three z planes at a time. If the z index
	///	exceeds, we update the pointers with data of the next planes.
	///	In the current function, we access the point through the data
	///	stored in these pointers, rather than accessing the file
	///	directly.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	/// <returns></returns>
	private bool IsValidPoint(int x, int y, int z)
	{
		int bits;
        byte curByte;
		byte checkByte =0;

		if(! (x > 0 && x <= _moldFileInfo.X && y > 0 && y <= _moldFileInfo.Y && z > 0 && z <= _moldFileInfo.Z))
            throw new ArgumentException("isValidPoint:1");
	
		if(! (_firstCall || (z >= _curZ && z <= _curZ + 3)))
            throw new ArgumentException("isValidPoint:2");
	
		if (_firstCall)
		{
			_curZ = z;
			UpdatePlaneData();
	
		}
		else if(z == _curZ + 3)
		{
			_curZ ++;
			UpdatePlaneData(); //update the data stored in the planes
		}
	
		//find the position in terms of individual bits
		if(z == _curZ)
			bits = _curBkOffsetBits + ((y - 1) * _moldFileInfo.X) + x;
	
		else if (z == _curZ + 1)
			bits = _curFtOffsetBits + ((y - 1) * _moldFileInfo.X) + x;
	
		else
			bits = _nextOffsetBits + ((y - 1) * _moldFileInfo.X) + x;
	
		var bitPos = bits%8;
		//in the byte of 8 bits. For eg, if the value of
		//bitPos is 0, then the bit to be accessed is the 8th one in the
		//current byte; If its 7 then it means the first element in the next byte.
	
		//find the byte that has to be accessed.
		if(bitPos > 0)
			bits = bits + 8 - bitPos; //if it is not divisible by 8 then
			//we make it so, by suitable addition
	
		var bytePos = bits/8;
	
		bytePos = bytePos - 1; //if we want to read the nth byte then we
								//need to bypass n-1 bytes
		if(z == _curZ)
			curByte = _curBkPlane[bytePos];

		else if (z == _curZ + 1)
			curByte = _curFtPlane[bytePos];
	
		else
			curByte = _nextPlane[bytePos];
	
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
		return (curByte & checkByte) == checkByte;
	}
	private void UpdatePlaneData()
	{
	    //Variable declaration
		int bits;
		int bytePos;
		int uintActualBytesToRead;

	    if (_firstCall)
		{
	
			_firstCall = false;

			//Assign to curBkPlane
			bits = ((_curZ - 1) * _moldFileInfo.X * _moldFileInfo.Y) + 1;
	
			_curBkOffsetBits = (byte) (bits%8);
	
			if(_curBkOffsetBits > 0)
				bits = bits + 8 - _curBkOffsetBits;
	
			bytePos = bits/8;
			bytePos += Constants.MOLD_HEADER_SIZE;
			bytePos -= 1;
	
			_fp.Seek(bytePos, SeekOrigin.Begin);
	
			//We have increased the value of bytesPerPlane by one to accomodate increase in size
			//due to sliding of bits. However, here we should ensure that we dont read one byte
			//more than the end of the file
			uintActualBytesToRead = _bytesPerPlane;
			if((bytePos + uintActualBytesToRead) > _fileSize)
				uintActualBytesToRead--; //Reduce by one
	
			if(_fp.Read(_curBkPlane, 0, uintActualBytesToRead) != uintActualBytesToRead) //Reading without using CArchive
				throw new Exception("Error reading file");
	
			//Set the offset bits after which the first value in the new plane( x=1, y=1 ) is stored
			if(_curBkOffsetBits == 0)
				_curBkOffsetBits = 8;
			_curBkOffsetBits--;
	
			//Assign to curFtPlane
			bits = (_curZ * _moldFileInfo.X * _moldFileInfo.Y) + 1;
	
			_curFtOffsetBits = (byte) (bits%8);
	
			if(_curFtOffsetBits > 0)
				bits = bits + 8 - _curFtOffsetBits;
	
			bytePos = bits/8;
			bytePos += Constants.MOLD_HEADER_SIZE;
			bytePos -= 1;

            _fp.Seek(bytePos, SeekOrigin.Begin);
	
			//We have increased the value of bytesPerPlane by one to accomodate increase in size
			//due to sliding of bits. However, here we should ensure that we dont read one byte
			//more than the end of the file
	
			uintActualBytesToRead = _bytesPerPlane;
			if((bytePos + uintActualBytesToRead) > _fileSize)
				uintActualBytesToRead--; //Reduce by one
	
			if(_fp.Read(_curFtPlane, 0, uintActualBytesToRead) != uintActualBytesToRead)
				throw new Exception("Error occured while reading");
	
			//Set the offset bits after which the first value in the new plane( x=1, y=1 ) is stored
			if(_curFtOffsetBits == 0)
				_curFtOffsetBits = 8;
			_curFtOffsetBits--;
		}
		else
		{
			//Shift by one z index
			var temp = _curBkPlane;
			_curBkPlane = _curFtPlane;
			_curFtPlane = _nextPlane;
			_nextPlane = temp; //we allocate curBkPlane's set of bytes to nextPlane so
			//that we don't lose the allocated memory space of curBkPlane.
	
			_curBkOffsetBits = _curFtOffsetBits;
			_curFtOffsetBits = _nextOffsetBits;
		}
	
		//If possible assign values to nextPlane or free the memory space
		if((_curZ + 2) <= _moldFileInfo.Z) //Such a plane exists
		{
	        bits = ((_curZ + 1) * _moldFileInfo.X * _moldFileInfo.Y) + 1; //accessing the plane with z index
			//which is the third with respect to curZ
	
			_nextOffsetBits = (byte) (bits%8);
	
			if(_nextOffsetBits > 0)
				bits = bits + 8 - _nextOffsetBits;
	
			bytePos = bits/8;
			bytePos += Constants.MOLD_HEADER_SIZE;
			bytePos -= 1;

            _fp.Seek(bytePos, SeekOrigin.Begin);
	
			//We have increased the value of bytesPerPlane by one to accomodate increase in size
			//due to sliding of bits. However, here we should ensure that we dont read one byte
			//more than the end of the file
	
			uintActualBytesToRead = _bytesPerPlane;
			if((bytePos + uintActualBytesToRead) > _fileSize)
				uintActualBytesToRead--; //Reduce by one
	
			if(_fp.Read(_nextPlane, 0, uintActualBytesToRead) != uintActualBytesToRead)
				throw new Exception("Error occured while reading.");
	
			//Set the offset bits after which the first value in the new plane( x=1, y=1 ) is stored
			if(_nextOffsetBits == 0)
				_nextOffsetBits = 8;
			_nextOffsetBits--;
		}
		else
		{
    		_nextPlane = null;
		}
	}
}

