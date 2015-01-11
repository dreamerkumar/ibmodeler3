using Ajubaa.IBModeler.Common;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;

namespace Ajubaa.IBModeler.Processor
{
    /// <summary>
    /// input values for creating a model mesh
    /// </summary>
    public class CreateMeshContract
    {
        public string ImageFolder { get; set; }

        /// <summary>
        /// parameters to strip the background
        /// </summary>
        public BackgroundStrippingParams BackgroundStrippingParams { get; set; }

        public double VariationIn3DCoordinates { get; set; }

        public System.Drawing.Color InvalidColor { get; set; }

        public int PtDensity { get; set; }

        /// <summary>
        /// click positions on the images
        /// </summary>
        public ClickInputs ClickInputs { get; set; }

        /// <summary>
        /// path to log the errors
        /// </summary>
        public string LogFilePath { get; set; }

        /// <summary>
        /// tells whether to save the processed image in a sub folder for troubleshooting purposes
        /// </summary>
        public bool SaveProcessedImages { get; set; }

        /// <summary>
        /// tells whether to create and save a bitmap depicting the loss of data after processing each image
        /// </summary>
        public bool SaveLossOfDataPerImage { get; set; }
        
        /// <summary>
        /// if the model extends beyond the disc, then this parameter contains 
        /// the extra width to be included on both sides of the disc as a percentage of the width of the disc
        /// </summary>
        public double PercentExtraWidth { get; set; }

        /// <summary>
        /// different images can have different bottom portions for the model
        /// this property stores the minimum height as a fraction of total height that will be applied to
        /// all the images to keep the height same throughout
        /// </summary>
        public double MinImageHeightRatio { get; set; }

        /// <summary>
        /// distance between the bottom most portion of model and the base disc
        /// as a percentage of the distance between the left and right edges of the disc
        /// </summary>
        public double BottomPaddingPercent { get; set; }

        /// <summary>
        /// sets a maximum angle upto which we should process the images
        /// any images beyond this will not be used in creating the mesh
        /// </summary>
        public int MaxAngleOfImageToProcessInDegress { get; set; }
    }
}
