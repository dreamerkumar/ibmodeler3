using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Ajubaa.TextureGenerator
{
    public static class PointExtensions
    {
        public static int GetIntX(this Point point)
        {
            return Convert.ToInt32(point.X);
        }

        public static int GetIntY(this Point point)
        {
            return Convert.ToInt32(point.Y);
        }
    }
}
