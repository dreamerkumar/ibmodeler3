using System;
using System.Drawing;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;

namespace Ajubaa.IBModeler.Common.UIContracts.BackroundStripping
{
    [Serializable]
    public class ScreenBasedParams
    {
        /// <summary>
        /// type of screen
        /// can be red, green, blue or an exact color
        /// </summary>
        public ScreenColorTypes ScreenColorTypes { get; set; }

        /// <summary>
        /// minimum difference that should exist between the screen color component
        /// and the other two components on the RGB spectrum
        /// </summary>
        public byte MinColorOffset { get; set; }

        /// <summary>
        /// the max allowed difference between the other two color components as a
        /// percent of of the difference between the screen color and other color components
        /// </summary>
        public byte MaxDiffPercent { get; set; }
        
        /// <summary>
        /// a specific rgb color to match against
        /// this value is set when the screencolortypes is exact exact color
        /// </summary>
        public SerializableColor ExactMatchColor { get; set; }
    }
}
