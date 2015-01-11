namespace Ajubaa.IBModeler.PtsToPolygons
{
    public class CuboidFileInfo
    {
        public int X { get; set; } //Number of points along the three axes
        public int Y { get; set; }
        public int Z { get; set; }
        public float MinX { get; set; } //Positions of the cuboid corners
        public float MaxX { get; set; }
        public float MinY { get; set; }
        public float MaxY { get; set; }
        public float MinZ { get; set; }
        public float MaxZ { get; set; }
    }
}
