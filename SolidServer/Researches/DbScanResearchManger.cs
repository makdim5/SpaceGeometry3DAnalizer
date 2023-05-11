using Newtonsoft.Json;
using SolidServer.SolidWorksPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolidServer.Utitlites;
using SolidServer.SolidWorksPackage.Cells;

namespace SolidServer.Researches {
    public class DbScanResearchManger : BaseResearchManager
    {
        public override Dictionary<string, object> DefineAreas()
        {
            var sendData = JsonWorker.SerializeNodesToPointsInJSON(wholeNodes);

            var task = Task.Run(() => ConnectionWorker.ConnectToClusterizationService(sendData));
            task.Wait();

            cutAreas = JsonConvert.DeserializeObject<List<Parallelepiped>>(task.Result);
            return new Dictionary<string, object>() { { "cutElementAreasCount", cutAreas.Count() } };
        }

        public override void CutArea(int index)
        {
            SolidWorksDrawer.CutParallelepiped(activeDoc, cutAreas.ElementAt(index) as Parallelepiped);
            Console.WriteLine($"Конец выреза промежуточной области - {index}");
        }
    }
}
