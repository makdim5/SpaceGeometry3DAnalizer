using App2.SolidWorksPackage.NodeWork;
using App2.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SolidWorksPackage.NodeWork
{
    public class ElementArea
    {
        public HashSet<Element> elements;

        public Point3D areaCenter;

        public double maxRadius;

        public ElementArea() { }

        public ElementArea(HashSet<Element> elements)
        {
            this.elements = elements;
            areaCenter = DefineAreaCenter();
            maxRadius = DefineAreaRadius();
        }

        public HashSet<Node> DefineInsideNodes(IEnumerable<Node> nodes)
        {
            var insideNodes = new HashSet<Node>();

            foreach (var item in nodes)
            {
                if (this.IsNodeInside(item))
                    insideNodes.Add(item);
            }

            return insideNodes;
        }

        public bool IsNodeInside(Node node)
        {
            double realDistance = MathHelper.DefineDistanceBetweenPoints(node.point, areaCenter);
            return realDistance < maxRadius*1.2;
        }

        public Point3D DefineAreaCenter()
        {
            double TemporableSumX = 0;
            double TemporableSumY = 0;
            double TemporableSumZ = 0;

            foreach (var element in elements)
            {
                foreach (var node in element.vertexNodes)
                {
                    TemporableSumX += node.point.x;
                    TemporableSumY += node.point.y;
                    TemporableSumZ += node.point.z;
                }
            }
            return new Point3D { x = TemporableSumX / elements.Count, 
                y = TemporableSumY / elements.Count, 
                z = TemporableSumZ / elements.Count };
        }

        public double DefineAreaRadius()
        {
            double maxRadius = 0, distance;


            foreach (var element in elements)
            {
                foreach (var node in element.vertexNodes)
                {
                    distance = MathHelper.DefineDistanceBetweenPoints(node.point, areaCenter);
                    if (distance > maxRadius)
                        maxRadius = distance;
                }
            }

            return maxRadius;
        }

        public override string ToString()
        {
            string res = "ElementArea {";

            foreach (Element e in elements)
            {
                res += e.ToString() + "\n";
            }
            res += "}\n\n";

            return res;
        }
    }
}
