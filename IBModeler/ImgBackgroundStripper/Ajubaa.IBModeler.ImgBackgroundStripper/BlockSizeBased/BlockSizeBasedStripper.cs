using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Ajubaa.IBModeler.ImgBackgroundStripper
{
    public class BlockSizeBasedStripper
    {
        private readonly ImgStripStatus _stripStatus;
        private readonly long[,] _blockIndices;
        private readonly bool _blocksOfInvalidPixels;

        public Dictionary<long, Block> Blocks { get; private set; }

        public BlockSizeBasedStripper(ImgStripStatus stripStatus, bool blocksOfInvalidPixels = true)
        {
            _stripStatus = stripStatus;
            _blocksOfInvalidPixels = blocksOfInvalidPixels;
            _blockIndices = new long[stripStatus.Width, stripStatus.Height];
            Blocks = new Dictionary<long, Block>();

            long blockCounter = 0;
            for (var y = 0; y < _stripStatus.Height; y++)
            {
                for (var x = 0; x < _stripStatus.Width; x++)
                {
                    if (SatisfiesCondition(y, x)) continue; //is a background pixel

                    //if any pixel around it is assigned to a block, take that number
                    var validNeighbors = GetValidNeighbors(y, x, _stripStatus.Width);

                    if (validNeighbors.Count() > 0)
                    {
                        //use the first block index
                        var indexToUse = validNeighbors.First();
                        var blockToUse = Blocks[indexToUse];

                        _blockIndices[x, y] = indexToUse;
                        blockToUse.AddPixelIndex(x, y);

                        //handle the condition where there can be neighbors with different block indices
                        //they all should be normalized to have one block index as they are all connected
                        foreach (var blockIndex in validNeighbors.Where(index => index != indexToUse))
                        {
                            //merge the blocks
                            var blockToMerge = Blocks[blockIndex];
                            foreach (var position in blockToMerge.PixelPositions)
                            {
                                _blockIndices[position.X, position.Y] = indexToUse;
                            }

                            blockToUse.MergeBlock(blockToMerge);
                            Blocks.Remove(blockIndex);
                        }
                       
                    }
                    else
                    {
                        //if none of the neighbors had a block assigned, then assign to a new block
                        blockCounter++;
                        var block = new Block();
                        
                        block.AddPixelIndex(x,y);
                        _blockIndices[x, y] = blockCounter;

                        Blocks.Add(blockCounter, block);
                    }
                }
            }
        }



        public void RemoveAllBlocksExceptForLargest()
        {
            var maxSize = GetMaxBlockSize();
            RemoveSmallPixelBlocks(maxSize);
        }

        public void RemoveSmallPixelBlocks(long maxSize)
        {
            foreach(var block in Blocks)
            {
                var value = block.Value;
                if (value.PixelCount >= maxSize) continue;
                
                //size count is less than the maximum allowed size so erase this block
                foreach(var position in value.PixelPositions)
                {
                    SetConditionAsSatisfied(position);
                }
            }
        }

        private long GetMaxBlockSize()
        {
            return Blocks.Max(x => x.Value.PixelCount);
        }

        private IEnumerable<long> GetValidNeighbors(int y, int x, int imgWidth)
        {
            var neighbors = GetSurroundingPtsToTheLeftAndTop(x, y, imgWidth);

            return neighbors.Select(pt => _blockIndices[pt.X, pt.Y]).Where(blockIndex => blockIndex != 0);
        }

        private static IEnumerable<Point> GetSurroundingPtsToTheLeftAndTop(int x, int y, int imgWidth)
        {
            var pts = new List<Point>();
            
            if (x - 1 >= 0) pts.Add(new Point(x - 1, y));

            if (x - 1 >= 0 && y - 1 >= 0) pts.Add(new Point(x - 1, y - 1));

            if (x + 1 < imgWidth && y - 1 >= 0) pts.Add(new Point(x + 1, y - 1));
            
            if (y - 1 >= 0) pts.Add(new Point(x, y - 1));

            return pts;
        }

        private bool SatisfiesCondition(int y, int x)
        {
            return _blocksOfInvalidPixels ? _stripStatus.IsInvalid(x, y) : _stripStatus.IsValid(x, y);
        }

        private void SetConditionAsSatisfied(Point position)
        {
            if (_blocksOfInvalidPixels)
                _stripStatus.SetInvalid(position.X, position.Y);
            else
                _stripStatus.SetValid(position.X, position.Y);
        }
    }
}
