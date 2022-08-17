using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace SolidWorksSimulationManager
{
    public class Node
    {
        public readonly int number;

        public readonly Point3D point;

        public readonly StressNode stress;

        public readonly StrainNode strain;


        public Node(int number , Point3D point, StressNode stress, StrainNode strain) {

            this.number = number;
            this.point = point;
            this.stress = stress;
            this.strain = strain;

        }

        public override string ToString()
        {
            string text = String.Format("Node:{0} X:{1:f5} Y:{2:f5} Z:{3:f5}", 
                number,
                point.X, 
                point.Y,
                point.Z);

            return text;
        }
    }
}
