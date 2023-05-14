using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SpaceOptimizerUWP.Models
{
    public class Element
    {
        public readonly int number;

        //Первые 4 нода это вершины, остальные 6 это центры ребер
        public readonly IEnumerable<Node> nodes;

        //Верщины
        public readonly HashSet<Node> vertexNodes;

        //Центр области 
        public readonly Point3D center;

        public Element() { }

        public Element(int number, IEnumerable<Node> nodes, Point3D center)
        {

            this.number = number;

            this.nodes = nodes;

            this.vertexNodes = new HashSet<Node>() {
                nodes.ElementAt(0) ,
                nodes.ElementAt(1) ,
                nodes.ElementAt(2) ,
                nodes.ElementAt(3) };

            this.center = center;

        }
        public bool Contains(Node node)
        {
            return Contains(nodes, node);
        }
        private static bool Contains(IEnumerable<Node> nodes, Node node)
        {
            return nodes.Contains(node);
        }

        public List<Point3D> GetNodesCoords()
        {
            var coords = new List<Point3D>();

            foreach (var node in vertexNodes)
            {
                coords.Add(node.point);
            }

            return coords;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Element {number} \n[");
            foreach (var node in vertexNodes)
            {
                sb.AppendLine(node.ToString());
            }
            sb.AppendLine($"]");
            return sb.ToString();
        }

    }
}
