using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.Utitlites
{
    public struct Point3D
    {
        public double x, y, z;

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
            return $"Point ({x},{y},{z})";
        }
    }

    public struct Vector3D
    {
        public double x, y, z;

        public Vector3D(double x, double y, double z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }

    public class MathHelper
    {
        public static List<Point3D> GetLessCoordinatesOfPyramid(
            IEnumerable<Point3D> vertexes, Point3D center, double lessCoefficient)
        {
            if (lessCoefficient < 0 && lessCoefficient > 1)
            {
                throw new ArgumentException("lessCoefficient must be positive or 0," +
                    $"given {lessCoefficient}");

            }
            var coefficient = 1 - lessCoefficient;
            var newVertexes = new List<Point3D>();
            for (int index = 0; index < vertexes.Count(); index++)
            {
                var item = vertexes.ElementAt(index);
                newVertexes.Add(new Point3D(
                    ChangeEndCoordinateOfLineSegment(center.x, item.x, coefficient),
                    ChangeEndCoordinateOfLineSegment(center.y, item.y, coefficient),
                    ChangeEndCoordinateOfLineSegment(center.z, item.z, coefficient)));

            }

            return newVertexes;
        }
        public static double DefineMaxVertexDistanceFromPyramidCenter(
            IEnumerable<Point3D> vertexes, Point3D center)
        {
            List<double> distances = new();
            
            foreach (var vertex in vertexes)
                distances.Add(DefineDistanceBetweenPoints(center, vertex));

            return distances.Max();
        }

        public static double DefineDistanceBetweenPoints(Point3D pointOne, Point3D pointTwo)
        {
            return Math.Sqrt(
                Math.Pow(pointTwo.x - pointOne.x, 2)+
                Math.Pow(pointTwo.y - pointOne.y, 2)+
                Math.Pow(pointTwo.z - pointOne.z, 2)
                );
        }

        public static List<Point3D> MinimizeCoordinatesOfPyramidPerItsCenter(
            IEnumerable<Point3D> vertexes, Point3D center, double lessCoefficient)
        {
            if (lessCoefficient < 0 && lessCoefficient > 1)
            {
                throw new ArgumentException("lessCoefficient must be positive or 0," +
                    $"given {lessCoefficient}");

            }
            var coefficient = 1- lessCoefficient;
            var newVertexes = new List<Point3D>();
            foreach (var item in vertexes)
            {
                
                newVertexes.Add(new Point3D(
                    ChangeEndCoordinateOfLineSegment(center.x, item.x, coefficient),
                    ChangeEndCoordinateOfLineSegment(center.y, item.y, coefficient),
                    ChangeEndCoordinateOfLineSegment(center.z, item.z, coefficient)));

            }

            return newVertexes;
        }

        public static double ChangeEndCoordinateOfLineSegment(
            double staticCoordinate, double endCoordinate, double coefficient)
        {
            
            return Math.Sqrt(coefficient) * (endCoordinate - staticCoordinate) + staticCoordinate;
        }

        public static List<Point3D> DetermineCentersPerArea(Dictionary<string, double> areaDimensions, int xAmount, int yAmount, int zAmount)
        {
            List<Point3D> points = new();

    
            List<double> x_coords = getInterval(areaDimensions["minX"], areaDimensions["maxX"], xAmount);
            List<double> y_coords = getInterval(areaDimensions["minY"], areaDimensions["maxY"], yAmount);
            List<double> z_coords = getInterval(areaDimensions["minZ"], areaDimensions["maxZ"], zAmount);

            foreach (var x in x_coords)
            {
                foreach (var y in y_coords)
                {
                    foreach (var z in z_coords)
                    {
                        points.Add(new Point3D(x, y, z));

                    }
                }
            }
            return points;
        }

        private static List<double> getInterval(double min, double max, int amount)
        {
            var step = (max - min) / amount;
            var interval = new List<double>();
            double vertex_coord = min + step/2;
            while (vertex_coord < max)
            {
                interval.Add(vertex_coord);
                vertex_coord += step;
            }

            return interval;
        }
    }
}
