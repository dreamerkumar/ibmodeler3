namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public interface IDistanceProcessor
    {
        IEdgePtsProcessor EdgeProcessor { get; set; }

        LinePtsProcessor EdgeLine { get; set; }

        double GetDistance(int y);
    }
}