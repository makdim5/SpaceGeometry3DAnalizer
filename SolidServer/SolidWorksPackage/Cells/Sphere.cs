using SolidServer.util.mathutils;

namespace SolidServer.SolidWorksPackage.Cells
{
    public class Sphere
    {
        public Point3D center;
        public double radious;
        public Sphere() { }

        public Sphere(Point3D center, double radious)
        {
            this.center = center;
            this.radious = radious;
        }

        public override string ToString()
        {
            return $"Sphere (center= {center}, rad= {radious})";
        }
    }
}
