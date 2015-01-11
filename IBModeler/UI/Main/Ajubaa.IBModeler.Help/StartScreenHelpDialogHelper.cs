using System;
using System.Linq;
using System.Windows;

namespace Ajubaa.IBModeler.Help
{
    public static class StartScreenHelpDialogHelper
    {
        private static ScreenStartHelpDialog _currentScreenStartDialog = null;
        private static ScreenStartDialogTypes _currentDialogType = ScreenStartDialogTypes.None;

        private static bool _backgroundScreenShownOnce; 
        private static bool _clickInputsShownOnce; 
        private static bool _autoConfigureShownOnce;  
        private static bool _createReadyShownOnce;  
        private static bool _meshCreatedShownOnce;
        private static bool _addSkinDialogShownOnce;  

        public static void ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes dialogType)
        {
            var windowAlreadyShownOnce = WindowAlreadyShownOnce(dialogType);

            if (windowAlreadyShownOnce) return;

            var configuredToShowWindowOnLoad = ConfiguredToShowWindowOnLoad(dialogType);

            if (!configuredToShowWindowOnLoad) return;

            ShowHelpDialogWindow(dialogType);
        }

        private static bool WindowAlreadyShownOnce(ScreenStartDialogTypes dialogType)
        {
            switch (dialogType)
            {
                case ScreenStartDialogTypes.BackgroundScreen:
                    return _backgroundScreenShownOnce;
                case ScreenStartDialogTypes.ClickInputs:
                    return _clickInputsShownOnce;
                case ScreenStartDialogTypes.AutoConfigure:
                    return _autoConfigureShownOnce;
                case ScreenStartDialogTypes.CreateReady:
                    return _createReadyShownOnce;
                case ScreenStartDialogTypes.MeshCreated:
                    return _meshCreatedShownOnce;
                case ScreenStartDialogTypes.AddSkinDialog:
                    return _addSkinDialogShownOnce;
                default:
                    return false;
            }
        }

        public static void ShowHelpDialogWindow(ScreenStartDialogTypes dialogType)
        {
            if(_currentScreenStartDialog != null && Application.Current.Windows.OfType<ScreenStartHelpDialog>().Count() > 0)
            {
                if (_currentDialogType == dialogType)
                {
                    _currentScreenStartDialog.Activate();
                    return;
                }
                _currentScreenStartDialog.Close();
            }
            ShowNewHelpDialogWindow(dialogType);
        }

        private static void ShowNewHelpDialogWindow(ScreenStartDialogTypes dialogType)
        {
            _currentDialogType = dialogType;
            _currentScreenStartDialog = new ScreenStartHelpDialog(dialogType);

            //if a dialog is already open, problems occur on trying to close additional help screens created on the side
            //so just create helps for dialogs as another modal window on top 
            if (dialogType == ScreenStartDialogTypes.AddSkinDialog || dialogType == ScreenStartDialogTypes.AdjustSkinDialog)
                _currentScreenStartDialog.ShowDialog();
            else
            {
                _currentScreenStartDialog.Show();
                _currentScreenStartDialog.Activate();
            }
            switch (dialogType)
            {
                case ScreenStartDialogTypes.BackgroundScreen:
                    _backgroundScreenShownOnce = true;
                    break;
                case ScreenStartDialogTypes.ClickInputs:
                    _clickInputsShownOnce = true;
                    break;
                case ScreenStartDialogTypes.AutoConfigure:
                    _autoConfigureShownOnce = true;
                    break;
                case ScreenStartDialogTypes.CreateReady:
                    _createReadyShownOnce = true;
                    break;
                case ScreenStartDialogTypes.MeshCreated:
                    _meshCreatedShownOnce = true;
                    break;
                case ScreenStartDialogTypes.AddSkinDialog:
                    _addSkinDialogShownOnce = true;
                    break;
                default:
                    break;
            }
        }

        private static bool ConfiguredToShowWindowOnLoad(ScreenStartDialogTypes dialogType)
        {
            bool configuredToHideWindow;
            switch (dialogType)
            {
                case ScreenStartDialogTypes.BackgroundScreen:
                    configuredToHideWindow = HideScreenStartDialogFlags.Flags.BackgroundScreen;
                    break;
                case ScreenStartDialogTypes.ClickInputs:
                    configuredToHideWindow = HideScreenStartDialogFlags.Flags.ClickInputs;
                    break;
                //case ScreenStartDialogTypes.AutoConfigure:
                //    configuredToHideWindow = HideScreenStartDialogFlags.Flags.AutoConfigure;
                //    break;
                case ScreenStartDialogTypes.CreateReady:
                    configuredToHideWindow = HideScreenStartDialogFlags.Flags.CreateReady;
                    break;
                case ScreenStartDialogTypes.MeshCreated:
                    configuredToHideWindow = HideScreenStartDialogFlags.Flags.MeshCreated;
                    break;
                //case ScreenStartDialogTypes.AddSkinDialog:
                //    configuredToHideWindow = HideScreenStartDialogFlags.Flags.AddSkinDialog;
                //    break;
                default:
                    return false;
            }
            return !configuredToHideWindow;
        }
    }
}
