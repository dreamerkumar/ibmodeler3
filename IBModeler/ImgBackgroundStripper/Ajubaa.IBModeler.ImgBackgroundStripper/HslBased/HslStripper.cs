using System;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    public static class HslStripper
    {
        public static int Process(Bitmap image, HslParams @params)
        {
            //Define the color values
            var rVals = new byte[255];
            var gVals = new byte[255];
            var bVals = new byte[255];
            rVals[0] = @params.R;
            gVals[0] = @params.G;
            bVals[0] = @params.B;

            var colorValues = 0;
            byte h;
            byte s;
            byte l;
            Hsl.Rgb2Hsl(rVals[0], gVals[0], bVals[0], out h, out s, out l);
            if (@params.HslSelectionType == HslSelectionType.Sat || @params.HslSelectionType == HslSelectionType.Lum)
            {
                colorValues = SetColorValues(rVals, @params, gVals, bVals, s, l, h);
            }
            //End of defining the color values

            //Loop through the image pixels to set them to the background color
            var pixelsModified = 0;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);

                    var matched = false;
                    switch (@params.HslSelectionType)
                    {
                        case HslSelectionType.CurAndBrighter:
                        case HslSelectionType.CurAndDarker:
                            byte tempH;
                            byte tempS;
                            byte tempL;
                            Hsl.Rgb2Hsl(color.R, color.G, color.B, out tempH, out tempS, out tempL);
                            matched = ((@params.HslSelectionType == HslSelectionType.CurAndBrighter && tempL >= l)
                                       || (@params.HslSelectionType == HslSelectionType.CurAndDarker && tempL <= l));
                            break;
                        case HslSelectionType.Sat:
                        case HslSelectionType.Lum:
                            for (var colorsCtr = 0; colorsCtr <= colorValues; colorsCtr++)
                            {
                                if (Math.Abs(color.R - rVals[colorsCtr]) <= 1 
                                    && Math.Abs(color.G - gVals[colorsCtr]) <= 1 
                                    && Math.Abs(color.B - bVals[colorsCtr]) <= 1)
                                {
                                    matched = true;
                                    break;
                                }
                            }
                            break;
                    }
                    if (!matched) continue;
                    image.SetPixel(x, y, @params.BackgroundColor);
                    pixelsModified++;
                }
            }
            return pixelsModified;
        }

        private static int SetColorValues(byte[] rVals, HslParams classicStripperParams, byte[] gVals, byte[] bVals, byte s, byte l, byte h)
        {
            var colorValues = 0;

            var val1 = classicStripperParams.HslSelectionType == HslSelectionType.Sat ? s : l;
            var val2 = classicStripperParams.LumSatVal;

            if (val1 != val2)
            {
                var ascending = val1 < val2;

                while ((ascending && val1 < val2) || (!ascending && val1 > val2))
                {
                    if (ascending)
                        val1++;
                    else
                        val1--;

                    colorValues++;

                    if (classicStripperParams.HslSelectionType == HslSelectionType.Sat)
                        Hsl.Hsl2Rgb(ref rVals[colorValues], ref gVals[colorValues], ref bVals[colorValues], h, val1, l);
                    else
                        Hsl.Hsl2Rgb(ref rVals[colorValues], ref gVals[colorValues], ref bVals[colorValues], h, s, val1);
                }
            }
            return colorValues;
        }
    }
}

