using System;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    public class ImgStripStatus
    {
        private readonly bool[,] _isInvalid;
        public long InvalidPixelCount { get; private set; }

        public ImgStripStatus(int imgWidth, int imgHeight)
        {
            _isInvalid = new bool[imgWidth,imgHeight];
        }

        public void SetInvalid(int x, int y)
        {
            if (IsInvalid(x, y)) return;

            _isInvalid[x, y] = true;
            InvalidPixelCount++;
        }

        public void SetValid(int x, int y)
        {
            if(IsValid(x,y)) return;

            _isInvalid[x, y] = false;
            InvalidPixelCount--;
        }

        public bool IsInvalid(int x, int y)
        {
            return _isInvalid[x, y];
        }

        public bool IsValid(int x, int y)
        {
            return !_isInvalid[x, y];
        }

        public void SaveImgWithInvalidPixels(Image sourceImg, string imgPath, Color invalidColor)
        {
            var imgToSave = GetImgWithInvalidPixelsSet(sourceImg, invalidColor);

            imgToSave.Save(imgPath);
        }

        private Bitmap GetImgWithInvalidPixelsSet(Image sourceImg, Color invalidColor)
        {
            if (sourceImg == null) throw new ArgumentNullException("sourceImg");

            //create a copy of the image
            var imgToSave = new Bitmap(sourceImg);

            //modify pixels
            for (var y = 0; y < imgToSave.Height; y++)
            {
                for (var x = 0; x < imgToSave.Width; x++)
                {
                    if (_isInvalid[x, y]) imgToSave.SetPixel(x, y, invalidColor);
                }
            }
            return imgToSave;
        }

        public int Width { get { return _isInvalid.GetLength(0); } }

        public int Height { get { return _isInvalid.GetLength(1); } }

    }
}
