using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Ajubaa.Common._3DModelView;
using Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints;
using Ajubaa.IBModeler.Help;
using Ajubaa.IBModeler.Processor;
using Ajubaa.IBModeler.Processor.Save;
using Ajubaa.IBModeler.Processor.Smoothen;
using Ajubaa.TextureGenerator;
using Microsoft.Win32;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Image = System.Drawing.Image;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void RegenerateEventHandler(object sender, EventArgs args);

    /// <summary>
    /// Interaction logic for ModelContainer.xaml
    /// </summary>
    public partial class ModelContainer
    {
        internal event RegenerateEventHandler RegenerateEvent;

        internal void InvokeRegenerateEvent(EventArgs args)
        {
            var handler = RegenerateEvent;
            if (handler != null) handler(this, args);
        }

        private readonly ModelMeshAndPositionNeighbors _modelMeshAndPositionNeighbors;
        private readonly CreateMeshContract _createMeshContract;
        private int _currentSmootheningCount = 0;
        private GeometryModel3D _currentModel;
        private Bitmap _currentTextureBitmap = null;
        private MinAndMaxTexCoodValueLimits[] _xCoodRangesFor4ImageTexture;
        private AdjustSkinParams _userXCoodRangesFor4ImageTexture;

        private bool? _hasValidLicense = null;
        public bool HasValidLicense
        {
            get { return true; }
            private set { _hasValidLicense = value; }
        }

        private string _productId = null;
        private TextureCoordinatesAndBitmap _2CornerTextureAndBitmap;
        private TextureCoordinatesAndBitmap _4CornerTextureAndBitmap;
        private TextureCoordinatesAndBitmap _8CornerTextureAndBitmap;
        private TextureTypeEnum _currentTextureType = TextureTypeEnum.None;

        public string ProductId
        {
            get
            {
                if (_productId == null) _productId = HardwareId.GetHardWareId();
                return _productId;
            }
        }

        public ModelContainer(ModelMeshAndPositionNeighbors modelMeshAndPositionNeighbors, CreateMeshContract createMeshContract)
        {
            InitializeComponent();
            
            _modelMeshAndPositionNeighbors = modelMeshAndPositionNeighbors;
            _createMeshContract = createMeshContract;

            //smoothen two times by default
            const int defaultSmoothening = 2;
            var newMesh = GetNewModifiableMeshWithNoSmoothing();
            var newModel = SmoothenProcessor.SmoothenMesh(defaultSmoothening, _modelMeshAndPositionNeighbors.PositionNeighbors, newMesh);
            _currentSmootheningCount = defaultSmoothening;
            SetSmootheningCountDisplayToCurrent();

            _currentModel = new GeometryModel3D
            {
                Geometry = newModel,
                BackMaterial = new DiffuseMaterial(Brushes.DarkOliveGreen)
            };

            SetNoTextureDisplay();

            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.MeshCreated);

            //scale left grid a little bit for monitors with resolution higher than 800*600
            try
            {
                var thisWindow = Window.GetWindow(this);

                var wpfScreen = WpfScreen.GetScreenFrom(thisWindow);
                var screenWidth = wpfScreen.DeviceBounds.Width;
                var scaleValue = 1.0;
                if (screenWidth >= 1200)
                    scaleValue = 1.4;
                else if (screenWidth >= 1000)
                {
                    scaleValue = 1.2;
                }
                var scaleTransform = new ScaleTransform(scaleValue, scaleValue);

                LeftGrid.LayoutTransform = scaleTransform;

                thisWindow.WindowState = WindowState.Maximized;
            }
            catch (Exception)
            { }
        }

        private void SetSmootheningCountDisplayToCurrent()
        {
            SmootheningCountDisplay.Text = _currentSmootheningCount.ToString();
            DisableSmoothenButton();
        }

        private void UpdateModelDisplay(GeometryModel3D model)
        {
            _currentModel = model;

            _trackPort.ModelData = GetModelData(_currentModel);
            _trackPort.UpdateModelDisplay();
        }

        private static ModelData GetModelData(GeometryModel3D geometryModel3D)
        {
            var bounds = geometryModel3D.Bounds;

            //calculate the center point from the bounds
            var centerPt = new Point3D
            {
                X = bounds.X + bounds.SizeX / 2.0,
                Y = bounds.Y + bounds.SizeY / 2.0,
                Z = bounds.Z + bounds.SizeZ / 2.0
            };
            var max = GetMaximum(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            
            var model3DGroup = new Model3DGroup();
            model3DGroup.Children.Add(geometryModel3D);

             return new ModelData
            {
                Model3DGroupDataForDisplay = model3DGroup,
                CenterPt = centerPt,
                MaximumSideLength = max
            };
        }

        private static MaterialGroup GetMaterial(Bitmap textureBitmap)
        {
            var materialGroup = new MaterialGroup();
            if (textureBitmap != null)
            {
                var memoryStream = new MemoryStream();
                textureBitmap.Save(memoryStream, ImageFormat.Bmp);

                // Create a BitmapImage with the Stream. 
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();

                //use this as the texture image 
                var brush = new ImageBrush(bitmapImage)
                {
                    //mapping will go wrong without setting viewport units
                    ViewportUnits = BrushMappingMode.Absolute
                };

                var emissiveMaterial = new EmissiveMaterial( brush) 
                {//Color = Color.FromArgb(255, 255, 255, 255)
                };
                materialGroup.Children.Add(emissiveMaterial);

                var diffuseMaterial = new DiffuseMaterial( brush)
                {
                    //Color = Color.FromArgb(255, 255, 255, 255),
                    //AmbientColor = Color.FromArgb(255, 255, 255, 255)
                };
                materialGroup.Children.Add(diffuseMaterial);

                //commenting out shiningness
                //var specularMaterial = new SpecularMaterial(brush, 185.33);
                //materialGroup.Children.Add(specularMaterial);
                
                return materialGroup;
            }
            //return a solid color brush
            var solidColorBrush = new SolidColorBrush { Color = Color.FromRgb(200, 200, 200) };
            materialGroup.Children.Add(new DiffuseMaterial(solidColorBrush));
            return materialGroup;
        }

        private static double GetMaximum(double val1, double val2, double val3)
        {
            var max = val1;
            if (val2 > max)
                max = val2;
            if (val3 > max)
                max = val3;
            return max;
        }

        private void DecreaseClick(object sender, RoutedEventArgs e)
        {
            var displayedCt = Convert.ToInt32(SmootheningCountDisplay.Text);
            if (displayedCt <= 0) return;

            displayedCt--;
            SmootheningCountDisplay.Text = displayedCt.ToString();

            EnableDisableSmoothingButton();
        }

        private void EnableDisableSmoothingButton()
        {
            if (IsDisplayTextEqualToCurrentSmoothingValue()) 
                DisableSmoothenButton(); 
            else 
                EnableSmoothenButton();
        }

        private void EnableSmoothenButton()
        {
            SmoothenButton.IsEnabled = true;
            SmoothenButton.Opacity = 1.0;
            SmoothenHelpText.Text = "Hit the smoothen button to apply the above selection";
        }

        private void DisableSmoothenButton()
        {
            SmoothenButton.IsEnabled = false;
            SmoothenHelpText.Text = "Change above counter value to enable smoothen button";
            SmoothenButton.Opacity = .7;
        }

        private void IncreaseClick(object sender, RoutedEventArgs e)
        {
            var displayedCt = Convert.ToInt32(SmootheningCountDisplay.Text);
            displayedCt++;
            SmootheningCountDisplay.Text = displayedCt.ToString();
            EnableDisableSmoothingButton();
        }

        private bool IsDisplayTextEqualToCurrentSmoothingValue()
        {
            var requestedSmoothenCount = Convert.ToInt32(SmootheningCountDisplay.Text);

            return (requestedSmoothenCount == _currentSmootheningCount);
        }

        private void Smoothen(object sender, RoutedEventArgs e)
        {
            if (IsDisplayTextEqualToCurrentSmoothingValue())
                return;

            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentGrid);

            var requestedSmoothenCount = Convert.ToInt32(SmootheningCountDisplay.Text);

            MeshGeometry3D newMesh = null;

            if (requestedSmoothenCount == 0)
            {
                newMesh = GetNewModifiableMeshWithNoSmoothing();
                _currentModel.Geometry = newMesh;
            }
            else if(requestedSmoothenCount < _currentSmootheningCount)
            {
                newMesh = GetNewModifiableMeshWithNoSmoothing();

                var newModel = SmoothenProcessor.SmoothenMesh(requestedSmoothenCount, _modelMeshAndPositionNeighbors.PositionNeighbors, newMesh);

                _currentModel.Geometry = newModel;
            }
            else if (requestedSmoothenCount > _currentSmootheningCount)
            {
                newMesh = (MeshGeometry3D) _currentModel.Geometry;
                var remainingCt = requestedSmoothenCount - _currentSmootheningCount;

                var newModel = SmoothenProcessor.SmoothenMesh(remainingCt, _modelMeshAndPositionNeighbors.PositionNeighbors, newMesh);
                
                _currentModel.Geometry = newModel;
            }

            _currentModel.Geometry = newMesh;

            _currentSmootheningCount = requestedSmoothenCount;
            
            UpdateModelDisplay(_currentModel);

            SetSmootheningCountDisplayToCurrent();

            quickProcessingWindow.Close();

            CurrentSmoothenCountMsg.Text = string.Format("Model is smoothened {0} times", _currentSmootheningCount);
        }

        private MeshGeometry3D GetNewModifiableMeshWithNoSmoothing()
        {
            var meshGeometry3D = _modelMeshAndPositionNeighbors.GetModifiableMesh();
            
            //copy over texture coordinates if they exist
            if (_currentModel != null)
            {
                var currentMesh = _currentModel.Geometry as MeshGeometry3D;
                if(currentMesh != null)
                {
                    meshGeometry3D.TextureCoordinates = currentMesh.TextureCoordinates;
                }
            }
            
            return meshGeometry3D;
        }

        private void SaveAsXaml(object sender, RoutedEventArgs e)
        {
            if (!LicenseCheckPassed())
                return;

            var dlg = new SaveFileDialog
            {
                FileName = "model",
                DefaultExt = ".xaml",
                Filter = "(Xaml file)|*.xaml",
                Title = "Save model as a xaml file. Texture will be saved as .bmp",
                CheckPathExists = true,
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != true) return;

            var filePath = dlg.FileName;
            SaveProcessor.SaveAsXaml(filePath, (MeshGeometry3D)_currentModel.Geometry, _currentTextureBitmap);
        }

        private bool LicenseCheckPassed()
        {
           return true;
        }

        private void SaveAs3DS(object sender, RoutedEventArgs e)
        {
            if (!LicenseCheckPassed())
                return;

            var dlg = new SaveFileDialog
            {
                FileName = "model",
                DefaultExt = ".3ds",
                Filter = "(3ds file)|*.3ds",
                Title = "Save model as a 3ds file. Texture will be saved as .jpg",
                CheckPathExists = true,
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != true) return;

            var filePath = dlg.FileName;
            SaveProcessor.SaveAs3DS(filePath, _currentModel, _currentTextureBitmap);
        }

        private void Regenerate(object sender, RoutedEventArgs e)
        {
            InvokeRegenerateEvent(new EventArgs());
        }

        private void Troubleshoot(object sender, RoutedEventArgs e)
        {
            var window = new Ajubaa.IBModeler.Help.Troubleshoot();
            window.Show();
        }

        private void AddSkinButtonClicked(object sender, RoutedEventArgs e)
        {
            const string changeSkinText = "Change Skin";
            const string addSkinText = "Add Skin";

            var skinWindow = new AddSkin(_currentTextureType);
            var result = skinWindow.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            var selectedParams = skinWindow.ReturnValue;
            switch (selectedParams.Item1)
            {
                case 0:
                    Generate2CornerTexture();
                    ApplyTextureOnCurrentModel(_2CornerTextureAndBitmap);
                    AddSkinTextBox.Text = changeSkinText;
                    _currentTextureType = TextureTypeEnum.FrontAndBack;
                    break;
                case 1:
                    Generate4CornerTexture();
                    ApplyTextureOnCurrentModel(_4CornerTextureAndBitmap);
                    AddSkinTextBox.Text = changeSkinText;
                    _currentTextureType = TextureTypeEnum.FourCorners;
                    break;
                case 2:
                    Generate8CornerTexture();
                    ApplyTextureOnCurrentModel(_8CornerTextureAndBitmap);
                    AddSkinTextBox.Text = changeSkinText;
                    _currentTextureType = TextureTypeEnum.EightCorners;
                    break;
                case 3:
                    _currentTextureBitmap = GetSingleColorTextureBitmap(selectedParams.Item2, selectedParams.Item3, selectedParams.Item4);
                    _currentModel.Material = GetMaterial(_currentTextureBitmap);
                    UpdateModelDisplay(_currentModel);
                    AddSkinTextBox.Text = changeSkinText;
                    _currentTextureType = TextureTypeEnum.SingleColor;
                    break;
                case 4:
                    SetNoTextureDisplay();
                    AddSkinTextBox.Text = addSkinText;
                    break;
            }
            
            AdjustSkinButton.Visibility = _currentTextureType == TextureTypeEnum.FourCorners && ThreeOrMoreImagesExist() ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool ThreeOrMoreImagesExist()
        {
            return _xCoodRangesFor4ImageTexture.Length == 3 || _xCoodRangesFor4ImageTexture.Length == 4;
        }

        private void SetNoTextureDisplay()
        {
            var color = Colors.Gray;
            _currentTextureBitmap = GetSingleColorTextureBitmap(color.R, color.G, color.B);
            _currentTextureType = TextureTypeEnum.None;
            _currentModel.Material = new DiffuseMaterial(Brushes.Gray);
            UpdateModelDisplay(_currentModel);
        }

        private void Generate2CornerTexture()
        {
            if (_2CornerTextureAndBitmap != null) return;
            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentGrid);
            var addTextureInfoForFrontAndBack = MainProcessor.GetAddTextureInfoForFrontAndBackImage(_createMeshContract);
            if (addTextureInfoForFrontAndBack == null)
            {
                MessageBox.Show("Error in generating textures: Cannot identify front and back photos.\nPlease check if photos are available from front to back.");
                quickProcessingWindow.Close();
                return;
            }
            _2CornerTextureAndBitmap = TextureProcessor.GenerateTexture(addTextureInfoForFrontAndBack, (MeshGeometry3D)_currentModel.Geometry, "");
            quickProcessingWindow.Close();
        }

        private void Generate4CornerTexture()
        {
            if (_4CornerTextureAndBitmap != null) return;
            var indices = MainProcessor.GetIndicesFor4CornerTexture(_createMeshContract.ClickInputs.Angles);
            if (indices == null)
            {
                MessageBox.Show("Error in generating textures: Cannot identify photos for four corners.\nPlease check if photos are available for a full 360 degree view.");
                return;
            }
            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentGrid);
            var add4CornerTexture = MainProcessor.GetAddTextureInfoForIndexCollection(_createMeshContract, indices);
            _4CornerTextureAndBitmap = TextureProcessor.GenerateTexture(add4CornerTexture, (MeshGeometry3D)_currentModel.Geometry, "");
            _xCoodRangesFor4ImageTexture = _4CornerTextureAndBitmap.XCoodRangesForEachImage;
            quickProcessingWindow.Close();
            if (indices.Length < 4)
            {
                MessageBox.Show(string.Format("Texture generated for {0} images as only {0} out of 4 images could be identified.", indices.Length));
            }
        }

        private void Generate8CornerTexture()
        {
            if(_8CornerTextureAndBitmap != null) return;
            var indices = MainProcessor.GetIndicesFor8CornerTexture(_createMeshContract.ClickInputs.Angles);
            if (indices == null)
            {
                MessageBox.Show("Error in generating textures: Cannot identify photos for eight corners.\nPlease check if photos are available for a full 360 degree view.");
                return;
            }
            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentGrid);
            var add8CornerTexture = MainProcessor.GetAddTextureInfoForIndexCollection(_createMeshContract, indices);
            _8CornerTextureAndBitmap = TextureProcessor.GenerateTexture(add8CornerTexture, (MeshGeometry3D)_currentModel.Geometry, "");
            quickProcessingWindow.Close();
            if (indices.Length < 8)
            {
                MessageBox.Show(string.Format("Texture generated for {0} images as only {0} out of 8 images could be identified.", indices.Length));
            }
        }

        private void ApplyTextureOnCurrentModel(TextureCoordinatesAndBitmap modelTextureAndBitmap)
        {
            if (modelTextureAndBitmap == null) return;
            _currentTextureBitmap = modelTextureAndBitmap.Bitmap;
            _currentModel.Material = GetMaterial(_currentTextureBitmap);
            ((MeshGeometry3D) _currentModel.Geometry).TextureCoordinates = modelTextureAndBitmap.TextureCoordinates;
            UpdateModelDisplay(_currentModel);
        }

        private static Bitmap GetSingleColorTextureBitmap(byte r, byte g, byte b)
        {
            const int widthAndHeight = 64;
            var bitmap = new Bitmap(widthAndHeight, widthAndHeight);
            using (var gfx = Graphics.FromImage(bitmap))
            using (var brush = new SolidBrush(System.Drawing.Color.FromArgb(r, g, b)))
            {
                gfx.FillRectangle(brush, 0, 0, widthAndHeight, widthAndHeight);
            }
            return bitmap;
        }

        #region unused methods originally creating for setting any given file on disk as texture
        private void WoodTexture(object sender, RoutedEventArgs e)
        {
            ImageTexture(@"\images\wood.jpg");
        }

        private void StoneTexture(object sender, RoutedEventArgs e)
        {
            ImageTexture(@"\images\stone.jpg");
        }

        private void PatternTexture(object sender, RoutedEventArgs e)
        {
            ImageTexture(@"\images\pattern.jpg");
        }

        private void FurTexture(object sender, RoutedEventArgs e)
        {
            ImageTexture(@"\images\fur.jpg");
        }

        private void ImageTexture(string imageName)
        {
            var image = Image.FromFile(Ajubaa.Common.ExecutionDirInfoHelper.GetExecutionPath() + imageName);

            //created new bitmap as GetMaterial was giving error on just trying to cast image to bitmap
            _currentTextureBitmap = new Bitmap(image, image.Width, image.Height);

            _currentModel.Material = GetMaterial(_currentTextureBitmap);

            UpdateModelDisplay(_currentModel);
        }
        #endregion

        private void AdjustSkin(object sender, RoutedEventArgs e)
        {
            if (_currentTextureType != TextureTypeEnum.FourCorners) return;

            var adjustSkinWindow = new AdjustSkin(_xCoodRangesFor4ImageTexture, _userXCoodRangesFor4ImageTexture);
            var result = adjustSkinWindow.ShowDialog();
            if (!result.HasValue || !result.Value || adjustSkinWindow.ReturnValue == null) return;

            var returnValue = adjustSkinWindow.ReturnValue;

            if (_4CornerTextureAndBitmap == null) return;
            var indices = MainProcessor.GetIndicesFor4CornerTexture(_createMeshContract.ClickInputs.Angles);
            if (indices == null)
            {
                MessageBox.Show("Error in generating textures: Cannot identify photos for four corners.\nPlease check if photos are available for a full 360 degree view.");
                return;
            }

            var quickProcessingWindow = new QuickProcessingWindowHelper(ParentGrid);
            var add4CornerTexture = MainProcessor.GetAddTextureInfoForIndexCollection(_createMeshContract, indices);
            if (add4CornerTexture.ImageInfos != null)
            {
                SetUserSuppliedLimit(0, add4CornerTexture, returnValue.FrontPhotoTexCoodValueLimits);
                SetUserSuppliedLimit(1, add4CornerTexture, returnValue.RightPhotoTexCoodValueLimits);
                SetUserSuppliedLimit(2, add4CornerTexture, returnValue.BackPhotoTexCoodValueLimits);
                SetUserSuppliedLimit(3, add4CornerTexture, returnValue.LeftPhotoTexCoodValueLimits);
            }
            _userXCoodRangesFor4ImageTexture = returnValue;

            _4CornerTextureAndBitmap = TextureProcessor.GenerateTexture(add4CornerTexture, (MeshGeometry3D)_currentModel.Geometry, "");
            quickProcessingWindow.Close();
            ApplyTextureOnCurrentModel(_4CornerTextureAndBitmap);
        }

        private static bool ValidLimitSupplied(MinAndMaxTexCoodValueLimits limits)
        {
            return limits != null && limits.Min >= 0 && limits.Max > 0;
        }

        private static void SetUserSuppliedLimit(int index, AddTextureInfo target, MinAndMaxTexCoodValueLimits limits)
        {
            if (target.ImageInfos.Length >= index+1 && ValidLimitSupplied(limits))
                target.ImageInfos[index].AllowedXLimits = new MinAndMaxTexCoodValueLimits { Min = limits.Min, Max = limits.Max };
        }
    }
}
