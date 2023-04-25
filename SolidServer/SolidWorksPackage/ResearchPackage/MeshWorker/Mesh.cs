using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class Mesh
    {
        public const double DEFAULT_ELEMENT_SIZE = 3.19728742;
        public const double DEFAULT_TOLERANCE = 0.15986437;
        public readonly double averageGlobalElementSize;

        public readonly double tolerance;

        public Mesh(double averageGlobalElementSize = DEFAULT_ELEMENT_SIZE,
            double tolerance = DEFAULT_TOLERANCE)
        {

            this.averageGlobalElementSize = averageGlobalElementSize;

            this.tolerance = tolerance;
        }


    }
}
