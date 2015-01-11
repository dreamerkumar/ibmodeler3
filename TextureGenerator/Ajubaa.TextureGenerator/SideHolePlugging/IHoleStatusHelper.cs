namespace Ajubaa.TextureGenerator
{
    public interface IHoleStatusHelper
    {
        HoleStatusEnum GetHoleStatus(double xCurrent, double xPrevious);
    }
}
