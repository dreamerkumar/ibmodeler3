using System;

namespace Ajubaa.IBModeler.CameraAngleFromImagePoints
{
    /// <summary>
    /// Assumption 1: Users will always rotate the model in the clockwise direction
    /// This will ensure that markers will always move from right to left as we rotate 
    /// the model.

    /// Assumption 2: Angular placement between the markers should not be more than 90 degrees.
    /// In fact, it should be satisfactorily less than 90 degrees so that we are able to identify
    /// a marker within a quadrant. Satisfactorily less to ensure that at least one of the 
    /// identified marker is well within the edges of the quadrant that we are considering.

    /// Assumption 3: While rotating and taking snaps of the model, the degree of rotation 
    /// between two successive snaps should not exceed the angular displacement between the 
    /// markers. This will enable us to identify the markers in the same sequence between two 
    /// successive snaps

    /// Suggestion: Suggest the users to place markers between angles of 45 degrees for best 
    /// results. It is much less than 90 degrees, while at the same time the possibility of 
    /// users rotating the model by more than 45 degrees between successive snaps is rare.

    /// Method of identifying the marker. 

    /// FIRST IMAGE
    /// We start moving from the axis of rotation to the left,
    /// searching for the first marker. We can assume that we have hit a marker when we have moved	
    /// from a Marker region to a PointerBase 

    /// After we identify the position of the marker, use the half width of the pointer region
    /// and the width from our marker position to the position of axis, to determine the angle
    /// of rotation. 
    /// After we have identified the first marker, now start moving to the right from this marker
    /// position to identify the next marker. Soon, this marker has to become the next current  
    /// marker

    /// Using similar rules, find the angle of rotation for the secondary marker and store it
    /// Also store the angular displacement between the primary marker and the secondary marker

    /// SECOND IMAGE ONWARDS
    /// Start moving from the axis of rotation to the left, to identify the first marker. Once we
    /// find it, check if this position is towards the left or right of the previous position of 
    /// our primary marker. 
    /// 
    /// If it is towards the left, then we just calculate the angle of rotation and store the 
    /// new position of our primary marker. Also update the angle of rotation for the secondary
    /// marker using the stored angular displacement between the two 

    /// If it is right at the position where our primary marker was, assume that we haven't 
    /// rotated the model in between the snaps and just skip this image
    /// 
    /// If it is towards the right, that means it is the secondary marker which has moved from 
    /// it's previous position to the left of the axis of rotation.
    /// 
    /// In this case, first find the new angle of rotation for the secondary marker. Using this
    /// information, find out the angular displacement since the last snap.
    /// 
    /// After that, make this secondary marker the primary marker
    /// 
    /// Then use similar method to identify a new secondary marker. 
    /// Calculate and store it's angle of rotation
    /// Also use the above information to calculate and store the angular displacement between
    /// the primary marker and this secondary marker
    /// 
    /// Continue the above process till you have processed all the images  
    /// </summary>
    internal class MarkerProcessor
    {
        private bool _markersInitialized; //Remains false until markers are initialized for the first time

        private double _primaryMarkerAngle; //Angle of rotation for the first marker
        private double _secondaryMarkerAngle; //Angle of rotation for the second marker
        private double _markersAngularDisplacement; // Angle by which the primary marker is ahead of the secondary marker

        // Assume the first photograph to be at 0 degrees from the positive z  axis which is 90 degrees from the right end of the pointer region
        // return all other angles as relative values based on the relative rotation of the markers
        private double _modelAngle; 

        public double[] GetRotationAngles(UserInputForAngleCalculation[] userInputForAngleCalculations)
        {
            var angles = new double[userInputForAngleCalculations.Length];
            for (var ctr = 0; ctr < userInputForAngleCalculations.Length; ctr++)
            {
                var input = userInputForAngleCalculations[ctr];
                angles[ctr] = GetNextAngleOfRotation(input.LeftOfCenterFirstMarkerXPos,
                    input.FirstRightOfFirstMarkerXPos,
                    input.LeftEdgePixelXPos,
                    input.RightEdgePixelXPos );
            }
            return angles;
        }

