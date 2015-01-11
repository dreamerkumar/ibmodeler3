using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.ImageAlterations;
using Ajubaa.IBModeler.ImgBackgroundStripper;
using Ajubaa.IBModeler.ImgToObj;
using Ajubaa.IBModeler.PtsToPolygons;
using Ajubaa.SurfaceSmoother.FullModelSmoother;
using Ajubaa.TextureGenerator;

namespace Ajubaa.IBModeler.Processor
{
    public static class MainProcessor
    {
        /// <summary>
        /// if set to true, datalosses will be calculated for the first image after each subsequent image is processed
        /// </summary>
        public static bool AnalyzeMoldForDataLosses { get; set; }
        
        /// <summary>
        /// used to calculate data loss for the first image after each subsequent image is processed
        /// the way to use this property is rather dirty. applyimage function has the onus to set the value of this property
        /// but it is upto the calling componenents to dispose off this property after it is not in use anymore
        /// </summary>
        public static ImageParams FirstImageParams { get; set; }

        public static ModelMeshAndTexture CreateDefaultModel(CreateModelContract inputParams)
        {
            var logger = new Logger(inputParams.LogFilePath);

            Stream moldDataStream;
            ApplyImages(inputParams, out moldDataStream);

            var createModelInfo = new CreateModelInfo
            {
                MoldData = moldDataStream,
                Minx = 1, Maxx = inputParams.PtDensity,
                Miny = 1, Maxy = inputParams.PtDensity,
                Minz = 1, Maxz = inputParams.PtDensity
            };

            logger.Log("Start model creation from mold points");
            var ptsToPolygons = new PointsToPolygons(createModelInfo);
            var model = ptsToPolygons.Process();
            logger.Log("End model creation from mold points.");

            if (moldDataStream != null)
                moldDataStream.Close();

            if (inputParams.SmoothingIterationCount > 0)
            {
                logger.Log(string.Format("Start smoothening {0} times", inputParams.SmoothingIterationCount));
                model.Positions = PaulBourkeSmoother.GetSmoothenedPositions(model.Positions, model.TriangleIndices,
                                                                           inputParams.SmoothingIterationCount);
                logger.Log("End smoothening.");
            }

            var addTextureInfo = GetAddTextureInfoForFrontAndBackImage(inputParams);

            var texture = TextureProcessor.GenerateTexture(addTextureInfo, model, inputParams.LogFilePath);
            
            logger.Log("Returning model mesh and texture");

            if (texture != null) model.TextureCoordinates = texture.TextureCoordinates;
            return new ModelMeshAndTexture
            {
                MeshGeometry = model, TextureBitmap = (texture == null? null : texture.Bitmap)
            };
        }
        
        public static AddTextureInfo? GetAddTextureInfoForFrontAndBackImage(CreateMeshContract inputParams)
        {
            var logger = new Logger(inputParams.LogFilePath);
            logger.Log("Start get add texture info");

            var angles = inputParams.ClickInputs.Angles;
            if (angles == null || angles.Length < 2) return null;

            var indexOfBackImage = GetIndexOfAngleClosestToBackOfModel(angles);
            if (indexOfBackImage == null || indexOfBackImage.Value <= 0) return null;

            var indexCollection = new[] { 0, indexOfBackImage.Value};
            
            var addTextureInfo = GetAddTextureInfoForIndexCollection(inputParams, indexCollection);

            logger.Log("End get add texture info");

            return addTextureInfo;
        }

        public static int[] GetIndicesForFrontAndBackTexture(double[] angles)
        {
            if (angles == null || angles.Length < 2) return null;
            var indexOfBackImage = GetIndexOfAngleClosestToBackOfModel(angles);
            if (!indexOfBackImage.HasValue) return null;
            return new[] {0, indexOfBackImage.Value};
        }

        public static int[] GetIndicesFor4CornerTexture(double[] angles)
        {
            const double variationLimit = Math.PI/4.0;

            var desiredAngles = new List<double> {0, Math.PI/2.0, Math.PI, 3.0*Math.PI/2.0};

            return (from desiredAngle in desiredAngles select GetIndexOfClosestAngle(angles, desiredAngle, variationLimit) 
                    into index where index != null select index.Value).ToArray();
        }

