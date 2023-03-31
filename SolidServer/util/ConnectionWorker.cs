﻿using SolidServer.SolidWorksPackage;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SolidServer.util
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
        private const string DETERMINE_CONDITIONS = "conditions";
        private const string DETERMINE_AREAS = "areas";
        private const string CUT_AREAS = "cut areas";
        private static bool keepListening = true;

        private const int SERVER_PORT = 8005;
        private const int CLIENT_PORT = 8004;
        private const string IP_ADDRESS = "127.0.0.1";


        public static async Task<string> ConnectToClusterizationService(string jsonData, string url= "http://127.0.0.1:5000/dbscan")
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
            var manager = new SolidWorksResearchManager();
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(IP_ADDRESS), SERVER_PORT);
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listenSocket.Bind(ipPoint);
                listenSocket.Listen(10);

                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (keepListening)
                {
                    Socket handler = listenSocket.Accept();
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    byte[] data = new byte[256];

                    do
                    {
                        bytes = handler.Receive(data);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (handler.Available > 0);

                    Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + builder.ToString());

                    try
                    {
                        DoCommand(builder.ToString(), handler, manager);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                        SendData("Упс, что-то пошло не так!", handler);
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Завершение работы сервера!");
        }

        private static void DoCommand(string data, Socket handler, SolidWorksResearchManager manager)
        {
            var dict  = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
            switch (dict["command"])
            {
                case OPEN_SW_MSG:
                    {
                        SolidWorksAppWorker.OpenSolidWorksApp();
                        break;
                    }
                case OPEN_SW_DOC:
                    {
                        SolidWorksAppWorker.OpenDocument(dict["docPath"]);
                        break;
                    }
                case OPEN_SIMULATION_MSG:
                    {
                        SolidWorksAppWorker.GetSimulation();
                        break;
                    }
                case CLOSE_SW_MSG:
                    {
                        SolidWorksAppWorker.CloseSolidWorksApp();
                        break;
                    }
                case DETERMINE_ACTIVE_DOC:
                    {
                        manager.DefineActiveDoc();
                        break;
                    }
                case DETERMINE_RESEARCH_RESULTS:
                    {
                        SendData(manager.GetCompletedStudyResults(), handler);
                        break;
                    }
                case DETERMINE_CONDITIONS:
                    {
                        SendData(manager.DefineCriticalValues(), handler);
                        break;
                    }
                case DETERMINE_AREAS:
                    {
                        manager.DefineAreas();
                        SendData("Области определены!", handler);
                        break;
                    }
                case CUT_AREAS:
                    {
                        manager.CutAreas();
                        SendData("Области вырезаны!", handler);
                        break;
                    }
                case CLOSE_SERVER:
                    {
                        keepListening = false;
                        break;
                    }
            }

        }

        private static void SendData(string data, Socket handler)
        {
            handler.Send(Encoding.Unicode.GetBytes(data));
        }
    }
}
