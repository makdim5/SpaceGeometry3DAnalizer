using ConsoleApp1.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.SolidWorksPackage.NodeWork;
using SolidServer.util.mathutils;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApp1.SolidWorksPackage.NodeWork
{
    public class ElementAreaWorker
    {
        public static List<ElementArea> DefineElementAreas(HashSet<Element> elements)
        {
            var areas = new List<ElementArea>();

            HashSet<Element> areaElements = new();
            HashSet<Element> currentAdjacentElements = new();
            HashSet<Element> newAdjacentElements = new();

            while (elements.Count > 0)
            {
                var firstElement = elements.First();
                newAdjacentElements = DefineAdjacentElementsForCurrentElement(firstElement, elements);
                currentAdjacentElements = newAdjacentElements;
                areaElements.UnionWith(newAdjacentElements);

                while (newAdjacentElements.Count > 0)
                {

                    elements.ExceptWith(currentAdjacentElements);
                    newAdjacentElements = new HashSet<Element>();
                    foreach (var elem in currentAdjacentElements)
                    {
                        newAdjacentElements.UnionWith(DefineAdjacentElementsForCurrentElement(elem, elements));
                    }

                    currentAdjacentElements = newAdjacentElements;
                    areaElements.UnionWith(newAdjacentElements);
                }

                ElementArea newArea = new ElementArea(areaElements);

                if (newArea.elements.Count > 0)
                {
                    areas.Add(newArea);
                }


                areaElements = new HashSet<Element>();
            }

            return areas;
        }


        public static HashSet<Node> ExceptFaceClosestNodes(IEnumerable<Node> cutNodes, List<FacePlane> facePlanes)
        {
            List<HashSet<Node>> nodeParts = new();
            for (int i = 0; i < System.Environment.ProcessorCount; i++)
            {
                HashSet<Node> part = new();
                for (int j = (int)i * cutNodes.Count() / System.Environment.ProcessorCount;
                    j < (int)(i + 1) * cutNodes.Count() / System.Environment.ProcessorCount;
                    j++)
                {
                    part.Add(cutNodes.ElementAt(j));
                }
                nodeParts.Add(part);
            }

            List<Thread> threads = new List<Thread>();
            List<HashSet<Node>> notCloseNodes = new();
            foreach (var part in nodeParts)
            {
                var thread = new Thread(() => { notCloseNodes.Add(ElementAreaWorker.ExceptCloseNodes(part, facePlanes)); });
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            HashSet<Node> rightNodes = new();
            foreach (var nodes in notCloseNodes)
            {
                rightNodes.UnionWith(nodes);
            }

            return rightNodes;
        }


        public static List<SimplePlane> GetPlanesFromAreaDims(ElementArea area)
        {
            var dims = area.dimensions;
            List<SimplePlane> planes = new List<SimplePlane>()  
            { 
             new SimplePlane
             (
                new Point3D (dims["minX"], dims["minY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["minY"], dims["maxZ"]),
                new Point3D(dims["minX"], dims["maxY"], dims["minZ"])
              ),
             new SimplePlane
             (
                new Point3D (dims["maxX"], dims["minY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["minY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["minY"], dims["maxZ"])
              ),
             new SimplePlane
             (
                new Point3D (dims["maxX"], dims["minY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["maxY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["minY"], dims["minZ"])
              ),
             new SimplePlane
             (
                new Point3D (dims["maxX"], dims["maxY"], dims["minZ"]),
                new Point3D(dims["maxX"], dims["minY"], dims["maxZ"]),
                new Point3D(dims["maxX"], dims["minY"], dims["minZ"])
              ),
             new SimplePlane
             (
                new Point3D (dims["maxX"], dims["maxY"], dims["minZ"]),
                new Point3D(dims["minX"], dims["maxY"], dims["maxZ"]),
                new Point3D(dims["minX"], dims["maxY"], dims["minZ"])
              ),
             new SimplePlane
             (
                new Point3D (dims["maxX"], dims["minY"], dims["maxZ"]),
                new Point3D(dims["minX"], dims["maxY"], dims["maxZ"]),
                new Point3D(dims["minX"], dims["minY"], dims["maxZ"])
              )
            };

            return planes;
        }


        public static ElementArea SqueezeArea(ElementArea area, double lessCoefficient = 0.0)
        {
            ElementArea result = area;
            if (lessCoefficient != 0.0)
            {
                var newElements = new HashSet<Element>();

                foreach (var item in area.elements)
                {
                    var nodePoints = item.GetMinimizedVertexes(lessCoefficient, area.areaCenter);
                    var nodes = new List<Node>();
                    for (int i = 0; i < 4; i++)
                    {
                        nodes.Add(new Node(item.vertexNodes.ElementAt(i).number, nodePoints[i],
                            item.vertexNodes.ElementAt(i).stress, item.vertexNodes.ElementAt(i).strain));
                    }
                    newElements.Add(new Element(item.number, nodes, item.center));
                }

                result = new ElementArea(newElements);
            }

            return result;
        }


        public static HashSet<Node> ExceptInsideNodes(IEnumerable<Node> nodes, List<ElementArea> areas)
        {
            var newNodes = new HashSet<Node>();

            foreach (var node in nodes) { newNodes.Add(node); }

            int i = 1;
            foreach (var area in areas)
            {
                Console.WriteLine($"Поиск схожих узлов в {i} области, общее количество узлов - {nodes.Count()}");

                var insideNodes = area.DefineInsideNodes(newNodes);
                Console.WriteLine($"Общее количество схожих узлов после поиска - {insideNodes.Count}, осталось узлов = {newNodes.Count - insideNodes.Count}");
                newNodes.ExceptWith(insideNodes);

                i++;
            }

            return newNodes;
        }

        public static HashSet<Node> ExceptCloseNodes(IEnumerable<Node[]> nodes, IEnumerable
            <SimplePlane> faces)
        {
            var allNodes = new HashSet<Node>();

            foreach (var node in nodes) 
            {
                allNodes.Add(node[0]);
                allNodes.Add(node[1]);
                allNodes.Add(node[2]);
            }


            return ExceptCloseNodes(allNodes,faces);
        }

        public static HashSet<Node> ExceptCloseNodes(HashSet<Node> allNodes, IEnumerable
            <SimplePlane> faces)
        {
            var exceptedCloseNodes = new HashSet<Node>();


            foreach (var face in faces)
            {
                var exNodes = DefineNesNodesToFace(allNodes, face);
                allNodes.ExceptWith(exNodes);
                exceptedCloseNodes.UnionWith(exNodes);

            }

            return exceptedCloseNodes;
        }
        public static HashSet<Node> DefineNesNodesToFace(HashSet<Node> nodes, SimplePlane face)
        {
            HashSet<Node> nesNodes = new();

            foreach (var node in nodes)
            {

                var dist = DefinePointToFaceDistance(node.point, face);

                if (dist < 1)
                {
                    nesNodes.Add(node);
                }

            }

            return nesNodes;
        }


        public static double DefinePointToFaceDistance(Point3D point, SimplePlane face)
        {
            
            double distance = Math.Abs(point.x * face.A + point.y * face.B + point.z * face.C + face.D) / 
                Math.Sqrt(face.A * face.A + face.B * face.B + face.C * face.C);

            return distance;

        }


        public static List<Feature> DrawElementArea(ModelDoc2 doc, ModelDoc2 additionalDoc, ElementArea area)
        {
            List<Feature> features = new List<Feature>();


            //foreach (var element in area.elements)
            //{
            //    var elementPyramid = new PyramidFourVertexArea(element.GetDrawingVertexes(0, area.areaCenter));
            //    features.Add(SolidWorksDrawer.DrawPyramid(doc, elementPyramid));
            //}

            // preparing
            doc.ClearSelection2(true);
            PartDoc? part = (PartDoc)doc;
            var swBody = (Body2)part.CreateNewBody();

            foreach (var surfaceNodes in GetBodySurfacesNodes(area.elements))
            {
                CreateSurface(new[] { surfaceNodes[0].point, surfaceNodes[1].point, surfaceNodes[2].point }, swBody);
            }

            swBody.CreateBodyFromSurfaces();



            //features.Add(swCombineBodiesFeatureData);
            return features;
        }

        private static void CreateSurface(Point3D[] points, Body2 swBody)
        {
            double[] nPt = new double[points.Length * 3];

            for (int i = 0; i < points.Length; i++)
            {
                nPt[i * 3 + 0] = points[i].x / 1000;
                nPt[i * 3 + 1] = points[i].y / 1000;
                nPt[i * 3 + 2] = points[i].z / 1000;
            }

            _ = swBody.CreatePlanarTrimSurfaceDLL(nPt, null);
        }

        public static IEnumerable<Node[]> GetBodySurfacesNodes(IEnumerable<Element> elements)
        {
            var list = new List<Node[]>();
            var surfacesNodes = new List<Node[]>();

            foreach (var element in elements)
            {
                surfacesNodes.AddRange(GetPyramideSurfacesNodes(element.vertexNodes.ToArray()));
            }

            foreach (var surfaceNodes in surfacesNodes)
            {
                if (list.Any(s => s == surfaceNodes)) { continue; }

                foreach (var item in surfacesNodes)
                {
                    if (surfaceNodes == item || list.Any(s => s == item)) { continue; }

                    if (surfaceNodes.All((n) => item.Any((n1) => n.number == n1.number)))
                    {
                        list.Add(item);
                        list.Add(surfaceNodes);
                        break;
                    }
                }
            }

            foreach (var surfaceNodes in list)
            {
                surfacesNodes.Remove(surfaceNodes);
            }

            return surfacesNodes;
        }

        private static IEnumerable<Node[]> GetPyramideSurfacesNodes(Node[] node)
        {
            var surfacesNodes = new List<Node[]>();

            for (int i = 0; i < node.Length; i++)
            {

                int n1Index = 0;
                int n2Index = 1;
                int n3Index = 2;

                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        n3Index = 3;
                        break;
                    case 2:
                        n2Index = 2;
                        n3Index = 3;
                        break;
                    case 3:
                        n1Index = 1;
                        n2Index = 2;
                        n3Index = 3;
                        break;
                }

                surfacesNodes.Add(new[] { node[n1Index], node[n2Index], node[n3Index] });
            }

            return surfacesNodes;
        }




        public static Dictionary<string, HashSet<Element>> MakeAreaElementsCategories(ElementArea area)
        {
            Dictionary<string, HashSet<Element>> categories = new Dictionary<string, HashSet<Element>>()
            {
                { "v", new HashSet<Element>() },
                { "e", new HashSet<Element>() },
                { "1n", new HashSet<Element>() },
                { "2n", new HashSet<Element>() },
                { "3n", new HashSet<Element>() },
                { "4n", new HashSet<Element>() },
            };

            var elems = new HashSet<Element>(area.elements);

            while (elems.Count() != 0)
            {
                var element = elems.First();
                string key = "";
                var neighbours = DefineAdjacentElementsForCurrentElement(element, area.elements);
                var amountOfNeighbours = neighbours.Count() - 1;
                if (amountOfNeighbours == 4)
                {
                    key = "4n";
                }
                else if (amountOfNeighbours == 3)
                {
                    key = "3n";
                }
                else if (amountOfNeighbours == 2)
                {
                    key = "2n";
                }
                else if (amountOfNeighbours == 1)
                {
                    key = "1n";
                }
                else if (amountOfNeighbours == 0)
                {
                    if (DefineAdjacentElementsForCurrentElement(element, area.elements, 2).Count() > 0)
                    {
                        key = "e";
                    }
                    else if (DefineAdjacentElementsForCurrentElement(element, area.elements, 1).Count() > 0)
                    {
                        key = "v";
                    }
                }
                if (key != "")
                {
                    categories[key].Add(element);
                    elems.Remove(element);
                }

            }

            return categories;
        }

        // vertexesAmountForAdjacency = 3 - по грани смежные, 2 - по ребру, 1 - по вершине
        public static HashSet<Element> DefineAdjacentElementsForCurrentElement(
            Element element, IEnumerable<Element> elements, int vertexesAmountForAdjacency = 3)
        {
            return new HashSet<Element>(from elem in elements
                                        where AreElementsAdjacent(elem, element, vertexesAmountForAdjacency)
                                        select elem);
        }

        public static bool AreElementsAdjacent(Element elementOne, Element elementTwo, int vertexesAmountForAdjacency = 3)
        {
            if (elementOne.number == elementTwo.number)
            {
                return true;
            }

            HashSet<Node> nodesOne = new HashSet<Node>(elementOne.vertexNodes);
            HashSet<Node> nodesTwo = new HashSet<Node>(elementTwo.vertexNodes);
            nodesOne.IntersectWith(nodesTwo);
            return nodesOne.Count == vertexesAmountForAdjacency;
        }
    }
}
