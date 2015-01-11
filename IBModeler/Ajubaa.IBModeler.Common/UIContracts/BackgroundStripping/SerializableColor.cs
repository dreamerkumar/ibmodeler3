using System;

namespace Ajubaa.IBModeler.Common.UIContracts.BackgroundStripping
{
    [Serializable]
    public class SerializableColor
    {
        public SerializableColor()
        {
            
        }

        public SerializableColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }
}
