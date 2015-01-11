using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Rectangle = System.Drawing.Rectangle;
namespace Ajubaa.IBModeler.GetCameraParamsInputThroughImgPoints
{
    public class WpfScreen
    {
        /// <summary>
        /// http://stackoverflow.com/questions/1927540/how-to-get-the-size-of-the-current-screen-in-wpf
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<WpfScreen> AllScreens()
        {
            foreach (var screen in Screen.AllScreens)
            {
                yield return new WpfScreen(screen);
            }
        }

        public static WpfScreen GetScreenFrom(Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            var screen = Screen.FromHandle(windowInteropHelper.Handle);
            var wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen GetScreenFrom(Point point)
        {
            var x = (int)Math.Round(point.X);
            var y = (int)Math.Round(point.Y);
            // are x,y device-independent-pixels ??
            var drawingPoint = new System.Drawing.Point(x, y);
            var screen = Screen.FromPoint(drawingPoint);
            var wpfScreen = new WpfScreen(screen);
            return wpfScreen;
        }

        public static WpfScreen Primary
        {
            get
            {
                return new WpfScreen(Screen.PrimaryScreen);
            }
        }

        private readonly Screen _screen;

        internal WpfScreen(Screen screen)
        {
            this._screen = screen;
        }

        public System.Windows.Shapes.Rectangle DeviceBounds
        {
            get
            {
                return GetRect(this._screen.Bounds);
            }
        }

        public System.Windows.Shapes.Rectangle WorkingArea
        {
            get
            {
                return GetRect(this._screen.WorkingArea);
            }
        }

        private static System.Windows.Shapes.Rectangle GetRect(Rectangle value)
        {
            // should x, y, width, hieght be device-independent-pixels ??
            return new System.Windows.Shapes.Rectangle
            {
                RadiusX = value.X,
                RadiusY = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get
            {
                return this._screen.Primary;
            }
        }

        public string DeviceName
        {
            get
            {
                return this._screen.DeviceName;
            }
        }
    }
}
