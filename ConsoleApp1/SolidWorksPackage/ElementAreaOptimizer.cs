using App2.util.mathutils;
using ConsoleApp1.SolidWorksPackage.NodeWork;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.SolidWorksPackage
{
    public class ElementAreaOptimizer
    {
        public class Facet
        {
            private Point3D p1;
            private Point3D p2;
            private Point3D p3;
            public Facet() { }

            public Facet(Point3D p1, Point3D p2, Point3D p3)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.p3 = p3;
            }

            public static bool operator ==(Facet f1, Facet f2)
            {
                return f1.Equals(f2);
            }

            public static bool operator !=(Facet f1, Facet f2)
            {
                return !f1.Equals(f2);
            }


            public override bool Equals(Object obj)
            {
                //Check for null and compare run-time types.
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    Facet p = (Facet)obj;
                    HashSet<Point3D> pointsOne = new HashSet<Point3D>() { p.p1 , p.p2, p.p3};
                    HashSet<Point3D> pointsTwo = new HashSet<Point3D>() { p1, p2, p3 };
                    pointsOne.ExceptWith(pointsTwo);
                    return pointsOne.Count == 0;
                }
            }

            public override int GetHashCode()
            {
                int hashcode = p1.GetHashCode();
                hashcode = 31 * hashcode + p2.GetHashCode();
                hashcode = 31 * hashcode + p3.GetHashCode();
                // и т.д. для остальный полей
                return hashcode;
            }
        }

        class Border
        {
            private HashSet<Facet> facets;
            public Border() { }
            public Border(ElementArea area) 
            { 
                facets = new();
                foreach (var element in area.elements)
                {
                    var adjElementsForCurrent = ElementAreaWorker.DefineAdjacentElementsForCurrentElement(element, area.elements);
                    facets.UnionWith(element.GetFreeFacets(adjElementsForCurrent));
                }
            }
        }
           
        public static bool isClosedLoop(List<Tuple<Point3D, Point3D>> segments)
        {
            bool closedLoop = true;

            List<Tuple<Point3D, Point3D>> loop = new() { segments[0]};
            while (loop.Count != segments.Count)
            {
                int prevCount = loop.Count;
                for (int i = prevCount; i < segments.Count; i++)
                {
                    if (loop[loop.Count - 1].Item2 == segments[i].Item1)
                    {
                        loop.Add(segments[i]);
                        break;
                    }
                }

                if ((loop.Count - prevCount) == 0) { closedLoop = false; break; }
            }
            return closedLoop;
        }
    }
}
