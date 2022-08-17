using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media.Media3D;
using SolidWorks.Interop.cosworks;

namespace SolidWorksSimulationManager
{
    public class StaticStudyResults
    {

        public readonly IEnumerable<Node> nodes;

        public readonly IEnumerable<Element> meshElements;

        public StaticStudyResults(ICWResults results, ICWMesh mesh) {

            this.nodes = GetNodes(
                mesh.GetNodes(),
                GetStress(results),
                GetStrain(results));

            this.meshElements = GetMeshElements(this.nodes, mesh.GetElements());

        }

        public IEnumerable<Element> GetElements(IEnumerable<Node> nodes) {

            HashSet<Element> findArea = new HashSet<Element>();

            foreach (Node node in nodes)
            {
                IEnumerable<Element> area =
                    meshElements.Where(
                        (element) => {
                            return element.Contains(node);
                        }
                    );

                findArea.UnionWith(area);
            }

            return findArea;
        }

        private static IEnumerable<Node> GetNodes(object[] nodes, object[] stress, object[] strain) {

            List<Node> result = new();

            for (int i = 0; i < stress.Length / 12; i++) {

                int offset;

                offset = i * 4;
                Point3D point = new Point3D(
                    (float)nodes[offset + 1] * 1000,
                    (float)nodes[offset + 2] * 1000,
                    (float)nodes[offset + 3] * 1000
                    );

                offset = i * (12);
                StressNode stressNode = new StressNode(
                    (float)stress[offset + 1],
                    (float)stress[offset + 2],
                    (float)stress[offset + 3],
                    (float)stress[offset + 4],
                    (float)stress[offset + 5],
                    (float)stress[offset + 6],
                    (float)stress[offset + 7],
                    (float)stress[offset + 8],
                    (float)stress[offset + 9],
                    (float)stress[offset + 10],
                    (float)stress[offset + 11]
                    );

                offset = i * (13);
                StrainNode strainNode = new StrainNode(
                    (float)strain[offset + 1],
                    (float)strain[offset + 2],
                    (float)strain[offset + 3],
                    (float)strain[offset + 4],
                    (float)strain[offset + 5],
                    (float)strain[offset + 6],
                    (float)strain[offset + 7],
                    (float)strain[offset + 8],
                    (float)strain[offset + 9],
                    (float)strain[offset + 10],
                    (float)strain[offset + 11],
                    (float)strain[offset + 12]
                    );

                result.Add(new Node(i + 1, point, stressNode, strainNode));
            }

            return result;
        }

        private static IEnumerable<Element> GetMeshElements(IEnumerable<Node> nodes, object[] elements ) {

            List<Element> result = new List<Element>();

            int offset = 16;

            for (int i = 0; i< elements.Length / offset; i++) {

                List<Node> meshElement = new List<Node>();

                int numberElement = (int)elements[i * offset];

                for(int item = 1;item <= 10;item++){

                    int number = (int)elements[i * offset + item];

                    Node node = nodes.FirstOrDefault( node => node.number == number);

                    meshElement.Add(node);
                }

                Point3D center = new Point3D(
                        (float)elements[i * offset + 11] * 1000,
                        (float)elements[i * offset + 12] * 1000,
                        (float)elements[i * offset + 13] * 1000);

                Element element = new Element(numberElement,meshElement, center);

                result.Add(element);

            }

            return result;

        }

        private static object[] GetStress(ICWResults results ,swsStrengthUnit_e unit = swsStrengthUnit_e.swsStrengthUnitPascal) {

            int error = 0;

            object[] result = results.GetStress(0, 1, null, (int)unit, out error);

            return result;
        }

        private static object[] GetStrain(ICWResults results)
        {
            int error = 0;

            object[] result = results.GetStrain(0, 1, null, out error);

            return result;
        }

        public override string ToString()
        {
            string result = String.Format("StaticStudy Nodes:{0} Elements:{1}", 
                nodes.Count(), 
                meshElements.Count()
                );

            return result;
        }
    }
}
