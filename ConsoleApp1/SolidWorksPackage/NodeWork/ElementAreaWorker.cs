using App2.SolidWorksPackage;
using App2.SolidWorksPackage.Cells;
using App2.SolidWorksPackage.NodeWork;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;


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

                Console.WriteLine($"Формирование новой области с количеством элементов  - {areaElements.Count}");
                areas.Add(new ElementArea(areaElements));

                areaElements = new HashSet<Element>();
            }

            return areas;
        }
        public static HashSet<Element> DefineAdjacentElementsForCurrentElement(
            Element element, IEnumerable<Element> elements)
        {
            return new HashSet<Element>(from elem in elements
                                        where AreElementsAdjacent(elem, element)
                                        select elem);
        }

        public static HashSet<Node> ExceptInsideNodes(IEnumerable<Node> nodes, List<ElementArea> areas)
        {
            var newNodes = new HashSet<Node>();

            foreach (var node in nodes) { newNodes.Add(node);}

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

        public static List<Feature> DrawElementArea(ModelDoc2 doc, ElementArea area)
        {
            List<Feature> features = new List<Feature>();
            foreach (var element in area.elements)
            {
                var elementPyramid = new PyramidFourVertexArea(element.GetDrawingVertexes(0.2));
                features.Add(SolidWorksDrawer.DrawPyramid(doc, elementPyramid));
            }
            return features;
        }

        public static bool AreElementsAdjacent(Element elementOne, Element elementTwo)
        {
            if (elementOne.number == elementTwo.number)
            {
                return true;
            }

            HashSet<Node> nodesOne = new HashSet<Node>(elementOne.vertexNodes);
            HashSet<Node> nodesTwo = new HashSet<Node>(elementTwo.vertexNodes);
            const int vertexesAmountForAdjacency = 3;
            nodesOne.IntersectWith(nodesTwo);
            return nodesOne.Count == vertexesAmountForAdjacency;
        }
    }
}
