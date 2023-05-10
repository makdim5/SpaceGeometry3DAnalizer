using System;
using System.Collections.Generic;
using System.Linq;
using SolidServer.SolidWorksPackage.Cells;
using SolidServer.SolidWorksPackage;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using SolidServer.AreaWorkPackage;

namespace SolidServer.Researches
{ 
    public class UnionClusterResearchManager: BaseResearchManager
    {
       
        public List<Sphere> sphereList;

       
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

            //using (StreamWriter writer = new StreamWriter("nodes.json", false))
            //{
            //    writer.Write(sendData);
            //}

            //System.Environment.Exit(0);

            string spheresJson;

            var task = Task.Run(() => ConnectionWorker.ConnectToClusterizationService(sendData, "http://127.0.0.1:5000/union"));
            task.Wait();
            spheresJson = task.Result;


            //spheresJson = File.ReadAllText(@"D:\My programming\SolidSpaceAnalizer\ML_Union_Algorithm\sph.json");


            Dictionary<string, List<Sphere>> result = JsonConvert.DeserializeObject<Dictionary<string, List<Sphere>>>(spheresJson);
            sphereList = result["spheres"];

            Console.WriteLine($"Окончание поиска областей. Их общее количество - {sphereList.Count}");
        }

        public override void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");

            foreach (var area in sphereList)
            {
                SolidWorksDrawer.CutSphiere(activeDoc, area);
                Console.WriteLine("Конец выреза промежуточной области");

            }
            Console.WriteLine("Конец выреза областей");
        }
    }
}
