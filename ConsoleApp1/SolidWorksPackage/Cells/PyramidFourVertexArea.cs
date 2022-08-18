using App2.SolidWorksPackage.NodeWork;
using App2.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage.Cells
{
    public class PyramidFourVertexArea
    {
        public readonly Point3D vertex1;
        public readonly Point3D vertex2;
        public readonly Point3D vertex3;
        public readonly Point3D vertex4;

        public PyramidFourVertexArea() {}

        public PyramidFourVertexArea(
            Point3D vertex1,
            Point3D vertex2,
            Point3D vertex3,
            Point3D vertex4)
        {

            
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.vertex3 = vertex3;
            this.vertex4 = vertex4;

        }

        public PyramidFourVertexArea(IEnumerable<Point3D> vertexes)
        {
            if (vertexes.Count() != 4)
            {
                throw new ArgumentException("PyramidFourVertexArea must have 4 vertexes," +
                    $"given {vertexes.Count()}");

            }

            this.vertex1 = vertexes.ElementAt(0);
            this.vertex2 = vertexes.ElementAt(1);
            this.vertex3 = vertexes.ElementAt(2);
            this.vertex4 = vertexes.ElementAt(3);

        }

        public PyramidFourVertexArea(IEnumerable<Node> vertexes)
        {
            if (vertexes.Count() != 4)
            {
                throw new ArgumentException("PyramidFourVertexArea must have 4 vertexes," +
                    $"given {vertexes.Count()}");

            }

            this.vertex1 = vertexes.ElementAt(0).point;
            this.vertex2 = vertexes.ElementAt(1).point;
            this.vertex3 = vertexes.ElementAt(2).point;
            this.vertex4 = vertexes.ElementAt(3).point;

        }


    }
}
