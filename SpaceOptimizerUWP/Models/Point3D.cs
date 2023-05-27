using System;

namespace SpaceOptimizerUWP.Models
{
    public class Point3D
    {
        public int Id { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Point3D()
        {
        }
        public Point3D(int id, double x, double y, double z)
        {
            this.Id = id;
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Point3D(double x, double y, double z)
        {
            this.x = x; this.y = y; this.z = z;
        }

        public static bool operator ==(Point3D p1, Point3D p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point3D p1, Point3D p2)
        {
            return !p1.Equals(p2);
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Point3D p = (Point3D)obj;
                return (x == p.x) && (y == p.y) && (z == p.z);
            }
        }

        public override int GetHashCode()
        {
            int hashcode = x.GetHashCode();
            hashcode = 31 * hashcode + y.GetHashCode();
            hashcode = 31 * hashcode + z.GetHashCode();

            return hashcode;
        }

        public override string ToString()
        {
            return $"Point (\nx={x},\ny={y},\nz={z}\n)";
        }
    }
}
