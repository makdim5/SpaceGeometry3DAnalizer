using Newtonsoft.Json;
using SolidServer.SolidWorksPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolidServer.Utitlites;
using SolidServer.SolidWorksPackage.Cells;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.AreaWorkPackage;

namespace SolidServer.Researches
{
    public class DbScanResearchManger : BaseResearchManager
    {
        public DbScanResearchManger(Dictionary<string, string> clasteringConfiguration, Dictionary<string, string> cutConfiguration) :base(clasteringConfiguration, cutConfiguration) { }
        public override Dictionary<string, object> DefineAreas()
        {
            var sendData = JsonConvert.SerializeObject(wholeNodes);

            var task = Task.Run(() => ConnectionWorker.ConnectToClusterizationService(sendData));
            task.Wait();

            var nodes_areas = JsonConvert.DeserializeObject<List<HashSet<Node>>>(task.Result);

            cutAreas = new List<Area>();
            foreach (HashSet<Node> nodes in nodes_areas)
            {
                cutAreas.Add(new Area(nodes));
            }
            return new Dictionary<string, object>() { { "cutAreas", cutAreas } };
        }
    }
}
