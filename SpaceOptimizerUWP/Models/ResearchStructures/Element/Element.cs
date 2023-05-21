using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SpaceOptimizerUWP.Models
{
    public class Element
    {
        public int Id { get; set; }
        public int number { get; set; }

        //Первые 4 нода это вершины, остальные 6 это центры ребер
        public IEnumerable<Node> nodes;

        //Верщины
        public HashSet<Node> vertexNodes;

        public HashSet<NodeIndex> vertexIndexes { get; set; }

        //Центр области 
        public Point3D center;

        public Element() { }

        public Element(int number, IEnumerable<Node> nodes)
        {

            this.number = number;

            this.nodes = nodes;

            this.vertexNodes = new HashSet<Node>() {
                nodes.ElementAt(0) ,
                nodes.ElementAt(1) ,
                nodes.ElementAt(2) ,
                nodes.ElementAt(3) };

            vertexIndexes = new();
            foreach (Node node in vertexNodes)
            {
                var index = new NodeIndex();
                index.nodeIndex = node.number;

                vertexIndexes.Add(index);
            }

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
