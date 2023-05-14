using System;


namespace SpaceOptimizerUWP.Models
{
    public class Node
    {
        public readonly int number;

        public readonly Point3D point;

        public readonly StressNodeParameters stress;

        public readonly StrainNodeParameters strain;

        public Node(int number , Point3D point, StressNodeParameters stress, StrainNodeParameters strain) {

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

            return text;
        }

        public override bool Equals(object о)
        {
            if (о != null && о is Node)
            {
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
