using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for SliderForScreenParams.xaml
    /// </summary>
    public partial class SliderForMeshDensity : UserControl
    {
        public SliderForMeshDensity()
        {
            InitializeComponent();
            SetDisplay();
        }
        
        private void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            SetDisplay();
        }

        private void DragStarted(object sender, DragStartedEventArgs e)
        {
            SetDisplay();
        }

        private void DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetDisplay();
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetDisplay();
        }

        private void SetDisplay()
        {
            Display.Text = string.Format("{0}", (int)Slider.Value);
        }
    }
}
