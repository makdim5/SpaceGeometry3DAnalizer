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

    }
}
