using System;
using System.Windows.Media.Media3D;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;


namespace Ajubaa.IBModeler.ImgToObj
{
    public class Params
    {
        #region fields
        public enum ProcessMoldAction
        {
            SetInvalidPoints,
            AnalyzeDataLoss
        }
        private TargetCuboid mTargetCuboid = new TargetCuboid();
        private bool mblnCamInCuboid;
        private double mdNear;
        private double mdFar;
        private double mdX;
        private double mdY;
        private double mdZ; //the distance between two points on the target cuboid
        private double mxFirst;
        private double myFirst;
        private double mzFirst;
        private double mxLast;
        private double myLast;
        private double mzLast;
        private Point3D[] marrCuboidFt = new Point3D[4];
        private Point3D[] marrCuboidBk = new Point3D[4];
        private Point3D[] marrFarPlane = new Point3D[4];
        private Point3D[] marrNearPlane = new Point3D[4];
        private MoldDataHandler mobjMoldDataHandler = new MoldDataHandler();
        private ProcessMoldParams mobjProcessMoldParams;
        //Added on 29-Jan-2004        
        //The fraction of the inter point distance that should be 
        //used to obtain the proximity value. 
        //Proximity value: Henceforth, even if a point is found
        //to be outside of a plane, but not farther than the proximity value, we will assume it to be
        //inside of the plane. This has been done so that no point is left out just because of an 
        //erroneous double value obtained as a result of a number of arithmetic operations.
        private double mdblProximityVal;
        private const double mcdblProximityConstant = 100.0f;
        private ImageHandler mobjImageHandler;
        private uint muintXPixels;
        private uint muintYPixels;
        #endregion fields

        #region properties
        public TargetCuboid Cuboid
        {
            get { return mTargetCuboid; }
        }
        public bool CamInCuboid
        {
            get { return mblnCamInCuboid; }
        }
        public double DNear
        {
            get { return mdNear; }
        }
        public double DFar
        {
            get { return mdFar; }
        }
        public double DX
        {
            get { return mdX; }
        }
        public double DY
        {
            get { return mdY; }
            
        }
        public double DZ
        {
            get { return mdZ; }
            
        }
        public double XFirst
        {
            get { return mxFirst; }
            
        }
        public double YFirst
        {
            get { return myFirst; }
            
        }
        public double ZFirst
        {
            get { return mzFirst; }
            
        }
        public double XLast
        {
            get { return mxLast; }
            
        }
        public double YLast
        {
            get { return myLast; }
            
        }
        public double ZLast
        {
            get { return mzLast; }
            
        }
        public Point3D[] CuboidFt
        {
            get { return marrCuboidFt; }
            
        }
        public Point3D[] CuboidBk
        {
            get { return marrCuboidBk; }
            
        }
        public Point3D[] FarPlane
        {
            get { return marrFarPlane; }
            
        }
        public Point3D[] NearPlane
        {
            get { return marrNearPlane; }
            
        }
        public MoldDataHandler MoldDataHandler
        {
            get { return mobjMoldDataHandler; }
           
        }
        public ProcessMoldParams MProcessMoldParams
        {
            get { return mobjProcessMoldParams; }
            
        }
        public double ProximityVal
        {
            get { return mdblProximityVal; }
            
        }
        public ImageHandler ImageHandler
        {
            get { return mobjImageHandler; }
            
        }
        public uint XPixels
        {
            get { return muintXPixels; }
            
        }
        public uint YPixels
        {
            get { return muintYPixels; }
            
        }
        #endregion

