using Newtonsoft.Json;
using SpaceOptimizerUWP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceOptimizerUWP.Services
{
    public class BackConnectorService
    {
        public bool CreateDBSCANResearchManagerInBack(BaseResearch managerConfig, CutConfig cutConfig, string type)
        {
            managerConfig.CheckIsRightAttributes();
            var isOk = false;
            try
            {
                if (type != ResearchType.DBSCAN && type != ResearchType.ADJACMENT)
                {
                    return isOk;
                }
                var task = Task.Run(() =>
                {
                    var data = new Dictionary<string, object>() {
                    { "managerType", type },
                    {"managerConfig", JsonConvert.SerializeObject(managerConfig) },
                    {"cutConfig", JsonConvert.SerializeObject(cutConfig) }
                    };
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "setmngr", data);

                });
                task.Wait();
                var res = task.Result;
                if (res == "ok")
                {
                    isOk = true;
                }
            }
            catch
            {
                throw new Exception("Возникли проблемы с созданием менеджера по причине разрыва" +
                    " соединения с сервером!");
            }

            return isOk;
        }

        public string DetermineResearchResults()
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "research", "");
                });
                task.Wait();
                return task.Result;
            }
            catch
            {
                throw new Exception("Возникли проблемы с определением результатов исследования" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public string DetermineCriticalNodes()
        {
            try
            {

                var task = Task.Run(() =>
                {
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "critical_nodes", "");
                });
                task.Wait();
                return task.Result;
            }
            catch
            {
                throw new Exception("Возникли проблемы с определением критических узлов" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public List<Area> DetermineAreas()
        {
            try
            {
                var task = Task.Run(() =>
                {
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "areas", "");
                });
                task.Wait();
                return JsonConvert.DeserializeObject<List<Area>>(task.Result);
            }
            catch
            {
                throw new Exception("Возникли проблемы с определением областей" +
                    " по причине разрыва соединения с сервером!");
            }
        }

        public bool CutArea(int counter)
        {
            bool isOk = false;
            try
            {
                var task = Task.Run(() =>
                {
                    var data = new Dictionary<string, object>() {
                    { "index", $"{counter}" }
                };
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "cut_areas", data);
                });
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
                return isOk;
            }
            catch
            {
                throw new Exception("Возникли проблемы с вырезом области" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public bool RunStudy()
        {
            bool isOk = false;
            try
            {
                var task = Task.Run(() =>
                {
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "runstudy", "");
                });
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
                return isOk;
            }
            catch
            {
                throw new Exception("Возникли проблемы с запуском исследования" +
                    " по причине разрыва соединения с сервером!");
            }
        }
    }
}
