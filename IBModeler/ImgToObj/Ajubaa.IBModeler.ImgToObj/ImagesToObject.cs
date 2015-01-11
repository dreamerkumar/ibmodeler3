using System;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgToObj
{
    public class ImagesToObject
    {
        private Params mp = null;
        private Logger _logger;
        public ImagesToObject()
        {
            throw new Exception("the parameterless constructor for ImagesToObject should not be invoked");
        }
        public ImagesToObject(ProcessMoldParams objProcessMoldParams, string logFilePath) 
        {
	        mp = new Params(objProcessMoldParams);
            _logger = new Logger(logFilePath);
        }
 
        public bool ProcessImage() 
        {
	        uint i, j; //counter variables
            uint uintPixelsProcessed = 0;
            uint invalidPixelCount = 0;
            for( j = 1 ; j<= mp.YPixels; j++) 
            {
                uint recLength = 0; //length of an invalid rectangle in pixels                
                for(i = 1 ; i <= mp.XPixels; i++) 
                {		
                    uintPixelsProcessed++;
        		
                    var currentClr = mp.ImageHandler.GetPixelAt1BasedIndex(Convert.ToInt32(i), Convert.ToInt32(j));    			
        			var blnIsSameRGBColor = CommonFunctions.IsSameRgbColor(currentClr,mp.MProcessMoldParams.ImageParams.InvalidColor);

                    if (blnIsSameRGBColor)
                    {
                        recLength++; //increase the length of the invalid rectangle if the current pixel is invalid	
                        invalidPixelCount++;
                    }
                    if(recLength > 0 ) 
                    {
                        if (!blnIsSameRGBColor) 
                        {//no more invalid pixel in the sequence
                            if (!HelperFunctions.SetInvalidPtsForImgPixelPositions(mp, i - 1 - recLength, i - 1, j))
                                return false;
                                //i-recLength to move to the start of invalid rectangle and -1 because
						        //right now we have moved one pixel ahead of the invalid rectangle

                            recLength = 0 ; 
				        } 
                        else if ( i == mp.XPixels) 
                        {//boundary is reached
                            if (!HelperFunctions.SetInvalidPtsForImgPixelPositions(mp, i - recLength, i, j))
                                return false;
                                recLength = 0 ; 
				        }
			        }
		        } //end of a horizontal line		        

	        }//end of all pixels 
            _logger.Log(string.Format("A total of {0} invaild pixels were found in processing the image with {1} pixels.", invalidPixelCount, uintPixelsProcessed));
        	
        	
	        //Update the mold file with the new number of files processed. 
	        mp.MoldDataHandler.UpdateMoldFileWithNewImg();
        	
	        return true;
        }
     }
}
