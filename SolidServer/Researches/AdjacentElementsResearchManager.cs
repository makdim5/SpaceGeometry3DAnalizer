﻿using System;
using SolidServer.AreaWorkPackage;
using System.Collections.Generic;
using SolidServer.SolidWorksPackage.ResearchPackage;
using System.Diagnostics;

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

            Console.WriteLine("Вывод на консоль списка всех элементов:");
            foreach (var elem in elems)
            {
                Console.WriteLine(elem.ToString());
            }
            Console.WriteLine("Вывод всех элементов окончен!");


            var newAreas = AreaWorker.DefineAreasPerElementsAdjacent(elems);
            var squeezedAreas = new List<Area>();
            foreach (var area in newAreas)
            {
                squeezedAreas.Add(AreaWorker.SqueezeArea(area,
                    Convert.ToDouble(managerConfiguration["squeezeCoef"] as String)));

            }
            cutAreas = squeezedAreas;

            foreach (var area in cutAreas)
            {
                area.nodes = area.GetNodes();
            }
            return cutAreas;
        }
    }
}
