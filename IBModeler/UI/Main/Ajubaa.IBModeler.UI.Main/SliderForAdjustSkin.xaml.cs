using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Ajubaa.TextureGenerator;

namespace Ajubaa.IBModeler.UI.Main
{
    /// <summary>
    /// Interaction logic for SliderForScreenParams.xaml
    /// </summary>
    public partial class SliderForAdjustSkin : UserControl
    {
        public SliderForAdjustSkin()
        {
            InitializeComponent();
            SetDisplay();
        }

        public void SetRange(MinAndMaxTexCoodValueLimits range, string label)
        {
            SetRange(range.Min, range.Max, label);
        }

        private void SetRange(double min, double max, string label)
        {
            Label.Text = label;
            SliderMin.Minimum = min;
            SliderMin.Maximum = max;
            SliderMin.Value = min;
            SliderMax.Minimum = min;
            SliderMax.Maximum = max;
            SliderMax.Value = max;
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

        private void SliderMin_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderMax.Value < SliderMin.Value) SliderMax.Value = SliderMin.Value;
            SetDisplay();
        }

        private void SliderMax_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderMin.Value > SliderMax.Value) SliderMin.Value = SliderMax.Value;
            SetDisplay();
        }

        private void SetDisplay()
        {
            DisplayMin.Text = string.Format("Min {0:0.00}", SliderMin.Value);
            DisplayMax.Text = string.Format("Max {0:0.00}", SliderMax.Value);
        }
    }
}
