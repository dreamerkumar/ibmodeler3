using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Ajubaa.IBModeler.ImageAlterations
{
    public class ImageResizer
    {
        /// <summary>
        /// Resizes an image using high-quality options.
        /// </summary>
        /// <param name="sourceFileName">Name of the source file.</param>
        /// <param name="destinationFileName">Name of the destination file.</param>
        /// <param name="width">The final width.</param>
        /// <param name="height">The final height.</param>
        public static Bitmap ResizeJpg(string sourceFileName, string destinationFileName, int width, int height)
        {
            using (var drawImg = Image.FromFile(sourceFileName, true))
            {
                var bp = GetResizedBitmap(width, height, drawImg);

                foreach (var pItem in drawImg.PropertyItems)
                {
                    bp.SetPropertyItem(pItem);
                }

                if (String.IsNullOrWhiteSpace(destinationFileName)) return bp;

                var codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo codec = null;
                foreach (var codecInfo   in codecs)
                {
                    if (codecInfo.MimeType.Equals("image/jpeg"))
                    {
                        codec = codecInfo;
                    }
                }

                if (codec != null)
                {
                    var encoderInstance = Encoder.Quality;
                    var encoderParametersInstance = new EncoderParameters(2);
                    var encoderParameterInstance = new EncoderParameter(encoderInstance, 80L);
                    encoderParametersInstance.Param[0] = encoderParameterInstance;
                    encoderInstance = Encoder.ColorDepth;
                    encoderParameterInstance = new EncoderParameter(encoderInstance, 24L);
                    encoderParametersInstance.Param[1] = encoderParameterInstance;
                    try
                    {
                        bp.Save(destinationFileName, codec, encoderParametersInstance);
                    }
                    catch (Exception e)
                    {
                        throw new ApplicationException("Cannot save image to " + destinationFileName + ".", e);
                    }
                }
                else
                {
                    throw new ApplicationException("Unexpected error. Could not find image/jpg codec.");
                }
                return bp;
            }
        }

        /// <summary>
        /// first calculates the resize width and height if required, and then resizes the image based on them
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageAlterationParams"></param>
        /// <returns></returns>
        public static Bitmap ResizeImage(Bitmap image, ImageAlterationParams imageAlterationParams)
        {
            if (imageAlterationParams.ResizeType == ResizeType.ComputeSizeBasedOnPtDensity)
                return GetResizedImage(image, imageAlterationParams.MoldPtDensity);

            if (imageAlterationParams.ResizeType == ResizeType.ResizeSufficiently)
                return GetSufficientlyResizedImage(image);

            if (imageAlterationParams.ResizeType == ResizeType.ToSpecifiedSizes)
                return GetResizedBitmap(imageAlterationParams.SpecificResizeWidth, imageAlterationParams.SpecificResizeHeight, image);

            throw new Exception("No resize type specified or could not handle the specified resize type");
        }

        private static Bitmap GetSufficientlyResizedImage(Bitmap image)
        {
            var minImageDimension = image.Width;

            if (image.Height < minImageDimension)
                minImageDimension = image.Height;

            return GetResizedImage(image, minImageDimension, ptDensity: 500);
        }

        public static Bitmap GetResizedImage(Bitmap image, int resizeFactor)
        {
            var minImageDimension = image.Width;

            if (image.Height < minImageDimension)
                minImageDimension = image.Height;

            return GetResizedImage(image, minImageDimension, resizeFactor);
        }

        private static Bitmap GetResizedImage(Bitmap image, int minImageDimension, int ptDensity)
        {
            //do not resize if sizes vary by less than 50 pixel
            if (minImageDimension - ptDensity <= 50) return image;

            var sourceWidth = image.Width;
            var sourceHeight = image.Height;

            float nPercent;

            var nPercentW = ((float)ptDensity / (float)sourceWidth);
            var nPercentH = ((float)ptDensity / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            image = GetResizedBitmap(destWidth, destHeight, image);
            return image;
        }

        private static Bitmap GetResizedBitmap(int width, int height, Image sourceImage)
        {
            var newSize = new Size(width, height);
            var resizedBitamp = new Bitmap(newSize.Width, newSize.Height);
            using (var g = Graphics.FromImage(resizedBitamp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                var rect = new Rectangle(0, 0, newSize.Width, newSize.Height);
                g.DrawImage(sourceImage, rect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel);
            }
            return resizedBitamp;
        }
    }
}
