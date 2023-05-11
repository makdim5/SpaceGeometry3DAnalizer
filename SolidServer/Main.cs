using SolidServer.Researches;
using SolidServer.Utitlites;
using System;
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
            var manager = new DbScanResearchManger();
            //var manager = new UnionClusterResearchManager();
            //var manager = new SolidWorksResearchManager();

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

