using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.util.mathutils
{
    public struct Point3D
    {
        public double x, y, z;

        public Point3D(double x, double y, double z)
        {
            this.x = x; this.y = y; this.z = z;
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
            for (int index=0; index < vertexes.Count(); index++)
            {
                var item = vertexes.ElementAt(index);
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
    }
}
