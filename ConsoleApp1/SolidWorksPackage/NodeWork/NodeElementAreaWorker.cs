using App2.SolidWorksPackage.NodeWork;
using App2.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SolidWorksPackage.NodeWork
{
    public class NodeElementAreaWorker
    {
        public static List<Tuple<ElementArea, Node, double>> distances = new();

        public static void DefineDistances(IEnumerable<ElementArea> areas, IEnumerable<Node> crashNodes)
        {
            foreach (ElementArea area in areas)
            {
                foreach(Node crashNode in crashNodes)
                {
                    distances.Add(new Tuple<ElementArea, Node, double>(
                        area,
                        crashNode,
                        MathHelper.DefineDistanceBetweenPoints(area.areaCenter, crashNode.point)
                        ));
                }
            }
        }
    }
}
