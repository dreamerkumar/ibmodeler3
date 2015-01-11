using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    /// <summary>
    ///Colour Space convertion is such a simple thing to want to do and yet is presented inacurately or incompletely everywhere.
    ///So, I'm listing what I believe to be the best code I can that converts between RGB colours and HSL (sometimes called HLS).
    ///Colours are specified on computers in terms of the intensity of each of the electron guns in the monitor.
    ///There are three guns, one Red, one Green and one Blue (ultimately this means there are just different coloured dots which is what you get on an LCD screen).
    ///The human eye can only perceive 256 levels of intensity, so a Byte is sufficient to store the intensity value.
    ///Unfortunately, when you want to describe a colour its not very easy to do accurately using RGB.
    ///The HSL Colour Space is supposed to be easier.
    ///H is the Hue or colour; S is the Saturation and L is the Lightness.
    ///The ranges are supposed to be:
    ///0-360 for Hue (degrees) - Red being at 0
    ///0-240 for Saturation and Lightness...
    ///H is undefined when S=0;
    ///The Colour Selector in Microsoft Windows, rewrites the standards (anyone surprised?) and uses:
    ///0-239 for Hue
    ///0-240 for Saturation and Lightness...
    ///H is 160 when S=0;
    ///
    ///The following code reproduces the conversion the Microsoft way.
    ///
    ///R,G,B values are from 0 to 255
    ///H=[0,239], S=[0,240], L=[0,240]
    ///if S==0, then H=160 
    /// </summary>
    public static class Hsl
    {
        public static HslColor GetHslColor(Bitmap img, int x, int y)
        {
            var imgColor = img.GetPixel(x, y);
            var hslColor = Rgb2Hsl(imgColor);
            return hslColor;
        }

        public static HslColor Rgb2Hsl(Color color)
        {
            byte h, s, l;
            Rgb2Hsl(color.R, color.G, color.B, out h, out s, out l);
            return new HslColor {H = h, S = s, L = l};
        }

        public static void Rgb2Hsl(byte r, byte g, byte b, out byte h, out byte s, out byte l)
        {
            r = Min(Max(0, r), 255);
            g = Min(Max(0, g), 255);
            b = Min(Max(0, b), 255);
            var minval = Min(Min(r, g), b);
            var maxval = Max(Max(r, g), b);
            var msum = (uint)(maxval + minval);
            l = (byte)(0.5 + (((float)msum * 240) / 510));
            if (maxval == minval)
            {
                s = 0;
                h = 160;
            }
            else
            {
                var delta = (float)maxval - minval;

                s = (byte)(0.5 + (240 * (delta / ((msum <= 255) ? msum : (510 - msum)))));

                var h1 = 0f;
                //      if(R==maxval) h=40*(  (G-B)/delta);
                //      else if(G==maxval) h=40*(2+(B-R)/delta);
                //      else if(B==maxval) h=40*(4+(R-G)/delta);
                //
                if (r == maxval)
                    h1 = 40 * ((g - b) / delta);
                else if (g == maxval)
                    h1 = 40 * (2 + (b - r) / delta);
                else if (b == maxval)
                    h1 = 40 * (4 + (r - g) / delta);

                if (h1 > 240)
                    h1 -= 240.0f;

                //Added by VK
                if (h1 < 0.0)
                    h1 = h1 + 240.0f;//end of addition by VK

                h = (byte)(0.5 + h1);
            }
        }

        public static void Hsl2Rgb(ref byte r, ref byte g, ref byte b, byte h, byte s, byte l)
        {
            h = Min(Max(0, h), 239);
            s = Min(Max(0, s), 240);
            l = Min(Max(0, l), 240);
            var H = h * 360 / 240.0f;
            var S = s / 240.0f;
            var L = l / 240.0f;
            if (S > 0)
            {
                float rm2;
                if (l <= 120)
                    rm2 = L + L * S;
                else
                    rm2 = L + S - L * S;
                var rm1 = 2 * L - rm2;
                r = ToRgb(rm1, rm2, H + 120);
                g = ToRgb(rm1, rm2, H);
                b = ToRgb(rm1, rm2, H - 120);
            }
            else
                r = g = b = (byte)(0.5 + L * 255);
        }
        
        public static byte ToRgb(float rm1, float rm2, float rh)
        {
            if (rh > 360)
                rh -= 360;
            else if (rh < 0)
                rh += 360;

            if (rh < 60)
                rm1 = rm1 + (rm2 - rm1) * rh / 60;
            else if (rh < 180)
                rm1 = rm2;
            else if (rh < 240)
                rm1 = rm1 + (rm2 - rm1) * (240 - rh) / 60;

            return (byte)(0.5 + rm1 * 255);
        }

        private static byte Max(byte p0, byte p1)
        {
            return p0 > p1 ? p0 : p1;
        }

        private static byte Min(byte p0, byte p1)
        {
            return p0 < p1 ? p0 : p1;
        }
    }
}
