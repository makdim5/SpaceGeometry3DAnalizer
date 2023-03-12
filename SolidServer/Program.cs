using ConsoleApp1.util;
using SolidServer.SolidWorksPackage;
using System;
using System.Linq;

// Чтобы спрятать консоль необходимо изменить тип выходных данных
// в настройках проекта на "Приложение Windows"

namespace SolidServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //ConnectionWorker.RunServer();
            DoResearch();
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
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Выполнение программы завершено!");
            


        }
    }
}