        #region constructors
        Params()
        {
            throw new Exception("The parameterless constructor for Params class should not be invoked.");
        }
        public Params(ProcessMoldParams objProcessMoldParams)
        {
            //Get the handle to the data object
            mobjProcessMoldParams = objProcessMoldParams;

            //initialize the MoldDataHandler object
            mobjMoldDataHandler = new MoldDataHandler(mobjProcessMoldParams.MoldData);
            
            mobjMoldDataHandler.RetrieveValues(ref mTargetCuboid, false);

            if (mTargetCuboid.uintXPoints == 0 || mTargetCuboid.uintYPoints == 0 || mTargetCuboid.uintZPoints == 0)
                throw new Exception("One or more of the points set along the three axes is invalid.");
            /* Modified on 28th Jan 2004: Rather than offsetting the points from the corners, now I
               am placing them right from one corner to the other. 
            //calculate the distance between two points on the target cuboid
            dX = (targetCuboid.fltMaxx-targetCuboid.fltMinx) /  ((double)targetCuboid.uintXPoints) ;
            dY = (targetCuboid.fltMaxy-targetCuboid.fltMiny) /  ((double)targetCuboid.uintYPoints) ;
            dZ = (targetCuboid.fltMaxz-targetCuboid.fltMinz) /  ((double)targetCuboid.uintZPoints) ;

            //Find the positions of the first cuboid point along the three axes
            xFirst = targetCuboid.fltMinx + dX / 2.0f;
            yFirst = targetCuboid.fltMiny + dY / 2.0f;
            zFirst = targetCuboid.fltMinz + dZ / 2.0f;

            //Find the positions of the last cuboid point along the three axes
            xLast  = targetCuboid.fltMaxx - dX / 2.0f;
            yLast  = targetCuboid.fltMaxy - dY / 2.0f;
            zLast  = targetCuboid.fltMaxz - dZ / 2.0f;
            */

            //Beginnning of modified code=======================================
            mdX = (mTargetCuboid.fltMaxx - mTargetCuboid.fltMinx) / ((double)(mTargetCuboid.uintXPoints - 1));
            mdY = (mTargetCuboid.fltMaxy - mTargetCuboid.fltMiny) / ((double)(mTargetCuboid.uintYPoints - 1));
            mdZ = (mTargetCuboid.fltMaxz - mTargetCuboid.fltMinz) / ((double)(mTargetCuboid.uintZPoints - 1));

            //Find the positions of the first cuboid point along the three axes
            mxFirst = mTargetCuboid.fltMinx;
            myFirst = mTargetCuboid.fltMiny;
            mzFirst = mTargetCuboid.fltMinz;

            //Find the positions of the last cuboid point along the three axes
            mxLast = mTargetCuboid.fltMaxx;
            myLast = mTargetCuboid.fltMaxy;
            mzLast = mTargetCuboid.fltMaxz;
            //End of modified code==============================================

            //Store the corners of the cuboid in an array
            //We start from the top left in anticlockwise direction
            marrCuboidFt[0] = new Point3D(mTargetCuboid.fltMinx, mTargetCuboid.fltMaxy, mTargetCuboid.fltMaxz);

            marrCuboidFt[1] = new Point3D(mTargetCuboid.fltMinx, mTargetCuboid.fltMiny, mTargetCuboid.fltMaxz);

            marrCuboidFt[2] = new Point3D(mTargetCuboid.fltMaxx, mTargetCuboid.fltMiny, mTargetCuboid.fltMaxz);

            marrCuboidFt[3] = new Point3D(mTargetCuboid.fltMaxx, mTargetCuboid.fltMaxy, mTargetCuboid.fltMaxz);

            marrCuboidBk[0] = new Point3D(mTargetCuboid.fltMinx, mTargetCuboid.fltMaxy, mTargetCuboid.fltMinz);

            marrCuboidBk[1] = new Point3D(mTargetCuboid.fltMinx, mTargetCuboid.fltMiny, mTargetCuboid.fltMinz);

            marrCuboidBk[2] = new Point3D(mTargetCuboid.fltMaxx, mTargetCuboid.fltMiny, mTargetCuboid.fltMinz);

            marrCuboidBk[3] = new Point3D(mTargetCuboid.fltMaxx, mTargetCuboid.fltMaxy, mTargetCuboid.fltMinz);


            //Added on 29-Jan-2004. See top of the file for details.
            mdblProximityVal = CommonFunctions.GetMin(mdX, mdY, mdZ) / Params.mcdblProximityConstant;

            if (!HelperFunctions.SetNearFarRectangle(
                    ref marrFarPlane, ref marrNearPlane, ref mdNear, ref mdFar, ref mblnCamInCuboid,
                    marrCuboidFt, marrCuboidBk,
                    mobjProcessMoldParams.ImageParams.cameraLocation, mobjProcessMoldParams.ImageParams.lookingAt,
                    mobjProcessMoldParams.ImageParams.CameraRatio, mobjProcessMoldParams.ImageParams.CameraAtInfinity,
                    mdblProximityVal))
                throw new Exception("The SetNearFarRectangle function failed");

            mobjImageHandler = new ImageHandler(mobjProcessMoldParams.ImageParams.Image);
            muintXPixels = Convert.ToUInt32(mobjImageHandler.Width);
            muintYPixels = Convert.ToUInt32(mobjImageHandler.Height);
        }
        #endregion
    }
}
