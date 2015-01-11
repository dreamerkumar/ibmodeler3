using System.Windows;
using System.Windows.Forms;
using Ajubaa.Common;
using Ajubaa.IBModeler.Help;

namespace Ajubaa.IBModeler.UI.Main
{
    internal delegate void SelectFolderEventHandler(object sender, SelectFolderEventArgs args);

    /// <summary>
    /// Interaction logic for SelectFolderContainer.xaml
    /// </summary>
    public partial class SelectFolderContainer
    {
        internal event SelectFolderEventHandler SelectFolderEvent;

        public SelectFolderContainer()
        {
            InitializeComponent();
        }

        private void SelectImageFolder(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = @"Select folder containing photos of the target model"
            };

            fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath)) 
                InvokeSelectFolderEvent(new SelectFolderEventArgs {FolderPath = fbd.SelectedPath});
        }

        internal void InvokeSelectFolderEvent(SelectFolderEventArgs args)
        {
            var handler = SelectFolderEvent;
            if (handler != null) handler(this, args);
        }

        private void SelectSampleImageFolder(object sender, RoutedEventArgs e)
        {
            var folderPath = ExecutionDirInfoHelper.GetExecutionPath() + @"\SamplePhotos";
            InvokeSelectFolderEvent(new SelectFolderEventArgs { FolderPath = folderPath });
        }

        private void OpenHowToTakePhotos(object sender, RoutedEventArgs e)
        {
            var window = new HowToTakePhotos();
            window.Show();
        }
    }
}
