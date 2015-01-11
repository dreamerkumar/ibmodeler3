using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ajubaa.IBModeler.Common
{
    public class Constants
    {
        public const int MOLD_HEADER_SIZE	=	141; 
        //41 bytes are used as of now while the rest have been kept for future use
        public const int MOLD_POS_FOR_IMAGE_COUNT = 37;

        public const int MOLD_HEADER_EXTRA_BYTES = 100;

        public const int UINT_DEFAULT_PT_DENSITY = 200;
    }
}
