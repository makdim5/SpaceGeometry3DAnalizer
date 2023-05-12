using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolidServer.Utitlites;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.AreaWorkPackage;

namespace SolidServer.Researches
{
    public class DbScanResearchManger : BaseResearchManager
    {
        public DbScanResearchManger(Dictionary<string, string> clasteringConfiguration, Dictionary<string, string> cutConfiguration) :base(clasteringConfiguration, cutConfiguration) { }
        public override Dictionary<string, object> DefineAreas()
        {
            Dictionary<string, object> dataToSend = new Dictionary<string, object>() {
                {"eps", 2},
                {"min_samples", 4},
                {"nodes", wholeNodes}
            };
            var sendData = JsonConvert.SerializeObject(dataToSend);

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
