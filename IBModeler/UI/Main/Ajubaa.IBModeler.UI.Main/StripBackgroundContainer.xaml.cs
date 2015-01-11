using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Help;
using Ajubaa.IBModeler.ImageAlterations;
using Ajubaa.IBModeler.Processor;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void StripBackgroundEventHandler(object sender, StripBackgroundEventArgs args);

    /// <summary>
    /// Interaction logic for StripBackgroundContainer.xaml
    /// </summary>
    public partial class StripBackgroundContainer
    {
        private readonly Color _invalidColor;
        internal event StripBackgroundEventHandler StripBackgroundEvent;

        private Bitmap _bitmap; 
        public StripBackgroundContainer(string testImageFilePath, Color invalidColor, BackgroundStrippingParams currentParams)
        {
            _invalidColor = invalidColor;
            InitializeComponent();
            
            SliderForOffSet.Slider.Minimum = 10;
            SliderForOffSet.Slider.Maximum = 255;
            SliderForOffSet.Label.Text = "Minimum offset";
            SliderForOffSet.Slider.Value = 25;

            SliderForMaxDiff.Slider.Minimum = 0;
            SliderForMaxDiff.Slider.Maximum = 100;
            SliderForMaxDiff.Label.Text = "Max diff between other 2 colors";
            SliderForMaxDiff.Slider.Value = 100;

            SliderForOtherColor.Slider.Minimum = 0;
            SliderForOtherColor.Slider.Value = 10;
            SliderForOtherColor.Slider.Maximum = 50;
            SliderForOtherColor.Label.Text = "Allowed variation percent";
            SliderForOtherColor.ToolTip = "Allowed variation in the color at the top left corner of the image";

            ComboBoxForScreenColor.SelectedIndex = 0;

            SetImage(testImageFilePath);

            //set current values
            if(currentParams != null && currentParams.ColorVariationBasedParams != null)
            {
                ComboBoxForScreenColor.SelectedIndex = 3;
                SliderForOtherColor.Slider.Value = currentParams.ColorVariationBasedParams.VariationPercent;
            }
            else if (currentParams != null && currentParams.ScreenBasedParams != null)
            {
                SliderForOffSet.Slider.Value = currentParams.ScreenBasedParams.MinColorOffset;
                SliderForMaxDiff.Slider.Value = currentParams.ScreenBasedParams.MaxDiffPercent;
                switch (currentParams.ScreenBasedParams.ScreenColorTypes)
                {
                    case ScreenColorTypes.BlueScreen:
                        ComboBoxForScreenColor.SelectedIndex = 1;
                        break;
                    case ScreenColorTypes.RedScreen:
                        ComboBoxForScreenColor.SelectedIndex = 2;
                        break;
                    default:
                        ComboBoxForScreenColor.SelectedIndex = 0;
                        break;
                }
            }
            SetSliderDisplayBasedOnScreenType();
            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.BackgroundScreen);
            ComboBoxForScreenColor.SelectionChanged += ScreenSelectionChanged;
            //change things for different monitor resolutions
            try
            {
                var thisWindow = Window.GetWindow(this);

                var wpfScreen = WpfScreen.GetScreenFrom(thisWindow);
                var screenWidth = wpfScreen.DeviceBounds.Width;
                if (screenWidth <= 1000)
                    TopMessage.MessageFontSize = 12; 
            }
            catch (Exception)
            { }
        }

        private void ScreenSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSliderDisplayBasedOnScreenType();
        }

        private void SetSliderDisplayBasedOnScreenType()
        {
            var selectedIndex = ComboBoxForScreenColor.SelectedIndex;
            if(selectedIndex == 3)
            {
                SliderForMaxDiff.Visibility = Visibility.Hidden;
                SliderForOffSet.Visibility = Visibility.Hidden;
                SliderForOtherColor.Visibility = Visibility.Visible;
            }
            else
            {
                SliderForMaxDiff.Visibility = Visibility.Visible;
                SliderForOffSet.Visibility = Visibility.Visible;
                SliderForOtherColor.Visibility = Visibility.Hidden;
            }
        }

        public void SetImage(string imagePath)
        {
            _bitmap = (Bitmap)Image.FromFile(imagePath);
            
            //resize to a base size so that it does not take too much time during test stripping
            var imageAlterationParams = new ImageAlterationParams
            {
                ResizeType = ResizeType.ResizeSufficiently
            };
            _bitmap = MainProcessor.ResizeImage(_bitmap, imageAlterationParams);

            var imageBrush = new ImageBrush(new BitmapImage(new Uri(imagePath, UriKind.Absolute)));
            SampleImageCanvas.Background = imageBrush;
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            var args = new StripBackgroundEventArgs
            {
                BackgroundStrippingParams = GetParams()
            };
            InvokeStripBackgroundEvent(args);
        }

        private void OnHoverOfComboBox(object sender, MouseEventArgs e)
        {
            
        }

        private void Test(object sender, RoutedEventArgs e)
        {
            var @params = GetParams();
            var testBitmap = (Bitmap)_bitmap.Clone();

            MainProcessor.StripBackground(testBitmap, @params);
            
            //Convert the image in resources to a Stream 
            Stream memoryStream = new MemoryStream();
            testBitmap.Save(memoryStream, ImageFormat.Png);
            
            // Create a BitmapImage with the Stream. 
            var bitmapImage = new BitmapImage(); 
            bitmapImage.BeginInit(); 
            bitmapImage.StreamSource = memoryStream; 
            bitmapImage.EndInit();

            //set the display with this new image brush
            var imageBrush = new ImageBrush(bitmapImage);
            SampleImageCanvas.Background = imageBrush;
        }

        private BackgroundStrippingParams GetParams()
        {
            if(ComboBoxForScreenColor.SelectedIndex == 3)
            {
                return new BackgroundStrippingParams
                {
                    BackgroundColor = GetSerializableColor(_invalidColor),
                    ColorVariationBasedParams = new ColorVariationBasedParams
                    {
                        VariationPercent = SliderForOtherColor.Slider.Value,
                        CompareColor = GetSerializableColor(_bitmap.GetPixel(0,0))
                    }
                };
            }

            var offset = SliderForOffSet.Slider.Value;
            var maxDiff = SliderForMaxDiff.Slider.Value;

            var screenParams = new ScreenBasedParams
            {
                MaxDiffPercent = (byte)maxDiff,
                MinColorOffset = (byte)offset
            };
            var screen = ComboBoxForScreenColor.SelectedIndex;

            switch (screen)
            {
                case 1:
                    screenParams.ScreenColorTypes = ScreenColorTypes.BlueScreen;
                    break;
                case 2:
                    screenParams.ScreenColorTypes = ScreenColorTypes.RedScreen;
                    break;
                default:
                    screenParams.ScreenColorTypes = ScreenColorTypes.GreenScreen;
                    break;
            }
            return new BackgroundStrippingParams {ScreenBasedParams = screenParams, BackgroundColor = GetSerializableColor(_invalidColor)};
        }

        private static SerializableColor GetSerializableColor(Color color)
        {
            return new SerializableColor(color.R, color.G, color.B);
        }

        internal void InvokeStripBackgroundEvent(StripBackgroundEventArgs args)
        {
            var handler = StripBackgroundEvent;
            if (handler != null) handler(this, args);
        }
    }
}
