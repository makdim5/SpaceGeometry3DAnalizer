using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;

namespace SolidWorksSimulationManager
{
    public class Element
    {
        public readonly int number;

        //Первые 4 нода это вершины, остальные 6 это центры ребер
        public readonly IEnumerable<Node> nodes;

        //Верщины
        public readonly IEnumerable<Node> vertexNodes;

        //Центр области 
        public readonly Point3D center;

        public Element(int number,IEnumerable<Node> nodes, Point3D center) {

            this.number = number;

            this.nodes = nodes;

            this.vertexNodes = new List<Node>() { 
                nodes.ElementAt(0) ,
                nodes.ElementAt(1) ,
                nodes.ElementAt(2) ,
                nodes.ElementAt(3) };

            this.center = center;

        }

        public bool Contains(Node node) {

            return Contains(nodes, node);

        }

        private static bool Contains(IEnumerable<Node> nodes, Node node) {

            return nodes.Contains(node);

        }

        public bool isAdjacent(Element element) {


            IEnumerable<Node> result = vertexNodes.Intersect(element.vertexNodes);

            return result?.Count() >= 3;

        }

    }
}
