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
            var newAreas = AreaWorker.DefineAreasPerElementsAdjacent(elems);
            var squeezedAreas = new List<Area>();
            foreach (var area in newAreas)
            {
                squeezedAreas.Add(AreaWorker.SqueezeArea(area, 0.8));

            }
            cutAreas = squeezedAreas;
            return new Dictionary<string, object>() { { "cutAreas", cutAreas } };
        }
    }
}
