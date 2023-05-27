using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SolidServer.Researches;
using SolidServer.SolidWorksApplicationPackage;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;

namespace SolidServer.Utitlites
{
    internal class ConnectionWorker
    {
        private const string OPEN_SW_MSG = "opensw";
        private const string OPEN_SW_DOC = "opendoc";
        private const string CLOSE_SW_MSG = "closesw";
        private const string CLOSE_SERVER = "closeserver";
        private const string OPEN_SIMULATION_MSG = "opensimulation";
        private const string DETERMINE_ACTIVE_DOC = "activedoc";
        private const string DETERMINE_RESEARCH_RESULTS = "research";
        private const string DETERMINE_CRITICAL_NODES = "critical_nodes";
        private const string DETERMINE_AREAS = "areas";
        private const string CUT_AREAS = "cut_areas";
        private const string INIT_MANAGER = "setmngr";
        private const string RUN_STUDY = "runstudy";
        private static bool keepListening = true;

        private const int SERVER_PORT = 8005;
        private const int CLIENT_PORT = 8004;
        private const string IP_ADDRESS = "127.0.0.1";

        private static HttpListener server = new HttpListener();
        private static HttpListenerContext context = null;
        private static BaseResearchManager manager = null;


        public static async Task<string> ConnectToClusterizationService(string jsonData, string url = "http://127.0.0.1:5000/dbscan")
        {
            HttpClient httpClient = new HttpClient();

            httpClient.Timeout = TimeSpan.FromMinutes(40);
            HttpResponseMessage response = await httpClient.PostAsync(url,
                 new StringContent(jsonData, Encoding.UTF8, "application/json"));
            string responce_json = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Данные по кластеризации получены!");
            return responce_json;
        }
        public static void RunServer()
        {
            try
            {
                server.Prefixes.Add($"http://127.0.0.1:{SERVER_PORT}/");
                server.Start();
                Console.WriteLine($"Сервер запущен на http://127.0.0.1:{SERVER_PORT}/. Ожидание подключений...");

                while (keepListening)
                {
                    context = server.GetContext();

                    try
                    {
                        DoCommand();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        SendData("Упс, что-то пошло не так!");
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Завершение работы сервера!");
        }

        private static void DoCommand()
        {
            string command = context.Request.Headers["Command"];
            string sendresult = "";
            try
            {
                switch (command)
                {
                    case OPEN_SW_MSG:
                        {
                            SolidWorksAppWorker.OpenSolidWorksApp();
                            sendresult = "ok";
                            Console.WriteLine("SolidWorks открыт!");
                            break;
                        }
                    case OPEN_SW_DOC:
                        {
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetJson());
                            Console.WriteLine(dict["docPath"]);
                            SolidWorksAppWorker.OpenDocument(dict["docPath"]);
                            sendresult = "ok";
                            Console.WriteLine("Документ SolidWorks открыт!");
                            break;
                        }
                    case CLOSE_SW_MSG:
                        {
                            SolidWorksAppWorker.CloseSolidWorksApp();
                            sendresult = "ok";
                            Console.WriteLine("SolidWorks закрыт!");
                            break;
                        }
                    case INIT_MANAGER:
                        {
                            var jsn = GetJson();
                            Console.WriteLine($"{jsn} ");
                            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsn);
                            var managerConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(dict["managerConfig"]));
                            var cutConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(dict["cutConfig"]));
                            managerConfig["meshParams"] = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(managerConfig["meshParams"]));
                            var managerType = dict["managerType"] as String;
                            if (managerType == "dbscan")
                            {
                                manager = new DbScanResearchManger(managerConfig, cutConfig);
                                sendresult = "ok";
                                Console.WriteLine($"Выбран менеджер {managerType}!");
                            }
                            else if (managerType == "adjacmentElements")
                            {
                                manager = new AdjacentElementsResearchManager(managerConfig, cutConfig);
                                sendresult = "ok";
                                Console.WriteLine($"Выбран менеджер {managerType}!");
                            }
                            else
                            {
                                Console.WriteLine($"Ошибка выбора менеджера!");
                                sendresult = "error";
                            }
                            break;
                        }
                    case DETERMINE_RESEARCH_RESULTS:
                        {
                            if (manager != null)
                            {
                                sendresult = (JsonConvert.SerializeObject(manager.GetCompletedStudyResults()));
                                Console.WriteLine("Результаты исследования прочтены!");
                            }
                            else { sendresult = "error"; }
                            break;
                        }
                    case DETERMINE_CRITICAL_NODES:
                        {
                            if (manager != null)
                            {
                                sendresult = (JsonConvert.SerializeObject(manager.DefineCriticalNodes()));
                                Console.WriteLine("Определение параметров алгоритма и критических узлов завершено!");
                            }
                            else { sendresult = "error"; }
                            break;
                        }
                    case DETERMINE_AREAS:
                        {
                            if (manager != null)
                            {
                                sendresult = (JsonConvert.SerializeObject(manager.DetermineCutAreas()));
                            }
                            else { sendresult = "error"; }
                            break;
                        }
                    case CUT_AREAS:
                        {
                            if (manager != null)
                            {
                                string jsn = GetJson();
                                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsn);
                                manager.CutArea(Convert.ToInt32(dict["index"]), manager.cutConfiguration);
                                Console.WriteLine($"Вырез {Convert.ToInt32(dict["index"])} области!");
                                sendresult = "ok";
                            }
                            else { sendresult = "error"; }
                            break;
                        }
                    case RUN_STUDY:
                        {
                            if (manager != null)
                            {
                                manager.RunStudy();
                                sendresult = "ok";
                            }
                            else { sendresult = "error"; }
                            break;
                        }
                    case CLOSE_SERVER:
                        {
                            keepListening = false;
                            break;
                        }
                    default:
                        {
                            sendresult = "Not found command!";
                            break;
                        }
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                sendresult = ex.Message;
            }
            SendData(sendresult);
        }

        private static void SendData(string data)
        {
            var response = context.Response;
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Flush();
        }

        private static string GetJson()
        {
            var request = context.Request;
            string text;
            using (var reader = new StreamReader(request.InputStream,
                                                 Encoding.UTF8))
            {
                text = reader.ReadToEnd();
            }

            return text;
        }
    }
}
