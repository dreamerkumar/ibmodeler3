using System;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Help;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void ClickedEventHandler(object sender, AllImgProcessedEventArgs args);

    internal delegate void AutoConfigureRequestedEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for ClickCanvasContainer.xaml
    /// </summary>
    public partial class ClickCanvasContainer
    {
        internal event AutoConfigureRequestedEventHandler AutoConfigureRequestedEvent;

        internal event ClickedEventHandler AllImagesProcessedEvent;

        public ClickCanvasContainer(string folderPath, ClickInputs clickInputs)
        {
            InitializeComponent();
            ImgListHandler.AllImgProcessedEvent += InvokeAllImageProcessed;
            ImgListHandler.AutoConfigureRequestedEvent += InvokeAutoConfigureRequestedEvent;

            ImgListHandler.GetUserInputForImages(folderPath, clickInputs);
            Loaded += ClickCanvasContainerLoaded;
        }

        static void ClickCanvasContainerLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.ClickInputs);
        }

        private void InvokeAllImageProcessed(object sender, AllImgProcessedEventArgs args)
        {
            var handler = AllImagesProcessedEvent;
            if (handler != null) handler(this, args);
        }

        internal void InvokeAutoConfigureRequestedEvent(object sender, EventArgs args)
        {
            var handler = AutoConfigureRequestedEvent;
            if (handler != null) handler(this, args);
        }
    }
}