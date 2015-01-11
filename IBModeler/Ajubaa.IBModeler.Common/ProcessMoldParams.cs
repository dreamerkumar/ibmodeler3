using System.IO;

namespace Ajubaa.IBModeler.Common
{
    public class ProcessMoldParams 
    {
        public Stream MoldData { get; set; }

        public bool SetColorForRetainedPixels { get; set; }

        public ImageParams ImageParams;

        public ProcessMoldParams()
        {
            SetColorForRetainedPixels = true;
        }
   }
}
