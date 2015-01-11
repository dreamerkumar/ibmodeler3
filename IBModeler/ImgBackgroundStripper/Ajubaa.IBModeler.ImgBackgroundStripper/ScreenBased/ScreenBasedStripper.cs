using System;
using System.Drawing;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    internal static class ScreenBasedStripper
    {
        internal static int Process(Bitmap image, ScreenBasedParams @params, Color backgroundColor)
        {
            var minColorOffset = @params.MinColorOffset;
            var percentMultiplier = ((float)@params.MaxDiffPercent) / 100.0f;

            var pixelsModified = 0;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    var matched = false;
                    switch (@params.ScreenColorTypes)
                    {
                        case ScreenColorTypes.RedScreen:
                            matched = GetMatchedForRedScreen(color, minColorOffset, percentMultiplier);
                            break;
                        case ScreenColorTypes.GreenScreen:
                            matched = GetMatchedForGreenScreen(color, minColorOffset, percentMultiplier);
                            break;
                        case ScreenColorTypes.BlueScreen:
                            matched = GetMatchedForBlueScreen(color, minColorOffset, percentMultiplier);
                            break;
                        case ScreenColorTypes.ExactColor:
                            if (color.B == @params.ExactMatchColor.B
                                && color.G == @params.ExactMatchColor.G
                                && color.R == @params.ExactMatchColor.R)
                                matched = true;
                            break;
                    }
                    if (!matched) continue;
                    image.SetPixel(x, y, backgroundColor);
                    pixelsModified++;
                }
            }
            return pixelsModified;
        }

        private static bool GetMatchedForBlueScreen(Color color, byte minColorOffset, float percentMultiplier)
        {
            var matched = false;
            if (color.B >= minColorOffset 
                && (color.G == 0 || color.B >= (color.G + minColorOffset)) 
                && (color.R == 0 || color.B >= (color.R + minColorOffset)))
            {
                var mainClrOffset = color.G > color.R ? (byte) (color.B - color.G) : (byte) (color.B - color.R);

                var allowedDiff = (int)(percentMultiplier * ((float)mainClrOffset));

                if (Math.Abs(color.R - color.G) <= allowedDiff)
                    matched = true;
            }
            return matched;
        }

        private static bool GetMatchedForGreenScreen(Color color, byte minColorOffset, float percentMultiplier)
        {
            var matched = false;
            if (color.G >= minColorOffset 
                && (color.B == 0 || color.G >= (color.B + minColorOffset)) 
                && (color.R == 0 || color.G >= (color.R + minColorOffset)))
            {
                var mainClrOffset = color.B > color.R ? (byte) (color.G - color.B) : (byte) (color.G - color.R);

                var allowedDiff = (int)(percentMultiplier * ((float)mainClrOffset));

                if (Math.Abs(color.B - color.R) <= allowedDiff)
                    matched = true;
            }
            return matched;
        }

        private static bool GetMatchedForRedScreen(Color color, byte minColorOffset, float percentMultiplier)
        {
            var matched = false;
            if (color.R >= minColorOffset && 
                (color.G == 0 || color.R >= (color.G + minColorOffset)) 
                && (color.B == 0 || color.R >= (color.B + minColorOffset)))
            {
                var mainClrOffset = color.G > color.B ? (byte) (color.R - color.G) : (byte) (color.R - color.B);

                var allowedDiff = (int)(percentMultiplier * ((float)mainClrOffset));
                                
                if (Math.Abs(color.B - color.G) <= allowedDiff)
                    matched = true;
            }
            return matched;
        }
    }
}
