using System;
using System.Collections.Generic;
using System.Linq;
using SolidServer.SolidWorksPackage.Cells;
using SolidServer.SolidWorksPackage;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SolidServer.Utitlites;

namespace SolidServer.Researches
{ 
    public class UnionClusterResearchManager: BaseResearchManager
    {
        public override Dictionary<string, object> DefineAreas()
        {
            var sendData = JsonWorker.SerializeNodesToPointsInJSON(wholeNodes);
            var task = Task.Run(() => ConnectionWorker.ConnectToClusterizationService(sendData, "http://127.0.0.1:5000/union"));
            task.Wait();
            string spheresJson = task.Result;
            Dictionary<string, List<Sphere>> result = JsonConvert.DeserializeObject<Dictionary<string, List<Sphere>>>(spheresJson);
            cutAreas = result["spheres"];

            return new Dictionary<string, object>() { { "cutElementAreasCount", cutAreas.Count() } };
        }

        public override void CutArea(int index)
        {
            SolidWorksDrawer.CutSphiere(activeDoc, cutAreas.ElementAt(index) as Sphere);
            Console.WriteLine($"Конец выреза промежуточной области - {index}");
        }
    }
}
