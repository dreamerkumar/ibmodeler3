using System.Drawing;
using Ajubaa.TextureGenerator.Entities;

namespace Ajubaa.TextureGenerator
{
    public class IndexRangeProcessor
    {
        public static ImgIndexRange GetNextIndexRangeFromLeft(int beyondThisIndex, Bitmap img, int y, bool validInvalidFlag)
        {
            var startIndex = -1;
            for (var ctr = beyondThisIndex + 1; ctr >= 0 && ctr < img.Width; ctr++)
            {
                var valid = TexImageOptimizer.IsValidPixel(ctr, y, img);
                if (valid == validInvalidFlag)
                {
                    startIndex = ctr;
                    break;
                }
            }

            if(startIndex < 0) return null;

            var endIndex = startIndex;
            for (var ctr = startIndex + 1; ctr >= 0 && ctr < img.Width; ctr++)
            {
                var valid = TexImageOptimizer.IsValidPixel(ctr, y, img);
                if (valid == validInvalidFlag)
                    endIndex = ctr;
                else
                    break; //looking for continuous stream of pixels of same type (valid or invalid)
            }
            
            return new ImgIndexRange {MinIndex = startIndex, MaxIndex = endIndex};
        }

        public static ImgIndexRange GetNextIndexRangeFromRight(int beyondThisIndex, Bitmap img, int y, bool validInvalidFlag)
        {
            var startIndex = -1;
            for (var ctr = beyondThisIndex - 1; ctr >= 0 && ctr < img.Width; ctr--)
            {
                var valid = TexImageOptimizer.IsValidPixel(ctr, y, img);
                if (valid == validInvalidFlag)
                {
                    startIndex = ctr;
                    break;
                }
            }

            if (startIndex < 0) return null;

            var endIndex = startIndex;
            for (var ctr = startIndex - 1; ctr >= 0 && ctr < img.Width; ctr--)
            {
                var valid = TexImageOptimizer.IsValidPixel(ctr, y, img);
                if (valid == validInvalidFlag)
                    endIndex = ctr;
                else
                    break; //looking for continuous stream of pixels of same type (valid or invalid)
            }

            return new ImgIndexRange { MinIndex = endIndex, MaxIndex = startIndex };
        }

    }
}
