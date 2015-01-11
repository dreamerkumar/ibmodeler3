using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Ajubaa.IBModeler.CameraAngleFromImagePoints;
using Ajubaa.IBModeler.Common;


namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints
{
    public delegate void AllImgProcessedEventHandler(object sender, AllImgProcessedEventArgs args);
    public delegate void AutoConfigureRequestedEventHandler(object sender, EventArgs args);

    public partial class ImgListHandler
    {
        public List<ImageClickInputDetails> ImageClickInputDetails;
        public string[] FilePaths;

        public event AllImgProcessedEventHandler AllImgProcessedEvent;
        public event AutoConfigureRequestedEventHandler AutoConfigureRequestedEvent;

        private int _processingImgIndex;

        public ImgListHandler()
        {
            InitializeComponent();
            IndividualImgHandler.IndividualImgProcessedEvent += IndividualImgProcessed;
            IndividualImgHandler.AutoConfigureRequestedEvent += InvokeAutoConfigureRequested;
            IndividualImgHandler.ProcessPreviousImgEvent += IndividualImgHandler_ProcessPreviousImgEvent;
        }

        private void InvokeAutoConfigureRequested(object sender, EventArgs args)
        {
            var handler = AutoConfigureRequestedEvent;
            if (handler != null) handler(this, args); 
        }

        public void GetUserInputForImages(string imageDirPath, ClickInputs existingInput)
        {
            ImageClickInputDetails = new List<ImageClickInputDetails>();

            if (existingInput != null && existingInput.ImageClickInputDetailsList != null)
                ImageClickInputDetails = DeepClone(existingInput.ImageClickInputDetailsList) as List<ImageClickInputDetails>;
            else
                ImageClickInputDetails = new List<ImageClickInputDetails>();

            FilePaths = GetImageFilesHelper.GetImageFilesFromLocation(imageDirPath);
            ProcessImage(0);
        }

        internal void IndividualImgProcessed(object sender, IndividualImgProcessedEventArgs args)
        {
            if (_processingImgIndex == ImageClickInputDetails.Count)
            {
                var clickInputDetails = new ImageClickInputDetails
                {
                    ClickPositionListForImages = args.ClickPositions, 
                    ImageName = args.ImageName,
                    RotateImageBy = args.RotateImageBy
                };
                ImageClickInputDetails.Add(clickInputDetails);
            }
            else if(_processingImgIndex < ImageClickInputDetails.Count)
            {
                ImageClickInputDetails[_processingImgIndex].ClickPositionListForImages = args.ClickPositions;
                ImageClickInputDetails[_processingImgIndex].ImageName = args.ImageName;
                ImageClickInputDetails[_processingImgIndex].RotateImageBy = args.RotateImageBy;
            }
            else throw new Exception("Error in processing next image logic.");

            if (_processingImgIndex < FilePaths.Length - 1)
            {
                ProcessNextImage();
            }
            else
            {
                CalculateAnglesAndReturn();
            }
        }

        private void ProcessNextImage()
        {
            var imgIndex = _processingImgIndex + 1;
            if(imgIndex >= FilePaths.Count()) throw  new Exception("Cannot go beyond the last image");

            ProcessImage(imgIndex);
        }

        private void IndividualImgHandler_ProcessPreviousImgEvent(object sender, EventArgs args)
        {
            if (_processingImgIndex <= 0) throw new Exception("No previous image to process.");

            var imgIndex = _processingImgIndex - 1;

            ProcessImage(imgIndex);
        }

        private void ProcessImage(int imgIndex)
        {
            List<ClickPositionOnImage> imgPositions = null;
            var rotateImageBy = 0.0;

            var nextImageName = Path.GetFileName(FilePaths[imgIndex]);
            var inputForNextImage = ImageClickInputDetails.FirstOrDefault(x => x.ImageName == nextImageName);
            if(inputForNextImage != null)
            {
                imgPositions = inputForNextImage.ClickPositionListForImages;
                rotateImageBy = inputForNextImage.RotateImageBy;
            }
            
            _processingImgIndex = imgIndex;
            IndividualImgHandler.ProcessImage(FilePaths[imgIndex], imgPositions, rotateImageBy, imgIndex+1, FilePaths.Count());
        }

        private void CalculateAnglesAndReturn()
        {
            if(AllImgProcessedEvent != null)
                AllImgProcessedEvent(this,
                new AllImgProcessedEventArgs
                {
                    ClickInputs = new ClickInputs{ImageClickInputDetailsList = ImageClickInputDetails, 
                        Angles = CalculateAngles(ImageClickInputDetails)}
                });
        }

        private static double[] CalculateAngles(IEnumerable<ImageClickInputDetails> imagePositionLists)
        {
            var userInputs = new List<UserInputForAngleCalculation>();
            foreach (var position in imagePositionLists)
            {
                var userInput = new UserInputForAngleCalculation();
                foreach (var markerPosition in position.ClickPositionListForImages)
                {
                    switch (markerPosition.PositionType)
                    {
                        case ClickPositionOnImageTypes.None:
                            break;
                        case ClickPositionOnImageTypes.LeftEndOfRotatingDisc:
                            userInput.LeftEdgePixelXPos = markerPosition.ClickXPos;
                            break;
                        case ClickPositionOnImageTypes.RightEndOfRotatingDisc:
                            userInput.RightEdgePixelXPos = markerPosition.ClickXPos;
                            break;
                        case ClickPositionOnImageTypes.BottomMostPartOfModel:
                            break;
                        case ClickPositionOnImageTypes.MarkerLeftFromCenter:
                            userInput.LeftOfCenterFirstMarkerXPos = markerPosition.ClickXPos;
                            break;
                        case ClickPositionOnImageTypes.MarkerRightFromCenter:
                            userInput.FirstRightOfFirstMarkerXPos = markerPosition.ClickXPos;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                userInputs.Add(userInput);
            }
            return AngleProcessor.GetRotationAngles(userInputs.ToArray());
        }
        
        /// <summary>
        /// quick and dirty way to create copies of objects
        /// passed objects need to be serializable
        /// http://stackoverflow.com/questions/222598/how-do-i-clone-a-generic-list-in-c
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepClone(object obj)  
        { 
          object objResult = null; 
          using (var  ms = new MemoryStream()) 
          { 
            var  bf =   new BinaryFormatter(); 
            bf.Serialize(ms, obj); 
 
            ms.Position = 0; 
            objResult = bf.Deserialize(ms); 
          } 
          return objResult; 
        } 
    }
}
