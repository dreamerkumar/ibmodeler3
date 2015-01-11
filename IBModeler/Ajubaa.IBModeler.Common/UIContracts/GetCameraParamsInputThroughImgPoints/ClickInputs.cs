using System;
using System.Collections.Generic;

namespace Ajubaa.IBModeler.Common
{
    [Serializable]
    public class ClickInputs
    {
        public List<ImageClickInputDetails> ImageClickInputDetailsList { get; set; }

        public double[] Angles { get; set; }
    }
}
