using System.Collections.Generic;
using System.Windows;

namespace Ajubaa.IBModeler.Help
{
    /// <summary>
    /// Interaction logic for ScreenStartFlagsConfigurationDialog.xaml
    /// </summary>
    public partial class ScreenStartFlagsConfigurationDialog : Window
    {
        public ScreenStartFlagsConfigurationDialog()
        {
            InitializeComponent();
            Loaded += ScreenStartFlagsConfigurationDialogLoaded;
        }

        void ScreenStartFlagsConfigurationDialogLoaded(object sender, RoutedEventArgs e)
        {
            var hideFlags = HideScreenStartDialogFlags.Flags;
            var sourceList = new List<CheckableItem<string>>
            {
                new CheckableItem<string>("Show help on load of strip background screen"){IsChecked = !hideFlags.BackgroundScreen},
                new CheckableItem<string>("Show help on load of click inputs screen"){IsChecked = !hideFlags.ClickInputs   },
                //new CheckableItem<string>("Show help on load of autoconfigure screen"){IsChecked = !hideFlags.AutoConfigure},
                new CheckableItem<string>("Show help on load of create model screen"){IsChecked = !hideFlags.CreateReady    },
                new CheckableItem<string>("Show help on load of model created screen"){IsChecked = !hideFlags.MeshCreated},
                //new CheckableItem<string>("Show help on load of add skin dialog"){IsChecked = !hideFlags.AddSkinDialog},
            };
            list.ItemsSource = sourceList;
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            var source = list.ItemsSource as IList<CheckableItem<string>>;
            if(source != null)
            {
                var saveList = new HideScreenStartDialogFlags
                {
                    BackgroundScreen = !source[0].IsChecked,
                    ClickInputs = !source[1].IsChecked,
                    //AutoConfigure = !source[2].IsChecked,
                    CreateReady = !source[2].IsChecked,
                    MeshCreated = !source[3].IsChecked,
                    //AddSkinDialog = !source[4].IsChecked
                };
                HideScreenStartDialogFlags.Flags = saveList;
            }
            Close();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
