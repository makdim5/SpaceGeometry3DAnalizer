using SolidServer.SolidWorksPackage.NodeWork;
using SolidServer.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SolidWorksPackage.NodeWork
{
    public class NodeElementAreaWorker
    {
        public static List<Tuple<ElementArea, Node, double>> area_distances = new();
        public static List<Tuple<ElementArea, Element, Node, double>> element_distances = new();

        public static void DefineDistances(IEnumerable<ElementArea> areas, IEnumerable<Node> crashNodes)
        {
            foreach (ElementArea area in areas)
            {
                foreach(Node crashNode in crashNodes)
                {
                    area_distances.Add(new Tuple<ElementArea, Node, double>(
                        area,
                        crashNode,
                        MathHelper.DefineDistanceBetweenPoints(area.areaCenter, crashNode.point)
                        ));
                }
            }
        }

        public static void DefineAreaElementDistances(IEnumerable<ElementArea> areas, IEnumerable<Node> crashNodes)
        {
            foreach (ElementArea area in areas)
            {
                foreach (var el in area.elements)
                {
                    foreach (Node crashNode in crashNodes)
                    {
                        element_distances.Add(new Tuple<ElementArea,Element, Node, double>(
                            area,el,
                            crashNode,
                            MathHelper.DefineDistanceBetweenPoints(area.areaCenter, crashNode.point)
                            ));
                    }
                }
            }
        }
    }
}
