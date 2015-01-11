using System;
using Ajubaa.IBModeler.Common.UIContracts.BackroundStripping;

namespace Ajubaa.IBModeler.UI.Main
{
    internal class StripBackgroundEventArgs : EventArgs
    {
        public BackgroundStrippingParams BackgroundStrippingParams { get; set; }
    }
}
