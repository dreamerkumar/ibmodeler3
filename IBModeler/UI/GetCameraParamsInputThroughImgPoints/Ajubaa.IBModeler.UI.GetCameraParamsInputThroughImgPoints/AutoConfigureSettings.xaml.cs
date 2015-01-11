using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ajubaa.IBModeler.AutoConfigureImgPoints;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.Help;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;

namespace Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints
{
    public delegate void AutoConfigureEventHandler(object sender, AutoConfigureSettingsEventArgs args);

    /// <summary>
    /// Interaction logic for AutoConfigureSettings.xaml
    /// </summary>
    public partial class AutoConfigureSettings : UserControl
    {
        private const string HorizontalLineName = "UpperYLimitForDisc";
        private string _folderPath;
        private Color _backgroundColor;
        private BackgroundStrippingParams _backgroundStrippingParams;
        private int? _minPixelsForBaseDisc;

        public event AutoConfigureEventHandler AutoConfigured;

        public AutoConfigureSettings()
        {
            InitializeComponent();
            ImageCanvas.MouseDown += OnMouseDown;
            ParentCanvas.Background = CreateBackgroundBrush();
        }

        private static Brush CreateBackgroundBrush()
        {
            // Create a LinearGradientBrush 
            var linearGradientBrush = new LinearGradientBrush { StartPoint = new Point(0, 0.5), EndPoint = new Point(0, 1) };
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.White, 0.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(Colors.LightGray, 1.0));

            return linearGradientBrush;
        }

        public void Initialize(string folderPath, BackgroundStrippingParams backgroundStrippingParams, Color backgroundColor)
        {
            _folderPath = folderPath;
            _backgroundStrippingParams = backgroundStrippingParams;
            _backgroundColor = backgroundColor;
            var firstImgPath = GetImageFilesHelper.GetImageFilesFromLocation(folderPath)[0];
            ImageCanvas.SetImage(firstImgPath);
            ClickHelpImage.Source = new BitmapImage(new Uri("ClickInputs/7.png", UriKind.Relative));
        }



        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ImageCanvas.RemoveDisplayObject(HorizontalLineName);

            var position = e.GetPosition(ImageCanvas);

            _minPixelsForBaseDisc = (int) (500 - position.Y);

            var leftPosition = new Point(0, position.Y);
            var rightPosition = new Point(ImageCanvas.Width - 1, position.Y);

            ImageCanvas.GetDashedLineJoiningTwoEdgesOfDisc(leftPosition, rightPosition, HorizontalLineName);
        }

        private void Return(object sender, RoutedEventArgs routedEventArgs)
        {
            if(_minPixelsForBaseDisc == null)
            {
                MessageBox.Show("Please mark the area in the image that contains just the base disc.");
                return;
            }
            else if(_minPixelsForBaseDisc < 10)
            {
                MessageBox.Show("The height of the base disc should at least be 10 pixels.");
                return;
            }

            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentCanvas);
            var processor = new AutoConfigureImgPoints.Processor();
            var markerPrcoessingParams = new MarkerProcessingParams
            {
                MarkerColorVariationPercent = 10,
                MarkerWidthPercent = 5,
                MaximumMarkerCount = 10
            };
            var @params = new AutoConfigureImgPointsParams
            {
                MarkerProcessingParams = markerPrcoessingParams,
                BackgroundColor = _backgroundColor,
                BackgroundStrippingParams = _backgroundStrippingParams,
                MinPixelsForBaseDisc = _minPixelsForBaseDisc.Value,
                MinPixelsForBaseDiscEndOfEdge = 10,
                ResizeHeight = 500,
                ResizeWidth = 500
            };
            var clickPositions = processor.GetClickPositionsForImgFolder(_folderPath, @params);

            

            var gotResults = clickPositions != null && clickPositions.Count > 0 
                && clickPositions.Any(x => (x.ClickPositionListForImages != null && x.ClickPositionListForImages.Count > 0));

            if (!gotResults)
            {
                var statusMessages = new List<string>
                {
                    "Autoconfigure was unsuccessful. No image positions could be determined.",
                    "Please click on the 'Requirements' button to check how to take photos to support autoconfigure.",
                    "Press the cancel button to go back to entering the positions manually."
                };
                quickProcessingWindow.Close();
                var messageBox = new HelpMessageBox(statusMessages, null);
                messageBox.ShowDialog();
                return;
            }

            var fileCount = GetImageFilesHelper.GetImageFilesFromLocation(_folderPath);
            var requiredCount = GetRequiredClickInputsCount(fileCount);
            var identifiedCount = 0;
            var innerStatusMessages = new List<string>();
            for (var ctr = 0; ctr < clickPositions.Count; ctr++)
            {
                var clickInput = clickPositions[ctr];
                var count = clickInput != null && clickInput.ClickPositionListForImages != null? clickInput.ClickPositionListForImages.Count : 0;
                identifiedCount+= count;
                
                if(ctr == 0)
                {
                    innerStatusMessages.Add(string.Format("{0}: {1} identified out of required 5.",clickInput == null? (ctr+1).ToString() : clickInput.ImageName, count));
                }
                else
                {
                    innerStatusMessages.Add(string.Format("{0}: {1} identified out of required 4.", clickInput == null ? (ctr + 1).ToString() : clickInput.ImageName, count));
                }
            }

            quickProcessingWindow.Close();

            if(identifiedCount == requiredCount)
            {
                var statusMessages = new List<string>
                {
                    "AutoConfigure Successful!",
                    string.Format("All required {0} click positions were identified.", requiredCount),
                    "We will now display all the click positions for your review.."
                };
                
                var messageBox = new HelpMessageBox(statusMessages, null);
                messageBox.ShowDialog();
            }
            else
            {
                var statusMessages = new List<string>
                {
                    "AutoConfigure was partially successful in identifying the click positions.",
                    string.Format("{0} out of total of {1} positions could be identified.", identifiedCount, requiredCount),
                    "Below is a break down of the identified positions.",
                    "Click close to to review the identified positions and fill in the missed ones..."
                };

                var messageBox = new HelpMessageBox(statusMessages, innerStatusMessages);
                messageBox.ShowDialog();
            }

            if (AutoConfigured != null)
                AutoConfigured(this,
                new AutoConfigureSettingsEventArgs
                {
                    ClickInputs = new ClickInputs
                    {
                        ImageClickInputDetailsList = clickPositions,
                        Angles = null
                    }
                });
        }

        /// <summary>
        /// 5 for the first image
        /// 4 for the rest of the images
        /// </summary>
        /// <param name="fileCount"></param>
        private static int GetRequiredClickInputsCount(string[] fileCount)
        {
            var requiredCount = 0;
            if(fileCount.Length > 0)
            {
                requiredCount += 5;
            }
            requiredCount += (fileCount.Length - 1)*4;
            return requiredCount;
        }

        private void OpenRequirementsWindow(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.AutoConfigureRequirements);
        }
    }
}
