using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceOptimizerUWP.Models
{
    public class Area
    {
        public HashSet<Element> elements;

        public HashSet<Node> nodes;

        public Point3D areaCenter;

        public double maxRadius;

        public Dictionary<string, double> dimensions;

        public double Volume;

        public Area()
        {
            nodes = new();
            elements = new();
        }

        public Area(HashSet<Node> nodes):this()
        {
            this.nodes = nodes;
            dimensions = DefineDimensions();
        }

        public Area(HashSet<Element> elements):this()
        {
            this.elements = elements;
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

            var dims = new Dictionary<string, double>()
            {
                { "minX", minX},
                { "maxX", maxX},
                { "minY", minY},
                { "maxY", maxY},
                { "minZ", minZ},
                { "maxZ", maxZ},

            };
            Volume = Math.Abs(dims["minX"] - dims["maxX"]) * Math.Abs(dims["minY"] - dims["maxY"]) * Math.Abs(dims["minZ"] - dims["maxZ"]);
           
            return dims;

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
            string res = $"ElementArea nodes count - {nodes.Count()}," +
                $" elements count  - {elements.Count()}";

            return res;
        }
    }
}
