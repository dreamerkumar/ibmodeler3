using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class PositionIndices
    {
        //stores the indices for positions at different x, y and z values (at any given time, only the position indices of two z indices are stored at any given time
        private int[][] _curBkPlaneIndices;
        private int[][] _curFtPlaneIndices;
        private readonly float _minX;
        private readonly float _minY;
        private readonly float _minZ;
        private readonly float _xSpan;
        private readonly float _ySpan;
        private readonly float _zSpan;

        private int _currBackZIndex;

        public Point3DCollection Positions { get; set; }

        public PositionIndices(int x, int y, float minX, float minY, float minZ, float xSpan, float ySpan, float zSpan)
        {
            _minX = minX;
            _zSpan = zSpan;
            _ySpan = ySpan;
            _xSpan = xSpan;
            _minZ = minZ;
            _minY = minY;

            _currBackZIndex = 1;
            _curBkPlaneIndices = GetNewIndicesArray(x, y);
            _curFtPlaneIndices = GetNewIndicesArray(x, y);

            Positions = new Point3DCollection();
        }
        
        public int AddPositionOrGetExistingPositionIndex(CubePt cubePt)
        {
            return AddPositionOrGetExistingPositionIndex(cubePt.X, cubePt.Y, cubePt.Z);
        }

        private int AddPositionOrGetExistingPositionIndex(int x, int y, int z)
        {
            if (!(z == _currBackZIndex || z == _currBackZIndex + 1))
                throw new ArgumentException("Passed z index is invalid.");

            var indexCollectionToUse = z == _currBackZIndex ? _curBkPlaneIndices : _curFtPlaneIndices;

            return GetStoredOrNewPositionIndex(indexCollectionToUse, x, y, z);
        }
        private static int[][] GetNewIndicesArray(int xCount, int yCount)
        {
            var indicesArray = new int[xCount][];
            for (var x = 0; x < xCount; x++)
                indicesArray[x] = new int[yCount];
            ResetPositionValues(indicesArray);
            return indicesArray;
        }
        private static void ResetPositionValues(IEnumerable<int[]> indicesArray)
        {
            foreach (var t in indicesArray)
                for (var y = 0; y < t.Length; y++)
                    t[y] = -1;
        }
        public void IncrementZIndex()
        {
            _currBackZIndex++;

            //Shift array index collection
            var tempIndices = _curBkPlaneIndices;
            _curBkPlaneIndices = _curFtPlaneIndices;
            _curFtPlaneIndices = tempIndices;

            ResetPositionValues(_curFtPlaneIndices);
        }
        private int GetStoredOrNewPositionIndex(int[][] indexCollectionToUse, int x, int y, int z)
        {
            if (!(1 <= x && x <= indexCollectionToUse.Length && 1 <= y && y <= indexCollectionToUse[0].Length))
                throw new ArgumentException(string.Format("Passed value of x and y (x: {0} y: {1}) is out of range of the index collection of length {2}", x, y, indexCollectionToUse[0].Length));

            var index = indexCollectionToUse[x - 1][y - 1]; //indexes are stored from zero instead of 1
            if (index != -1) 
                return index;//already added

            //need to add a new position
            var position = new Point3D
            {
                X = _minX + _xSpan * (x - 1),
                Y = _minY + _ySpan * (y - 1),
                Z = _minZ + _zSpan * (z - 1)
            };
            Positions.Add(position);

            var newPosition = Positions.Count - 1;

            if (indexCollectionToUse[x - 1][y - 1] != -1)
                throw new Exception("Logical error in add new position");

            indexCollectionToUse[x - 1][y - 1] = newPosition;

            return newPosition;
        }
    }
}
