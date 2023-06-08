using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.AreaWorkPackage
{
    public class Area
    {
        public int Id;
        public HashSet<Element> elements;

        public HashSet<Node> nodes;

        public Point3D areaCenter;
        public string areaName;

        public double maxRadius;

        public Dictionary<string, double> dimensions;

        public double Volume;

        public Area()
        {
            areaCenter = new Point3D();
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
            HashSet<Node> nodes = GetNodes();
                      
            List<double> x_coords = new List<double>();
            List<double> y_coords = new List<double>();
            List<double> z_coords = new List<double>();

            var nodesList = nodes.ToList();

            nodesList.ForEach(node => 
            {
                x_coords.Add(node.point.x);
                y_coords.Add(node.point.y);
                z_coords.Add(node.point.z);
            
            });

            double minX = x_coords.Min(), maxX = x_coords.Max(),
                 minY =y_coords.Min(), maxY = y_coords.Max(),
                 minZ = z_coords.Min(), maxZ = z_coords.Max();

            
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
