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
        public SolidWorksResearchManager(Dictionary<string, string> clasteringConfiguration, Dictionary<string, string> cutConfiguration) : base(clasteringConfiguration, cutConfiguration) { }
        public override Dictionary<string, object> DefineAreas()
        {
            var elems = studyResults.GetElements(wholeNodes) as HashSet<Element>;

            var newAreas = AreaWorker.DefineElementAreas(elems);

            List<Area> general = new();

            foreach (var area in newAreas)
            {
                HashSet<Node> wholeNodes = area.GetNodes();
                wholeNodes.ExceptWith(AreaWorker.ExceptFaceClosestNodes(wholeNodes, facePlanes));
                var newElems = studyResults.GetElements(wholeNodes) as HashSet<Element>;
                foreach (var a in AreaWorker.DefineElementAreas(newElems))
                {
                    var elemsCats = AreaWorker.MakeAreaElementsCategories(a);
                    var union = elemsCats["4n"];
                    union.UnionWith(elemsCats["3n"]);
                    union.UnionWith(elemsCats["2n"]);
                    if (union.Count > 0)
                    {
                        var newElementArea = (new Area(union));
                        Console.WriteLine($"Формирование новой области с количеством элементов  - {newElementArea.elements.Count}");
                        general.Add(newElementArea);
                    }
                }
            }
            cutAreas = general;
            return new Dictionary<string, object>() { { "cutAreas", cutAreas } };
        }
    }
}
