namespace Ajubaa.IBModeler.CameraAngleFromImagePoints
{
    public static class AngleProcessor
    {
        public static double[] GetRotationAngles(UserInputForAngleCalculation[] userInputForAngleCalculations)
        {
            var markerProcessor = new MarkerProcessor();

            var angles = new double[userInputForAngleCalculations.Length];
            for (var ctr = 0; ctr < userInputForAngleCalculations.Length; ctr++)
            {
                var input = userInputForAngleCalculations[ctr];

                angles[ctr] = markerProcessor.GetNextAngleOfRotation(
                    input.LeftOfCenterFirstMarkerXPos,
                    input.FirstRightOfFirstMarkerXPos,
                    input.LeftEdgePixelXPos,
                    input.RightEdgePixelXPos);
            }
            return angles;
        }
    }
}