        internal double GetNextAngleOfRotation(double leftOfCenterFirstMarkerXPos, double firstRightOfFirstMarkerXPos, double discLeftEnd, double discRightEnd)
        {
            //get angle for the first marker
            var firstMarkerAngle = GetAngleOfRotation(discLeftEnd, discRightEnd, leftOfCenterFirstMarkerXPos);

            var setSecondaryMarker = false;
            if (!_markersInitialized)
            {
                //we need to initialize the primary marker
                InitializeMarkers(firstMarkerAngle);
            
                setSecondaryMarker = true;
            }
            else
            {
                if (firstMarkerAngle == _primaryMarkerAngle)
                    throw new Exception("Picture taken without rotating the disc.");

                if (firstMarkerAngle > _primaryMarkerAngle)
                {
                    //our primary marker is good for now and so we just need to set the new angles for the markers
                    //add the relative rotation to the angle of rotation for the model
                    SetNewAnglesForMarkers(firstMarkerAngle);
                }
                else
                {
                    //The position of this marker is towards the right of the primary marker, so this cannot be the primary marker
                    //this time it's time to replace the markers
                    SwitchMarkers(firstMarkerAngle);

                    setSecondaryMarker = true; //we will need to identify a secondary marker
                }
            }
            if(setSecondaryMarker)
                SetSecondaryMarker(discLeftEnd, discRightEnd, leftOfCenterFirstMarkerXPos, firstRightOfFirstMarkerXPos);

            return _modelAngle;
        }

        private void InitializeMarkers(double firstMarkerAngle)
        {
            _primaryMarkerAngle = firstMarkerAngle;
            _modelAngle = 0.0; //assume that the first snap is always taken at zero degrees
            _markersInitialized = true;
        }

        private void SetNewAnglesForMarkers(double firstMarkerAngle)
        {
            var increment = firstMarkerAngle - _primaryMarkerAngle;
            _modelAngle += increment;
            _primaryMarkerAngle = firstMarkerAngle;

            //the secondary marker's value can also be updated using existing values
            _secondaryMarkerAngle = _primaryMarkerAngle - _markersAngularDisplacement;
        }

        private void SwitchMarkers(double firstMarkerAngle)
        {
            _primaryMarkerAngle = firstMarkerAngle;
            var increment = firstMarkerAngle - _secondaryMarkerAngle;
            _modelAngle += increment;
        }

        private void SetSecondaryMarker(double discLeftEnd, double discRightEnd, double leftOfCenterFirstMarkerXPos, double firstRightOfFirstMarkerXPos)
        {
            //find the next marker position towards the right of primary marker
            //intCtr is now incremented by one after initializing the primary marker
            var secMarkerPos = firstRightOfFirstMarkerXPos;

            if (secMarkerPos == leftOfCenterFirstMarkerXPos)
                throw new Exception("could not get a secondary marker after the first one.");

            if (secMarkerPos >= discRightEnd)
                throw new Exception("This is not a valid secondary pointer.");

            //Set the secondary marker angle
            _secondaryMarkerAngle = GetAngleOfRotation(discLeftEnd, discRightEnd, secMarkerPos);

            //Also set the angular displacement
            _markersAngularDisplacement = _primaryMarkerAngle - _secondaryMarkerAngle;
        }

        /// <summary>
        /// Gives the positive angle of rotation in radians
        /// in the clockwise direction with respect to the right 
        /// end of the pointer region
        /// </summary>
        /// <param name="discRightEnd"></param>
        /// <param name="markerPos"></param>
        /// <param name="discLeftEnd"></param>
        /// <returns></returns>
        private static double GetAngleOfRotation(double discLeftEnd, double discRightEnd, double markerPos)
        {
            //pointer width
            var discWidth = discRightEnd - discLeftEnd;

            //position of the center
            var radius = discWidth / 2.0;

            var relativeMarkerPos = markerPos - discLeftEnd;

            var markerPosFromCenter = relativeMarkerPos - radius;

            //use the formula rcosA = MarkerPosFromCenter
            return Math.Acos(markerPosFromCenter / radius);
        }
    }
}
