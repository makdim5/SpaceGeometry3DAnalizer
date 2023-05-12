using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.AreaWorkPackage
{
    public class Area
    {
        public HashSet<Element> elements;

        public HashSet<Node> nodes;

        public Point3D areaCenter;

        public double maxRadius;

        public Dictionary<string, double> dimensions;

        public Area()
        {
            nodes = new();
            elements = new();
        }

        public Area(HashSet<Node> nodes):this()
        {
            this.nodes = nodes;
        }

        public Area(HashSet<Element> elements):this()
        {
            this.elements = elements;
            areaCenter = DefineAreaCenterThroughElements();
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

        public Point3D DefineAreaCenterThroughElements()
        {
            if (!elements.Any())
            {
                return new Point3D(0, 0, 0);
            }
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

        public Point3D DefineAreaCenterThroughNodes()
        {
            if (!nodes.Any())
            {
                return new Point3D(0,0,0);
            }
            double TemporableSumX = 0;
            double TemporableSumY = 0;
            double TemporableSumZ = 0;

            foreach (var node in nodes)
            {

                TemporableSumX += node.point.x;
                TemporableSumY += node.point.y;
                TemporableSumZ += node.point.z;

            }
            return new Point3D
            {
                x = TemporableSumX / nodes.Count,
                y = TemporableSumY / nodes.Count,
                z = TemporableSumZ / nodes.Count
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

            double min1coef = 1, min2coef = 1;

            return new Dictionary<string, double>()
            {
                { "minX", minX*min1coef },
                { "maxX", maxX*min2coef },
                { "minY", minY*min1coef },
                { "maxY", maxY*min2coef },
                { "minZ", minZ*min1coef },
                { "maxZ", maxZ*min2coef },

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

        public double DefineAreaRadiusThroughDimensions()
        {
            var dims = DefineDimensions();
            var minLengths = new List<double>() 
            {
                Math.Abs(dims["maxX"] - dims["minX"]),
                Math.Abs(dims["maxY"] - dims["minY"]),
                Math.Abs(dims["maxZ"] - dims["minZ"]),
            };

            double radius = minLengths.Min() / 2;

            return radius;
        }

        public HashSet<Node> GetNodes()
        {
            var areaNodes = this.nodes;
            if (elements.Count() > 0)
            {
                areaNodes = new HashSet<Node>();
                foreach (var element in elements)
                {
                    areaNodes.UnionWith(element.vertexNodes);
                }
            }
            
            return areaNodes;
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
