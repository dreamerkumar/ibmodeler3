namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public abstract class DistanceProcessor : IDistanceProcessor
    {
        protected void ConfigureEdgeLine(int pixelsToFeed)
        {
            var edgePts = EdgeProcessor.GetEdgePtsAtImgBottom(pixelsToFeed);
            EdgeLine = new LinePtsProcessor(edgePts);
        }

        #region Implementation of IDistanceProcessor

        public IEdgePtsProcessor EdgeProcessor { get; set; }

        public LinePtsProcessor EdgeLine { get; set; }

        public abstract double GetDistance(int y);

        #endregion
    }
}
