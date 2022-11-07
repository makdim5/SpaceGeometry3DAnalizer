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


        public static List<Feature> DrawElementArea(ModelDoc2 doc, ModelDoc2 additionalDoc, ElementArea area)
        {
            List<Feature> features = new List<Feature>();

            Dictionary<string, HashSet<Element>> areaElementsCategories = MakeAreaElementsCategories(area);

            foreach (var category in areaElementsCategories)
            {
                Console.WriteLine($"Вырезаемая область имеет в категории {category.Key} количество элементов -  {category.Value.Count()}");
            }

            foreach (var category in areaElementsCategories)
            {
                Console.WriteLine($"Вырез области по категории {category.Key} ...");
                foreach (var element in category.Value)
                {
                    var elementPyramid = new PyramidFourVertexArea(element.GetDrawingVertexes(0.8, area.areaCenter));
                    features.Add(SolidWorksDrawer.DrawPyramid(doc, elementPyramid));
                    SolidWorksDrawer.DrawPyramid(additionalDoc, elementPyramid, 0);
                }
            }


            return features;
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
