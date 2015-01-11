using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ajubaa.IBModeler.Help;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for AddSkin.xaml
    /// </summary>
    public partial class AddSkin : Window
    {
        private readonly TextureTypeEnum _currentTextureType;

        public AddSkin(TextureTypeEnum currentTextureType)
        {
            //default texture selection should be front and back
            _currentTextureType = currentTextureType == TextureTypeEnum.None? TextureTypeEnum.FrontAndBack : currentTextureType;
            InitializeComponent();
            Loaded += AddSkinLoaded;
        }

        void AddSkinLoaded(object sender, RoutedEventArgs e)
        {
            addSkinOption.SelectedIndex = (int) _currentTextureType;
            StartScreenHelpDialogHelper.ShowScreenStartHelpDialogIfFlagIsTrue(ScreenStartDialogTypes.AddSkinDialog);
        }

        public Tuple<int, byte, byte, byte> ReturnValue { get; private set; }

        private void AddSkinButtonClicked(object sender, RoutedEventArgs e)
        {
            var selectedIndex = addSkinOption.SelectedIndex;
            switch(selectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 4:
                    ReturnValue = new Tuple<int, byte, byte, byte>(selectedIndex, 0,0,0);
                    break;
                case 3:
                    var color = skinColorCombo.SelectedColor as SolidColorBrush;
                    if(color == null)
                    {
                        MessageBox.Show("Please select a color from the provided dropdown.");
                        return;
                    }
                    ReturnValue = new Tuple<int, byte, byte, byte>(3,color.Color.R, color.Color.G, color.Color.B);
                    break;
            }
            this.DialogResult = true;
            Close();
        }

        private void CancelButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListBoxItem_Selected_Front_Back(object sender, RoutedEventArgs e)
        {
            HelpImage.Source = new BitmapImage(new Uri("images/skin_count_selection/front_back.png", UriKind.Relative));
            skinColorCombo.Visibility = Visibility.Collapsed;
            HelpImage.Visibility = Visibility.Visible;
        }

        private void ListBoxItem_Selected_Four(object sender, RoutedEventArgs e)
        {
            HelpImage.Source = new BitmapImage(new Uri("images/skin_count_selection/four_corners.png", UriKind.Relative));
            skinColorCombo.Visibility = Visibility.Collapsed;
            HelpImage.Visibility = Visibility.Visible;
        }

        private void ListBoxItem_Selected_Eight(object sender, RoutedEventArgs e)
        {
            HelpImage.Source = new BitmapImage(new Uri("images/skin_count_selection/eight_corners.png", UriKind.Relative));
            skinColorCombo.Visibility = Visibility.Collapsed;
            HelpImage.Visibility = Visibility.Visible;
        }

        private void ListBoxItem_Selected_Single_Color(object sender, RoutedEventArgs e)
        {
            skinColorCombo.Visibility = Visibility.Visible;
            HelpImage.Visibility = Visibility.Collapsed;
        }

        private void ListBoxItem_Selected_No_Skin(object sender, RoutedEventArgs e)
        {
            skinColorCombo.Visibility = Visibility.Collapsed;
            HelpImage.Visibility = Visibility.Collapsed;
        }

        private void HelpButtonClicked(object sender, RoutedEventArgs e)
        {
            StartScreenHelpDialogHelper.ShowHelpDialogWindow(ScreenStartDialogTypes.AddSkinDialog);
        }
    }
}
