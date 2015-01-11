using System.Drawing;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Help;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void AutoConfigureEventHandler(object sender, AutoConfigureSettingsEventArgs args);

    /// <summary>
    /// Interaction logic for AutoConfigureContainer.xaml
    /// </summary>
    public partial class AutoConfigureContainer
    {
        internal event AutoConfigureEventHandler AutoConfigured;

        public AutoConfigureContainer(string folderPath, BackgroundStrippingParams backgroundStrippingParams, Color backgroundColor)
        {
            InitializeComponent();
            AutoConfigureControl.AutoConfigured += InvokeAutoConfiguredEvent;
            AutoConfigureControl.Initialize(folderPath, backgroundStrippingParams, backgroundColor);
            Loaded += AutoConfigureContainerLoaded;
        }

        static void AutoConfigureContainerLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.AutoConfigure);
        }

        internal void InvokeAutoConfiguredEvent(object sender, AutoConfigureSettingsEventArgs args)
        {
            var handler = AutoConfigured;
            if (handler != null) handler(this, args);
        }
    }
}