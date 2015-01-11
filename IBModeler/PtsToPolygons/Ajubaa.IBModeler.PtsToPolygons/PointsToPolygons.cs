using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class PointsToPolygons
    {
        private readonly CreateModelInfo _createModelInfo;

        //objects
        private readonly CubeStatus _cubeStatus;
        private readonly FileReadPoints _fileReadPoints;
        private readonly int _minx;
        private readonly int _maxx;
        private readonly int _miny;
        private readonly int _maxy;
        private readonly int _minz;
        private readonly int _maxz;

        private readonly CuboidFileInfo _cuboidFileInfo = new CuboidFileInfo();

        //length of the sides of individual cubes
        private readonly float _xSpan; 
        private readonly float _ySpan;
        private readonly float _zSpan;
        private readonly PositionIndices _positionIndices;
        private readonly Int32Collection _triangleIndices = new Int32Collection();

        public PointsToPolygons(CreateModelInfo createModelInfo)
        {
            _createModelInfo = createModelInfo;

            if (!(createModelInfo.Minx >= 0 && createModelInfo.Maxx >= 0 && createModelInfo.Miny >= 0 && createModelInfo.Maxy >= 0 && createModelInfo.Minz >= 0 && createModelInfo.Maxz >= 0))
                throw new ArgumentException("Invalid input parameters");
            
            if (!(createModelInfo.Maxx == 0 || (createModelInfo.Maxx > createModelInfo.Minx && createModelInfo.Maxx > 1))
                && (createModelInfo.Maxy == 0 || (createModelInfo.Maxy > createModelInfo.Miny && createModelInfo.Maxy > 1))
                && (createModelInfo.Maxz == 0 || (createModelInfo.Maxz > createModelInfo.Minz && createModelInfo.Maxz > 1)))
                throw new ArgumentException("Invalid input parameters");

            _cubeStatus = null;
            _fileReadPoints = null;

            _fileReadPoints = new FileReadPoints(_createModelInfo.MoldData);

            _cuboidFileInfo = _fileReadPoints.GetCuboidValues();

            //less than the number of points
            if (_createModelInfo.Minx < 0 || _createModelInfo.Minx > _cuboidFileInfo.X || _createModelInfo.Miny < 0 || _createModelInfo.Miny > _cuboidFileInfo.Y || _createModelInfo.Minz < 0 || _createModelInfo.Minz > _cuboidFileInfo.Z)
                throw new Exception("Logical error: Index out of range.");

            //Calculate the lengths of the sides of the  individual cubes
            //Number of cubes is one less than the number of points so have subtracted by 1 to get the count of cubes
            _xSpan = (_cuboidFileInfo.MaxX - _cuboidFileInfo.MinX) / (float)(_cuboidFileInfo.X -1);

            _ySpan = (_cuboidFileInfo.MaxY - _cuboidFileInfo.MinY) / (float)(_cuboidFileInfo.Y - 1);

            _zSpan = (_cuboidFileInfo.MaxZ - _cuboidFileInfo.MinZ) / (float)(_cuboidFileInfo.Z -1);

            if (_createModelInfo.Minx == 0)
                _minx = 1;
            else
                _minx = _createModelInfo.Minx;

            if (_createModelInfo.Maxx == 0)
                _maxx = _cuboidFileInfo.X;
            else
                _maxx = _createModelInfo.Maxx - 1; //cube index is one less than the max points

            if (_createModelInfo.Miny == 0)
                _miny = 1;
            else
                _miny = _createModelInfo.Miny;

            if (_createModelInfo.Maxy == 0)
                _maxy = _cuboidFileInfo.Y;
            else
                _maxy = _createModelInfo.Maxy - 1; //cube index is one less than the max points

            if (_createModelInfo.Minz == 0)
                _minz = 1;
            else
                _minz = _createModelInfo.Minz;

            if (_createModelInfo.Maxz == 0)
                _maxz = _cuboidFileInfo.Z;
            else
                _maxz = _createModelInfo.Maxz - 1; //cube index is one less than the max points

            _positionIndices = new PositionIndices(_maxx + 1, _maxy + 1, _cuboidFileInfo.MinX, _cuboidFileInfo.MinY, _cuboidFileInfo.MinZ, _xSpan, _ySpan, _zSpan);
            
            _cubeStatus = new CubeStatus();
            _cubeStatus.InitCubeStatus(_maxx + 1 - _minx, _maxy + 1 - _miny);
        }
        
        public MeshGeometry3D Process()
        {
            var found = false;
            var prevValid = false;
            var cubeCorners = new CubeCorners(); //To store the cube corners
            var flags = new SpaceRangeFlags(); //shared faces

            //identify a new object and generate outer surfaces for it
            for (var z = _minz; z <= _maxz; z++)
            {
                for (var y = _miny; y <= _maxy; y++)
                {
                    for (var x = _minx; x <= _maxx; x++)
                    {
                        //get the cube
                        try
                        {
                            if (x == _minx)
                                cubeCorners = _fileReadPoints.GetCube(x, y, z);
                            else
                                cubeCorners = GetNextXCube(cubeCorners, x, y, z);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        if (IsValidCube(cubeCorners))
                        {
                            if (!found)
                                found = true;

                            //set the faces
                            flags.Initialize();
                            
                            //Faces at min Values
                            if (x == _minx)
                                flags.Minx = false;
                            else
                                flags.Minx = prevValid;

                            if (y == _miny)
                                flags.Miny = false;
                            else
                                flags.Miny = IsValidCube(x, y - 1, PlaneType.CurrentPlane);

                            if (z == _minz)
                                flags.Minz = false;
                            else
                                flags.Minz = IsValidCube(x, y, PlaneType.BackPlane);

                            //Faces at max Values
                            try
                            {
                                if (x == _maxx)
                                    flags.Maxx = false;
                                else
                                    flags.Maxx = _fileReadPoints.IsFaceWithValidPt(x + 1, y, z, CubeFaceType.MaxX);

                                if (y == _maxy)
                                    flags.Maxy = false;
                                else
                                    flags.Maxy = _fileReadPoints.IsFaceWithValidPt(x, y + 1, z, CubeFaceType.MaxY);

                                if (z == _maxz)
                                    flags.Maxz = false;
                                else
                                    flags.Maxz = _fileReadPoints.IsFaceWithValidPt(x, y, z + 1, CubeFaceType.MaxZ);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            //generate the surfaces
                            try
                            {
                                DrawSurfaces(x, y, z, cubeCorners, flags);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                            //Set the indicator as valid in the cubes file
                            SetStatusAsValid(x, y);

                            //Set the flag for the next iteration
                            prevValid = true;
                        }
                        else
                            prevValid = false;
                    }
                }
                if (z != _maxz)
                {
                    //Reassign planes for new z index
                    _cubeStatus.GoToNextPlane(); 
                    //Reset position index collection
                    _positionIndices.IncrementZIndex();
                }
            }
            if (!found)
                throw new Exception("No polygons were generated.");

            return new MeshGeometry3D { TriangleIndices = _triangleIndices, Positions = _positionIndices.Positions};
        }

        private CubeCorners GetNextXCube(CubeCorners curCubeCorners, int x, int y, int z)
        {
            //Half of the points are to be retrieved from the file
            var nextCubeCorners = _fileReadPoints.GetNextXFace(x, y, z);

            //Set the other four points of the cube using the values of the current cube
            nextCubeCorners.BackTopLeft = curCubeCorners.BackTopRight;
            nextCubeCorners.BackBottomLeft = curCubeCorners.BackBottomRight;
            nextCubeCorners.FrontTopLeft = curCubeCorners.FrontTopRight;
            nextCubeCorners.FrontBottomLeft = curCubeCorners.FrontBottomRight;

            return nextCubeCorners;
        }
        
        private static bool IsValidCube(CubeCorners cubeCorners)
        {
            byte validPts = 0;

            if (cubeCorners.BackTopLeft)
                validPts++;
            if (cubeCorners.BackBottomLeft)
                validPts++;
            if (cubeCorners.BackBottomRight)
                validPts++;
            if (cubeCorners.BackTopRight)
                validPts++;
            if (cubeCorners.FrontTopLeft)
                validPts++;
            if (cubeCorners.FrontBottomLeft)
                validPts++;
            if (cubeCorners.FrontBottomRight)
                validPts++;
            if (cubeCorners.FrontTopRight)
                validPts++;

            if (validPts > 4)
                return true;
            
            if (validPts == 4)
            {
                //The cube can form a closed figure only if the four points do not lie on a plane diognal faces
                if ((cubeCorners.BackTopLeft && cubeCorners.BackBottomLeft && cubeCorners.BackBottomRight && cubeCorners.BackTopRight) 
                    || (cubeCorners.FrontTopLeft && cubeCorners.FrontBottomLeft && cubeCorners.FrontBottomRight && cubeCorners.FrontTopRight) 
                    || (cubeCorners.BackTopLeft && cubeCorners.BackBottomLeft && cubeCorners.FrontTopLeft && cubeCorners.FrontBottomLeft) 
                    || (cubeCorners.BackBottomRight && cubeCorners.BackTopRight && cubeCorners.FrontBottomRight && cubeCorners.FrontTopRight) 
                    || (cubeCorners.BackTopLeft && cubeCorners.BackTopRight && cubeCorners.FrontTopLeft && cubeCorners.FrontTopRight) 
                    || (cubeCorners.BackBottomLeft && cubeCorners.BackBottomRight && cubeCorners.FrontBottomLeft && cubeCorners.FrontBottomRight)
                    || (cubeCorners.BackTopLeft && cubeCorners.FrontTopLeft && cubeCorners.BackBottomRight && cubeCorners.FrontBottomRight)
                    || (cubeCorners.BackBottomLeft && cubeCorners.FrontBottomLeft && cubeCorners.BackTopRight && cubeCorners.FrontTopRight)
                    || (cubeCorners.FrontTopLeft && cubeCorners.FrontTopRight && cubeCorners.BackBottomLeft && cubeCorners.BackBottomRight)
                    || (cubeCorners.FrontBottomLeft && cubeCorners.FrontBottomRight && cubeCorners.BackTopLeft && cubeCorners.BackTopRight)
                    || (cubeCorners.FrontTopLeft && cubeCorners.FrontBottomLeft && cubeCorners.BackBottomRight && cubeCorners.BackTopRight)
                    || (cubeCorners.FrontBottomRight && cubeCorners.FrontTopRight && cubeCorners.BackTopLeft && cubeCorners.BackBottomLeft))
                    return false;

                return true;
            } //number of valid points are less than 4
            return false;
        }
        
        private bool IsValidCube(int x, int y, PlaneType planeType)
        {
            return _cubeStatus.IsValidCube(x + 1 - _minx, y + 1 - _miny, planeType);
        }
        
        private void SetStatusAsValid(int x, int y)
        {
            _cubeStatus.SetStatusAsValid(x + 1 - _minx, y + 1 - _miny);
        }
        
        private void DrawSurfaces(int x, int y, int z, CubeCorners cubeCorners, SpaceRangeFlags sharedFaces)
        {
            bool flgCutCorner6;
            byte ctrMinx;
            byte ctrMaxx;
            byte ctrMiny;
            byte ctrMaxy;
            byte ctrMinz;
            byte ctrMaxz;
            byte ctrDgP2;
            byte ctrDgP3;
            byte ctrDgP4;
            byte ctrDgP5;
            byte ctrDgP6;
            
            //Initialize
            var flgCutCorner5 = flgCutCorner6 = false;

            var ctr = ctrMinx = ctrMaxx = ctrMiny = ctrMaxy = ctrMinz = ctrMaxz = 0;
            var ctrDgP1 = ctrDgP2 = ctrDgP3 = ctrDgP4 = ctrDgP5 = ctrDgP6 = 0;

            //Calculate the number of overall valid points and valid points along different faces
            if (cubeCorners.BackBottomLeft)
            {
                ctrMinz++;
                ctrMiny++;
                ctrMinx++;
                ctrDgP2++;
                ctrDgP4++;
                ctrDgP5++;
                ctr++;
            }
            if (cubeCorners.BackBottomRight)
            {
                ctrMinz++;
                ctrMiny++;
                ctrMaxx++;
                ctrDgP1++;
                ctrDgP3++;
                ctrDgP5++;
                ctr++;
            }
            if (cubeCorners.BackTopLeft)
            {
                ctrMinz++;
                ctrMaxy++;
                ctrMinx++;
                ctrDgP2++;
                ctrDgP3++;
                ctrDgP6++;
                ctr++;
            }
            if (cubeCorners.BackTopRight)
            {
                ctrMinz++;
                ctrMaxy++;
                ctrMaxx++;
                ctrDgP1++;
                ctrDgP4++;
                ctrDgP6++;
                ctr++;
            }
            if (cubeCorners.FrontBottomLeft)
            {
                ctrMaxz++;
                ctrMiny++;
                ctrMinx++;
                ctrDgP1++;
                ctrDgP4++;
                ctrDgP6++;
                ctr++;
            }
            if (cubeCorners.FrontBottomRight)
            {
                ctrMaxz++;
                ctrMiny++;
                ctrMaxx++;
                ctrDgP2++;
                ctrDgP3++;
                ctrDgP6++;
                ctr++;
            }
            if (cubeCorners.FrontTopLeft)
            {
                ctrMaxz++;
                ctrMaxy++;
                ctrMinx++;
                ctrDgP1++;
                ctrDgP3++;
                ctrDgP5++;
                ctr++;
            }
            if (cubeCorners.FrontTopRight)
            {
                ctrMaxz++;
                ctrMaxy++;
                ctrMaxx++;
                ctrDgP2++;
                ctrDgP4++;
                ctrDgP5++;
                ctr++;
            }

            //first draw the possible rectangles or triangles on all the non shared faces

            //left face
            if (!sharedFaces.Minx) //Draw only if the face is not shared
            {
                if (ctrMinx == 4)
                {
                    {
                        var t = new CubeCorners();
                        t.BackBottomLeft = t.BackTopLeft = t.FrontBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinX);
                    }
                    {
                        var t = new CubeCorners();
                        t.FrontBottomLeft = t.FrontTopLeft = t.BackTopLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinX);
                    }
                }
                else if (ctrMinx == 3)
                {
                    var t = new CubeCorners
                    {
                        BackBottomLeft = cubeCorners.BackBottomLeft,
                        BackTopLeft = cubeCorners.BackTopLeft,
                        FrontBottomLeft = cubeCorners.FrontBottomLeft,
                        FrontTopLeft = cubeCorners.FrontTopLeft
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinX);
                }
            }

            //right face
            if (!sharedFaces.Maxx) //Draw only if the face is not shared
            {
                if (ctrMaxx == 4)
                {
                    {
                        var t = new CubeCorners();
                        t.BackBottomRight = t.BackTopRight = t.FrontBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxX);
                    }
                    {
                        var t = new CubeCorners();
                        t.FrontBottomRight = t.FrontTopRight = t.BackTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxX);
                    }
                }
                else if (ctrMaxx == 3)
                {
                    var t = new CubeCorners
                    {
                        BackBottomRight = cubeCorners.BackBottomRight,
                        BackTopRight = cubeCorners.BackTopRight,
                        FrontBottomRight = cubeCorners.FrontBottomRight,
                        FrontTopRight = cubeCorners.FrontTopRight
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxX);
                }
            }

            //front face
            if (!sharedFaces.Maxz) //Draw only if the face is not shared
            {
                if (ctrMaxz == 4)
                {
                    {
                        var t = new CubeCorners();
                        t.FrontBottomLeft = t.FrontTopLeft = t.FrontTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxZ);
                    }
                    {
                        var t = new CubeCorners();
                        t.FrontBottomRight = t.FrontTopRight = t.FrontBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxZ);
                    }
                }
                else if (ctrMaxz == 3)
                {
                    var t = new CubeCorners
                    {
                        FrontBottomLeft = cubeCorners.FrontBottomLeft,
                        FrontBottomRight = cubeCorners.FrontBottomRight,
                        FrontTopLeft = cubeCorners.FrontTopLeft,
                        FrontTopRight = cubeCorners.FrontTopRight
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxZ);
                }
            }

            //back face
            if (!sharedFaces.Minz) //Draw only if the face is not shared
            {
                if (ctrMinz == 4)
                {
                    var t = new CubeCorners();
                    t.BackBottomLeft = t.BackTopLeft = t.BackTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinZ);

                    t = new CubeCorners();
                    t.BackBottomRight = t.BackTopRight = t.BackBottomLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinZ);
                }
                else if (ctrMinz == 3)
                {
                    var t = new CubeCorners
                    {
                        BackBottomLeft = cubeCorners.BackBottomLeft,
                        BackBottomRight = cubeCorners.BackBottomRight,
                        BackTopLeft = cubeCorners.BackTopLeft,
                        BackTopRight = cubeCorners.BackTopRight
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinZ);
                }
            }

            //top face
            if (!sharedFaces.Maxy) //Draw only if the face is not shared
            {
                if (ctrMaxy == 4)
                {
                    var t = new CubeCorners();
                    t.FrontTopLeft = t.FrontTopRight = t.BackTopLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxY);

                    t = new CubeCorners();
                    t.BackTopLeft = t.BackTopRight = t.FrontTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxY);

                }
                else if (ctrMaxy == 3)
                {
                    var t = new CubeCorners
                    {
                        BackTopLeft = cubeCorners.BackTopLeft,
                        BackTopRight = cubeCorners.BackTopRight,
                        FrontTopLeft = cubeCorners.FrontTopLeft,
                        FrontTopRight = cubeCorners.FrontTopRight
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MaxY);
                }
            }

            //bottom face
            if (!sharedFaces.Miny) //Draw only if the face is not shared
            {
                if (ctrMiny == 4)
                {
                    var t = new CubeCorners();
                    t.FrontBottomLeft = t.FrontBottomRight = t.BackBottomLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinY);

                    t = new CubeCorners();
                    t.BackBottomLeft = t.BackBottomRight = t.FrontBottomRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinY);
                }
                else if (ctrMiny == 3)
                {
                    var t = new CubeCorners
                    {
                        BackBottomLeft = cubeCorners.BackBottomLeft,
                        BackBottomRight = cubeCorners.BackBottomRight,
                        FrontBottomLeft = cubeCorners.FrontBottomLeft,
                        FrontBottomRight = cubeCorners.FrontBottomRight
                    };
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.MinY);
                }
            }

            switch (ctr)
            {
                case 8:
                    //All triangles along the faces which are drawn above
                    break;
                case 7:
                    //All triangles along faces are drawn above
                    //search for the invalid corner with all three valid neighbouring corners(done at the
                    //Bottom)

                    break;
                case 6:
                    //Look for any invalid side out of the possible 12 sides of the cube
                    if ((!cubeCorners.FrontBottomRight && !cubeCorners.FrontTopRight) || (!cubeCorners.BackBottomLeft && !cubeCorners.BackTopLeft))
                    {
                        var t = new CubeCorners();
                        t.BackTopRight = t.FrontTopLeft = t.FrontBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane1);

                        t = new CubeCorners();
                        t.FrontBottomLeft = t.BackTopRight = t.BackBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane1);
                    }
                    else if ((!cubeCorners.FrontBottomLeft && !cubeCorners.FrontTopLeft) || (!cubeCorners.BackBottomRight && !cubeCorners.BackTopRight))
                    {
                        var t = new CubeCorners();
                        t.BackTopLeft = t.FrontTopRight = t.BackBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane2);

                        t = new CubeCorners();
                        t.BackBottomLeft = t.FrontTopRight = t.FrontBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane2);

                    }
                    else if ((!cubeCorners.FrontBottomLeft && !cubeCorners.BackBottomLeft) || (!cubeCorners.FrontTopRight && !cubeCorners.BackTopRight))
                    {
                        var t = new CubeCorners();
                        t.FrontTopLeft = t.BackTopLeft = t.FrontBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane3);

                        t = new CubeCorners();
                        t.FrontBottomRight = t.BackTopLeft = t.BackBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane3);

                    }
                    else if ((!cubeCorners.FrontTopLeft && !cubeCorners.BackTopLeft) || (!cubeCorners.FrontBottomRight && !cubeCorners.BackBottomRight))
                    {
                        var t = new CubeCorners();
                        t.FrontBottomLeft = t.BackBottomLeft = t.FrontTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane4);

                        t = new CubeCorners();
                        t.FrontTopRight = t.BackBottomLeft = t.BackTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane4);
                    }
                    else if ((!cubeCorners.FrontBottomLeft && !cubeCorners.FrontBottomRight) || (!cubeCorners.BackTopLeft && !cubeCorners.BackTopRight))
                    {
                        var t = new CubeCorners();
                        t.FrontTopLeft = t.FrontTopRight = t.BackBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane5);

                        t = new CubeCorners();
                        t.BackBottomLeft = t.FrontTopRight = t.BackBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane5);
                    }
                    else if ((!cubeCorners.FrontTopLeft && !cubeCorners.FrontTopRight) || (!cubeCorners.BackBottomLeft && !cubeCorners.BackBottomRight))
                    {
                        var t = new CubeCorners();
                        t.BackTopLeft = t.BackTopRight = t.FrontBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane6);

                        t = new CubeCorners();
                        t.FrontBottomRight = t.BackTopLeft = t.FrontBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane6);
                    }
                    else
                        //Identify the two invalid points and join the neighbours for each of them
                        flgCutCorner6 = true;

                    break;
                case 5:
                    //		y
                    //		The closed figure having five valid points can be categorized into two:
                    //		One in which there are four coplanar points and a point away from this plane.
                    //		In the second type we can have all five points as non-coplanar and in this case, each
                    //		invalid point forms a side of the cube, with three points; all of which will be valid.
                    //
                    //Check for four coplanar points along the twelve planes
                    if (ctrMinx == 4)
                    {
                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.BackTopRight = cubeCorners.BackTopRight;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.BackTopLeft = t.BackBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomLeft = t.FrontBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomLeft = t.FrontTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopLeft = t.BackTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                    }
                    else if (ctrMaxx == 4)
                    {
                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;

                        var t = CopyFrom(temp);
                        t.FrontTopRight = t.FrontBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.BackBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.BackTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.FrontTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    }
                    else if (ctrMiny == 4)
                    {
                        var temp = new CubeCorners();

                        //assign the single point outside of the plane
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.BackTopRight = cubeCorners.BackTopRight;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.FrontBottomLeft = t.BackBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomLeft = t.BackBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.FrontBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.FrontBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    }
                    else if (ctrMaxy == 4)
                    {
                        var temp = new CubeCorners();

                        //assign the single point outside of the plane
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;

                        var t = CopyFrom(temp);
                        t.FrontTopLeft = t.BackTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopLeft = t.BackTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.FrontTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopRight = t.FrontTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    }
                    else if (ctrMinz == 4)
                    {
                        var temp = new CubeCorners();

                        //assign the single point outside of the plane
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.BackTopLeft = t.BackBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomLeft = t.BackBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.BackTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.BackTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    }
                    else if (ctrMaxz == 4)
                    {
                        var temp = new CubeCorners();

                        //assign the single point outside of the plane
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.BackTopRight = cubeCorners.BackTopRight;

                        var t = CopyFrom(temp);
                        t.FrontTopLeft = t.FrontBottomLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomLeft = t.FrontBottomRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.FrontTopRight = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopRight = t.FrontTopLeft = true;

                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    }
                    else if (ctrDgP1 == 4)
                    {
                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.FrontTopLeft = t.FrontBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomLeft = t.BackBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.BackTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.FrontTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane

                        t = new CubeCorners();
                        t.FrontBottomLeft = t.FrontTopLeft = t.BackTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane1);

                        t = new CubeCorners();
                        t.BackTopRight = t.BackBottomRight = t.FrontBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane1);
                    }
                    else if (ctrDgP2 == 4)
                    {
                        var temp = new CubeCorners();

                        //assign the single point outside of the plane
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.BackTopRight = cubeCorners.BackTopRight;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;

                        var t = CopyFrom(temp);
                        t.BackTopLeft = t.BackBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomLeft = t.FrontBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.FrontTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopRight = t.BackTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane
                        t = new CubeCorners();
                        t.BackBottomLeft = t.BackTopLeft = t.FrontTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane2);

                        t = new CubeCorners();
                        t.FrontTopRight = t.FrontBottomRight = t.BackBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane2);
                    }
                    else if (ctrDgP3 == 4)
                    {
                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.BackTopRight = cubeCorners.BackTopRight;
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.BackTopLeft = t.FrontTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopLeft = t.FrontBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.BackBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.BackTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane
                        t = new CubeCorners();
                        t.FrontTopLeft = t.BackTopLeft = t.BackBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane3);

                        t = new CubeCorners();
                        t.BackBottomRight = t.FrontBottomRight = t.FrontTopLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane3);
                    }
                    else if (ctrDgP4 == 4)
                    {

                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;

                        var t = CopyFrom(temp);
                        t.BackBottomLeft = t.FrontBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomLeft = t.FrontTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopRight = t.BackTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.BackBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane
                        t = new CubeCorners();
                        t.BackBottomLeft = t.BackTopRight = t.FrontTopRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane4);

                        t = new CubeCorners();
                        t.FrontTopRight = t.FrontBottomLeft = t.BackBottomLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane4);
                    }
                    else if (ctrDgP5 == 4)
                    {

                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackTopLeft = cubeCorners.BackTopLeft;
                        temp.BackTopRight = cubeCorners.BackTopRight;
                        temp.FrontBottomLeft = cubeCorners.FrontBottomLeft;
                        temp.FrontBottomRight = cubeCorners.FrontBottomRight;

                        var t = CopyFrom(temp);
                        t.FrontTopLeft = t.FrontTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontTopRight = t.BackBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomRight = t.BackBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackBottomLeft = t.FrontTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane
                        t = new CubeCorners();
                        t.FrontTopLeft = t.FrontTopRight = t.BackBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane5);

                        t = new CubeCorners();
                        t.BackBottomRight = t.BackBottomLeft = t.FrontTopLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane5);
                    }
                    else if (ctrDgP6 == 4)
                    {

                        var temp = new CubeCorners();
                        //assign the single point outside of the plane
                        temp.BackBottomLeft = cubeCorners.BackBottomLeft;
                        temp.BackBottomRight = cubeCorners.BackBottomRight;
                        temp.FrontTopLeft = cubeCorners.FrontTopLeft;
                        temp.FrontTopRight = cubeCorners.FrontTopRight;

                        var t = CopyFrom(temp);
                        t.BackTopLeft = t.BackTopRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.BackTopRight = t.FrontBottomRight = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomRight = t.FrontBottomLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        t = CopyFrom(temp);
                        t.FrontBottomLeft = t.BackTopLeft = true;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                        //draw the plane
                        t = new CubeCorners();
                        t.BackTopLeft = t.BackTopRight = t.FrontBottomRight = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane6);

                        t = new CubeCorners();
                        t.FrontBottomRight = t.FrontBottomLeft = t.BackTopLeft = true;
                        StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.DgPlane6);
                    }
                    else
                        flgCutCorner5 = true;
                    break;
                case 4:

                    //Logic: There are four points and all can form a group with the other three to form
                    //triangles. So if a corner is valid, we set it to invalid, so that only three points
                    //are left in the set of cube corners passed for making the triangle. We do this for
                    //all the triangles

                    if (cubeCorners.BackBottomLeft)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.BackBottomLeft = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);

                    }
                    if (cubeCorners.BackBottomRight)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.BackBottomRight = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.BackTopLeft)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.BackTopLeft = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.BackTopRight)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.BackTopRight = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.FrontBottomLeft)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.FrontBottomLeft = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.FrontBottomRight)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.FrontBottomRight = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.FrontTopLeft)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.FrontTopLeft = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    if (cubeCorners.FrontTopRight)
                    {
                        var t = CopyFrom(cubeCorners);
                        t.FrontTopRight = false;
                        if (!t.TriangleOnCubeFace())
                            StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.OfFour);
                    }
                    break;
            }
            //Draw triangles for invalid corners connected to all three valid neighbours
            if (ctr == 7 || flgCutCorner5 || flgCutCorner6)
            {
                if (!cubeCorners.BackBottomLeft)
                {
                    var t = new CubeCorners();
                    t.FrontBottomLeft = t.BackBottomRight = t.BackTopLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                }
                if (!cubeCorners.BackBottomRight)
                {
                    var t = new CubeCorners();
                    t.FrontBottomRight = t.BackBottomLeft = t.BackTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                }
                if (!cubeCorners.BackTopLeft)
                {
                    var t = new CubeCorners();
                    t.BackBottomLeft = t.FrontTopLeft = t.BackTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                    
                }
                if (!cubeCorners.BackTopRight)
                {
                    var t = new CubeCorners();
                    t.BackBottomRight = t.FrontTopRight = t.BackTopLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                }
                if (!cubeCorners.FrontBottomLeft)
                {
                    var t = new CubeCorners();
                    t.FrontBottomRight = t.FrontTopLeft = t.BackBottomLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                }
                if (!cubeCorners.FrontBottomRight)
                {
                    var t = new CubeCorners();
                    t.FrontBottomLeft = t.BackBottomRight = t.FrontTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                }
                if (!cubeCorners.FrontTopLeft)
                {
                    var t = new CubeCorners();
                    t.FrontBottomLeft = t.FrontTopRight = t.BackTopLeft = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);

                }
                if (!cubeCorners.FrontTopRight)
                {
                    var t = new CubeCorners();
                    t.FrontTopLeft = t.FrontBottomRight = t.BackTopRight = true;
                    StoreTriangle(x, y, z, cubeCorners, t, CubeFaceType.Inner);
                }
            }
        }

        private static CubeCorners CopyFrom(CubeCorners value)
        {
            return new CubeCorners
            {
                BackBottomLeft = value.BackBottomLeft,
                BackTopLeft = value.BackTopLeft,
                BackBottomRight = value.BackBottomRight,
                BackTopRight = value.BackTopRight,
                FrontBottomLeft = value.FrontBottomLeft,
                FrontBottomRight = value.FrontBottomRight,
                FrontTopLeft = value.FrontTopLeft,
                FrontTopRight = value.FrontTopRight
            };
        }

        private void StoreTriangle(int x, int y, int z, CubeCorners cubeCorners, CubeCorners vertices, CubeFaceType faceType)
        {
            //We need to decide which side should the normal to the plane
            //containing the triangle should face. The normal points in the
            //direction in which the vertices are stored in anticlockwise sequence.
            //Hence in effect we need to decide the sequence, in which the vertices are to be stored.

            byte ctrVertices;
            CubeCornerTypes temp;
            var cubeCornerTypes = new CubeCornerTypes[3];
            bool cCW;

            //Check whether only three vertices have been sent for the triangle.
            byte validPts = 0;

            if (vertices.BackTopLeft)
                validPts++;
            if (vertices.BackBottomLeft)
                validPts++;
            if (vertices.BackBottomRight)
                validPts++;
            if (vertices.BackTopRight)
                validPts++;
            if (vertices.FrontTopLeft)
                validPts++;
            if (vertices.FrontBottomLeft)
                validPts++;
            if (vertices.FrontBottomRight)
                validPts++;
            if (vertices.FrontTopRight)
                validPts++;

            if (validPts != 3)
                throw new Exception("vertex not three for triangle.");

            //Note : This piece of code can be removed later on in the release version.
            //If the defined face is set as CubeFaceType.OF_FOUR then check for triangles along the diognal faces
            if (faceType == CubeFaceType.OfFour)
            {
                byte ctrDgP2;
                byte ctrDgP3;
                byte ctrDgP4;
                byte ctrDgP5;
                byte ctrDgP6;
                var ctrDgP1 = ctrDgP2 = ctrDgP3 = ctrDgP4 = ctrDgP5 = ctrDgP6 = 0;

                if (vertices.BackBottomLeft)
                {
                    ctrDgP2++;
                    ctrDgP4++;
                    ctrDgP5++;
                }

                if (vertices.BackBottomRight)
                {
                    ctrDgP1++;
                    ctrDgP3++;
                    ctrDgP5++;
                }

                if (vertices.BackTopLeft)
                {
                    ctrDgP2++;
                    ctrDgP3++;
                    ctrDgP6++;
                }

                if (vertices.BackTopRight)
                {
                    ctrDgP1++;
                    ctrDgP4++;
                    ctrDgP6++;
                }

                if (vertices.FrontBottomLeft)
                {
                    ctrDgP1++;
                    ctrDgP4++;
                    ctrDgP6++;
                }

                if (vertices.FrontBottomRight)
                {
                    ctrDgP2++;
                    ctrDgP3++;
                    ctrDgP6++;
                }

                if (vertices.FrontTopLeft)
                {
                    ctrDgP1++;
                    ctrDgP3++;
                    ctrDgP5++;
                }

                if (vertices.FrontTopRight)
                {
                    ctrDgP2++;
                    ctrDgP4++;
                    ctrDgP5++;
                }

                if (ctrDgP1 == 3)
                    faceType = CubeFaceType.DgPlane1;

                else if (ctrDgP2 == 3)
                    faceType = CubeFaceType.DgPlane2;

                else if (ctrDgP3 == 3)
                    faceType = CubeFaceType.DgPlane3;

                else if (ctrDgP4 == 3)
                    faceType = CubeFaceType.DgPlane4;

                else if (ctrDgP5 == 3)
                    faceType = CubeFaceType.DgPlane5;

                else if (ctrDgP6 == 3)
                    faceType = CubeFaceType.DgPlane6;
            }


            //Case 1: If the points are along the left plane of the cube
            if (faceType == CubeFaceType.MinX)
            {
                ctrVertices = 0;
                if (vertices.BackTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopLeft;
                    ctrVertices++;
                }
                if (vertices.BackBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomLeft;
                    ctrVertices++;
                }
                if (vertices.FrontBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomLeft;
                    ctrVertices++;
                }
                if (vertices.FrontTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopLeft;
                }
                //store the vertices in the CCW order for the left face
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 1

            //Case 2: If the points are along the right face of the cube
            if (faceType == CubeFaceType.MaxX)
            {
                ctrVertices = 0;
                if (vertices.FrontTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopRight;
                    ctrVertices++;
                }
                if (vertices.FrontBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomRight;
                    ctrVertices++;
                }
                if (vertices.BackBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomRight;
                    ctrVertices++;
                }
                if (vertices.BackTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopRight;
                }

                //Store the vertices in the CCW order for the right face
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 2


            //Case 3: If the points are along the back face of the cube
            if (faceType == CubeFaceType.MinZ)
            {
                ctrVertices = 0;
                if (vertices.BackTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopLeft;
                    ctrVertices++;
                }
                if (vertices.BackTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopRight;
                    ctrVertices++;
                }
                if (vertices.BackBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomRight;
                    ctrVertices++;
                }
                if (vertices.BackBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomLeft;
                }

                //Store the vertices in the CCW order
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 3


            //Case 4: If the points are along the front face of the cube
            if (faceType == CubeFaceType.MaxZ)
            {
                ctrVertices = 0;
                if (vertices.FrontTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopLeft;
                    ctrVertices++;
                }
                if (vertices.FrontBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomLeft;
                    ctrVertices++;
                }
                if (vertices.FrontBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomRight;
                    ctrVertices++;
                }
                if (vertices.FrontTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopRight;
                }

                //Store the vertices in the CCW order
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 4


            //Case 5: If the points are along the bottom face of the cube
            if (faceType == CubeFaceType.MinY)
            {
                ctrVertices = 0;
                if (vertices.BackBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomLeft;
                    ctrVertices++;
                }
                if (vertices.BackBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomRight;
                    ctrVertices++;
                }
                if (vertices.FrontBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomRight;
                    ctrVertices++;
                }
                if (vertices.FrontBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomLeft;
                }

                //Store the vertices in the CCW order
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 5

            //Case 6: If the points are along the top face of the cube
            if (faceType == CubeFaceType.MaxY)
            {
                ctrVertices = 0;
                if (vertices.BackTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopLeft;
                    ctrVertices++;
                }
                if (vertices.FrontTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopLeft;
                    ctrVertices++;
                }
                if (vertices.FrontTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopRight;
                    ctrVertices++;
                }
                if (vertices.BackTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopRight;
                }

                //Store the vertices in the CCW order
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 6


            //Case 7: Triangle cutting a corner
            //There can be eight such triangles

            //Case i
            //Around backTopLeft
            if (vertices.BackBottomLeft && vertices.FrontTopLeft && vertices.BackTopRight)
            {

                if (!cubeCorners.BackTopLeft)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.BackBottomLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontTopLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackTopRight;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.FrontTopLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.BackBottomLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackTopRight;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case ii:
            //Around backBottomLeft
            if (vertices.FrontBottomLeft && vertices.BackTopLeft && vertices.BackBottomRight)
            {

                if (!cubeCorners.BackBottomLeft)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.FrontBottomLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomRight;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontBottomLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomRight;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case iii:
            //Around backBottomRight
            if (vertices.BackBottomLeft && vertices.BackTopRight && vertices.FrontBottomRight)
            {

                if (!cubeCorners.BackBottomRight)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.BackBottomLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.BackTopRight;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontBottomRight;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.BackTopRight;
                    cubeCornerTypes[1] = CubeCornerTypes.BackBottomLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontBottomRight;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case iv:
            //Around backTopRight
            if (vertices.BackTopLeft && vertices.FrontTopRight && vertices.BackBottomRight)
            {
                if (!cubeCorners.BackTopRight)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontTopRight;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomRight;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.FrontTopRight;
                    cubeCornerTypes[1] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomRight;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case v:
            //Around frontTopLeft
            if (vertices.BackTopLeft && vertices.FrontBottomLeft && vertices.FrontTopRight)
            {
                if (!cubeCorners.FrontTopLeft)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontBottomLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontTopRight;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.FrontBottomLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.BackTopLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontTopRight;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case vi:
            //Around frontBottomLeft
            if (vertices.FrontBottomRight && vertices.FrontTopLeft && vertices.BackBottomLeft)
            {
                if (!cubeCorners.FrontBottomLeft)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.FrontBottomRight;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontTopLeft;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomLeft;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.FrontTopLeft;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontBottomRight;
                    cubeCornerTypes[2] = CubeCornerTypes.BackBottomLeft;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case vii:
            //Around frontBottomRight
            if (vertices.BackBottomRight && vertices.FrontTopRight && vertices.FrontBottomLeft)
            {
                if (!cubeCorners.FrontBottomRight)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.BackBottomRight;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontTopRight;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontBottomLeft;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.FrontTopRight;
                    cubeCornerTypes[1] = CubeCornerTypes.BackBottomRight;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontBottomLeft;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //Case viii:
            //Around frontTopRight
            if (vertices.FrontBottomRight && vertices.BackTopRight && vertices.FrontTopLeft)
            {
                if (!cubeCorners.FrontTopRight)
                {
                    //anti-clockwise when viewed from the corner
                    cubeCornerTypes[0] = CubeCornerTypes.FrontBottomRight;
                    cubeCornerTypes[1] = CubeCornerTypes.BackTopRight;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontTopLeft;
                }
                else
                {
                    //clockwise
                    cubeCornerTypes[0] = CubeCornerTypes.BackTopRight;
                    cubeCornerTypes[1] = CubeCornerTypes.FrontBottomRight;
                    cubeCornerTypes[2] = CubeCornerTypes.FrontTopLeft;
                }
                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            }

            //End of case 7

            //Case 8: If the triangle has a component in the x and z directions only.
            //Check whether all the points are along the two possible diognal planes
            var xZPlane = false;
            ctrVertices = 0;
            if (vertices.BackBottomLeft)
                ctrVertices++;
            if (vertices.BackTopLeft)
                ctrVertices++;
            if (vertices.FrontTopRight)
                ctrVertices++;
            if (vertices.FrontBottomRight)
                ctrVertices++;
            if (ctrVertices == 3)
                xZPlane = true;
            else
            {
                ctrVertices = 0;
                if (vertices.BackBottomRight)
                    ctrVertices++;
                if (vertices.BackTopRight)
                    ctrVertices++;
                if (vertices.FrontTopLeft)
                    ctrVertices++;
                if (vertices.FrontBottomLeft)
                    ctrVertices++;
                if (ctrVertices == 3)
                    xZPlane = true;
            }

            if (xZPlane)
            {
                //Decide whether the vertices should be in CW or CCW direction

                //Logic: If there's a valid point in front of any of the points of the given triangle
                //then the triangle is not facing the front and hence the vertices should be aligned
                //in CW order, else the triangle is facing the front and hence the vertices should be
                //aligned in CCW order when looking from the front

                if (faceType == CubeFaceType.DgPlane1)
                {
                    if (cubeCorners.FrontBottomRight || cubeCorners.FrontTopRight)
                        cCW = false;
                    else
                        cCW = true;
                }
                else if (faceType == CubeFaceType.DgPlane2)
                {
                    if (cubeCorners.FrontBottomLeft || cubeCorners.FrontTopLeft)
                        cCW = false;
                    else
                        cCW = true;
                }
                else
                {
                    if ((vertices.BackTopLeft && cubeCorners.FrontTopLeft) 
                        || (vertices.BackTopRight && cubeCorners.FrontTopRight) 
                        || (vertices.BackBottomLeft && cubeCorners.FrontBottomLeft) 
                        || (vertices.BackBottomRight && cubeCorners.FrontBottomRight))
                        cCW = false; //Store the vertices in the CW order looking from front
                    else
                        cCW = true; //Store the vertices in the CCW order looking from front
                }

                //Intially arrange the vertices in CCW direction
                ctrVertices = 0;

                //First priority to top left corner
                if (vertices.BackTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopLeft;
                    ctrVertices++;
                }
                else if (vertices.FrontTopLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopLeft;
                    ctrVertices++;
                }

                //Second priority to bottom left corner
                if (vertices.BackBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomLeft;
                    ctrVertices++;
                }
                else if (vertices.FrontBottomLeft)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomLeft;
                    ctrVertices++;
                }

                //Third priority to bottom right corner
                if (vertices.BackBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomRight;
                    ctrVertices++;
                }
                else if (vertices.FrontBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomRight;
                    ctrVertices++;
                }

                //Last priority to top right corner
                if (ctrVertices != 3)
                {
                    if (vertices.BackTopRight)
                    {
                        cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopRight;
                    }
                    else if (vertices.FrontTopRight)
                    {
                        cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopRight;
                    }
                }

                if (!cCW)
                {
                    //Swap two vertices
                    temp = cubeCornerTypes[1];
                    cubeCornerTypes[1] = cubeCornerTypes[2];
                    cubeCornerTypes[2] = temp;
                }

                StoreTriangle(x, y, z, cubeCornerTypes);
                return;
            } //End of Case 8

            //Case 9:
            //If all the previous cases are not satisfied, then this is the default case

            //Decide whether the vertices should be in CW or CCW direction

            //Logic: If there's a valid point on top of any of the points of the given triangle
            //then the triangle is not facing the top and hence the vertices should be aligned
            //in CW order, else the triangle is facing the top and hence the vertices should be
            //aligned in CCW order
            if (faceType == CubeFaceType.DgPlane3)
            {
                if (cubeCorners.FrontTopRight || cubeCorners.BackTopRight)
                    cCW = false;
                else
                    cCW = true;
            }
            else if (faceType == CubeFaceType.DgPlane4)
            {
                if (cubeCorners.FrontTopLeft || cubeCorners.BackTopLeft)
                    cCW = false;
                else
                    cCW = true;
            }
            else if (faceType == CubeFaceType.DgPlane5)
            {
                if (cubeCorners.BackTopLeft || cubeCorners.BackTopRight)
                    cCW = false;
                else
                    cCW = true;
            }
            else if (faceType == CubeFaceType.DgPlane6)
            {
                if (cubeCorners.FrontTopLeft || cubeCorners.FrontTopRight)
                    cCW = false;
                else
                    cCW = true;
            }
            else
            {
                if ((vertices.BackBottomLeft && cubeCorners.BackTopLeft) 
                    || (vertices.BackBottomRight && cubeCorners.BackTopRight) 
                    || (vertices.FrontBottomLeft && cubeCorners.FrontTopLeft) 
                    || (vertices.FrontBottomRight && cubeCorners.FrontTopRight))
                    cCW = false; //Store the vertices in the CW order looking from top
                else
                    cCW = true; //Store the vertices in the CCW order looking from top
            }

            //Intially arrange the vertices in CCW direction
            ctrVertices = 0;

            //First priority to back left corner
            if (vertices.BackTopLeft)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopLeft;
                ctrVertices++;
            }
            else if (vertices.BackBottomLeft)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomLeft;
                ctrVertices++;
            }

            //Next priority to front left corner
            if (vertices.FrontTopLeft)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopLeft;
                ctrVertices++;
            }
            else if (vertices.FrontBottomLeft)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomLeft;
                ctrVertices++;
            }

            //Next priority to front right corner
            if (vertices.FrontTopRight)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontTopRight;
                ctrVertices++;
            }
            else if (vertices.FrontBottomRight)
            {
                cubeCornerTypes[ctrVertices] = CubeCornerTypes.FrontBottomRight;
                ctrVertices++;
            }

            //Last priority to back right corner
            if (ctrVertices != 3)
            {
                if (vertices.BackTopRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackTopRight;
                }
                else if (vertices.BackBottomRight)
                {
                    cubeCornerTypes[ctrVertices] = CubeCornerTypes.BackBottomRight;
                }
            }

            if (!cCW)
            {
                //Swap two vertices
                temp = cubeCornerTypes[1];
                cubeCornerTypes[1] = cubeCornerTypes[2];
                cubeCornerTypes[2] = temp;
            }

            StoreTriangle(x, y, z, cubeCornerTypes);
            return;
            //End of Case 9
        }

        private void StoreTriangle(int x, int y, int z, IList<CubeCornerTypes> vertices)
        {
           if (!(vertices[0] == CubeCornerTypes.BackTopLeft || vertices[0] == CubeCornerTypes.BackBottomLeft || vertices[0] == CubeCornerTypes.BackBottomRight || vertices[0] == CubeCornerTypes.BackTopRight || vertices[0] == CubeCornerTypes.FrontTopLeft || vertices[0] == CubeCornerTypes.FrontBottomLeft || vertices[0] == CubeCornerTypes.FrontBottomRight || vertices[0] == CubeCornerTypes.FrontTopRight))
                throw new Exception("Logical error1");
            if (!(vertices[1] == CubeCornerTypes.BackTopLeft || vertices[1] == CubeCornerTypes.BackBottomLeft || vertices[1] == CubeCornerTypes.BackBottomRight || vertices[1] == CubeCornerTypes.BackTopRight || vertices[1] == CubeCornerTypes.FrontTopLeft || vertices[1] == CubeCornerTypes.FrontBottomLeft || vertices[1] == CubeCornerTypes.FrontBottomRight || vertices[1] == CubeCornerTypes.FrontTopRight))
                throw new Exception("Logical error2");
            if (!(vertices[2] == CubeCornerTypes.BackTopLeft || vertices[2] == CubeCornerTypes.BackBottomLeft || vertices[2] == CubeCornerTypes.BackBottomRight || vertices[2] == CubeCornerTypes.BackTopRight || vertices[2] == CubeCornerTypes.FrontTopLeft || vertices[2] == CubeCornerTypes.FrontBottomLeft || vertices[2] == CubeCornerTypes.FrontBottomRight || vertices[2] == CubeCornerTypes.FrontTopRight))
                throw new Exception("Logical error3");

            for (var ctr = 0; ctr <= 2; ctr++)
            {
                var cubeCornerType = vertices[ctr];

                var cubePt = GetPt(cubeCornerType, x, y, z);

                var position = _positionIndices.AddPositionOrGetExistingPositionIndex(cubePt);
                _triangleIndices.Add(position);
            }
        }

        private static CubePt GetPt(CubeCornerTypes cubeCornerType, int x, int y, int z)
        {
            switch (cubeCornerType)
            {
                case CubeCornerTypes.BackTopLeft: 
                    return new CubePt {X = x, Y = y + 1, Z = z};
                case CubeCornerTypes.BackBottomLeft: 
                    return new CubePt {X = x, Y = y, Z = z};
                case CubeCornerTypes.BackBottomRight: 
                    return new CubePt {X = x + 1, Y = y, Z = z};
                case CubeCornerTypes.BackTopRight: 
                    return new CubePt {X = x + 1, Y = y + 1, Z = z};
                case CubeCornerTypes.FrontTopLeft: 
                    return new CubePt {X = x, Y = y + 1, Z = z + 1};
                case CubeCornerTypes.FrontBottomLeft: 
                    return new CubePt {X = x, Y = y, Z = z + 1};
                case CubeCornerTypes.FrontBottomRight: 
                    return new CubePt {X = x + 1, Y = y, Z = z + 1};
                case CubeCornerTypes.FrontTopRight: 
                    return new CubePt {X = x + 1, Y = y + 1, Z = z + 1};
                default:
                    throw new Exception("Invalid call to GetPt. Invalid value for cubeCornerType.");
            }
        }
    }
}