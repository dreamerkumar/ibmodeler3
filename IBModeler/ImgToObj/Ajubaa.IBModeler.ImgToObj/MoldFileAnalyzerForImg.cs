using System;
using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;

namespace Ajubaa.IBModeler.ImgToObj
{
    public static class MoldFileAnalyzerForImg
    {
        public static int ProcessImage(ProcessMoldParams moldParams) 
        {
            var pixelsWithAllPointsLost = 0;
            var parameters = new Params(moldParams);
	        for(var j = 1; j<= parameters.YPixels; j++)
	        {
	            for(var i = 1; i <= parameters.XPixels; i++) 
                {		
                    var isSameRgbColor = IsSameRgbColor(i, j, parameters.MProcessMoldParams.ImageParams.InvalidColor, parameters.ImageHandler);
                    if (isSameRgbColor) continue;

                    var blnAValidPtFound = false;
                    if (!HelperFunctions.ProcessImgPixelPositions(parameters, (uint)i, (uint)i, (uint)j, MoldActionType.CheckForAllPtsSetToInvalid, ref blnAValidPtFound))
                    {
                        return -1;
                    }

                    if (!blnAValidPtFound)
                    {
                        pixelsWithAllPointsLost++;
                        parameters.ImageHandler.SetLostDataColorForPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                    }
                    else if(parameters.MProcessMoldParams.SetColorForRetainedPixels)
                        parameters.ImageHandler.SetRetainedDataColorForPixel(Convert.ToInt32(i), Convert.ToInt32(j));
                } 		        
	        } 

            return pixelsWithAllPointsLost;
        }

        private static bool IsSameRgbColor(int i, int j, Color invalidColor, ImageHandler imageHandler)
        {
            var currentClr = imageHandler.GetPixelAt1BasedIndex(Convert.ToInt32(i), Convert.ToInt32(j));

            return CommonFunctions.IsSameRgbColor(currentClr, invalidColor);
        }
    }
}
