using System.Linq;
using System;
using SolidServer.AreaWorkPackage;
using System.Collections.Generic;
using SolidServer.SolidWorksPackage.ResearchPackage;

// Before start open SolidWorks and <3d Part document>
// with "Simulation" AddIn (NOT "Flow Simulation")

namespace SolidServer.Researches
{ 
    public class SolidWorksResearchManager : BaseResearchManager
    {
        public override Dictionary<string, object> DefineAreas()
        {
            var elems = studyResults.GetElements(wholeNodes) as HashSet<Element>;

            var newAreas = ElementAreaWorker.DefineElementAreas(elems);

            List<ElementArea> general = new();

            foreach (var area in newAreas)
            {
                HashSet<Node> wholeNodes = area.GetNodes();
                wholeNodes.ExceptWith(ElementAreaWorker.ExceptFaceClosestNodes(wholeNodes, facePlanes));
                var newElems = studyResults.GetElements(wholeNodes) as HashSet<Element>;
                foreach (var a in ElementAreaWorker.DefineElementAreas(newElems))
                {
                    var elemsCats = ElementAreaWorker.MakeAreaElementsCategories(a);
                    var union = elemsCats["4n"];
                    union.UnionWith(elemsCats["3n"]);
                    union.UnionWith(elemsCats["2n"]);
                    if (union.Count > 0)
                    {
                        var newElementArea = (new ElementArea(union));
                        Console.WriteLine($"Формирование новой области с количеством элементов  - {newElementArea.elements.Count}");
                        general.Add(newElementArea);
                    }
                }
            }
            cutAreas = general;
            return new Dictionary<string, object>() { { "cutElementAreasCount", cutAreas.Count() } };
        }

        public override void CutArea(int index)
        {
            ElementAreaWorker.DrawElementArea(activeDoc, cutAreas.ElementAt(index) as ElementArea);
            Console.WriteLine($"Конец выреза промежуточной области - {index}");
        }
    }
}
