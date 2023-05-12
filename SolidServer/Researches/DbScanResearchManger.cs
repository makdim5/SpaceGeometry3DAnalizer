using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolidServer.Utitlites;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.AreaWorkPackage;
using System;

namespace SolidServer.Researches
{
    public class DbScanResearchManger : BaseResearchManager
    {
        public DbScanResearchManger(Dictionary<string, object> clasteringConfiguration, Dictionary<string, string> cutConfiguration) :base(clasteringConfiguration, cutConfiguration) { }
        public override Dictionary<string, object> DefineAreas()
        {
            Dictionary<string, object> dataToSend = new Dictionary<string, object>() {
                {"eps", Convert.ToDouble(managerConfiguration["eps"])}, 
                {"min_samples", Convert.ToInt32(managerConfiguration["minSamples"])},
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
