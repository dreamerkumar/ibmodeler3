using System;
using System.Drawing;
using System.Windows.Media.Media3D;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.ImageAlterations;
using Ajubaa.IBModeler.ImgBackgroundStripper;

namespace Ajubaa.IBModeler.Processor
{
    public class ImageProcessor
    {
        public static ImageParams GetImageParams(double variationIn3DCoordinates, BackgroundStrippingParams strippingParams, ImageClickInputDetails clickInput, double angle, ImageAlterationParams imageAlterationParams)
        {
            var imageData = GetRotatedCroppedResizedAndStrippedImage(strippingParams, clickInput, imageAlterationParams);

            //get camera ratio
            var cameraRatio = GetCamRatio(imageData, variationIn3DCoordinates);

            var cameraLocationPt = GetCamPos(variationIn3DCoordinates * 4.0, angle, 0.0);
            var lookingAtPt = GetCamDir(angle, 0.0, 0.0);

            return new ImageParams
            {
                InvalidColor = imageAlterationParams.InvalidColor,
                Image = imageData,
                CameraAtInfinity = true,
                cameraLocation = cameraLocationPt,
                lookingAt = lookingAtPt,
                CameraRatio = cameraRatio
            };
        }

        private static Bitmap GetRotatedCroppedResizedAndStrippedImage(BackgroundStrippingParams strippingParams, ImageClickInputDetails clickInput, ImageAlterationParams imageAlterationParams)
        {
            //pickup image from the source location
            var image = (Bitmap)Image.FromFile(String.Format(@"{0}\{1}", imageAlterationParams.ImageFolder, clickInput.ImageName));

            //rotate
            image = MainProcessor.RotateImg(image, (float)clickInput.RotateImageBy, imageAlterationParams.InvalidColor);

            //crop
            image = ImageCropper.GetCroppedImage(clickInput, image, imageAlterationParams);

            //resize
            image = ImageResizer.ResizeImage(image, imageAlterationParams);

            //strip background
            BackgroundStripper.StripBackground(image, strippingParams);

            return image;
        }

        private static CameraRatio GetCamRatio(Image croppedImage, double variationIn3DCoordinates)
        {
            var bigger = croppedImage.Height > croppedImage.Width ? croppedImage.Height : croppedImage.Width;
            var smaller = croppedImage.Height < croppedImage.Width ? croppedImage.Height : croppedImage.Width;

            var biggerRange = variationIn3DCoordinates;
            var smallerRange = (double)smaller * biggerRange / (double)bigger;

            var yRangeAtInfinity = croppedImage.Height > croppedImage.Width ? biggerRange : smallerRange;
            var xRangeAtInfinity = croppedImage.Height < croppedImage.Width ? biggerRange : smallerRange;

            return new CameraRatio { xRangeAtInfinity = xRangeAtInfinity, yRangeAtInfinity = yRangeAtInfinity };
        }

        private static Point3D GetCamPos(double cameraDistance, double angleOfRotation, double cameraYPos)
        {
            return new Point3D 
            {
                X = cameraDistance * Math.Sin(angleOfRotation),
                Y = cameraYPos,
                Z = cameraDistance * Math.Cos(angleOfRotation)
            };
        }

        private static Point3D GetCamDir(double angleOfRotation, double cameraYPos, double cameraDeviation)
        {
            //deviation is in the +ve X direction
            return new Point3D
            {
                X = cameraDeviation * Math.Cos(angleOfRotation),
                Y = cameraYPos,
                Z = -cameraDeviation * Math.Sin(angleOfRotation)
            };
        }
    }
}
