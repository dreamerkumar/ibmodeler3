using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ajubaa.Common;
using Ajubaa.IBModeler.Common;
using MessageBox = System.Windows.MessageBox;

namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ImgListHandler.AllImgProcessed += ImgListHandler_AllImgProcessed;
        }

        static void ImgListHandler_AllImgProcessed(object sender, AllImgProcessedEventArgs args)
        {
            var angleDisplay = new StringBuilder();
            foreach (var radianangle in args.ClickInputs.Angles)
            {
                angleDisplay.Append(string.Format("Rotated by: {0} degrees \n", Math.Round(radianangle*180.0/Math.PI, 2 )));
            }
            MessageBox.Show(angleDisplay.ToString());
            
            //Serialize object
            var serializer = new XmlSerializer(typeof(ClickInputs));
            var outputXmlFilePath = ExecutionDirInfoHelper.CreateUniqueOutputPath() + @"\ClickInputs.xml";
            var tw = new StreamWriter(outputXmlFilePath);
            serializer.Serialize(tw, args);
            tw.Close();

            //Test deserialization
            serializer = new XmlSerializer(typeof(ClickInputs));
            var tr = new StreamReader(outputXmlFilePath);
            var deserialized = (ClickInputs)serializer.Deserialize(tr);
            if (deserialized.Angles.Length != args.ClickInputs.Angles.Length)
                throw new Exception("Not properly deserialized");
            tr.Close(); 
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result != System.Windows.Forms.DialogResult.OK) 
                return;

            var selectedPath = folderBrowserDialog.SelectedPath;
            ImgListHandler.GetUserInputForImages(selectedPath);
            SelectDirectory.Visibility = Visibility.Hidden;
        }
    }
}
