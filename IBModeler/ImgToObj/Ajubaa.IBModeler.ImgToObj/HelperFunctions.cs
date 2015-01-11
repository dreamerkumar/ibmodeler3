using System;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.ImgToObj
{
    public enum MoldActionType
    {
        SetInvalidPts,
        CheckForAllPtsSetToInvalid
    }
    public static class HelperFunctions
    {
        public static bool SetNearFarRectangle(ref Point3D[] arrFarPlane, ref Point3D[] arrNearPlane, ref double dNear, ref double dFar, ref bool blnCamInCuboid,
                Point3D[] arrCuboidFt, Point3D[] arrCuboidBk, Point3D inCameraLocation, Point3D inLookingAt, CameraRatio cameraRatio, bool blnCameraAtInfinity, double dblProximityValue)
        {

            //////////////////Variable Declaration////////////////////////////////////////////////////
            //constants for the plane equation
            double l;
            double m;
            double n;
            double k; //constant for the plane passing through inCameraLocation
            double kFarPlane; //constant in the far plane equation
            double kNearPlane; //constant in the near plane equation

            double sign;
            double tempVal;
            bool flgFound; //flag to indicate whether the target cuboid is within the aperture of the
            //image taken
            double maxConst = 0.0f; //Stores the maximum value that should be added to the equation of plane
            //passing through inCameraLocation, so that a far plane is decided  
            //which covers up all the cuboid points to be processed
            uint i; //Counter variable

            Vector3D vDirection = new Vector3D();
            Vector3D upDirection = new Vector3D();
            Vector3D rightDirection = new Vector3D();//directions on the far plane
            double dtr, t; //used in calculating distance of midPt from the camera location
            Point3D midPt = new Point3D();
            Point3D midPtNear = new Point3D(); //the point at the center of the far rectangle
            double xScope, yScope; //half lengths of the area covered by the camera on Far rectangle
            double distance; //distance of midPt from the camera location
            /////////////////////////////////////////////////////////////////////////////////////////


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~step 1~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Find the equation of the plane passing through the inCameraLocation. Equation is in  */
            /* the form x l + y m + z n + k = 0                                                     */

            //get the direction of view
            vDirection = new Vector3D(inLookingAt.X, inLookingAt.Y, inLookingAt.Z) - new Vector3D(inCameraLocation.X, inCameraLocation.Y, inCameraLocation.Z);

            if (vDirection.Length == 0)
                throw new Exception("The looking vector was found to be null");


            //divide by modulus to get the unit vector
            vDirection = vDirection / vDirection.Length;

            //directional cosines of the plane
            l = vDirection.X;
            m = vDirection.Y;
            n = vDirection.Z;

            //constant for the equation of plane 
            k = -(inCameraLocation.X * l + inCameraLocation.Y * m + inCameraLocation.Z * n);


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~step 2~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Get the equation of the far plane parallel to this plane and enclosing all the       */
            /* Cuboid points and the CameraLocation on the same side                                */

            //target cuboid points which are in range should lie on the same side of the plane passing
            //through CameraLocation
            sign = PlaneEquation.ValueInPlaneEquation(inLookingAt, l, m, n, k);


            //If any of the target cuboid points gives the same sign, then the cuboid has to be 
            //processed, otherwise it lies outside the purview of the taken snapshot
            flgFound = false;

            for (i = 0; i <= 7; i++)
            {
                if (i <= 3)
                    tempVal = PlaneEquation.ValueInPlaneEquation(arrCuboidFt[i], l, m, n, k);
                else
                    tempVal = PlaneEquation.ValueInPlaneEquation(arrCuboidBk[i - 4], l, m, n, k);
                //i-4 is done to traverse from 0 to 3 for back face after traversing through 
                //the front face 
                if (CommonFunctions.HasSameSigns(sign, tempVal))
                {
                    if (flgFound == false)
                    {
                        maxConst = tempVal;
                        flgFound = true;
                    }
                    else if (CommonFunctions.ModValue(maxConst) < CommonFunctions.ModValue(tempVal))
                        maxConst = tempVal;
                }
            }

            if (flgFound)
                kFarPlane = k - maxConst; // constant for the equation of the far plane 
            else
                throw new Exception("Image shot out of target");



            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ step 3 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Make the bounding rectangle on the far plane. We need to find out the four        */
            /* directions from the central point on the plane along which the four corner points */
            /* lie. If the view direction would have been along the -ve z axis, then the upward  */
            /* direction would have been the positive direction of y axis and moving towards     */
            /* right would mean moving in the positive direction of x axis.                      */


            //check whether the vector is along the x, y or z axis
            if (vDirection.X != 0 && vDirection.Y == 0 && vDirection.Z == 0)
            {
                //aligned along the x axis
                upDirection = new Vector3D(0.0f, 1.0f, 0.0f);

                if (vDirection.X > 0)
                    rightDirection = new Vector3D(0.0f, 0.0f, 1.0f);
                else
                    rightDirection = new Vector3D(0.0f, 0.0f, -1.0f);

            }
            else if (vDirection.Y != 0 && vDirection.X == 0 && vDirection.Z == 0)
            {
                //aligned along the y axis
                if (vDirection.Y > 0)
                    upDirection = new Vector3D(0.0f, 0.0f, 1.0f);
                else
                    upDirection = new Vector3D(0.0f, 0.0f, -1.0f);

                rightDirection = new Vector3D(1.0f, 0.0f, 0.0f);


            }
            else if (vDirection.Z != 0 && vDirection.Y == 0 && vDirection.X == 0)
            {
                //aligned along the z axis
                upDirection = new Vector3D(0.0f, 1.0f, 0.0f);

                if (vDirection.Z > 0)
                    rightDirection = new Vector3D(-1.0f, 0.0f, 0.0f);
                else
                    rightDirection = new Vector3D(1.0f, 0.0f, 0.0f);


            }
            else if (vDirection.X == 0 && vDirection.Y != 0 && vDirection.Z != 0)
            {
                //In the yz plane
                if (vDirection.Z < 0)
                    rightDirection = new Vector3D(1.0f, 0.0f, 0.0f);
                else
                    rightDirection = new Vector3D(-1.0f, 0.0f, 0.0f);

                upDirection = Vector3D.CrossProduct(rightDirection, vDirection);

            }
            else if (vDirection.X != 0 && vDirection.Y == 0 && vDirection.Z != 0)
            {
                //In the xz plane
                upDirection = new Vector3D(0.0f, 1.0f, 0.0f);

                rightDirection = Vector3D.CrossProduct(vDirection, upDirection);

            }
            else if (vDirection.X != 0 && vDirection.Y != 0 && vDirection.Z == 0)
            {
                //In the xy plane
                if (vDirection.X > 0)
                    rightDirection = new Vector3D(0.0f, 0.0f, 1.0f);
                else
                    rightDirection = new Vector3D(0.0f, 0.0f, -1.0f);

                upDirection = Vector3D.CrossProduct(rightDirection, vDirection);

            }
            else if (vDirection.X != 0 && vDirection.Y != 0 && vDirection.Z != 0)
            {
                //Not along the axis, neither along the three planes

                if ((vDirection.Z < 0 && vDirection.X > 0)
                        || (vDirection.Z > 0 && vDirection.X < 0))
                    upDirection = Vector3D.CrossProduct(new Vector3D(vDirection.X, 0.0f, 0.0f), vDirection);
                else
                    upDirection = Vector3D.CrossProduct(vDirection, new Vector3D(vDirection.X, 0.0f, 0.0f));

                rightDirection = Vector3D.CrossProduct(vDirection, upDirection);
            }


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Step 4~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Find the co-ordinates of the point at which the perpendicular line from the         */
            /* CameraLocation intersects the far plane. Since the point lies on the line it should */
            /* satisfy the following equations : x = x1 + l t; y = y1 + m t; z = z1 + n t;         */
            /* Putting the values for the point in the equation of the plane: xl+ym+zn+kFarPlane=0 */
            /* we can get the value of t.                                                          */
            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

            dtr = l * l + m * m + n * n;

            if (dtr == 0)
                throw new Exception("Division by zero attempted in set near far rectangle function");

            t = -(kFarPlane + inCameraLocation.X * l + inCameraLocation.Y * m
                            + inCameraLocation.Z * n) / dtr;

            midPt = new Point3D(inCameraLocation.X + l * t, inCameraLocation.Y + m * t,
                        inCameraLocation.Z + n * t);


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Step 5~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /*~Adjust the magnitude of the vectors so that they cover the required area in        ~*/
            /*~the far plane																      ~*/

            if (blnCameraAtInfinity)
            {
                if (cameraRatio.xRangeAtInfinity <= 0
                    || cameraRatio.yRangeAtInfinity <= 0)
                    throw new Exception("Invalid range at infinity values found in the set near far rectangle function.");

                //Correction done on 28th Jan 2004. The infinite range values should also be 
                //divided by two
                //xScope = dataPtr.cameraRatio.xRangeAtInfinity;
                //yScope = dataPtr.cameraRatio.yRangeAtInfinity;
                xScope = cameraRatio.xRangeAtInfinity / 2.0f;
                yScope = cameraRatio.yRangeAtInfinity / 2.0f;
            }
            else
            {
                distance = (CommonFunctions.GetVector(midPt) - CommonFunctions.GetVector(inCameraLocation)).Length;
                xScope = (distance * cameraRatio.xRatio) / 2.0f;
                yScope = (distance * cameraRatio.yRatio) / 2.0f;
            }

            if (upDirection.Length != 0 && rightDirection.Length != 0)
            {
                upDirection = upDirection * (yScope / upDirection.Length);
                rightDirection = rightDirection * (xScope / rightDirection.Length);
            }
            else
                throw new Exception("Division by zero attempted in set near far rectangle function.");


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Step 6~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Using the vector equations and this point, find the corners of the far plane        */

            //top left
            arrFarPlane[0].X = midPt.X + upDirection.X - rightDirection.X;
            arrFarPlane[0].Y = midPt.Y + upDirection.Y - rightDirection.Y;
            arrFarPlane[0].Z = midPt.Z + upDirection.Z - rightDirection.Z;
            //bottom left
            arrFarPlane[1].X = midPt.X - upDirection.X - rightDirection.X;
            arrFarPlane[1].Y = midPt.Y - upDirection.Y - rightDirection.Y;
            arrFarPlane[1].Z = midPt.Z - upDirection.Z - rightDirection.Z;
            //bottom right
            arrFarPlane[2].X = midPt.X - upDirection.X + rightDirection.X;
            arrFarPlane[2].Y = midPt.Y - upDirection.Y + rightDirection.Y;
            arrFarPlane[2].Z = midPt.Z - upDirection.Z + rightDirection.Z;
            //top right
            arrFarPlane[3].X = midPt.X + upDirection.X + rightDirection.X;
            arrFarPlane[3].Y = midPt.Y + upDirection.Y + rightDirection.Y;
            arrFarPlane[3].Z = midPt.Z + upDirection.Z + rightDirection.Z;


            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Step 7~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /* Find distance of the Near Plane from Camera if it lies outside the cuboid. ~~~~~~~~*/

            if (WithinFrustum(inCameraLocation, arrCuboidFt, arrCuboidBk, dblProximityValue))
                blnCamInCuboid = true;
            else
            {
                blnCamInCuboid = false;

                //find the distance of the near plane from the camera location

                for (i = 0; i <= 7; i++)
                {

                    if (i <= 3)
                        tempVal = PlaneEquation.ValueInPlaneEquation(arrCuboidFt[i], l, m, n,
                                                kFarPlane);
                    else
                        tempVal = PlaneEquation.ValueInPlaneEquation(arrCuboidBk[i - 4], l, m, n,
                                                kFarPlane);
                    if (i == 0)
                        maxConst = tempVal;
                    else if (CommonFunctions.ModValue(maxConst) < CommonFunctions.ModValue(tempVal))
                        maxConst = tempVal;
                }

                kNearPlane = kFarPlane - maxConst; //constant for the near plane	

                dNear = CommonFunctions.ModValue(k - kNearPlane); //distances of near and far plane 
                dFar = CommonFunctions.ModValue(k - kFarPlane);	 //from the CameraLocation	
            }

            /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~Step 8~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
            /*~~~~~ If the object is at infinity then define the corners of the near plane ~~~~~~~*/
            if (blnCameraAtInfinity)
            {
                //mblnCamAtInfinity = true ;        		
                if (blnCamInCuboid)
                    throw new Exception("Error occured inside the set near far rectangle function: Camera at infinity found inside the cuboid.");

                //get the middle point of the near plane 
                midPtNear = CommonFunctions.GetMiddlePoint(inCameraLocation, midPt, dNear, dFar);

                /* Using the vector equations and this point, find the corners of the near plane  */

                //top left
                arrNearPlane[0].X = midPtNear.X + upDirection.X - rightDirection.X;
                arrNearPlane[0].Y = midPtNear.Y + upDirection.Y - rightDirection.Y;
                arrNearPlane[0].Z = midPtNear.Z + upDirection.Z - rightDirection.Z;
                //bottom left
                arrNearPlane[1].X = midPtNear.X - upDirection.X - rightDirection.X;
                arrNearPlane[1].Y = midPtNear.Y - upDirection.Y - rightDirection.Y;
                arrNearPlane[1].Z = midPtNear.Z - upDirection.Z - rightDirection.Z;
                //bottom right
                arrNearPlane[2].X = midPtNear.X - upDirection.X + rightDirection.X;
                arrNearPlane[2].Y = midPtNear.Y - upDirection.Y + rightDirection.Y;
                arrNearPlane[2].Z = midPtNear.Z - upDirection.Z + rightDirection.Z;
                //top right
                arrNearPlane[3].X = midPtNear.X + upDirection.X + rightDirection.X;
                arrNearPlane[3].Y = midPtNear.Y + upDirection.Y + rightDirection.Y;
                arrNearPlane[3].Z = midPtNear.Z + upDirection.Z + rightDirection.Z;

            } //else 
            //mblnCamAtInfinity = false ;

            return true;
        }

        public static uint GetClosestPtIndex(double inFltPosition, bool blnStartPt, TargetCuboid targetCuboid, double dX, double dblProximityValue)
        {

            double fltDivVal, fltRemainder;
            uint uintReturnVal;

            if (blnStartPt)
            {
                if (inFltPosition >= targetCuboid.fltMinx)
                {
                    if (inFltPosition > targetCuboid.fltMaxx)
                        uintReturnVal = 0; //No points can be set as invalid
                    else
                    {
                        fltDivVal = (inFltPosition - targetCuboid.fltMinx) / dX;

                        uintReturnVal = CommonFunctions.GetUIntFromDouble(fltDivVal);

                        fltRemainder = fltDivVal - (double)uintReturnVal;

                        //18-March-11: THIS IS AN IMPORTANT THING. I SPENT A WHOLE DAY DEBUGGING WHY THE OUTPUT FROM
                        //IBModeler 2.0 IS COMING OUT TO BE DIFFERENT THAN THIS CODE. FINALLY, LANDED HERE WHICH EXPLAINS WHY.
                        //Change on 29-April-08 don't see any reason why the proximity calculation for
                        //start point should be any different than that of the end of point so making them the
                        //same. This will resolve the issue: If the same value is passed for the start point
                        //and the end point, then the value of start point comes out to be greater than end point
                        //which does not make sense
                        //if (fltRemainder > dblProximityValue)
                        //    uintReturnVal++;
                        if ((1.0f - fltRemainder) <= dblProximityValue)
                            uintReturnVal++;
                        //end of change on 29-April-08

                        uintReturnVal++; //All point indices start from one and not from zero
                    }
                }
                else
                    uintReturnVal = 1;
            }
            else
            {
                if (inFltPosition <= targetCuboid.fltMaxx)
                {
                    if (inFltPosition < targetCuboid.fltMinx)
                        uintReturnVal = 0; //No points can be set as invalid
                    else
                    {

                        fltDivVal = (inFltPosition - targetCuboid.fltMinx) / dX;

                        uintReturnVal = CommonFunctions.GetUIntFromDouble(fltDivVal);

                        fltRemainder = fltDivVal - (double)uintReturnVal;

                        if ((1.0f - fltRemainder) <= dblProximityValue)
                            uintReturnVal++;

                        uintReturnVal++; //All point indices start from one and not from zero
                    }
                }
                else
                    uintReturnVal = targetCuboid.uintXPoints; //Assign the maximum value
            }
            return uintReturnVal;
        }

        public static bool SetInvalidPtsForImgPixelPositions(Params mp, uint inX1, uint inX2, uint inY)
        {
            bool blnAValidPtFound = false; //filler param
            return ProcessImgPixelPositions(mp, inX1, inX2, inY, MoldActionType.SetInvalidPts, ref blnAValidPtFound);
        }
        public static bool ProcessImgPixelPositions(Params mp, uint inX1, uint inX2, uint inY, MoldActionType actionType, ref bool blnAValidPtFound)
        {

            #region variable definitions
            Point3D[] frstmFt = new Point3D[4]; //end points of the frustum or pyramid
            Point3D[] frstmBk = new Point3D[4];

            Point3D upFLeft, upFRight, downFLeft, downFRight;//points at the same height on the 
            //far rectangle as that of the corners of the invalid rectangle
            Point3D upNLeft, upNRight, downNLeft, downNRight;//points at the same height on the 
            //far rectangle as that of the corners of the invalid rectangle

            double miny, maxy, minz, maxz; //defines the limit within which cuboid points
            //are to be considered	
            uint y1, z1; //first indexes from where we start traversing the cuboid points
            uint y2, z2; //Last indexes
            double y1Pos, z1Pos;
            #endregion variable definitions
            
            #region define the back face on the far rectangle

            #region first get the heights of the four sides of the invalid rectangle
            if (inY == mp.YPixels)
            { //top most pixel
                upFLeft = mp.FarPlane[0];
                upFRight = mp.FarPlane[3];

            }
            else
            {
                upFLeft = CommonFunctions.GetMiddlePoint(mp.FarPlane[1], mp.FarPlane[0], inY, mp.YPixels);
                upFRight = CommonFunctions.GetMiddlePoint(mp.FarPlane[2], mp.FarPlane[3], inY, mp.YPixels);
            }



            if (inY == 1)
            { //inY is one for the bottommost pixel
                downFLeft = mp.FarPlane[1];
                downFRight = mp.FarPlane[2];
            }
            else
            {
                downFLeft = CommonFunctions.GetMiddlePoint(mp.FarPlane[1], mp.FarPlane[0], inY - 1, mp.YPixels);
                downFRight = CommonFunctions.GetMiddlePoint(mp.FarPlane[2], mp.FarPlane[3], inY - 1, mp.YPixels);
            }
            #endregion

            #region now move these points horizontally to make the actual invalid rectangle

            if (inX1 == 0)
            {
                frstmBk[0] = upFLeft;
                frstmBk[1] = downFLeft;

            }
            else
            {
                frstmBk[0] = CommonFunctions.GetMiddlePoint(upFLeft, upFRight, inX1, mp.XPixels);
                frstmBk[1] = CommonFunctions.GetMiddlePoint(downFLeft, downFRight, inX1, mp.XPixels);
            }

            if (inX2 == mp.XPixels)
            {
                frstmBk[3] = upFRight;
                frstmBk[2] = downFRight;

            }
            else
            {
                frstmBk[3] = CommonFunctions.GetMiddlePoint(upFLeft, upFRight, inX2, mp.XPixels);
                frstmBk[2] = CommonFunctions.GetMiddlePoint(downFLeft, downFRight, inX2, mp.XPixels);

            }
            #endregion move horizontally
            #endregion back face

            #region define the front face
            if (mp.MProcessMoldParams.ImageParams.CameraAtInfinity)
            {
                #region first get the heights of the four sides of the invalid rectangle
                if (inY == mp.YPixels)
                { //top most pixel
                    upNLeft = mp.NearPlane[0];
                    upNRight = mp.NearPlane[3];

                }
                else
                {
                    upNLeft = CommonFunctions.GetMiddlePoint(mp.NearPlane[1], mp.NearPlane[0], inY, mp.YPixels);
                    upNRight = CommonFunctions.GetMiddlePoint(mp.NearPlane[2], mp.NearPlane[3], inY, mp.YPixels);
                }

                if (inY == 1)
                { //inY is one for the bottommost pixel
                    downNLeft = mp.NearPlane[1];
                    downNRight = mp.NearPlane[2];

                }
                else
                {
                    downNLeft = CommonFunctions.GetMiddlePoint(mp.NearPlane[1], mp.NearPlane[0], inY - 1, mp.YPixels);
                    downNRight = CommonFunctions.GetMiddlePoint(mp.NearPlane[2], mp.NearPlane[3], inY - 1, mp.YPixels);
                }
                #endregion

                #region now move these points horizontally to make the actual invalid rectangle
                if (inX1 == 0)
                {
                    frstmFt[0] = upNLeft;
                    frstmFt[1] = downNLeft;

                }
                else
                {
                    frstmFt[0] = CommonFunctions.GetMiddlePoint(upNLeft, upNRight, inX1, mp.XPixels);
                    frstmFt[1] = CommonFunctions.GetMiddlePoint(downNLeft, downNRight, inX1, mp.XPixels);
                }

                if (inX2 == mp.XPixels)
                {
                    frstmFt[3] = upNRight;
                    frstmFt[2] = downNRight;

                }
                else
                {
                    frstmFt[3] = CommonFunctions.GetMiddlePoint(upNLeft, upNRight, inX2, mp.XPixels);
                    frstmFt[2] = CommonFunctions.GetMiddlePoint(downNLeft, downNRight, inX2, mp.XPixels);
                }
                #endregion move horizontally

            }
            else if (!mp.CamInCuboid)
            { //camera outside the target cuboid but not at infinity

                #region use the distance formula
                frstmFt[0] = CommonFunctions.GetMiddlePoint(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[0], mp.DNear, mp.DFar);
                frstmFt[1] = CommonFunctions.GetMiddlePoint(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[1], mp.DNear, mp.DFar);
                frstmFt[2] = CommonFunctions.GetMiddlePoint(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[2], mp.DNear, mp.DFar);
                frstmFt[3] = CommonFunctions.GetMiddlePoint(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[3], mp.DNear, mp.DFar);
                #endregion
            }
            #endregion front face

            #region calculate the equations of the bounding planes
            var objBoundingPlanes = new BoundingPlanes();
            if (mp.CamInCuboid)
            {
                objBoundingPlanes.BackPlane = new PlaneEquation(frstmBk[3], frstmBk[2], frstmBk[1]);//back
                objBoundingPlanes.RightPlane = new PlaneEquation(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[2], frstmBk[3]);//right
                objBoundingPlanes.TopPlane = new PlaneEquation(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[3], frstmBk[0]);//top
                objBoundingPlanes.LeftPlane = new PlaneEquation(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[0], frstmBk[1]);//left
                objBoundingPlanes.BottomPlane = new PlaneEquation(mp.MProcessMoldParams.ImageParams.cameraLocation, frstmBk[1], frstmBk[2]);//bottom

            }
            else
            {
                objBoundingPlanes.FrontPlane = new PlaneEquation(frstmFt[0], frstmFt[1], frstmFt[2]);//front
                objBoundingPlanes.BackPlane = new PlaneEquation(frstmBk[3], frstmBk[2], frstmBk[1]);//back
                objBoundingPlanes.RightPlane = new PlaneEquation(frstmFt[2], frstmBk[2], frstmBk[3]);//right
                objBoundingPlanes.TopPlane = new PlaneEquation(frstmFt[3], frstmBk[3], frstmBk[0]);//top
                objBoundingPlanes.LeftPlane = new PlaneEquation(frstmFt[0], frstmBk[0], frstmBk[1]);//left
                objBoundingPlanes.BottomPlane = new PlaneEquation(frstmFt[1], frstmBk[1], frstmBk[2]);//bottom
            }
            #endregion

            #region get the coordinate ranges within which the invalid frustum or pyramid lies
            if (!mp.CamInCuboid)
            {
                //minx = Min(frstmFt[0].X, frstmFt[1].X, frstmFt[2].X, frstmFt[3].X, 
                //		   frstmBk[0].X, frstmBk[1].X, frstmBk[2].X, frstmBk[3].X );

                //	maxx = Max(frstmFt[0].X, frstmFt[1].X, frstmFt[2].X, frstmFt[3].X, 
                //		   frstmBk[0].X, frstmBk[1].X, frstmBk[2].X, frstmBk[3].X );

                miny = CommonFunctions.Min(frstmFt[0].Y, frstmFt[1].Y, frstmFt[2].Y, frstmFt[3].Y,
                           frstmBk[0].Y, frstmBk[1].Y, frstmBk[2].Y, frstmBk[3].Y);

                maxy = CommonFunctions.Max(frstmFt[0].Y, frstmFt[1].Y, frstmFt[2].Y, frstmFt[3].Y,
                           frstmBk[0].Y, frstmBk[1].Y, frstmBk[2].Y, frstmBk[3].Y);

                minz = CommonFunctions.Min(frstmFt[0].Z, frstmFt[1].Z, frstmFt[2].Z, frstmFt[3].Z,
                           frstmBk[0].Z, frstmBk[1].Z, frstmBk[2].Z, frstmBk[3].Z);

                maxz = CommonFunctions.Max(frstmFt[0].Z, frstmFt[1].Z, frstmFt[2].Z, frstmFt[3].Z,
                           frstmBk[0].Z, frstmBk[1].Z, frstmBk[2].Z, frstmBk[3].Z);
            }
            else
            {
                //minx = Min(mp.MProcessMoldParams.imgParams.cameraLocation.X,
                //		   frstmBk[0].X, frstmBk[1].X, frstmBk[2].X, frstmBk[3].X );

                //maxx = Max(mp.MProcessMoldParams.imgParams.cameraLocation.X,
                //		   frstmBk[0].X, frstmBk[1].X, frstmBk[2].X, frstmBk[3].X );

                miny = CommonFunctions.Min(mp.MProcessMoldParams.ImageParams.cameraLocation.Y,
                           frstmBk[0].Y, frstmBk[1].Y, frstmBk[2].Y, frstmBk[3].Y);

                maxy = CommonFunctions.Max(mp.MProcessMoldParams.ImageParams.cameraLocation.Y,
                           frstmBk[0].Y, frstmBk[1].Y, frstmBk[2].Y, frstmBk[3].Y);

                minz = CommonFunctions.Min(mp.MProcessMoldParams.ImageParams.cameraLocation.Z,
                           frstmBk[0].Z, frstmBk[1].Z, frstmBk[2].Z, frstmBk[3].Z);

                maxz = CommonFunctions.Max(mp.MProcessMoldParams.ImageParams.cameraLocation.Z,
                           frstmBk[0].Z, frstmBk[1].Z, frstmBk[2].Z, frstmBk[3].Z);
            }


            //check whether min values lie outside target cuboid
            if ( //minx > xLast || maxx < xFirst || 
                miny > mp.YLast || maxy < mp.YFirst ||
                minz > mp.ZLast || maxz < mp.ZFirst)
                return true; //no points effected

            //set the starting and ending values within range of target cuboid

            /*Modified on 29-Jan-2004 Using proximity value in the calculations
            //min values
            if(minx <= xFirst) 
                x1 = 1 ; 
            else 
                x1 = 1 + CommonFunctions.GetIntFromFloat ((minx - xFirst) /dX) ;
        		
            if(miny <= yFirst) 
                y1 = 1 ;
            else 
                y1 = 1 + CommonFunctions.GetIntFromFloat ((miny - yFirst) /dY) ;
        	
            if(minz <= zFirst) 
                z1 = 1 ;
            else 
                z1 = 1 + CommonFunctions.GetIntFromFloat ((minz - zFirst) /dZ) ;
            */
            //	if(minx <= xFirst + fltProximityValue) 
            //		x1 = 1 ; 
            //	else 
            //	x1 = 1 + CommonFunctions.GetIntFromFloat ((minx - (xFirst + fltProximityValue)) /dX) ;

            if (miny <= mp.YFirst)
                y1 = 1;
            else
                y1 = 1 + CommonFunctions.GetUIntFromDouble((miny - (mp.YFirst + mp.ProximityVal)) / mp.DY);

            if (minz <= mp.ZFirst)
                z1 = 1;
            else
                z1 = 1 + CommonFunctions.GetUIntFromDouble((minz - (mp.ZFirst + mp.ProximityVal)) / mp.DZ);
            //End of modification==============================================

            //	x1Pos = xFirst + dX * ( (double)(x1-1) ) ;
            y1Pos = mp.YFirst + mp.DY * ((double)(y1 - 1));
            z1Pos = mp.ZFirst + mp.DZ * ((double)(z1 - 1));

            /*Modified on 29-Jan-2004 Using proximity value in the calculations
            //max values
            if(maxx >= xLast)
                x2 = targetCuboid.uintXPoints;
            else
                x2 = 1 + CommonFunctions.GetIntFromFloat ((maxx - xFirst) /dX) ;


            if(maxy >= yLast)
                y2 = targetCuboid.uintYPoints ;
            else
                y2 = 1 + CommonFunctions.GetIntFromFloat ((maxy - yFirst) /dY) ;


            if(maxz >= zLast)
                z2 = targetCuboid.uintZPoints ;
            else
                z2 = 1 + CommonFunctions.GetIntFromFloat ((maxz - zFirst) /dZ) ;
            */
            //	if(maxx >= xLast - fltProximityValue)
            //		x2 = targetCuboid.uintXPoints ;
            //	else
            //		x2 = 1 + CommonFunctions.GetIntFromFloat ((maxx - (xFirst - fltProximityValue)) /dX) ;


            if (maxy >= mp.YLast)
                y2 = mp.Cuboid.uintYPoints;
            else
                y2 = 1 + CommonFunctions.GetUIntFromDouble((maxy - (mp.YFirst - mp.ProximityVal)) / mp.DY);


            if (maxz >= mp.ZLast)
                z2 = mp.Cuboid.uintZPoints;
            else
                z2 = 1 + CommonFunctions.GetUIntFromDouble((maxz - (mp.ZFirst - mp.ProximityVal)) / mp.DZ);
            //End of modification	
            #endregion

            //process the evaluated mold points using the bounding plane values
            return ProcessMoldPoints(mp, y1, z1, y2, z2, y1Pos, z1Pos, objBoundingPlanes, actionType, ref blnAValidPtFound);
        }

        public static bool ProcessMoldPoints(Params mp, uint y1, uint z1, uint y2, uint z2, double y1Pos, double z1Pos, BoundingPlanes objBoundingPlanes, MoldActionType actionType, ref bool blnAValidPtFound)
        {
            Point3D startPoint = new Point3D();
            Point3D endPoint = new Point3D();
            Point3D testPoint = new Point3D();
            byte btPointsFound, btCtr;
            PlaneEquation curPlane = new PlaneEquation(1.0f, 0.0f, 0.0f, 0.0f);//temp initialization
            double dblTempPt;
            bool blnInside;
            uint iy;
            uint iz;
            double y, z;   //variables used while traversing
            uint uintInvalidPtX1, uintInvalidPtX2; //X1 and X2 specify the starting and ending indices
            //of the range of invalid points found in a horizontal line at a particular y and z value

            //traverse through the array of points to set the points as invalid	
            for (z = z1Pos, iz = z1; iz <= z2; z += mp.DZ, iz++)
            { //all pts

                //Since the line is along x axis, the y and z value for the point of 
                //intersection will remain the same

                testPoint.Z = z;

                for (y = y1Pos, iy = y1; iy <= y2; y += mp.DY, iy++)
                { //pts along x-y plane	

                    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                    //Major change: Checking each point along any axis is taking too much of time which is 
                    //undesirable. Now onwards, I have decided to change this to find all the invalid points at 
                    //one go. First we find the points of intersection of the line with the faces of the invalid
                    //region, such that the points of intersection lie inside the invalid region. We should always
                    //get two such points if there exists an invalid region for the particular value of y and z.
                    //
                    //After identifying the points of intersection, we will set all the points that lie in between
                    //them as invalid.


                    btPointsFound = 0;
                    //Since the line is along x axis, the y and z value for the point of 
                    //intersection will remain the same
                    testPoint.Y = y;



                    //Try the point of intersection with the bounding planes. We should get two such points
                    byte btTotalPlanes;
                    if (!mp.CamInCuboid)
                        btTotalPlanes = 6;
                    else
                        btTotalPlanes = 5;
                    for (btCtr = 1; btCtr <= btTotalPlanes; btCtr++)
                    {
                        switch (btCtr)
                        {
                            case 1:
                                curPlane = objBoundingPlanes.LeftPlane;
                                break;
                            case 2:
                                curPlane = objBoundingPlanes.RightPlane;
                                break;
                            case 3:
                                curPlane = objBoundingPlanes.TopPlane;
                                break;
                            case 4:
                                curPlane = objBoundingPlanes.BottomPlane;
                                break;
                            case 5:
                                curPlane = objBoundingPlanes.BackPlane;
                                break;
                            case 6:
                                curPlane = objBoundingPlanes.FrontPlane;
                                break;
                        }

                        if (curPlane.A != 0.0f)
                        { //Not parallel to the X axis

                            //Find the x value of the point of intersection
                            testPoint.X = (curPlane.B * y
                                + curPlane.C * z + curPlane.D) /
                                (-curPlane.A);

                            if (!mp.CamInCuboid)
                                blnInside = HelperFunctions.WithinFrustum(testPoint, objBoundingPlanes, mp.ProximityVal);
                            else
                                blnInside = HelperFunctions.WithinPyramid(testPoint, objBoundingPlanes, mp.ProximityVal);

                            if (blnInside && btPointsFound == 1)
                            { //compare for duplicate points
                                if (CommonFunctions.ModValue(testPoint.X - startPoint.X) <= mp.ProximityVal)
                                    //The points can be assumed to be the same
                                    blnInside = false;
                            }
                            if (blnInside)
                            {
                                btPointsFound++;
                                if (btPointsFound == 2)
                                    endPoint = testPoint;
                                else
                                    startPoint = testPoint;
                            }
                        }

                        if (btPointsFound == 2)
                            break; //Don't look further if two points are already found
                    }


                    if (btPointsFound > 0)
                    { //Find invalid points and set their status in the file
                        if (btPointsFound == 2)
                        {
                            if (endPoint.X < startPoint.X)
                            { //Swap the points
                                dblTempPt = endPoint.X;
                                endPoint.X = startPoint.X;
                                startPoint.X = dblTempPt;
                            }
                        }
                        else
                        {
                            //Just one point has been found
                            endPoint = startPoint;
                        }

                        uintInvalidPtX1 = HelperFunctions.GetClosestPtIndex(startPoint.X, true, mp.Cuboid, mp.DX, mp.ProximityVal);
                        if (uintInvalidPtX1 == 0)
                            continue;


                        uintInvalidPtX2 = HelperFunctions.GetClosestPtIndex(endPoint.X, false, mp.Cuboid, mp.DX, mp.ProximityVal);
                        if (uintInvalidPtX2 == 0 || (uintInvalidPtX1 > uintInvalidPtX2))
                            continue;

                        if(actionType == MoldActionType.SetInvalidPts)
                        {
                            //set the points as invalid in the file
                            if (!mp.MoldDataHandler.SetPointRanges(uintInvalidPtX1, uintInvalidPtX2, iy, iz))
                                return false;
                        }
                        else if(actionType == MoldActionType.CheckForAllPtsSetToInvalid)
                        {
                            //traverse the mold points and see if any point is valid
                            if (!mp.MoldDataHandler.CheckPointRangesForValidity(uintInvalidPtX1, uintInvalidPtX2, iy, iz, ref blnAValidPtFound))
                                return false;
                            if (blnAValidPtFound) //Currently we will only lookout for the presence of any valid point even if it is just one
                                return true;
                        }
                        else
                            throw new Exception("Invalid mold action type");
                    }

                } //y	
            } //z

            return true;
        }        
        
        #region WithinFrustum, WithinPyramid Functions
        public static bool WithinFrustum(Point3D inP, Point3D[] inArrFt, Point3D[] inArrBk, double mdblProximityValue)
        {
            if (!IsInsideOfPlane(inP, inArrFt[0], inArrFt[1], inArrFt[2], mdblProximityValue))//front
                return false;
            else if (!IsInsideOfPlane(inP, inArrBk[3], inArrBk[2], inArrBk[1], mdblProximityValue))//back
                return false;
            else if (!IsInsideOfPlane(inP, inArrFt[2], inArrBk[2], inArrBk[3], mdblProximityValue))//right
                return false;
            else if (!IsInsideOfPlane(inP, inArrFt[3], inArrBk[3], inArrBk[0], mdblProximityValue))//top
                return false;
            else if (!IsInsideOfPlane(inP, inArrFt[0], inArrBk[0], inArrBk[1], mdblProximityValue))//left
                return false;
            else if (!IsInsideOfPlane(inP, inArrFt[1], inArrBk[1], inArrBk[2], mdblProximityValue))//bottom
                return false;
            else
                return true;
        }

        public static bool WithinFrustum(Point3D inP, BoundingPlanes boundingPlanes, double mdblProximityValue)
        {

            if (!IsInsideOfPlane(inP, boundingPlanes.FrontPlane, mdblProximityValue))//front
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.BackPlane, mdblProximityValue))//back
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.RightPlane, mdblProximityValue))//right
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.TopPlane, mdblProximityValue))//top
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.LeftPlane, mdblProximityValue))//left
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.BottomPlane, mdblProximityValue))//bottom
                return false;
            else
                return true;
        }

        public static bool WithinPyramid(Point3D inP, Point3D inEnd, Point3D[] inArrBk, double mdblProximityValue)
        {
            if (!IsInsideOfPlane(inP, inArrBk[3], inArrBk[2], inArrBk[1], mdblProximityValue))//back
                return false;
            else if (!IsInsideOfPlane(inP, inEnd, inArrBk[2], inArrBk[3], mdblProximityValue))//right
                return false;
            else if (!IsInsideOfPlane(inP, inEnd, inArrBk[3], inArrBk[0], mdblProximityValue))//top
                return false;
            else if (!IsInsideOfPlane(inP, inEnd, inArrBk[0], inArrBk[1], mdblProximityValue))//left
                return false;
            else if (!IsInsideOfPlane(inP, inEnd, inArrBk[1], inArrBk[2], mdblProximityValue))//bottom
                return false;
            else
                return true;
        }

        public static bool WithinPyramid(Point3D inP, BoundingPlanes boundingPlanes, double mdblProximityValue)
        {
            if (!IsInsideOfPlane(inP, boundingPlanes.BackPlane, mdblProximityValue))//back
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.RightPlane, mdblProximityValue))//right
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.TopPlane, mdblProximityValue))//top
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.LeftPlane, mdblProximityValue))//left
                return false;
            else if (!IsInsideOfPlane(inP, boundingPlanes.BottomPlane, mdblProximityValue))//bottom
                return false;
            else
                return true;
        }
        #endregion

        #region InsideOfPlane Functions
        public static bool IsInsideOfPlane(Point3D inPoint, PlaneEquation inPlnValues, double dblProximityValue)
        {
            if ((inPoint.X * inPlnValues.A + inPoint.Y * inPlnValues.B +
                inPoint.Z * inPlnValues.C + inPlnValues.D) <= dblProximityValue)
                return true;
            else
                return false;
        }

        public static bool IsInsideOfPlane(Point3D inPoint, Point3D inP1, Point3D inP2, Point3D inP3, double dblProximityValue)
        {
            double l, m, n, k;
            Vector3D normal = Vector3D.CrossProduct(CommonFunctions.GetVector(inP2) - CommonFunctions.GetVector(inP1), (CommonFunctions.GetVector(inP3) - CommonFunctions.GetVector(inP1)));

            l = normal.X;
            m = normal.Y;
            n = normal.Z;
            k = -(inP1.X * l + inP1.Y * m + inP1.Z * n);

            /* Modified on 29-Jan-04. Even if the point is outside but close enough to the plane, 
            //we will consider it to be inside of it.=================================================
            if(valFromPlaneEqn(inPoint, l, m, n, k) <= 0 ) 
                return true ;
            else 
                return false ;
            */
            if (PlaneEquation.ValueInPlaneEquation(inPoint, l, m, n, k) <= dblProximityValue)
                return true;
            else
                return false;


            //End of modification=====================================================================
        }
        #endregion
    }
}
