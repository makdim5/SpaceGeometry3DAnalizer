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

        public Dictionary<string, Node> dimensions;

        public ElementArea()
        {
            dimensions = new();
        }

        public ElementArea(HashSet<Element> elements) : base()
        {
            this.elements = elements;
            areaCenter = DefineAreaCenter();
            maxRadius = DefineAreaRadius();
            dimensions  = DefineDimensions();
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
            return realDistance < maxRadius * 1.2;
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
            return new Point3D
            {
                x = TemporableSumX / elements.Count,
                y = TemporableSumY / elements.Count,
                z = TemporableSumZ / elements.Count
            };
        }

        public Dictionary<string, Node> DefineDimensions()
        {
            var nodes = new HashSet<Node>();

            foreach(var element in elements)
            {
                foreach(var node in element.vertexNodes)
                {
                    nodes.Add(node);
                }
            }

            Node minX = nodes.ElementAt(0), maxX = nodes.ElementAt(1),
                 minY = nodes.ElementAt(2), maxY = nodes.ElementAt(3),
                 minZ = nodes.ElementAt(4), maxZ = nodes.ElementAt(5);

            foreach(Node node in nodes)
            {
                if (minX.point.x > node.point.x)
                {
                    minX = node;
                }
                if (maxX.point.x < node.point.x)
                {
                    maxX = node;
                }

                if (minY.point.y > node.point.y)
                {
                    minY = node;
                }

                if (maxY.point.y < node.point.y)
                {
                    maxY = node;
                }

                if (minZ.point.z > node.point.z)
                {
                    minZ = node;
                }

                if (maxZ.point.z < node.point.z)
                {
                    maxZ = node;
                }
            }

            return new Dictionary<string, Node>()
            {
                { "minX", minX },
                { "maxX", maxX },
                { "minY", minY },
                { "maxY", maxY },
                { "minZ", minZ },
                { "maxZ", maxZ },

            };

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

            foreach (var e in dimensions)
            {
                res += e.ToString() + "\n";
            }
            res += "}\n\n";

            return res;
        }
    }
}
