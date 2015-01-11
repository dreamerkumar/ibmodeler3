using System;
using System.Drawing;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    public static class BackgroundStripper
    {
        public static void StripBackground(Bitmap image, BackgroundStrippingParams @params)
        {
            if(@params.ScreenBasedParams != null)
            {
                ScreenBasedStripper.Process(image, @params.ScreenBasedParams, GetColor(@params.BackgroundColor));
            }
            else if(@params.ColorVariationBasedParams != null)
            {
                ColorVariationPercentBasedStripper.Process(image, 
                    @params.ColorVariationBasedParams.VariationPercent,
                    GetColor(@params.BackgroundColor),
                    GetColor(@params.ColorVariationBasedParams.CompareColor));
            }
        }

        private static Color GetColor(SerializableColor backgroundColor)
        {
            if (backgroundColor == null)
                return new Color();
            return Color.FromArgb(backgroundColor.R, backgroundColor.G, backgroundColor.B);
        }
    }
}
