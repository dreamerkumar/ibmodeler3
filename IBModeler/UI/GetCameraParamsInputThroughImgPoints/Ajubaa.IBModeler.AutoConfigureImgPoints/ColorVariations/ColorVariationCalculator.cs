using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public static class ColorVariationCalculator
    {
        public static List<double> GetColorVariationList(Bitmap image, int x1, int x2, int y)
        {
            var prevColor = GetStartingColorToMatch(x1, image, y);

            var variationList = new List<double>();

            for (var x = x1; x <= x2; x++)
            {
                var currColor = image.GetPixel(x, y);

                var variationR = Math.Abs(currColor.R - prevColor.R) / 255.0;
                var variationG = Math.Abs(currColor.G - prevColor.G) / 255.0;
                var variationB = Math.Abs(currColor.B - prevColor.B) / 255.0;

                var netVariation = (variationR + variationG + variationB) / 3.0;

                variationList.Add(netVariation);

                prevColor = currColor;
            }

            return variationList;
        }

        public static List<double> GetColorVariationFromASinglePtList(Bitmap image, int x1, int x2, int y, int compareWithX)
        {
            var compareColor = image.GetPixel(compareWithX, y);

            var variationList = new List<double>();

            for(var x = x1; x <= x2; x++)
            {
                var currColor = image.GetPixel(x, y);

                var variationR = Math.Abs(currColor.R - compareColor.R)/255.0;
                var variationG = Math.Abs(currColor.G - compareColor.G) / 255.0;
                var variationB = Math.Abs(currColor.B - compareColor.B)/255.0;

                var netVariation = (variationR + variationG + variationB)/3.0;

                variationList.Add(netVariation);
            }

            return variationList;
        }

        private static Color GetStartingColorToMatch(int x1, Bitmap image, int y)
        {
            Color prevColor;
            if(x1 == 0)
            {
                prevColor = image.GetPixel(x1, y);
                
            }
            else if(x1 > 0)
            {
                prevColor = image.GetPixel(x1 - 1, y);
            }
            else
            {
                throw new Exception("x1 should be 0 or greater.");
            }
            return prevColor;
        }
    }
}
