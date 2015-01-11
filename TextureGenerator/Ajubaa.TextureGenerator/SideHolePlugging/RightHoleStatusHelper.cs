
namespace Ajubaa.TextureGenerator
{
    public class RightHoleStatusHelper : IHoleStatusHelper
    {
        public HoleStatusEnum GetHoleStatus(double xCurrent, double xPrevious)
        {
            if (xCurrent > xPrevious) return HoleStatusEnum.Increasing;
            if (xCurrent < xPrevious) return HoleStatusEnum.Decreasing;
            return HoleStatusEnum.Same;
        }
    }
}
