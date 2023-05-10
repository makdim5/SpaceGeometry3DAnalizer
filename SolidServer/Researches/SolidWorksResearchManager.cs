using System.Linq;
using System;
using SolidServer.AreaWorkPackage;

// Before start open SolidWorks and <3d Part document>
// with "Simulation" AddIn (NOT "Flow Simulation")

namespace SolidServer.Researches
{ 
    public class SolidWorksResearchManager : BaseResearchManager
    {
     
        public override void DefineAreas()
        {
            Console.WriteLine("Начало поиска областей");
            cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue,
                criticalValue, areas, activeDoc, facePlanes);
            Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");

        }

        public override void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");
            foreach (var area in cutElementAreas)
            {

                areas.Add(area);
                ElementAreaWorker.DrawElementArea(activeDoc, area);
                Console.WriteLine("Конец выреза промежуточной области");

            }
            Console.WriteLine("Конец выреза областей");
        }

      

    }
}
