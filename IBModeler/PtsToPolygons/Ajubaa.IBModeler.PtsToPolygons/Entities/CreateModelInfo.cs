using System.IO;

namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class CreateModelInfo
    {
        public Stream MoldData { get; set; }

        public int Minx { get; set; }

        public int Maxx { get; set; }

        public int Miny { get; set; }

        public int Maxy { get; set; }

        public int Minz { get; set; }

        public int Maxz { get; set; }
    }
}