        public static int[] GetIndicesFor8CornerTexture(double[] angles)
        {
            const double variationLimit = Math.PI / 8.0;

            var desiredAngles = new List<double> { 0, Math.PI/4.0, Math.PI/2.0, 3*Math.PI/4.0, Math.PI, 5*Math.PI/4.0, 3*Math.PI/2.0, 7*Math.PI/4.0 };

            return (from desiredAngle in desiredAngles select GetIndexOfClosestAngle(angles, desiredAngle, variationLimit)
                        into index where index != null select index.Value).ToArray();
        }

        public static AddTextureInfo GetAddTextureInfoForIndexCollection(CreateMeshContract inputParams, ICollection<int> indexCollection)
        {
            var logger = new Logger(inputParams.LogFilePath);
            logger.Log("Start GetAddTextureInfoForIndexCollection");

            var angles = inputParams.ClickInputs.Angles;
            var addTextureInfo = new AddTextureInfo
            {
                ImageInfos = new AddTexImageInfo[indexCollection.Count]
            };

            var imageAlterationParams = new ImageAlterationParams
            {
                MoldPtDensity = inputParams.PtDensity,
                MinImageHeightRatio = inputParams.MinImageHeightRatio,
                PercentExtraWidth = inputParams.PercentExtraWidth,
                ImageFolder = inputParams.ImageFolder,
                InvalidColor = inputParams.InvalidColor,
                ResizeType = ResizeType.ResizeSufficiently,
                BottomPaddingPercent = inputParams.BottomPaddingPercent
            };

            var ctr = 0;
            foreach (var index in indexCollection)
            {
                var clickInput = inputParams.ClickInputs.ImageClickInputDetailsList[index];
                var angle = angles[index];
                
                var imageParams = ImageProcessor.GetImageParams(inputParams.VariationIn3DCoordinates, inputParams.BackgroundStrippingParams, clickInput, angle, imageAlterationParams);

                addTextureInfo.ImageInfos[ctr] = new AddTexImageInfo { CameraLocation = imageParams.cameraLocation, ImageBitmap = imageParams.Image, LookingAt = imageParams.lookingAt };
                if (ctr == 0)
                {
                    //set the camera ratio
                    addTextureInfo.CameraRatio = new TextureGenerator.CameraRatio { XRangeAtInfinity = imageParams.CameraRatio.xRangeAtInfinity, YRangeAtInfinity = imageParams.CameraRatio.yRangeAtInfinity };
                }

                //make sure the rest of the images are resized to the same size as the first one so that all texture images are of the same ht
                imageAlterationParams.ResizeType = ResizeType.ToSpecifiedSizes;
                imageAlterationParams.SpecificResizeHeight = imageParams.Image.Height;
                imageAlterationParams.SpecificResizeWidth = imageParams.Image.Width;

                ctr++;
            }
            logger.Log("End GetAddTextureInfoForIndexCollection");
            return addTextureInfo;
        }

        public static void ApplyImages(CreateMeshContract inputParams, out Stream moldData)
        {
            var logger = new Logger(inputParams.LogFilePath);

            var angles = inputParams.ClickInputs.Angles;

            Stream moldDataPtr = null;
            for (var index = 0; index < angles.Length; index++)
            {
                ApplyImage(inputParams, index, ref moldDataPtr, logger);
            }
            moldData = moldDataPtr;
        }

