using System;
using SolidServer.AreaWorkPackage;
using System.Collections.Generic;
using SolidServer.SolidWorksPackage.ResearchPackage;

// Before start open SolidWorks and <3d Part document>
// with "Simulation" AddIn (NOT "Flow Simulation")

namespace SolidServer.Researches
{
    public class AdjacentElementsResearchManager : BaseResearchManager
    {
        public AdjacentElementsResearchManager(Dictionary<string, object> clasteringConfiguration,
            Dictionary<string, string> cutConfiguration) : base(clasteringConfiguration,
                cutConfiguration) { }
        public override List<Area> DefineAreas()
        {
            // поиск областей по смежным элементам
            var elems = studyResults.GetElements(wholeNodes,
               Convert.ToInt32(managerConfiguration["nodesIntersectionAmount"])) as HashSet<Element>;
            var newAreas = AreaWorker.DefineAreasPerElementsAdjacent(elems);
            var squeezedAreas = new List<Area>();
            foreach (var area in newAreas)
            {
                squeezedAreas.Add(AreaWorker.SqueezeArea(area,
                    Convert.ToDouble(managerConfiguration["squeezeCoef"] as String)));

            }
            cutAreas = squeezedAreas;
            return cutAreas;
        }
    }
}
