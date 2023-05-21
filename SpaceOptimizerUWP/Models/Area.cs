using System;
using System.Collections.Generic;
using System.Linq;

namespace SpaceOptimizerUWP.Models
{
    public class Area
    {
        public int Id { get; set; }
        public HashSet<Element> elements { get; set; }

        public HashSet<Node> nodes { get; set; }

        public Point3D areaCenter;

        public double maxRadius { get; set; }

        public Dictionary<string, double> dimensions;

        public double Volume { get; set; }

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
