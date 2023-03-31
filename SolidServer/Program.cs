using ConsoleApp1.SolidWorksPackage;
using SolidServer.SolidWorksPackage;
using System;
using System.Linq;

// Чтобы спрятать консоль необходимо изменить тип выходных данных
// в настройках проекта на "Приложение Windows"

namespace SolidServer
{
    internal class Program
    {
        static void Main()
        {
            //ConnectionWorker.RunServer();

            DoDBSCANResearch();
            Console.ReadLine();

        }

        static void DoResearch()
        {
            try
            {
                var manager = new SolidWorksResearchManager();
                manager.DefineActiveDoc();
                manager.GetCompletedStudyResults();
                manager.DefineCriticalValues();
                manager.DefineAreas();
                while (manager.cutElementAreas.Count() > 0)
                {
                    manager.CutAreas();
                    manager.RunStudy();
                    manager.GetCompletedStudyResults();
                    manager.DefineAreas();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Выполнение программы завершено!");
        }

        static void DoDBSCANResearch()
        {
            try
            {
                var manager = new DbScanResearchManger();
                manager.DefineActiveDoc();
                manager.GetCompletedStudyResults();
                manager.DefineCriticalValues();
                manager.DefineAreas();
                //manager.CutAreas();
                while (manager.areasList.Count() > 0)
                {
                    manager.CutAreas();
                    manager.RunStudy();
                    manager.GetCompletedStudyResults();
                    manager.DefineAreas();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Выполнение программы завершено!");
        }

        static void DoUnionResearch()
        {
            try
            {
                var manager = new UnionClusterResearchManager();
                manager.DefineActiveDoc();
                manager.GetCompletedStudyResults();
                manager.DefineCriticalValues();
                manager.DefineAreas();
                manager.CutAreas();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Выполнение программы завершено!");

        }
    }
}

