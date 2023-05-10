using Newtonsoft.Json;
using SolidServer.SolidWorksPackage;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolidServer.Utitlites;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.SolidWorksApplicationPackage;
using SolidServer.AreaWorkPackage;
using SolidServer.SolidWorksPackage.Cells;
using System.IO;

namespace SolidServer.Researches { 
    public class DbScanResearchManger: BaseResearchManager
    {
        public List<Parallelepiped> areasList;

        
        public override void DefineAreas()
        {
            Console.WriteLine("Начало поиска областей");
            var crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue,
                studyResults.DefineMinMaxStressValues(param)["max"]);

            if (studyResults.DefineMinMaxStressValues(param)["max"] >= criticalValue)
            {
                Console.WriteLine($" Были найдены узлы с превышенной нагрузкой в количестве : " +
                    $"{crashNodes.Count()}");
                return;
            }

            var findNodes = studyResults.DefineNodesPerStressParam(param, minvalue, maxvalue);


            HashSet<Node> wholeNodes = new HashSet<Node>(findNodes);
            wholeNodes.ExceptWith(ElementAreaWorker.ExceptFaceClosestNodes(wholeNodes, facePlanes));
            Console.WriteLine($" Количество найденных узлов: {wholeNodes.Count()}");
            var sendData = JsonWorker.SerializeNodesToPointsInJSON(wholeNodes);

            var task = Task.Run(() => ConnectionWorker.ConnectToClusterizationService(sendData));
            task.Wait();

            areasList = JsonConvert.DeserializeObject<List<Parallelepiped>>(task.Result);

            Console.WriteLine($"Окончание поиска областей. Их общее количество - {areasList.Count}");
        }

        public override void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");

            foreach (var area in areasList)
            {
                SolidWorksDrawer.CutParallelepiped(activeDoc, area);
                Console.WriteLine("Конец выреза промежуточной области");

            }
            Console.WriteLine("Конец выреза областей");
        }

        

    }
}
