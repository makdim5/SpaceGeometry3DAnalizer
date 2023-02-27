using SolidServer.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SolidServer.SolidWorksPackage.NodeWork
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
                point.x, 
                point.y,
                point.z);

            //text += $"\n-Stress:\n{stress}\n-Strain:\n{strain}";

            return text;
        }

        public override bool Equals(object о)
        {
            if (о != null && о is Node)
            {

                // Теперь проверим, что данный объект Person

                // и текущий объект (this) несут одинаковую информацию.

               Node temp = (Node)о;

                if (temp.point.x == this.point.x && temp.point.y == this.point.y && temp.point.z == this.point.z)
                    return true;

            }

            return false;

        }

        public override int GetHashCode()
        {
            int hashcode = point.x.GetHashCode();
            hashcode = 31 * hashcode + point.y.GetHashCode();
            hashcode = 31 * hashcode + point.z.GetHashCode();
            return hashcode;
        }
    }
}
