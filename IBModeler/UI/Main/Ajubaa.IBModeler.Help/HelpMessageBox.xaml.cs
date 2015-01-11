using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.Help
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class HelpMessageBox
    {
        public HelpMessageBox(IEnumerable<string> doubleSpacedMessages, IEnumerable<string> singleSpacedMessages)
        {
            InitializeComponent();
            if (doubleSpacedMessages != null)
                foreach(var doubleSpacedMsg in doubleSpacedMessages)
                {
                    DoubleSpacedMessages.Inlines.Add(doubleSpacedMsg);
                    DoubleSpacedMessages.Inlines.Add(new LineBreak());
                    DoubleSpacedMessages.Inlines.Add(new LineBreak());
                }
            if (singleSpacedMessages != null && singleSpacedMessages.Count() > 0)
            {
                foreach (var singleSpacedMsg in singleSpacedMessages)
                {
                    SingleSpacedMessages.Inlines.Add(singleSpacedMsg);
                    SingleSpacedMessages.Inlines.Add(new LineBreak());
                }
            }
            else
            {
                SingleSpacedMessagesContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
