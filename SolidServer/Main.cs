using SolidServer.Researches;
using SolidServer.Utitlites;
using System;
using System.Collections.Generic;
using System.Diagnostics;

// Чтобы спрятать консоль необходимо изменить тип выходных данных
// в настройках проекта на "Приложение Windows"

namespace SolidServer
{
    internal class MainProgramm
    {
        static void Main()
        {
            //StartAsServer();
            DoResearch();
        }

        static void DoResearch()
        {
            Dictionary<string, string> elementCusteringConfiguration = new Dictionary<string, string>();
            Dictionary<string, string> dbscanCusteringConfiguration = new Dictionary<string, string>();
            Dictionary<string, string> cutConfiguration = new Dictionary<string, string>() 
            {
                {"cutType", "node"},
                {"nodeCutWay", "figure"},
                {"figureType", "rect" }
            };
            var manager = new DbScanResearchManger(dbscanCusteringConfiguration, cutConfiguration);
            //var manager = new SolidWorksResearchManager(elementCusteringConfiguration , cutConfiguration);

            manager.RunInLoop();
            Console.WriteLine("Выполнение программы завершено!");
            Console.ReadLine();
        }

        static void StartAsServer()
        {
            var url = "xamluidemosolid://";
            var psi = new ProcessStartInfo();
            psi.UseShellExecute = true;
            psi.FileName = url;
            Process.Start(psi);
            ConnectionWorker.RunServer();
        }

    }
}

