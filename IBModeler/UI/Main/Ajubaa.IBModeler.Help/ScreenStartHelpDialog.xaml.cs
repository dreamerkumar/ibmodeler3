using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using Ajubaa.IBModeler.Help.DialogHelps;

namespace Ajubaa.IBModeler.Help
{
    /// <summary>
    /// Interaction logic for ScreenStartHelpDialog.xaml
    /// </summary>
    public partial class ScreenStartHelpDialog
    {
        private readonly ScreenStartDialogTypes _dialogType;

        public ScreenStartHelpDialog(ScreenStartDialogTypes dialogType)
        {
            _dialogType = dialogType;
            InitializeComponent();
            Loaded += ScreenStartHelpDialogLoaded;
        }

        private void WebsiteLinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }

        void ScreenStartHelpDialogLoaded(object sender, RoutedEventArgs e)
        {
            switch (_dialogType)
            {
                case ScreenStartDialogTypes.BackgroundScreen:
                    HelpContents.Children.Add(new BackgroundScreen());
                    break;
                case ScreenStartDialogTypes.ClickInputs:
                    HelpContents.Children.Add(new ClickInputs());
                    break;
                case ScreenStartDialogTypes.AutoConfigure:
                    HelpContents.Children.Add(new AutoConfigure());
                    break;
                case ScreenStartDialogTypes.CreateReady:
                    HelpContents.Children.Add(new CreateReady());
                    break;
                case ScreenStartDialogTypes.MeshCreated:
                    HelpContents.Children.Add(new MeshCreated());
                    break;
                case ScreenStartDialogTypes.AddSkinDialog:
                    HelpContents.Children.Add(new AddSkinDialog());
                    DoNotShowCheckBox.Visibility = Visibility.Collapsed;
                    break;
                case ScreenStartDialogTypes.ModelCreationInProcess:
                    HelpContents.Children.Add(new ModelCreationInProcess());
                    break;
                case ScreenStartDialogTypes.AutoConfigureRequirements:
                    HelpContents.Children.Add(new AutoConfigureRequirements());
                    DoNotShowCheckBox.Visibility = Visibility.Collapsed;
                    break;
                case ScreenStartDialogTypes.AdjustSkinDialog:
                    HelpContents.Children.Add(new AdjustSkinDialog());
                    DoNotShowCheckBox.Visibility = Visibility.Collapsed;
                    break;
                case ScreenStartDialogTypes.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            SaveHideMsgFlag();
            Close();
        }

        private void SaveHideMsgFlag()
        {
            if (!DoNotShowCheckBox.IsChecked.GetValueOrDefault()) return;

            var flags = HideScreenStartDialogFlags.Flags;
            var aFlagValueChanged = false;
            switch(_dialogType)
            {
                case ScreenStartDialogTypes.BackgroundScreen:
                    if(!flags.BackgroundScreen) { flags.BackgroundScreen = aFlagValueChanged = true; }
                    break;
                case ScreenStartDialogTypes.ClickInputs:
                    if (!flags.ClickInputs) { flags.ClickInputs = aFlagValueChanged = true; }
                    break;
                //case ScreenStartDialogTypes.AutoConfigure:
                //    if (!flags.AutoConfigure) { flags.AutoConfigure = aFlagValueChanged = true; }
                //    break;
                case ScreenStartDialogTypes.CreateReady:
                    if (!flags.CreateReady) { flags.CreateReady = aFlagValueChanged = true; }
                    break;
                case ScreenStartDialogTypes.MeshCreated:
                    if (!flags.MeshCreated) { flags.MeshCreated = aFlagValueChanged = true; }
                    break;
                //case ScreenStartDialogTypes.AddSkinDialog:
                //    if (!flags.AddSkinDialog) { flags.AddSkinDialog = aFlagValueChanged = true; }
                //    break;
                default:
                    return;
            }
            if(aFlagValueChanged)
            {
                HideScreenStartDialogFlags.Flags = flags;
            }
        }
    }
}
