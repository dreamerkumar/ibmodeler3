using System;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;

namespace Ajubaa.IBModeler.Common.UIContracts.BackroundStripping
{
    [Serializable]
    public class BackgroundStrippingParams
    {
        /// <summary>
        /// the color to set for the background areas while stripping
        /// </summary>
        public SerializableColor BackgroundColor { get; set; }
        
        /// <summary>
        /// parameter values if the background is a screen
        /// </summary>
        public ScreenBasedParams ScreenBasedParams { get; set; }

        /// <summary>
        /// parameter values if background is to be stripped based on variation from a given color
        /// </summary>
        public ColorVariationBasedParams ColorVariationBasedParams { get; set; }

    }
}
