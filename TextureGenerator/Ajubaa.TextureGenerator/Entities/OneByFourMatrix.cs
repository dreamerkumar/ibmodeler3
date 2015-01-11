namespace Ajubaa.TextureGenerator
{
    public class OneByFourMatrix
    {
        public double[] Val = new double[4];
        public OneByFourMatrix() {}
        public OneByFourMatrix(double[] val)
        {
            for (uint uintColumns = 0; uintColumns < 4; uintColumns++)
            {
                Val[uintColumns] = val[uintColumns];
            }
        }
    }
}