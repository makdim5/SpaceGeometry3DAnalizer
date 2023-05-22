using System;


namespace SpaceOptimizerUWP.Models
{
    public class Node
    {
        public int Id { get; set; }
        public int number { get; set; }

        public Point3D point { get; set; }

        public StressNodeParameters stress { get; set; }

        public StrainNodeParameters strain { get; set; }

        public Node() { }
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

            //text += "\nStrain:\n" + strain.ToString() + "\nStress:\n" + stress.ToString();

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

        
    }
}
