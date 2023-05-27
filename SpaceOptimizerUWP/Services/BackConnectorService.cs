using Newtonsoft.Json;
using SpaceOptimizerUWP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpaceOptimizerUWP.Services
{
    public class BackConnectorService
    {
        public static bool CreateResearchManagerInBack(BaseResearch managerConfig, CutConfig cutConfig)
        {
            managerConfig.CheckIsRightAttributes();
            var isOk = false;
            string type = managerConfig.DetermineType();
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
                    {"managerConfig", managerConfig.ToJsonDict() },
                    {"cutConfig", cutConfig.ToJsonDict() }
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с созданием менеджера по причине разрыва" +
                    " соединения с сервером!");
            }

            return isOk;
        }

        public static string DetermineResearchResults()
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с определением результатов исследования" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public static string DetermineCriticalNodes()
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с определением критических узлов" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public static List<Area> DetermineAreas()
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с определением областей" +
                    " по причине разрыва соединения с сервером!");
            }
        }

        public static bool CutArea(int counter)
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с вырезом области" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public static bool LoadAreas(List<Area> areas)
        {
            bool isOk = false;
            try
            {
                var task = Task.Run(() =>
                {
                    var data = new Dictionary<string, object>() {
                    { "areas", JsonConvert.SerializeObject(areas) }
                };
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "loadareas", data);
                });
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
                return isOk;
            }
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с вырезом области" +
                    " по причине разрыва соединения с сервером!");
            }

        }

        public static bool RunStudy()
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
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с запуском исследования" +
                    " по причине разрыва соединения с сервером!");
            }
        }

        public static bool OpenDocument(string path)
        {
            bool isOk = false;
            try
            {
                var data = new Dictionary<string, string>() { { "docPath", path } };
                var task = Task.Run(() => new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync<Dictionary<string, string>>("", "opendoc", data));
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
                return isOk;
            }
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с открытием документа" +
                    " по причине разрыва соединения с сервером!");
            }
        }

        public static bool OpenSolidWorks()
        {
            bool isOk = false;
            try
            {
                var task = Task.Run(() => new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "opensw", ""));
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с открытием SolidWorks" +
                    " по причине разрыва соединения с сервером!");
            }
            return isOk;
        }

        public static bool CloseSolidWorks()
        {
            bool isOk = false;
            try
            {
                var task = Task.Run(() => new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "closesw", ""));
                task.Wait();
                if (task.Result == "ok")
                {
                    isOk = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Возникли проблемы с закрытием SolidWorks" +
                    " по причине разрыва соединения с сервером!");
            }
            return isOk;
        }
    }
}
