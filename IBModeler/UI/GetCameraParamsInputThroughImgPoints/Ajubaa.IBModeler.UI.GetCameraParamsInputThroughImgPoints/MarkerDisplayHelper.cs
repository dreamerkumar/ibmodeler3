using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Ajubaa.IBModeler.UI.GetCameraParamsInputThroughImgPoints
{
    public static class MarkerDisplayHelper
    {
        internal static Line GetCenterLine(Point position, double minHeight, double height)
        {
            var dashes = new DoubleCollection {2, 2};
            var brush = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop {Color = Colors.Red, Offset = 0.0},
                    new GradientStop {Color = Colors.Blue, Offset = 1.0},
                }
            };
            return new Line
            {
                X1 = position.X,
                X2 = position.X,
                Y1 = minHeight,
                Y2 = height,
                Stroke = brush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 1,
                StrokeDashArray = dashes,
                StrokeDashCap  = PenLineCap.Round
            };
        }

        internal static Line GetHorizontalDashedLine(Point p1, Point p2)
        {
            var dashes = new DoubleCollection { 2, 2 };
            var brush = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop {Color = Colors.Red, Offset = 0.0},
                    new GradientStop {Color = Colors.Blue, Offset = 1.0},
                }
            };
            return new Line
            {
                X1 = p1.X,
                X2 = p2.X,
                Y1 = p1.Y,
                Y2 = p2.Y,
                Stroke = brush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 1,
                StrokeDashArray = dashes,
                StrokeDashCap = PenLineCap.Round
            };
        }


        internal static Line GetMarkerDisplay(Point position, double minHeight, double height)
        {
            var brush = new LinearGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop {Color = Colors.Red, Offset = 0.0},
                    new GradientStop {Color = Colors.Blue, Offset = 1.0},
                }
            };

            return new Line
            {
                Stroke = brush,
                X1 = position.X,
                X2 = position.X,
                Y1 = minHeight,
                Y2 = height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 1
            };
        }

        internal static Line MarkWithVerticalLineAndSolidColorBrush(Point position, double minHeight, double height)
        {
            var myLine = new Line
            {
                Stroke = Brushes.LightSteelBlue,
                X1 = position.X,
                X2 = position.X,
                Y1 = minHeight,
                Y2 = height,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                StrokeThickness = 1,
                Opacity = .5
            };

            return myLine;
        }

        internal static Rectangle GetBottomLineDisplay(Point position, double width)
        {
            return new Rectangle
                       {
                           Fill = Brushes.LightSteelBlue,
                           Width = width,
                           Height = position.Y,
                           Opacity = .25,
                           Stroke = Brushes.Black
                       };
        }

        internal static Rectangle GetRectangleOnLeft(Point position, double height)
        {
            var brush = new LinearGradientBrush
                            {
                                GradientStops = new GradientStopCollection
                                                    {
                                                        new GradientStop {Color = Colors.Red, Offset = 0.0},
                                                        new GradientStop {Color = Colors.Blue, Offset = 1.0},
                                                    }
                            };
            return new Rectangle
                       {
                           Fill = brush,
                           Width = position.X,
                           Height = height,
                           Opacity = .5,
                           Stroke = Brushes.Black
                
                       };
        }

        internal static Rectangle GetRectangleOnRight(Point position, double width, double height)
        {
            var brush = new LinearGradientBrush
                            {
                                GradientStops = new GradientStopCollection
                                                    {
                                                        new GradientStop {Color = Colors.Red, Offset = 0.0},
                                                        new GradientStop {Color = Colors.Blue, Offset = 1.0},
                                                    }
                            };
            var myLine = new Rectangle
                             {
                                 Fill = brush,
                                 Width = width - position.X,
                                 Height = height,
                                 Opacity = .5,
                                 Stroke = Brushes.Black
                             };
            Canvas.SetLeft(myLine, position.X);
            return myLine;
        }
    }
}
