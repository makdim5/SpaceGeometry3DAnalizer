using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.SolidWorksPackage.Cells
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

            vertex1 = vertexes.ElementAt(0);
            vertex2 = vertexes.ElementAt(1);
            vertex3 = vertexes.ElementAt(2);
            vertex4 = vertexes.ElementAt(3);
        }
        public PyramidFourVertexArea(IEnumerable<Node> vertexes)
        {
            if (vertexes.Count() != 4)
            {
                throw new ArgumentException("PyramidFourVertexArea must have 4 vertexes," +
                    $"given {vertexes.Count()}");

            }
            vertex1 = vertexes.ElementAt(0).point;
            vertex2 = vertexes.ElementAt(1).point;
            vertex3 = vertexes.ElementAt(2).point;
            vertex4 = vertexes.ElementAt(3).point;
        }
    }
}
