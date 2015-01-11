using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Point = System.Windows.Point;

namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints
{
    internal class ImageCanvas : Canvas
    {
        internal void SetImage(string imagePath)
        {
            var imageBrush = new ImageBrush(new BitmapImage(new Uri(imagePath, UriKind.Absolute)));
            Background = imageBrush;
        }

        internal void SetImage(Bitmap image)
        {
            //Convert the image in resources to a Stream 
            Stream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);

            // Create a BitmapImage with the Stream. 
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();

            //set the display with this new image brush
            var imageBrush = new ImageBrush(bitmapImage);
            Background = imageBrush;
        }

        internal void GetDashedLineJoiningTwoEdgesOfDisc(Point left, Point right, string objectName)
        {
            var line = MarkerDisplayHelper.GetHorizontalDashedLine(left, right);

            line.Name = objectName;
            Children.Add(line);
        }

        internal void MarkWithVerticalDashedLine(Point position, string objectName)
        {
            var line = MarkerDisplayHelper.GetCenterLine(position, MinHeight, Height);

            line.Name = objectName;
            Children.Add(line);
        }

        internal void AddMarkerDisplay(Point position, string objectName)
        {
            var line = MarkerDisplayHelper.GetMarkerDisplay(position, MinHeight, Height);

            line.Name = objectName;
            Children.Add(line);
        }

        internal void MarkWithHorizontalLine(Point position, string objectName)
        {
            var bottomLineDisplay = MarkerDisplayHelper.GetBottomLineDisplay(position, Width);

            bottomLineDisplay.Name = objectName;
            Children.Add(bottomLineDisplay);
        }

        internal void DrawRectangleOnLeft(Point position, string objectName)
        {
            var rectangleOnLeft = MarkerDisplayHelper.GetRectangleOnLeft(position, Height);

            rectangleOnLeft.Name = objectName;
            Children.Add(rectangleOnLeft);
        }

        internal void DrawRectangleOnRight(Point position, string objectName)
        {
            var rectangleOnRight  = MarkerDisplayHelper.GetRectangleOnRight(position, Width, Height);

            rectangleOnRight.Name = objectName;
            Children.Add(rectangleOnRight);
        }

        internal void RemoveAllMarkers()
        {
            if (Children != null && Children.Count > 0)
                Children.Clear();
        }

        internal void RemoveDisplayObject(string objName)
        {
            if (Children != null && Children.Count > 0)
            {
                var objectToRemove = Children.Cast<object>().FirstOrDefault(child => ((FrameworkElement) child).Name == objName);
                if(objectToRemove != null)
                    Children.Remove(((FrameworkElement)objectToRemove));
            }
        }
    }
}
