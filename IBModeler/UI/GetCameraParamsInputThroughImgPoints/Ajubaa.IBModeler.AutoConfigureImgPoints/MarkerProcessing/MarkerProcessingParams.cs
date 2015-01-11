namespace Ajubaa.IBModeler.AutoConfigureImgPoints
{
    public class MarkerProcessingParams
    {
        /// <summary>
        /// indicates by what percent the color should change to qualify as a marker
        /// </summary>
        public double MarkerColorVariationPercent { get; set; }
        
        /// <summary>
        /// indicates the maximum width that a marker can span as a percent of width of the disc
        /// </summary>
        public double MarkerWidthPercent { get; set; }

        /// <summary>
        /// indicates the maximum number of markings that can show up in an image
        /// </summary>
        public double MaximumMarkerCount { get; set; }

        /// <summary>
        /// constructor to set default values
        /// </summary>
        public MarkerProcessingParams()
        {
            MarkerColorVariationPercent = 10;
            MarkerWidthPercent = 5;
            MaximumMarkerCount = 10;
        }
    }
}
