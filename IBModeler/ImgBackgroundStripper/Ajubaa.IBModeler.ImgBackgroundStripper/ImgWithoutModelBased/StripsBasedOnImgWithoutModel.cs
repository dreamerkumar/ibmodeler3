using System;
using Ajubaa.Common;
using System.Drawing;
using System.IO;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    /// <summary>
    /// This is left as work in progress...
    /// At the time of writing this comment I don't remember the original idea behind writing this background stripper.
    /// Testing it didn't give any good results, but still keeping the code just in case I remember the original idea and make use of it
    /// </summary>
    public class StripsBasedOnImgWithoutModel
    {
        public static string StripBackground(string sourceImagePath, byte allowedVariationInR, byte allowedVariationInG, byte allowedVariationInB, Color invalidColor)
        {
            var sourceImgHandler = new ImageHandler(sourceImagePath);

            for (var htCtr = 1; htCtr <= sourceImgHandler.Height; htCtr++)
            {
                for (var wtCtr = 1; wtCtr <= sourceImgHandler.Width; wtCtr++)
                {
                    var baseColorAtLocation = sourceImgHandler.GetPixelAt1BasedIndex(wtCtr, htCtr);
                    var sourceColorAtLocation = sourceImgHandler.GetPixelAt1BasedIndex(wtCtr, htCtr);
                    if(DoesColorMatch(baseColorAtLocation, sourceColorAtLocation, allowedVariationInR, allowedVariationInG, allowedVariationInB))
                    {
                        sourceImgHandler.SetColorForPixel(wtCtr, htCtr, invalidColor);
                    }
                }
            }

            try
            {
                var filePath = sourceImagePath.Replace(@"/", @"\");
                var lastIndex = filePath.LastIndexOf(@"\");
                var fileName = filePath.Substring(lastIndex + 1);
                var dirPath = filePath.Substring(0, lastIndex);
                dirPath += @"\" + "modifiedbyibmodeler3";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var Dir = new DirectoryInfo(dirPath);
                var okToSave = false;
                var ctr = 0;
                while (!okToSave)
                {
                    var checkFileName = string.Format("modified{0}-" + fileName, ctr);
                    var FileList = Dir.GetFiles(checkFileName, SearchOption.TopDirectoryOnly);
                    if (FileList == null || FileList.Length <= 0)
                    {
                        okToSave = true;
                        fileName = checkFileName;
                    }
                    ctr++;
                }
                var completePath = dirPath + @"\" + fileName;
                sourceImgHandler.SaveBitmap(completePath);
                return completePath;
            }
            catch (Exception ex)
            {
                throw new Exception("The following exception occured while trying to get the list of images from the folder path: " + ex.Message);
            }
        }
        private static bool DoesColorMatch(Color baseColorAtLocation, Color sourceColorAtLocation, byte allowedVariationInR, byte allowedVariationInG, byte allowedVariationInB)
        {
            return (IsColorDiffInAllowedRange(baseColorAtLocation.R, sourceColorAtLocation.R, allowedVariationInR)
                && IsColorDiffInAllowedRange(baseColorAtLocation.G, sourceColorAtLocation.G, allowedVariationInG)
                && IsColorDiffInAllowedRange(baseColorAtLocation.B, sourceColorAtLocation.B, allowedVariationInB));
        }
        private static bool IsColorDiffInAllowedRange(byte color1, byte color2, byte allowedRange)
        {
            if (color1 > color2)
            {
                return (color1 - color2 <= allowedRange);
            }
            else if (color2 > color1)
            {
                return (color2 - color1 <= allowedRange);
            }
            else
                return true;
        }

    }
}
