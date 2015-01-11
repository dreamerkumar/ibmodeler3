using System;
using System.Windows;
using System.Windows.Media;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Help;
using Ajubaa.IBModeler.Processor;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void ModelSettingsEventHandler(object sender, ModelSettingsEventArgs args);
    internal delegate void ChangeImageFolderEventHandler(object sender, EventArgs args);
    internal delegate void ReenterScreenSettingsEventHandler(object sender, EventArgs args);
    internal delegate void ReenterClickPositionsEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for ModelSettingsContainer.xaml
    /// </summary>
    public partial class ModelSettingsContainer
    {
        #region events
        internal event ModelSettingsEventHandler ModelSettingsEvent;
        internal void InvokeModelSettingsEvent(ModelSettingsEventArgs args)
        {
            var handler = ModelSettingsEvent;
            if (handler != null) handler(this, args);
        }
        internal event ChangeImageFolderEventHandler ChangeImageFolderEvent;
        internal void InvokeChangeImageFolderEvent(EventArgs args)
        {
            var handler = ChangeImageFolderEvent;
            if (handler != null) handler(this, args);
        }
        internal event ReenterClickPositionsEventHandler ReenterClickPositionsEvent;
        internal void InvokeReenterClickPositionsEvent(EventArgs args)
        {
            var handler = ReenterClickPositionsEvent;
            if (handler != null) handler(this, args);
        }
        internal event ReenterScreenSettingsEventHandler ReenterScreenSettingsEvent;
        internal void InvokeReenterScreenSettingsEvent(EventArgs args)
        {
            var handler = ReenterScreenSettingsEvent;
            if (handler != null) handler(this, args);
        }
        #endregion events


        public ModelSettingsContainer()
        {
            InitializeComponent();
            SliderForMeshDensity.Slider.Minimum = 50;
            SliderForMeshDensity.Slider.Maximum = 300;
            SliderForMeshDensity.Label.Text = "Mesh Density";
            SliderForMeshDensity.Display.Text = "120";
            SliderForMeshDensity.Slider.Value = 120;

            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.CreateReady);

            //scale grid a little bit for monitors with resolution less than 600 height
            try
            {
                var thisWindow = Window.GetWindow(this);

                var wpfScreen = WpfScreen.GetScreenFrom(thisWindow);
                var screenHeight = wpfScreen.DeviceBounds.Height;
                if (screenHeight <= 600)
                {
                    const double scaleValue = .8;
                    var scaleTransform = new ScaleTransform(scaleValue, scaleValue);
                    ParentGrid.LayoutTransform = scaleTransform;
                }

                thisWindow.WindowState = WindowState.Maximized;
            }
            catch (Exception)
            { }
        }
        private void Next(object sender, RoutedEventArgs e)
        {
            var maxAngleOfImageToProcessInDegress = GetMaxAngleOfImageToProcessInDegress();

            var modelSettings = new ModelSettingsEventArgs
            {
                MeshDensity = (int) SliderForMeshDensity.Slider.Value,
                SaveProcessedImages = SaveImages.IsChecked.HasValue? (bool)SaveImages.IsChecked : false,
                MaxAngleOfImageToProcessInDegress = maxAngleOfImageToProcessInDegress
            };
            MainProcessor.AnalyzeMoldForDataLosses = AnalyzeCutOutData.IsChecked.HasValue? (bool)AnalyzeCutOutData.IsChecked : false;
            InvokeModelSettingsEvent(modelSettings);
        }

        private int GetMaxAngleOfImageToProcessInDegress()
        {
            var maxAngleOfImageToProcessInDegress = 160;
            var temp = 0;
            if (Int32.TryParse(MaxImageAngle.Text, out temp))
            {
                if (temp > 0)
                {
                    maxAngleOfImageToProcessInDegress = temp;
                }
            }
            if (maxAngleOfImageToProcessInDegress > 360) maxAngleOfImageToProcessInDegress = 360;
            return maxAngleOfImageToProcessInDegress;
        }

        private void ChangeImageFolder(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel this process and start with a different set of images?", "Cancel Confirmation", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            InvokeChangeImageFolderEvent(new EventArgs());
        }

        private void ReenterClickPositions(object sender, RoutedEventArgs e)
        {
            InvokeReenterClickPositionsEvent(new EventArgs());
        }

        private void ReenterScreenSettings(object sender, RoutedEventArgs e)
        {
            InvokeReenterScreenSettingsEvent(new EventArgs());
        }
    }
}