        public static void ApplyImage(CreateMeshContract inputParams, int index, ref Stream moldDataPtr, Logger logger)
        {
            var clickInput = inputParams.ClickInputs.ImageClickInputDetailsList[index];
            var angle = inputParams.ClickInputs.Angles[index];

            var imageAlterationParameters = new ImageAlterationParams
            {
                MoldPtDensity = inputParams.PtDensity,
                MinImageHeightRatio = inputParams.MinImageHeightRatio,
                PercentExtraWidth = inputParams.PercentExtraWidth,
                ImageFolder = inputParams.ImageFolder,
                InvalidColor = inputParams.InvalidColor,
                ResizeType = ResizeType.ComputeSizeBasedOnPtDensity,
                BottomPaddingPercent = inputParams.BottomPaddingPercent
            };
            var imageParams = ImageProcessor.GetImageParams(inputParams.VariationIn3DCoordinates, inputParams.BackgroundStrippingParams, clickInput, angle, imageAlterationParameters);

            if (moldDataPtr == null)
                moldDataPtr = CreateMoldDataStream(imageParams.Image.Width, imageParams.Image.Height, (float)inputParams.VariationIn3DCoordinates, inputParams.PtDensity, logger);

            var processMoldParams = new ProcessMoldParams
            {
                MoldData = moldDataPtr,
                ImageParams = imageParams
            };
            var imagesToObject = new ImagesToObject(processMoldParams, inputParams.LogFilePath);
            imagesToObject.ProcessImage();

            #region track_changes_for_troubleshooting

            if (inputParams.SaveProcessedImages)
                SaveProcessedImages(processMoldParams.ImageParams.Image, inputParams.ImageFolder, "ibmodeler3_modified_images", index, angle, logger);

            var imageParamsToBeReused = false;
            if (AnalyzeMoldForDataLosses)
            {
                //if first time, just set values for subsequent use
                if(index == 0)
                {
                    FirstImageParams = imageParams;
                    imageParamsToBeReused = true;
                }
                else
                {
                    var processMoldParamsForAnalysis = new ProcessMoldParams
                    {
                        MoldData = moldDataPtr,
                        ImageParams = FirstImageParams,
                        SetColorForRetainedPixels = false
                    };
                    int pixelsWithAllPointsLost = MoldFileAnalyzerForImg.ProcessImage(processMoldParamsForAnalysis);
                    if (pixelsWithAllPointsLost > 0)
                    {
                        SaveDataLossImages(FirstImageParams.Image, inputParams.ImageFolder, "ibmodeler3_data_loss_analysis", index, angle, pixelsWithAllPointsLost, logger);
                    }
                }
            }

            #endregion

            //this will dispose off the image data from memory
            if(!imageParamsToBeReused) imageParams.Dispose();

            logger.Log(string.Format("Image applied for camera at an angle of {0} degrees", Math.Round(angle * 180 / Math.PI, 2)));
        }
    
        private static Stream CreateMoldDataStream(double width, double height, float variationIn3DCoordinates, int ptDensity, Logger logger)
        {
            float xAndZVariation;
            float yVariation;
            if (width > height)
            {
                xAndZVariation = variationIn3DCoordinates;
                yVariation = ((float)height) * variationIn3DCoordinates / ((float)(width));
            }
            else if (height > width)
            {
                yVariation = variationIn3DCoordinates;
                xAndZVariation = ((float)width) * variationIn3DCoordinates / ((float)(height));
            }
            else
            {
                xAndZVariation = yVariation = variationIn3DCoordinates;
            }

            var moldData = MoldFileCreator.CreateNewMoldFile(ptDensity, xAndZVariation, yVariation, xAndZVariation);
            logger.Log("Mold created.");
            return moldData;
        }

        public static int? GetIndexOfAngleClosestToBackOfModel(double[] angles)
        {
            const double angularVariationLimit = Math.PI/2.0;
            const double perfectAngle = Math.PI;

            return GetIndexOfClosestAngle(angles, perfectAngle, angularVariationLimit);
        }

        /// <summary>
        /// given a desired angle, gives the index of the angle which is closest to that value
        /// If there is no entry which is that close, then returns null
        /// handles the case of angle values greater than 2PI as well
        /// </summary>
        /// <param name="angles"></param>
        /// <param name="desiredAngle"></param>
        /// <param name="angularVariationLimit"></param>
        /// <returns></returns>
        private static int? GetIndexOfClosestAngle(IList<double> angles, double desiredAngle, double angularVariationLimit)
        {
            const double resetValue = 2*Math.PI;

            double? closestAngle = null;
            int? indexOfClosestAngle = null;

            //Loop through the list of images to find the image that is at an angle closest to the above value
            for (var ctr = 0; ctr < angles.Count; ctr++) //Start from the second image
            {
                var currentAngle = angles[ctr];

                while (currentAngle > resetValue)
                {
                    currentAngle -= resetValue;
                }

                var angularDisplacement = Math.Abs(desiredAngle - currentAngle);

                if(closestAngle == null || closestAngle.Value > angularDisplacement)
                {
                    closestAngle = angularDisplacement;
                    indexOfClosestAngle = ctr;
                }
            }

            //Cannot consider the angle if it is not within limits
            return closestAngle == null || closestAngle.Value > angularVariationLimit ? null : indexOfClosestAngle;
        }

        public static ClickInputs GetClickInputsFromFile(string filePathToClicksInput)
        {
            var serializer = new XmlSerializer(typeof(ClickInputs));
            var tr = new StreamReader(filePathToClicksInput);
            var clickInputs = (ClickInputs)serializer.Deserialize(tr);
            tr.Close();
            return clickInputs;
        }

