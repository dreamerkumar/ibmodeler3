using System;
using System.Drawing;

namespace Ajubaa.Common
{
    public class ImageHandler
    {
        private readonly Bitmap _loadedBitmapImage;
        private readonly Color _lostDataColor = Color.FromArgb(255, 0,0);
        private readonly Color _retainedDataColor = Color.FromArgb(0, 255, 0);

        public int Width { get; set; }

        public int Height { get; set; }

        public Bitmap BitmapImg { get { return _loadedBitmapImage; }}

        public ImageHandler(string strBitmapFilePath)
        {
            var objImage = Image.FromFile(strBitmapFilePath);
            _loadedBitmapImage = (Bitmap)objImage;
            Height = objImage.Height;
            Width = objImage.Width;            
        }

        public ImageHandler(Bitmap image)
        {
            _loadedBitmapImage = image;
            Height = _loadedBitmapImage.Height;
            Width = _loadedBitmapImage.Width;
        }

        public Color GetPixelAt1BasedIndex(int intX, int intY)
        {
            intX--; //Bitmap class stores the index 0 based
            intY = Height - intY;//Bitmap class stores pixels top to bottom
            return _loadedBitmapImage.GetPixel(intX, intY);
        }

        public void SetLostDataColorForPixel(int intX, int intY)
        {
            intX--; //Bitmap class stores the index 0 based
            intY = Height - intY;//Bitmap class stores pixels top to bottom
            _loadedBitmapImage.SetPixel(intX, intY, _lostDataColor);
        }

        public void SetRetainedDataColorForPixel(int intX, int intY)
        {
            intX--; //Bitmap class stores the index 0 based
            intY = Height - intY;//Bitmap class stores pixels top to bottom
            _loadedBitmapImage.SetPixel(intX, intY, _retainedDataColor);
        }

        public void SetColorForPixel(int intX, int intY, Color color)
        {
            intX--; //Bitmap class stores the index 0 based
            intY = Height - intY;//Bitmap class stores pixels top to bottom
            _loadedBitmapImage.SetPixel(intX, intY, color);
        }

        public void SaveBitmap(string strFilePath)
        {
            _loadedBitmapImage.Save(strFilePath);
        }
    }
}
