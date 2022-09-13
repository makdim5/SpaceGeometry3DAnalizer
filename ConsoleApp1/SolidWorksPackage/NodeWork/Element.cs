﻿using App2.util.mathutils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App2.SolidWorksPackage.NodeWork
{
    public class Element
    {
        public readonly int number;

        //Первые 4 нода это вершины, остальные 6 это центры ребер
        public readonly IEnumerable<Node> nodes;

        //Верщины
        public readonly IEnumerable<Node> vertexNodes;

        //Центр области 
        public readonly Point3D center;

        public Element() { }

        public Element(int number, IEnumerable<Node> nodes, Point3D center)
        {

            this.number = number;

            this.nodes = nodes;

            this.vertexNodes = new List<Node>() {
                nodes.ElementAt(0) ,
                nodes.ElementAt(1) ,
                nodes.ElementAt(2) ,
                nodes.ElementAt(3) };

            this.center = center;

        }

        public bool Contains(Node node)
        {

            return Contains(nodes, node);

        }

        private static bool Contains(IEnumerable<Node> nodes, Node node)
        {

            return nodes.Contains(node);

        }

        public bool isAdjacent(Element element)
        {


            IEnumerable<Node> result = vertexNodes.Intersect(element.vertexNodes);

            return result?.Count() >= 3;

        }

        public IEnumerable<Point3D> GetDrawingVertexes(double coefficient=0.03)
        {
            return MathHelper.GetLessCoordinatesOfPyramid
                (
                    new List<Point3D>()
                        {
                            vertexNodes.ElementAt(0).point,
                            vertexNodes.ElementAt(1).point,
                            vertexNodes.ElementAt(2).point,
                            vertexNodes.ElementAt(3).point
                        },
                    center,
                    coefficient
                );
        }

        public HashSet<Node> DefineInsideNodes(IEnumerable<Node> nodes)
        {
            var insideNodes = new HashSet<Node>();

            foreach(var item in nodes)
            {
                if (this.IsNodeInside(item))
                    insideNodes.Add(item);
            }

            return insideNodes;
        }

        public bool IsNodeInside(Node node)
        {
            double maxDistance = MathHelper.DefineMaxVertexDistanceFromPyramidCenter(
                GetNodexCoords(), center);

            double realDistance = MathHelper.DefineDistanceBetweenPoints(node.point, center);
            return realDistance < maxDistance;
        }

        public List<Point3D> GetNodexCoords()
        {
            var coords = new List<Point3D>();

            foreach (var node in vertexNodes)
            {
                coords.Add(node.point);
            }

            return coords;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Element {number} \n[");
            foreach (var node in vertexNodes)
            {
                sb.AppendLine(node.ToString());
            }
            sb.AppendLine($"]");
            return sb.ToString();
        }

    }
}
