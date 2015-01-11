using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Ajubaa.IBModeler.Processor
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

                if (string.IsNullOrWhiteSpace(destinationFileName)) return bp;

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

        public static Bitmap GetResizedBitmap(int width, int height, Image sourceImage)
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
