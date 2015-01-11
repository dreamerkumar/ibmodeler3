using System.Windows;
using Ajubaa.IBModeler.Help;
using Ajubaa.TextureGenerator;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for AdjustSkin.xaml
    /// </summary>
    public partial class AdjustSkin
    {
        private readonly MinAndMaxTexCoodValueLimits[] _xCoodRangesForEachImage;
        private readonly AdjustSkinParams _userXCoodRangesFor4ImageTexture;
        private bool _leftPhotoExists;
        public AdjustSkinParams ReturnValue { get; set; }

        public AdjustSkin(MinAndMaxTexCoodValueLimits[] xCoodRangesForEachImage, AdjustSkinParams userXCoodRangesFor4ImageTexture)
        {
            _xCoodRangesForEachImage = xCoodRangesForEachImage;
            _userXCoodRangesFor4ImageTexture = userXCoodRangesFor4ImageTexture;
            InitializeComponent();
            Loaded += AdjustSkinLoaded;
        }

        void AdjustSkinLoaded(object sender, RoutedEventArgs e)
        {
            if (_xCoodRangesForEachImage == null || !(_xCoodRangesForEachImage.Length == 3 || _xCoodRangesForEachImage.Length == 4)) return;

            _leftPhotoExists = _xCoodRangesForEachImage.Length == 4;

            FrontImageMinMaxTexCoodSlider.SetRange(_xCoodRangesForEachImage[0], "Front Photo");
            RightImageMinMaxTexCoodSlider.SetRange(_xCoodRangesForEachImage[1], "Right Photo");
            BackImageMinMaxTexCoodSlider.SetRange(_xCoodRangesForEachImage[2], "Back Photo");

            if (_leftPhotoExists)
                LeftImageMinMaxTexCoodSlider.SetRange(_xCoodRangesForEachImage[3], "Left Photo");
            else
                LeftImageMinMaxTexCoodSlider.Visibility = Visibility.Hidden;

            if(_userXCoodRangesFor4ImageTexture != null)
            {
                SetPreviouslyDefinedValues(_userXCoodRangesFor4ImageTexture.FrontPhotoTexCoodValueLimits, FrontImageMinMaxTexCoodSlider);
                SetPreviouslyDefinedValues(_userXCoodRangesFor4ImageTexture.BackPhotoTexCoodValueLimits, BackImageMinMaxTexCoodSlider);
                SetPreviouslyDefinedValues(_userXCoodRangesFor4ImageTexture.LeftPhotoTexCoodValueLimits, LeftImageMinMaxTexCoodSlider);
                SetPreviouslyDefinedValues(_userXCoodRangesFor4ImageTexture.RightPhotoTexCoodValueLimits, RightImageMinMaxTexCoodSlider);
            }
        }

        private static void SetPreviouslyDefinedValues(MinAndMaxTexCoodValueLimits previouslyDefinedValues, SliderForAdjustSkin slider)
        {
            if (previouslyDefinedValues == null || slider == null) return;

            slider.SliderMin.Value = previouslyDefinedValues.Min;
            slider.SliderMax.Value = previouslyDefinedValues.Max;
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            var returnValue = new AdjustSkinParams
            {
                FrontPhotoTexCoodValueLimits = GetSliderValue(FrontImageMinMaxTexCoodSlider),
                BackPhotoTexCoodValueLimits = GetSliderValue(BackImageMinMaxTexCoodSlider),
                LeftPhotoTexCoodValueLimits = GetSliderValue(LeftImageMinMaxTexCoodSlider),
                RightPhotoTexCoodValueLimits = GetSliderValue(RightImageMinMaxTexCoodSlider)
            };
            ReturnValue = returnValue;
            DialogResult = true;
            Close();
        }

        private static MinAndMaxTexCoodValueLimits GetSliderValue(SliderForAdjustSkin slider)
        {
            return slider != null && slider.Visibility == Visibility.Visible ? new MinAndMaxTexCoodValueLimits
                { Min = slider.SliderMin.Value, Max = slider.SliderMax.Value } : null;
        }

        private void HelpButtonClicked(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.AdjustSkinDialog);
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListBoxItemSelectedLeftRight(object sender, RoutedEventArgs e)
        {
            if (FrontImageMinMaxTexCoodSlider != null) FrontImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (BackImageMinMaxTexCoodSlider != null) BackImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (RightImageMinMaxTexCoodSlider != null) RightImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (LeftImageMinMaxTexCoodSlider != null) LeftImageMinMaxTexCoodSlider.Visibility = _leftPhotoExists ? Visibility.Visible : Visibility.Hidden;
        }

        private void ListBoxItemSelectedFrontBack(object sender, RoutedEventArgs e)
        {
            if (FrontImageMinMaxTexCoodSlider != null) FrontImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (BackImageMinMaxTexCoodSlider != null) BackImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (LeftImageMinMaxTexCoodSlider != null) LeftImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
            if (RightImageMinMaxTexCoodSlider != null) RightImageMinMaxTexCoodSlider.Visibility = Visibility.Visible;
        }
    }
}
