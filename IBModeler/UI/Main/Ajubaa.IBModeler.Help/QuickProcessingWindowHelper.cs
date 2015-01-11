using System.Windows;
using System.Windows.Threading;

namespace Ajubaa.IBModeler.Help
{
    public class QuickProcessingWindowHelper
    {
        private readonly QuickProcessingWindow _quickProcessingWindow;
        private readonly FrameworkElement _disableElement;

        /// <summary>
        /// Displays a quick processing window. 
        /// Forces display to make sure user sees the window before the processing starts
        /// </summary>
        /// <param name="disableElement"></param>
        public QuickProcessingWindowHelper(FrameworkElement disableElement)
        {
            _disableElement = disableElement;
            _quickProcessingWindow = new QuickProcessingWindow { Height = 88, Width = 600 };
            _quickProcessingWindow.Show();
            ForceUiToUpdate();

            //http://stackoverflow.com/questions/9970562/disabling-all-clicks-for-a-short-period-after-clicking-a-image
            //disable user interaction
            VisualStateManager.GoToElementState(_disableElement, "Busy", true); 

        }

        public void Close()
        {
            _quickProcessingWindow.Close();

            //enable user interaction
            VisualStateManager.GoToElementState(_disableElement, "Ready", true); 
        }

        /// <summary>
        /// this function forces the UI to update display
        /// useful when we want to display a window before starting a time taking process
        /// </summary>
        private static void ForceUiToUpdate()
        {
            var frame = new DispatcherFrame();

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate(object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
        }
    }
}
