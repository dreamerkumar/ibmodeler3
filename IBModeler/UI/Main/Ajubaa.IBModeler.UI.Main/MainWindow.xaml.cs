using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Help;
using Ajubaa.IBModeler.Processor;
using Color = System.Drawing.Color;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region containers
        private SelectFolderContainer _selectFolderContainer;
        private ClickCanvasContainer _clickCanvasContainer;
        private ModelSettingsContainer _modelSettingsContainer;
        private ProcessContainer _processContainer;
        private ModelContainer _modelContainer;
        private StripBackgroundContainer _stripBackgroundContainer;
        private AutoConfigureContainer _autoConfigureSettingsContainer;
        #endregion

        private string _folderPath;
        
        private int? _meshDensity;
        private bool _saveProcessedImages;
        private ClickInputs _clickInputs;
        private ModelMeshAndPositionNeighbors _modelMeshAndPositionNeighbors;
        private CreateMeshContract _createMeshContract;

        #region fields used by background screen container
        private string _firstImageFilePath;
        private BackgroundStrippingParams _backgroundStrippingParams;
        private readonly Color _invalidColor = Color.FromArgb(200, 200, 200);
        private int _maxAngleToProcessForMesh;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindowLoaded;
            WindowState = WindowState.Maximized;
            this.Closed += MainWindowClosed;

            DisplaySelectFolderContainer();
        }

        static void MainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        static void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            //trick to find out the names of all the fonts added in the fonts folder
            //var fonts = new List<FontFamily>();
            //foreach (var fontFamily in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./Fonts/"))
            //{
            //    fonts.Add(fontFamily);
            //    //fontFamily will give you the actual name of the font to embed
            //}

            //BaseGrid.Background = CreateBackgroundBrush();
        }

        public Brush CreateBackgroundBrush()
        {
            // Create a LinearGradientBrush 
            var linearGradientBrush = new LinearGradientBrush(){StartPoint = new Point(1.0,0.0), EndPoint = new Point(1.0,1.0)};
            linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb( 125,125,125), 1.0));
            linearGradientBrush.GradientStops.Add(new GradientStop(System.Windows.Media.Color.FromRgb(75, 75, 75), 0.0));

            return linearGradientBrush;
        }

        private void ResetValues()
        {
            _folderPath = null;
            _clickInputs = null;
            _modelMeshAndPositionNeighbors = null;
            _createMeshContract = null;
            _firstImageFilePath = null;
            _backgroundStrippingParams = null;
            _meshDensity = null;
        }

        #region handlers (serializes returned input and calls up setter functions)
        private void SelectFolderHandler(object sender, SelectFolderEventArgs args)
        {
            var folderPath = args.FolderPath;

            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Folder does not exist. It might have been moved or deleted.");
                return;
            }

            var imageFiles = GetImageFilesHelper.GetImageFilesFromLocation(folderPath);

            if (imageFiles == null || imageFiles.Length <= 0)
            {
                MessageBox.Show("Folder does not contain any images with jpg or jpeg format.");
                return;
            }

            SetFolderPath(folderPath, imageFiles);
        }
        private void StripBackgroundHandler(object sender, StripBackgroundEventArgs args)
        {
            XmlSerializerHelper.Serialize(args.BackgroundStrippingParams, GetBackgroundStrippingParamsFilePath(_folderPath));

            SetStripBackgroundParams(args.BackgroundStrippingParams);
        }
        private void AllImagesProcessedHandler(object sender, AllImgProcessedEventArgs args)
        {
            XmlSerializerHelper.Serialize(args.ClickInputs, GetClickInputsFilePath(_folderPath));

            SetClickInputs(args.ClickInputs);
        }
        private void ModelSettingsHandler(object sender, ModelSettingsEventArgs args)
        {
            if (_clickInputs != null && _clickInputs.ImageClickInputDetailsList != null && _clickInputs.ImageClickInputDetailsList.Count > 0)
            {
                SetModelSettings(args.MeshDensity, args.SaveProcessedImages, args.MaxAngleOfImageToProcessInDegress);
            }
            else
                MessageBox.Show("Image positions not available. Click on 'Reenter Click Positions' to add them.");
        }
        private void ProcessedHandler(object sender, ProcessedEventArgs args)
        {
            SetModelDisplay(args.ModelMeshAndPositionNeighbors, args.CreateMeshContract);
        }
        private void AutoConfigureEventHandler(object sender, AutoConfigureSettingsEventArgs args)
        {
            var autoconfiguredClickInputs = args.ClickInputs;
            if (_autoConfigureSettingsContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_autoConfigureSettingsContainer);
                _autoConfigureSettingsContainer = null;
            }
            DisplayClickCanvasContainer(autoconfiguredClickInputs);
        }
        private void AutoConfigureRequestedEventHandler(object sender, EventArgs args)
        {
            if (_clickCanvasContainer != null)
            {
                BaseGrid.Children.Remove(_clickCanvasContainer);
                _clickCanvasContainer = null;
            }        
            DisplayAutoConfigureSettings();
        }
        #endregion

        #region helpers to set values and go to next step
        private void SetFolderPath(string folderPath, string[] imageFiles)
        {
            _folderPath = folderPath;
            _firstImageFilePath = imageFiles[0];

            //remove existing display 
            if (_selectFolderContainer != null)
            {
                BaseGrid.Children.Remove(_selectFolderContainer);
                _selectFolderContainer = null;
            }

            //check if screen params can be deserialized from file
            var filePath = GetBackgroundStrippingParamsFilePath(_folderPath);
            var deserialized = XmlSerializerHelper.Deserialize<BackgroundStrippingParams>(filePath);
            if (deserialized != null)
                SetStripBackgroundParams(deserialized);
            else
                DisplayStripBackgroundContainer();
        }
        private void SetStripBackgroundParams(BackgroundStrippingParams @params)
        {
            _backgroundStrippingParams = @params;

            if (_stripBackgroundContainer != null)
            {
                BaseGrid.Children.Remove(_stripBackgroundContainer);
                _stripBackgroundContainer = null;
            }

            //check if click inputs can be deserialized from file
            var filePath = GetClickInputsFilePath(_folderPath);
            var deserialized = XmlSerializerHelper.Deserialize<ClickInputs>(filePath);
            if (deserialized != null)
                SetClickInputs(deserialized);
            else
                DisplayClickCanvasContainer(_clickInputs);
        }
        private void SetClickInputs(ClickInputs clickInputs)
        {
            _clickInputs = clickInputs;

            if (_clickCanvasContainer != null)
            {
                BaseGrid.Children.Remove(_clickCanvasContainer);
                _clickCanvasContainer = null;
            }

            //set new display
            DisplayModelSettingsContainer();
        }
        private void SetModelSettings(int meshGeometry, bool saveProcessedImages, int maxAngleToProcessForMesh)
        {
            _meshDensity = meshGeometry;
            _saveProcessedImages = saveProcessedImages;
            _maxAngleToProcessForMesh = maxAngleToProcessForMesh;
            if (_modelSettingsContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_modelSettingsContainer);
                _modelSettingsContainer = null;
            }

            DisplayProcessContainer();
        }
        private void SetModelDisplay(ModelMeshAndPositionNeighbors modelMeshAndPositionNeighbors, CreateMeshContract createMeshContract)
        {
            _modelMeshAndPositionNeighbors = modelMeshAndPositionNeighbors;
            _createMeshContract = createMeshContract;

            if (_processContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_processContainer);
                _processContainer = null;
            }

            DisplayModelViewContainer();
        }
        #endregion

        #region display container helpers
        private void DisplaySelectFolderContainer()
        {
            //this is the first step of the process so reset all values before display
            ResetValues();
            _selectFolderContainer = new SelectFolderContainer();
            _selectFolderContainer.SelectFolderEvent += SelectFolderHandler;
            BaseGrid.Children.Add(_selectFolderContainer);
            Grid.SetRow(_selectFolderContainer, 1);
            CancelButton.Visibility = Visibility.Hidden;
            HelpButton.Visibility = Visibility.Hidden;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.SelectFolder;
        }
        private void DisplayStripBackgroundContainer()
        {
            _stripBackgroundContainer = new StripBackgroundContainer(_firstImageFilePath, _invalidColor, _backgroundStrippingParams);
            _stripBackgroundContainer.StripBackgroundEvent += StripBackgroundHandler;
            BaseGrid.Children.Add(_stripBackgroundContainer);
            Grid.SetRow(_stripBackgroundContainer, 1);
            CancelButton.Content = "Cancel";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.BackgroundScreen;
        }
        private void DisplayClickCanvasContainer(ClickInputs clickInputs)
        {
            _clickCanvasContainer = new ClickCanvasContainer(_folderPath, clickInputs);
            _clickCanvasContainer.AllImagesProcessedEvent += AllImagesProcessedHandler;
            _clickCanvasContainer.AutoConfigureRequestedEvent += AutoConfigureRequestedEventHandler;
            BaseGrid.Children.Add(_clickCanvasContainer);
            Grid.SetRow(_clickCanvasContainer, 1);
            CancelButton.Content = "Cancel";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.ClickInputs;
        }
        private void DisplayAutoConfigureSettings()
        {
            _autoConfigureSettingsContainer = new AutoConfigureContainer(_folderPath, _backgroundStrippingParams, _invalidColor);
            _autoConfigureSettingsContainer.AutoConfigured+= AutoConfigureEventHandler;
            BaseGrid.Children.Add(_autoConfigureSettingsContainer);
            Grid.SetRow(_autoConfigureSettingsContainer, 1);
            CancelButton.Content = "Cancel";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.AutoConfigure;
        }
        private void DisplayModelSettingsContainer()
        {
            _modelSettingsContainer = new ModelSettingsContainer();
            _modelSettingsContainer.ModelSettingsEvent += ModelSettingsHandler;
            _modelSettingsContainer.ChangeImageFolderEvent += ChangeImageFolderEventFromModelSettings;
            _modelSettingsContainer.ReenterScreenSettingsEvent += ReenterScreenSettingsFromModelSettings;
            _modelSettingsContainer.ReenterClickPositionsEvent += ReenterClickPositionsFromModelSettings;
            BaseGrid.Children.Add(_modelSettingsContainer);
            Grid.SetRow(_modelSettingsContainer, 1);
            CancelButton.Content = "Cancel";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.CreateReady;
        }

        private void DisplayProcessContainer()
        {
            var contract = GetCreateMeshContract();
            _processContainer = new ProcessContainer(contract);
            _processContainer.ProcessedEvent += ProcessedHandler;
            _processContainer.SuccessfullyCancelled += ProcessCancelled;
            BaseGrid.Children.Add(_processContainer);
            Grid.SetRow(_processContainer, 1);
            CancelButton.Content = "Cancel";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.ModelCreationInProcess;
        }

        private void DisplayModelViewContainer()
        {
            _modelContainer = new ModelContainer(_modelMeshAndPositionNeighbors, _createMeshContract);
            _modelContainer.RegenerateEvent += RegenerateModel;
            BaseGrid.Children.Add(_modelContainer);
            Grid.SetRow(_modelContainer, 1);
            CancelButton.Content = "New";
            CancelButton.Visibility = Visibility.Visible;
            HelpButton.Visibility = Visibility.Visible;
            ScreenTypeStatusHelper.CurrentScreenTypesEnum = ScreenTypesEnum.MeshCreated;
        }
        #endregion

        #region helpers
        private CreateMeshContract GetCreateMeshContract()
        {
            return new CreateMeshContract
            {
                ImageFolder = _folderPath,
                InvalidColor = _invalidColor,
                LogFilePath = "",
                PtDensity = _meshDensity.Value,
                BackgroundStrippingParams = _backgroundStrippingParams,
                VariationIn3DCoordinates = 5.0,
                ClickInputs = _clickInputs,
                SaveProcessedImages = _saveProcessedImages,
                BottomPaddingPercent = _clickInputs.GetBottomPaddingPercent(),
                MinImageHeightRatio = _clickInputs.GetMinImageHeightRatio(),
                MaxAngleOfImageToProcessInDegress = _maxAngleToProcessForMesh

            };
        }
        private static string GetClickInputsFilePath(string folderPath)
        {
            const string fileName = @"ClickInputs.xml";
            return string.Format(@"{0}\{1}", folderPath, fileName);
        }
        private static string GetBackgroundStrippingParamsFilePath(string folderPath)
        {
            const string fileName = @"BackgroundStrippingParams.xml";
            return string.Format(@"{0}\{1}", folderPath, fileName);
        }
        #endregion

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            if (!ConfirmExit()) return;

            if (_stripBackgroundContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_stripBackgroundContainer);
                _stripBackgroundContainer = null;

                if (_backgroundStrippingParams != null)
                    DisplayModelSettingsContainer();
                else
                    DisplaySelectFolderContainer();
                return;
            }

            if (_processContainer != null)
            {
                _processContainer.Cancel();
                return;
            }

            if(_clickCanvasContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_clickCanvasContainer);
                _clickCanvasContainer = null;

                if(_clickInputs != null)
                    DisplayModelSettingsContainer();
                else
                    DisplaySelectFolderContainer();

                return;
            }

            if (_modelSettingsContainer != null)
            {
                //remove existing display 
                BaseGrid.Children.Remove(_modelSettingsContainer);
                _modelSettingsContainer = null;
                DisplaySelectFolderContainer();
                return;
            }

            if (_modelContainer != null)
            {
                BaseGrid.Children.Remove(_modelContainer);
                _modelContainer = null;
                DisplaySelectFolderContainer();
                return;
            }

            if(_autoConfigureSettingsContainer != null)
            {
                BaseGrid.Children.Remove(_autoConfigureSettingsContainer);
                _autoConfigureSettingsContainer = null;
                DisplayClickCanvasContainer(_clickInputs);
                return;
            }
        }
        private void ProcessCancelled(object sender, EventArgs args)
        {
            if (_processContainer != null)
            {
                BaseGrid.Children.Remove(_processContainer);
                _processContainer = null;
            }
            DisplayModelSettingsContainer();
        }

        private void ChangeImageFolderEventFromModelSettings(object sender, EventArgs args)
        {
            BaseGrid.Children.Remove(_modelSettingsContainer);
            _modelSettingsContainer = null;
            DisplaySelectFolderContainer();
        }

        private void ReenterScreenSettingsFromModelSettings(object sender, EventArgs args)
        {
            BaseGrid.Children.Remove(_modelSettingsContainer);
            _modelSettingsContainer = null;
            DisplayStripBackgroundContainer();
        }

        private void ReenterClickPositionsFromModelSettings(object sender, EventArgs args)
        {
            BaseGrid.Children.Remove(_modelSettingsContainer);
            _modelSettingsContainer = null;
            DisplayClickCanvasContainer(_clickInputs);
        }
        private static bool ConfirmExit()
        {
            return MessageBox.Show("Are you sure you want to cancel?", "Cancel Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void RegenerateModel(object sender, EventArgs args)
        {
            if (_modelContainer != null)
            {
                BaseGrid.Children.Remove(_modelContainer);
                _modelContainer = null;
            }

            //set new display
            DisplayModelSettingsContainer();
        }

        private void HelpLinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }

        private void WebsiteLinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }

        private void Help(object sender, RoutedEventArgs e)
        {
            switch (ScreenTypeStatusHelper.CurrentScreenTypesEnum)
            {
                case ScreenTypesEnum.SelectFolder:
                    break;
                case ScreenTypesEnum.BackgroundScreen:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.BackgroundScreen);
                    break;
                case ScreenTypesEnum.ClickInputs:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.ClickInputs);
                    break;
                case ScreenTypesEnum.AutoConfigure:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.AutoConfigure);
                    break;
                case ScreenTypesEnum.CreateReady:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.CreateReady);
                    break;
                case ScreenTypesEnum.MeshCreated:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.MeshCreated);
                    break;
                case ScreenTypesEnum.ModelCreationInProcess:
                    StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.ModelCreationInProcess);
                    break;
                case ScreenTypesEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ClickStartScreenDialogOptions(object sender, RoutedEventArgs e)
        {
            var startScreenDialogOptionsWindow = new ScreenStartFlagsConfigurationDialog { Height = 220, Width = 400 };
            var result = startScreenDialogOptionsWindow.ShowDialog();
            if (!result.HasValue || !result.Value) return;
        }
    }
}
