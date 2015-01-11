using System;

namespace Ajubaa.TextureGenerator
{
    public class MinAndMaxIndices
    {
        public int Min { get; set; }

        public int Max { get;  set; }

        public bool InitializedToZeroZero { get; set; }

        public int Width
        {
            get
            {
                if (Min == 0 && Max == 0)
                    return 0;

                return Max + 1 - Min;
            }
        }
    }
}
