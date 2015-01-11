using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajubaa.IBModeler.Processor
{
    public enum ResizeType
    {
        ToMaxAllowedResolution,
        DoNotResizeAtAll,
        ToSpecifiedSizes,
        ComputeSizeBasedOnPtDensity
    }
}
