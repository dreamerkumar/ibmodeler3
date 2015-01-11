
namespace Ajubaa.TextureGenerator
{
    public class LeftHoleStatusHelper : IHoleStatusHelper
    {
        public HoleStatusEnum GetHoleStatus(double xCurrent, double xPrevious)
        {
            if (xCurrent > xPrevious) return HoleStatusEnum.Decreasing;
            if (xCurrent < xPrevious) return HoleStatusEnum.Increasing;
            return HoleStatusEnum.Same;
        }
    }
}
