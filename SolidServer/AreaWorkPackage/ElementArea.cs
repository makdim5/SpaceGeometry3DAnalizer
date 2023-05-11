using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.AreaWorkPackage
{
    public class ElementArea
    {
        public HashSet<Element> elements;

        public Point3D areaCenter;

        public double maxRadius;

        public Dictionary<string, double> dimensions;

        public ElementArea()
        {
           
        }

        public ElementArea(HashSet<Element> elements)
        {
            this.elements = elements;
            areaCenter = DefineAreaCenter();
            maxRadius = DefineAreaRadius();
            dimensions = DefineDimensions();
            
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
                
                    TemporableSumX += element.center.x;
                    TemporableSumY += element.center.y;
                    TemporableSumZ += element.center.z;
                
            }
            return new Point3D
            {
                x = TemporableSumX / elements.Count,
                y = TemporableSumY / elements.Count,
                z = TemporableSumZ / elements.Count
            };
        }

        public Dictionary<string, double> DefineDimensions()
        {
            var nodes = GetNodes();

            double minX = nodes.First().point.x, maxX = nodes.First().point.x,
                 minY = nodes.First().point.y, maxY = nodes.First().point.y,
                 minZ = nodes.First().point.z, maxZ = nodes.First().point.z;

            foreach(Node node in nodes)
            {
                if (minX > node.point.x)
                {
                    minX = node.point.x;
                }
                if (maxX < node.point.x)
                {
                    maxX = node.point.x;
                }

                if (minY > node.point.y)
                {
                    minY = node.point.y;
                }

                if (maxY < node.point.y)
                {
                    maxY = node.point.y;
                }

                if (minZ > node.point.z)
                {
                    minZ = node.point.z;
                }

                if (maxZ < node.point.z)
                {
                    maxZ = node.point.z;
                }
            }

            return new Dictionary<string, double>()
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

        public HashSet<Node> GetNodes()
        {
            var nodes = new HashSet<Node>();
            foreach (var element in elements)
            {
                nodes.UnionWith(element.vertexNodes);
            }
            return nodes;
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
