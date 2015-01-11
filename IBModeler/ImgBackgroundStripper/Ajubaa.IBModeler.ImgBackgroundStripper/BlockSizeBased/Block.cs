using System.Collections.Generic;
using System.Drawing;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    public class Block
    {
        public long PixelCount { get { return PixelPositions.Count; } }

        public List<Point> PixelPositions { get; set; }

        public Block()
        {
            PixelPositions = new List<Point>();
        }

        public void AddPixelIndex(int x, int y)
        {
            PixelPositions.Add(new Point(x, y));
        }

        public void MergeBlock(Block blockToMerge)
        {
            PixelPositions.AddRange(blockToMerge.PixelPositions);
        }
    }
}
