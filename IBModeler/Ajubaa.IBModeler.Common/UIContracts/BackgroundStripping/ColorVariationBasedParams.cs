using System;
using System.Drawing;
using Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping;

namespace Ajubaa.IBModeler.Common.UIContracts.BackroundStripping
{
    [Serializable]
    public class ColorVariationBasedParams
    {
        /// <summary>
        /// allowed variation in the rgb values of a given color from the compare color
        /// </summary>
        public double VariationPercent;

        /// <summary>
        /// color to compare with
        /// </summary>
        public SerializableColor CompareColor { get; set; }


    }
}
