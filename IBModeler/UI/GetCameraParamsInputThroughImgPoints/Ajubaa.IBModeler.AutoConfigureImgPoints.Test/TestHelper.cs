using System;
using System.Drawing;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.Processor;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints.Test
{
    public static class TestHelper
    {
        internal static Color GetBackgroundColor()
        {
            return Color.FromArgb(200, 200, 200);
        }

        private static SerializableColor GetSerializableColor(Color color)
        {
            return new SerializableColor(color.R, color.G, color.B);
        }

        internal static Bitmap GetStrippedFirstMarioImage(Color backgroundColor)
        {
            var filePath = String.Format(@"{0}\mario\IMG_0944.JPG", ExecutionDirInfoHelper.GetInputDirPath());
            return GetStrippedImage(filePath);
        }

        internal static Bitmap GetStrippedTestImage()
        {
            var filePath = String.Format(@"{0}\TestImage.png", ExecutionDirInfoHelper.GetInputDirPath());
            return GetStrippedImage(filePath);
        }

        internal static Bitmap GetStrippedTestImageForPerfectlyVerticalBase(Color backgroundColor)
        {
            var filePath = String.Format(@"{0}\TestImage1.png", ExecutionDirInfoHelper.GetInputDirPath());
            return GetStrippedImage(filePath);
        }

        private static Bitmap GetStrippedImage(string filePath)
        {
            var image = (Bitmap)Image.FromFile(filePath);
            var strippingParams = GetScreenBasedParamsForFirstMarioImage();
            MainProcessor.StripBackground(image, strippingParams);
            return image;
        }

        internal static BackgroundStrippingParams GetScreenBasedParamsForFirstMarioImage()
        {
            return new BackgroundStrippingParams
            {
                ScreenBasedParams = new ScreenBasedParams
                {
                    MaxDiffPercent = 100,
                    MinColorOffset = 56,
                    ScreenColorTypes = ScreenColorTypes.GreenScreen
                },
                BackgroundColor = GetSerializableColor(GetBackgroundColor())
            };
        }
    }
}
