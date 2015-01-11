using System.Drawing;

namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public interface IEdgePtsProcessor
    {
        Point GetEdgePt(int y);

        Point[] GetEdgePtsAtImgBottom(int count);
    }
}
