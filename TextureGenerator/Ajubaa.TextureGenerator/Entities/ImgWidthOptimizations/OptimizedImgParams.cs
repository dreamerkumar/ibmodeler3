namespace Ajubaa.TextureGenerator
{
    public class OptimizedImgParams
    {
        public int OptimizedImgMin { get; set; }

        public int OptimizedImgMax { get; set; }

        public bool InitializedToZeroZero { get; set; }

        public int OptimizedImgWidth
        {
            get
            {
                if (OptimizedImgMin == 0 && OptimizedImgMax == 0)
                    return 0;

                return OptimizedImgMax + 1 - OptimizedImgMin;
            }
        }

        public MinAndMaxIndices[] XLimitsAtYIndices { get; set; }
    }
}
