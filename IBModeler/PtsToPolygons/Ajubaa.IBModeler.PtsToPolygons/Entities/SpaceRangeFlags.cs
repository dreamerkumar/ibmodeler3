namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class SpaceRangeFlags
    {
        public bool Minx { get; set; }
        public bool Maxx { get; set; }
        public bool Miny { get; set; }
        public bool Maxy { get; set; }
        public bool Minz { get; set; }
        public bool Maxz { get; set; }

        public void Initialize()
        {
            Minx = Maxx = Miny = Maxy = Minz = Maxz = false;
        }
    }
}
