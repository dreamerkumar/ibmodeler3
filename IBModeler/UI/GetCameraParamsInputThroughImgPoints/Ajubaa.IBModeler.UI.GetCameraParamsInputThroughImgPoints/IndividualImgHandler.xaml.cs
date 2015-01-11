using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Processor;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;

namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints
{
    internal delegate void IndividualImgProcessedEventHandler(object sender, IndividualImgProcessedEventArgs args);

    internal delegate void ProcessPreviousImgEventHandler(object sender, EventArgs args);

    internal delegate void IIHAutoConfigureRequestedEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for IndividualImgHandler.xaml
    /// </summary>
    internal partial class IndividualImgHandler
    {
        private Bitmap _currentImage;
        private Point _leftEdgePositionBeforeRotation;
        private readonly System.Drawing.Color _bkColor = System.Drawing.Color.FromArgb(200, 200, 200);
        private double _rotationAngleToRealign;
        private string _imageName;

        internal event IndividualImgProcessedEventHandler IndividualImgProcessedEvent;
        internal event IIHAutoConfigureRequestedEventHandler AutoConfigureRequestedEvent;
        internal event ProcessPreviousImgEventHandler ProcessPreviousImgEvent;

        public ClickPositionOnImageTypes CurrentClickPositionOnImageType { get; set; }
        public List<ClickPositionOnImage> ClickSequences { get; set; }

        public IndividualImgHandler()
        {
            InitializeComponent();
            CurrentClickPositionOnImageType = ClickPositionOnImageTypes.None;
            
            ImageCanvas.MouseDown += OnMouseDown;
            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            ParentCanvas.Background = CreateBackgroundBrush();
            
            // Create a LinearGradientBrush for status
            var linearGradientBrush = new LinearGradientBrush();
            linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb( 102, 141, 170), 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 17, 27), 1.0));

            StatusPanel.Background = linearGradientBrush;

            //rescale based on screen resolution
            RescaleToBest();
        }

        //try to rescale the window for a best fit
        private void RescaleToBest()
        {
            try
            {
                var thisWindow = Window.GetWindow(this);

                var wpfScreen = WpfScreen.GetScreenFrom(thisWindow);
                var screenWidth = wpfScreen.DeviceBounds.Width;
                var screenHeight = wpfScreen.DeviceBounds.Height;

                var desiredHorizontalScaling = GetDesiredHorizontalScaling(screenWidth);
                var desiredVerticalScaling = GetDesiredVerticalScaling(screenHeight);

                var lowerValue = desiredHorizontalScaling;
                if (desiredVerticalScaling < lowerValue)
                    lowerValue = desiredVerticalScaling;

                var scaleTransform = new ScaleTransform(lowerValue, lowerValue);

                ParentCanvas.LayoutTransform = scaleTransform;

                thisWindow.WindowState = WindowState.Maximized;
            }
            catch (Exception)
            { }
        }

        private double GetDesiredHorizontalScaling(double screenWidth)
        {
            const double desiredMargin = 200;
            if (screenWidth < desiredMargin)
                return 1.0; //not much we can do ..screen too small

            var desiredCanvasWidth = screenWidth - desiredMargin;

            var canvasWidth = ParentCanvas.Width;

            var scaling = desiredCanvasWidth / canvasWidth;
            return scaling;
        }

        private double GetDesiredVerticalScaling(double screenHeight)
        {
            const double desiredMargin = 200;
            if(screenHeight < desiredMargin)
                return 1.0; //not much we can do ..screen too small

            var desiredCanvasHeight = screenHeight - desiredMargin;

            var canvasHeight = ParentCanvas.Height;

            var scaling = desiredCanvasHeight/canvasHeight;
            return scaling;
        }

        private static Brush CreateBackgroundBrush()
        {
            // Create a LinearGradientBrush 
            var linearGradientBrush = new LinearGradientBrush {StartPoint = new Point(0,0.5), EndPoint = new Point(0,1)};
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 1.0));

            return linearGradientBrush;
        }

        public void ProcessImage(string imagePath, List<ClickPositionOnImage> copyPositions, double rotateImageBy, int currentImageNumber, int totalImageCount)
        {
            _leftEdgePositionBeforeRotation = new Point(0,0);
            _rotationAngleToRealign = rotateImageBy;

            _imageName = Path.GetFileName(imagePath);

            //the dimensions of the _currentImage and the image canvas should be the same so rotation will work right
            _currentImage = MainProcessor.ResizeJpg(imagePath, null, (int) ImageCanvas.Width, (int) ImageCanvas.Height);

            //rotate image if edges of disc are defined and a rotation angle is already specified
            if (copyPositions != null && copyPositions.Exists(x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc)
                && copyPositions.Exists(x => x.PositionType == ClickPositionOnImageTypes.RightEndOfRotatingDisc)
                && _rotationAngleToRealign > 0)
            {
                var rotatedImg = MainProcessor.RotateImg(_currentImage, (float) _rotationAngleToRealign, _bkColor);
                ImageCanvas.SetImage(rotatedImg);
            }
            else
            {
                ImageCanvas.SetImage(_currentImage);    
            }

            ClickSequences = GetClickSequencesForFirstImage();
            if (copyPositions != null)
            {
                foreach (var copyPosition in copyPositions)
                {
                    CopyPositions(copyPosition);
                }
            }
            
            //do not ask for the bottom most part, if it is not the first image
            if(currentImageNumber > 1)
                ClickSequences.Remove(ClickSequences.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.BottomMostPartOfModel));

            SetNextClickPositionType();

            Status.Text = string.Format("Image {0} of {1}", currentImageNumber, totalImageCount);

            Previous.Visibility = currentImageNumber > 1? Visibility.Visible : Visibility.Hidden;
            Next.Content = currentImageNumber == totalImageCount ? "Save" : "Next";
        }

        private void SetNextClickPositionType()
        {
            var nextInSequence = ClickSequences.FirstOrDefault(x => x.Processed == false);
            CurrentClickPositionOnImageType = nextInSequence != null ? nextInSequence.PositionType : ClickPositionOnImageTypes.None;
            SetClickInputHelpImage();
        }

        private void CopyPositions(ClickPositionOnImage copyPosition)
        {
            var matchingInList = ClickSequences.FirstOrDefault(x => x.PositionType == copyPosition.PositionType);
            if (matchingInList == null) return;

            matchingInList.AllowedHeight = copyPosition.AllowedHeight;
            matchingInList.AllowedWidth = copyPosition.AllowedWidth;
            matchingInList.ClickXPos = copyPosition.ClickXPos;
            matchingInList.ClickYPos = copyPosition.ClickYPos;
            matchingInList.Processed = true;
            MarkPositionOnCanvas(new Point {X = copyPosition.ClickXPos, Y = copyPosition.ClickYPos}, copyPosition.PositionType);
        }

        private static List<ClickPositionOnImage> GetClickSequencesForFirstImage()
        {
            return new List<ClickPositionOnImage>
            {
                new ClickPositionOnImage { PositionType = ClickPositionOnImageTypes.LeftEndOfRotatingDisc},
                new ClickPositionOnImage { PositionType = ClickPositionOnImageTypes.RightEndOfRotatingDisc},
                new ClickPositionOnImage { PositionType = ClickPositionOnImageTypes.MarkerLeftFromCenter},
                new ClickPositionOnImage { PositionType = ClickPositionOnImageTypes.MarkerRightFromCenter},
                new ClickPositionOnImage { PositionType = ClickPositionOnImageTypes.BottomMostPartOfModel}
            };
        }

        void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var position = mouseButtonEventArgs.GetPosition(ImageCanvas);
            if (CurrentClickPositionOnImageType == ClickPositionOnImageTypes.None)
                return;

            if (!AllowedRangeHelper.IsInAllowedRange(position, ImageCanvas.Width, ImageCanvas.Height, CurrentClickPositionOnImageType, ClickSequences))
                return;
            
            var matchingInList = ClickSequences.FirstOrDefault(x => x.PositionType == CurrentClickPositionOnImageType);
            if(matchingInList == null)
                throw new Exception("Click type could not be recognized.");

            if(CurrentClickPositionOnImageType == ClickPositionOnImageTypes.RightEndOfRotatingDisc)
            {
                //get the left edge position
                var leftEdge = ClickSequences.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
                if (leftEdge == null)
                    throw new Exception("Left edge not found in the list.");

                //get rotation angle to realign the left and right edges
                var rotationAngleToRealign = MainProcessor.GetRotationAngleToRealign(leftEdge, 
                new ClickPositionOnImage
                {
                    ClickXPos = position.X, ClickYPos = position.Y, 
                    PositionType = ClickPositionOnImageTypes.RightEndOfRotatingDisc, 
                    AllowedHeight = ImageCanvas.Height, AllowedWidth = ImageCanvas.Width
                });

                //realign the x and y positions of the ends of the disc in line with the rotated image
                var newRightEdgePosition = RotationHelper.GetRotatedPosition(rotationAngleToRealign, position);

                if(newRightEdgePosition == null || !(0 <= newRightEdgePosition.Value.X && newRightEdgePosition.Value.X < ImageCanvas.Width)
                    || !(0 <= newRightEdgePosition.Value.Y && newRightEdgePosition.Value.Y < ImageCanvas.Height))
                {
                    MessageBox.Show("Either the selection of the top left and right corners of the disc is wrong or the disc in the photo is in too much slanting.");
                    return;
                }

                var leftEdgePositionBeforeRotation = new Point(leftEdge.ClickXPos, leftEdge.ClickYPos);
                var newLeftEdgePosition = RotationHelper.GetRotatedPosition(rotationAngleToRealign, leftEdgePositionBeforeRotation);
                if (newLeftEdgePosition == null || !(0 <= newLeftEdgePosition.Value.X && newLeftEdgePosition.Value.X < ImageCanvas.Width)
                    || !(0 <= newLeftEdgePosition.Value.Y && newLeftEdgePosition.Value.Y < ImageCanvas.Height))
                {
                    MessageBox.Show("Either the selection of the top left and right corners of the disc is wrong or the disc in the photo is in too much slanting.");
                    return;
                }

                _rotationAngleToRealign = rotationAngleToRealign;
                _leftEdgePositionBeforeRotation = leftEdgePositionBeforeRotation;

                //the dimensions of the _currentImage and the image canvas are the same so rotation will work right
                var rotatedImg = MainProcessor.RotateImg(_currentImage, (float) _rotationAngleToRealign, _bkColor);

                ImageCanvas.SetImage(rotatedImg);

                //set the realigned click positions for top left edge and top right edge
                SetClickPositions(newRightEdgePosition.Value, matchingInList);
                SetClickPositions(newLeftEdgePosition.Value, leftEdge);
                
                //update left edge display
                ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.LeftEndOfRotatingDisc);
                MarkPositionOnCanvas(newLeftEdgePosition.Value, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);

                //update right edge display
                MarkPositionOnCanvas(newRightEdgePosition.Value, CurrentClickPositionOnImageType);
            }
            else
            {
                SetClickPositions(position, matchingInList);
                MarkPositionOnCanvas(position, CurrentClickPositionOnImageType);
            }
            
            SetNextClickPositionType();
        }

        private void UndoRealignLeftEdge()
        {
            var leftEdge = ClickSequences.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
            if (leftEdge == null)
                throw new Exception("Left edge not found in the list.");

            //rotate and reposition
            SetClickPositions(_leftEdgePositionBeforeRotation, leftEdge);

            //update left edge display
            ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.LeftEndOfRotatingDisc);
            MarkPositionOnCanvas(_leftEdgePositionBeforeRotation, ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
        }

        private void SetClickPositions(Point position, ClickPositionOnImage clickPos)
        {
            clickPos.ClickXPos = position.X;
            clickPos.ClickYPos = position.Y;
            clickPos.AllowedWidth = ImageCanvas.Width;
            clickPos.AllowedHeight = ImageCanvas.Height;
            clickPos.Processed = true;
        }

        private void MarkPositionOnCanvas(Point position, ClickPositionOnImageTypes positionType)
        {
            switch (positionType)
            {
                case ClickPositionOnImageTypes.BottomMostPartOfModel:
                    ImageCanvas.MarkWithHorizontalLine(position, DisplayMarkerNames.BottomMostPartOfModel);
                    break;
                case ClickPositionOnImageTypes.LeftEndOfRotatingDisc:
                    ImageCanvas.DrawRectangleOnLeft(position, DisplayMarkerNames.LeftEndOfRotatingDisc);
                    break;
                case ClickPositionOnImageTypes.RightEndOfRotatingDisc:
                    ImageCanvas.DrawRectangleOnRight(position, DisplayMarkerNames.RightEndOfRotatingDisc);
                    //draw center line
                    var leftPosition = ClickSequences.FirstOrDefault(x => x.PositionType == ClickPositionOnImageTypes.LeftEndOfRotatingDisc);
                    if(leftPosition != null && leftPosition.Processed)
                    {
                        var centerPosition = new Point { X = leftPosition.ClickXPos + ((position.X - leftPosition.ClickXPos)/2.0), Y = 0 };
                        ImageCanvas.MarkWithVerticalDashedLine(centerPosition, DisplayMarkerNames.CenterLine);
                        ImageCanvas.GetDashedLineJoiningTwoEdgesOfDisc(new Point{X = leftPosition.ClickXPos, Y = leftPosition.ClickYPos}, position, DisplayMarkerNames.DashedLineJoiningTwoEdgesOfDisc);
                    }
                    break;
                case ClickPositionOnImageTypes.MarkerLeftFromCenter:
                    ImageCanvas.AddMarkerDisplay(position, DisplayMarkerNames.MarkerLeftFromCenter);
                    break;
                case ClickPositionOnImageTypes.MarkerRightFromCenter:
                    ImageCanvas.AddMarkerDisplay(position, DisplayMarkerNames.MarkerRightFromCenter);
                    break;
            }
        }

        private void UndoClick(object sender, RoutedEventArgs e)
        {
            var clickPositionToRemove = ClickSequences.LastOrDefault(x => x.Processed == true);

            if(!ClearOutClickPosition(clickPositionToRemove))
                return;

            SetNextClickPositionType();
    
            //remove corresponding marker display
            RemoveDisplayForAClickType(clickPositionToRemove);

            if(clickPositionToRemove.PositionType == ClickPositionOnImageTypes.RightEndOfRotatingDisc)
            {
                if(_leftEdgePositionBeforeRotation.X <= 0 && _leftEdgePositionBeforeRotation.Y <= 0)
                {
                    //undo of realigning of left edge will not be possible
                    //fire undo one more time to remove the left edge too
                    UndoClick(this, new RoutedEventArgs());
                }
                else
                {
                    //undo realigning of left edge
                    UndoRealignLeftEdge();
                }
            }
        }

        private void RemoveDisplayForAClickType(ClickPositionOnImage clickPositionToRemove)
        {
            switch(clickPositionToRemove.PositionType)
            {
                case ClickPositionOnImageTypes.BottomMostPartOfModel:
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.BottomMostPartOfModel);
                    break;
                case ClickPositionOnImageTypes.LeftEndOfRotatingDisc:
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.LeftEndOfRotatingDisc);
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.CenterLine);
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.DashedLineJoiningTwoEdgesOfDisc);
                    break;
                case ClickPositionOnImageTypes.RightEndOfRotatingDisc:
                    //undo the rotation
                    ImageCanvas.SetImage(_currentImage);
                    
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.RightEndOfRotatingDisc);
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.CenterLine);
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.DashedLineJoiningTwoEdgesOfDisc);
                    break;
                case ClickPositionOnImageTypes.MarkerLeftFromCenter:
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.MarkerLeftFromCenter);
                    break;
                case ClickPositionOnImageTypes.MarkerRightFromCenter:
                    ImageCanvas.RemoveDisplayObject(DisplayMarkerNames.MarkerRightFromCenter);
                    break;
            }
        }

        private static bool ClearOutClickPosition(ClickPositionOnImage clickPositionReference)
        {
            if (clickPositionReference == null) 
                return false;

            clickPositionReference.AllowedHeight = 0.0;
            clickPositionReference.AllowedWidth = 0.0;
            clickPositionReference.ClickXPos = 0.0;
            clickPositionReference.ClickYPos = 0.0;
            clickPositionReference.Processed = false;

            return true;
        }

        private void AutoConfigureClicked(object sender, RoutedEventArgs e)
        {
            var handler = AutoConfigureRequestedEvent;
            if (handler != null) handler(this, new EventArgs());
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentClickPositionOnImageType != ClickPositionOnImageTypes.None)
            {
                MessageBox.Show("Please mark all positions before going to the next image.");
                return;
            }
            ImageCanvas.RemoveAllMarkers();
            if (IndividualImgProcessedEvent != null)
            {
                if (ClickSequences != null) IndividualImgProcessedEvent(this, new IndividualImgProcessedEventArgs 
                    { ClickPositions = ClickSequences, RotateImageBy = _rotationAngleToRealign,
                    ImageName = _imageName});
            }
        }

        private void SetClickInputHelpImage()
        {
            int sourceImage = 0;
            switch(CurrentClickPositionOnImageType)
            {
                case ClickPositionOnImageTypes.BottomMostPartOfModel:
                    sourceImage = 1;
                    ClickHelpText1.Text = "Click on the bottommost part of the model in the picture. This point should be above the rotating disc.";
                    ClickHelpText2.Text = "Everything else below this point will be ignored.";
                    ClickHelpText3.Text= "We ask for this input for all photos to keep tabs on any vertical shift in the camera position between shots.";
                    break;
                case ClickPositionOnImageTypes.LeftEndOfRotatingDisc:
                    sourceImage = 2;
                   ClickHelpText1.Text = "Click on the top left end of the rotating disc. ";
                   ClickHelpText2.Text = "We ask for the left and the right end of the disc for all images to keep tabs on any horizontal shift in the camera position between shots.";
                    ClickHelpText3.Text = "";
                    break;
                case ClickPositionOnImageTypes.RightEndOfRotatingDisc:
                    sourceImage = 3;
                    ClickHelpText1.Text = "Click on the top right end of the rotating disc.";
                    ClickHelpText2.Text = "";
                    ClickHelpText3.Text = "";
                    break;

                case ClickPositionOnImageTypes.MarkerLeftFromCenter:
                    sourceImage = 4;
                    ClickHelpText1.Text = "Click on the first marking that you see when moving from the center of the disc to the left as shown above." ;
                    ClickHelpText2.Text = "Remember that it should be the first marking from the center to the left.";
                    ClickHelpText3.Text = "Markings help to figure out how much have we rotated the disc since the last shot.";
                    break;
                case ClickPositionOnImageTypes.MarkerRightFromCenter:
                    sourceImage = 5;
                    ClickHelpText1.Text = "Click on the first marking that you see when moving from the center of the disc to the right as shown above.";
                    ClickHelpText2.Text = "Remember that it should be the first marking from the center to the right.";
                    ClickHelpText3.Text = "";
                    break;
                case ClickPositionOnImageTypes.None:
                default:
                    sourceImage = 6;
                    ClickHelpText1.Text = "We are done with the markings for this image.";
                    ClickHelpText2.Text = "Click undo to revert your click inputs.";
                    ClickHelpText3.Text = "Click next to go to the next image.";
                    break;
            }
            
            ClickHelpImage.Source = new BitmapImage(new Uri(string.Format("ClickInputs/{0}.png", sourceImage), UriKind.Relative));
        }

        private static Point GetRotatedPositionDeprecated(float rotationAngleToRealign, Point currentPosition, int displayWidth, int displayHeight)
        {
            System.Drawing.Color bkColor = System.Drawing.Color.FromArgb(200, 200, 200);

            var bitmap = new Bitmap(displayWidth, displayHeight);
            var g = Graphics.FromImage(bitmap);
            g.Clear(bkColor);
            g.Dispose();

            var positionColor = System.Drawing.Color.DarkViolet;
            bitmap.SetPixel((int) currentPosition.X, (int) currentPosition.Y, positionColor);
            bitmap.SetPixel((int)currentPosition.X + 1, (int)currentPosition.Y, positionColor);
            bitmap.SetPixel((int)currentPosition.X - 1, (int)currentPosition.Y, positionColor);
            bitmap.SetPixel((int)currentPosition.X, (int)currentPosition.Y + 1, positionColor);
            bitmap.SetPixel((int)currentPosition.X, (int)currentPosition.Y - 1, positionColor);
            bitmap.SetPixel((int)currentPosition.X + 1, (int)currentPosition.Y + 1, positionColor);
            bitmap.SetPixel((int)currentPosition.X - 1, (int)currentPosition.Y - 1, positionColor);

            
            var rotatedBitmap = MainProcessor.RotateImg(bitmap, rotationAngleToRealign, bkColor);

            for(var y = 0; y < displayHeight; y++)
            {
                for(var x = 0; x < displayWidth; x++)
                {
                    var color = rotatedBitmap.GetPixel(x, y);
                    if( color.R == positionColor.R && color.G == positionColor.G && color.B == positionColor.B)
                        return new Point(x,y);
                }
            }

            throw new Exception("Could not find rotated position");
        }

        private void PreviousClick(object sender, RoutedEventArgs e)
        {
            ImageCanvas.RemoveAllMarkers();
            var handler = ProcessPreviousImgEvent;
            if (handler != null) handler(this, new EventArgs());
        }
    }
}