        /// <summary>
        /// The width of the object can be more than the diameter of the base disc
        /// In such scenarios, we will need to keep some extra padding on each side of disc
        /// while cropping the image. This function calculates the amount by which we should pad.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static double GetExtraPadding(CreateMeshContract contract)
        {
            var maxExtraPaddingPercent = 0.0;
            var bottomPaddingPercent = contract.BottomPaddingPercent;
            //for each image
            foreach (var clickInput in contract.ClickInputs.ImageClickInputDetailsList)
            {
                var imageName = clickInput.ImageName;

                var bitmapImg = (Bitmap)Image.FromFile(string.Format(@"{0}\{1}", contract.ImageFolder, imageName));

                //resize to 100 (small size)
                const int newWidth = 100;
                var newHeight = newWidth * bitmapImg.Height / bitmapImg.Width;

                var imageAlterationParams = new ImageAlterationParams
                {
                    ResizeType = ResizeType.ToSpecifiedSizes,
                    SpecificResizeHeight = newHeight,
                    SpecificResizeWidth = newWidth
                };

                var resizedImage = ImageResizer.ResizeImage(bitmapImg, imageAlterationParams);

                //strip background
                BackgroundStripper.StripBackground(resizedImage, contract.BackgroundStrippingParams);

                //get actual borders for this image
                var origImageCorners = ImageCorners.GetImageCornersFromClickInputs(clickInput, contract.MinImageHeightRatio, bottomPaddingPercent);
                var actualImageCorners = ImageCorners.GetActualImageCorners(origImageCorners, resizedImage, clickInput.ClickPositionListForImages);

                //calculate extra padding percent
                var padding = ExtraPadding.GetExtraPaddingPercent(resizedImage, actualImageCorners,
                                                                  contract.InvalidColor);

                //check if it is more than the maximum so far
                if (maxExtraPaddingPercent < padding)
                    maxExtraPaddingPercent = padding;
            }
            return maxExtraPaddingPercent;
        }

        public static Bitmap ResizeImage(Bitmap image, ImageAlterationParams imageAlterationParams)
        {
            return ImageResizer.ResizeImage(image, imageAlterationParams);
        }

        public static void StripBackground(Bitmap bitmap, BackgroundStrippingParams @params)
        {
            BackgroundStripper.StripBackground(bitmap, @params);
        }

        public static Bitmap RotateImg(Bitmap origImg, float angle, Color bkColor)
        {
            return ImageRotator.RotateImg(origImg, angle, bkColor);
        }

        public static float GetRotationAngleToRealign(ClickPositionOnImage leftOfDisc, ClickPositionOnImage rightOfDisc)
        {
            return ImageRotator.GetRotationAngleToRealign(leftOfDisc, rightOfDisc);
        }

        public static Bitmap ResizeJpg(string sourceFileName, string destinationFileName, int width, int height)
        {
            return ImageResizer.ResizeJpg(sourceFileName, destinationFileName, width, height);
        }

        #region helper functions for track_changes_for_troubleshooting
        private static void SaveDataLossImages(Image image, string folderName, string subFolderName, int imgIndex, double angleInRadian, int pixelsLostCount, Logger logger)
        {
            try
            {
                var imageFolderPath = GetCreatedFolderPath(folderName, subFolderName);
                var imagePath = string.Format(@"{0}\Image_{1}_Angle_{2}_Pixels_Lost_{3}.jpg", imageFolderPath, imgIndex+1, Math.Round(angleInRadian * 180 / Math.PI, 0), pixelsLostCount);
                image.Save(imagePath);
            }
            catch (Exception exception)
            {
                logger.Log(string.Format("Following error occured while trying to save the image for tracking data loss: {0}", exception));
            }
        }

        private static void SaveProcessedImages(Image image, string folderName, string subFolderName, int imgIndex, double angleInRadian, Logger logger)
        {
            try
            {
                var imageFolderPath = GetCreatedFolderPath(folderName, subFolderName);
                var imagePath = string.Format(@"{0}\Image_{1}_Angle_{2}.jpg", imageFolderPath, imgIndex+1, Math.Round(angleInRadian * 180 / Math.PI, 0));
                image.Save(imagePath);
            }
            catch (Exception exception)
            {
                logger.Log(string.Format("Following error occured while trying to save the image for tracking background stripping: {0}", exception));
            }
        }

        private static string GetCreatedFolderPath(string folderName, string subFolderName)
        {
            var imageFolderPath = string.Format(@"{0}\{1}", folderName, subFolderName);
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }
            return imageFolderPath;
        }
        #endregion

    }
}
