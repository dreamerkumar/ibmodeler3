namespace Ajubaa.TextureGenerator
{
    public class FourByFourMatrix
    {
        public double[,] Val = new double[4, 4];
        public FourByFourMatrix() {}
        public FourByFourMatrix(double[,] inVal)
        {
            for (uint uintRows = 0; uintRows < 4; uintRows++)
            {
                for (uint uintColumns = 0; uintColumns < 4; uintColumns++)
                {
                    Val[uintRows, uintColumns] = inVal[uintRows, uintColumns];
                }
            }
        }
    }
}