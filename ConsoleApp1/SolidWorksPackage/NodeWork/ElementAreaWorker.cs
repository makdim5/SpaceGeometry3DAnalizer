using App2.SolidWorksPackage.NodeWork;
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

        public static bool AreElementsAdjacent(Element elementOne, Element elementTwo)
        {
            HashSet<Node> nodesOne = new HashSet<Node>(elementOne.vertexNodes);
            HashSet<Node> nodesTwo = new HashSet<Node>(elementTwo.vertexNodes);
            const int vertexesAmountForAdjacency = 3;
            nodesOne.IntersectWith(nodesTwo);
            return nodesOne.Count == vertexesAmountForAdjacency;
        }
    }
}
