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
            Dictionary<string, object> elementCusteringConfiguration = new Dictionary<string, object>()
            {
                { "meshParams", new Dictionary<string, string>()
                    {
                        {"Quality", "1" },
                        {"UseJacobianCheck", "2" },
                        {"MesherType", "1" },
                        {"MinElementsInCircle","8" },
                        {"GrowthRatio", "1,4" },
                        {"SaveSettingsWithoutMeshing", "0" },
                        {"Unit", "0" }
                    }
                },
                {"filterParam", "VON" },
                {"coef1", "0,2" },
                {"coef2", "0,1" },
                {"materialParam", "SIGXT" },
                {"squeezeCoef", "0,8" },
                {"nodesIntersectionAmount", "2" }
            };
            Dictionary<string, object> dbscanCusteringConfiguration = new Dictionary<string, object>() 
            {
                { "meshParams", new Dictionary<string, string>()
                    {
                        {"Quality", "1" },
                        {"UseJacobianCheck", "2" },
                        {"MesherType", "1" },
                        {"MinElementsInCircle","8" },
                        {"GrowthRatio", "1,4" },
                        {"SaveSettingsWithoutMeshing", "0" },
                        {"Unit", "0" }
                    }
                },
                {"filterParam", "VON" },
                {"coef1", "0,2" },
                {"coef2", "0,1" },
                {"materialParam", "SIGXT" },
                {"eps", "2" },
                {"minSamples", "4"}
            };
            Dictionary<string, string> cutConfiguration = new Dictionary<string, string>() 
            {
                {"cutType", "node"},
                {"nodeCutWay", "figure"},
                {"figureType", "rect" }
            };
            //var manager = new DbScanResearchManger(dbscanCusteringConfiguration, cutConfiguration);
            var manager = new SolidWorksResearchManager(elementCusteringConfiguration, cutConfiguration);

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

