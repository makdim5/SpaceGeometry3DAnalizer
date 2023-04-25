using SolidServer.Utitlites;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class SimplePlane
    {
        public double A = 1;
        public double B = 1;
        public double C = 1;
        public double D = 1;

        public SimplePlane() { }

        public SimplePlane (Point3D point1, Point3D point2, Point3D point3) 
        {
            DefinePlaneCoffs(point1, point2, point3);
        }

        protected void DefinePlaneCoffs(Point3D point1, Point3D point2, Point3D point3)
        {
            A = (point2.y - point1.y) * (point3.z - point1.z) - (point3.y - point1.y) * (point2.z - point1.z);
            B = -1 * ((point2.x - point1.x) * (point3.z - point1.z) - (point3.x - point1.x) * (point2.z - point1.z));
            C = (point2.x - point1.x) * (point3.y - point1.y) - (point3.x - point1.x) * (point2.y - point1.y);
            D = -1 * (point1.x * A + point1.y * B + point1.z * C);
        }
    }
}
