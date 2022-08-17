using App2.Simulation.Study;
using App2.SolidWorksPackage;
using App2.SolidWorksPackage.Simulation.FeatureFace;
using App2.SolidWorksPackage.Simulation.MaterialWorker;
using App2.SolidWorksPackage.Simulation.MeshWorker;
using App2.SolidWorksPackage.Simulation.Study;
using SolidWorks.Interop.cosworks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace App2
{
    // Before start open SolidWorks and <3d Part document>
    //  with "Simulation" AddIn (NOT "Flow Simulation")
    internal class Program
    {
        [HandleProcessCorruptedStateExceptions, SecurityCritical]
        static void Main(string[] args)
        {
            Console.WriteLine("Console SW TEST APP\n");

            
            SolidWorksAppWorker.DefineSolidWorksApp();
            SolidWorksAppWorker.DefineActiveSolidWorksDocument();
            Console.WriteLine("App and doc are here!\n");

            Console.WriteLine("Создание исследования запущено ...");

            Material material = MaterialManager.GetMaterials()["Медь"];
            var mesh = new Mesh();

            var document = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
            FeatureFaceManager faceManager = new FeatureFaceManager(document);

            // Set fixed faces
            faceManager.DefineFace("Грань 1", FaceType.Fixed);
            var fixFaces = faceManager.GetFacesPerType(FaceType.Fixed);

            // Set loaded faces
            faceManager.DefineFace("Грань 2", FaceType.ForceLoad, 100);
            var loadFaces = faceManager.GetFacesPerType(FaceType.ForceLoad);


            StaticStudyRecord studyRecord = new StaticStudyRecord(0, material, fixFaces, loadFaces, mesh);

            StudyManager studyManager;
            StaticStudy study;
            try
            {
                studyManager = new StudyManager();
                study = studyManager.CreateStudy(studyRecord);
                Console.WriteLine("Создание исследования завершено. Проведение исследования начато ...");
                study.RunStudy();
                Console.WriteLine("Проведение исследования завершено успешно!" +
                    $" Результаты исследования:{study.GetResult()} ");    
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex);
            }
            Console.ReadLine();
            
        }
    }
}
