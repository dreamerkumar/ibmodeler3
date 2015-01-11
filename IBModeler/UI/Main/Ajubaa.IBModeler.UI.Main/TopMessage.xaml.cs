using System.Windows.Controls;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for TopMessage.xaml
    /// </summary>
    public partial class TopMessage : UserControl
    {
        public string Text
        {
            set { Message.Text = value; }
        }
        public double MessageFontSize
        {
            set { Message.FontSize = value; }
        }
        
        public TopMessage()
        {
            InitializeComponent();
        }
    }
}
