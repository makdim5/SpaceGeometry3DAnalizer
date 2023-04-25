namespace SolidServer.SolidWorksPackage.Cells
{
    public class Parallelepiped
    {
        public double minX;
        public double minY;
        public double minZ;
        public double maxX;
        public double maxY;
        public double maxZ;

        public Parallelepiped() { }

        public Parallelepiped(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            this.minX = minX;
            this.minY = minY;
            this.minZ = minZ;
            this.maxX = maxX;
            this.maxY = maxY;
            this.maxZ = maxZ;
        }

        public override string ToString()
        {
            return $"Parallelepiped (minX={minX}, maxX={maxX}, minY={minY}, maxY={maxY}, minZ={minZ}, maxZ={maxZ})";
        }
    }
}
