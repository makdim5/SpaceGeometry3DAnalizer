using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using App2.Simulation.Study;
using App2.SolidWorksPackage;
using App2.SolidWorksPackage.Simulation.FeatureFace;
using App2.SolidWorksPackage.Simulation.MaterialWorker;
using App2.SolidWorksPackage.Simulation.MeshWorker;
using App2.SolidWorksPackage.Simulation.Study;
using SolidWorks.Interop.cosworks;
using SolidWorks.Interop.swconst;
using System;

namespace App2.SolidWorksPackage
{
    internal class SolidWorksObjectDefiner
    {
        public static HashSet<Face> GetFaces(ModelDoc2 swDoc)
        {

            HashSet<object> result = new HashSet<object>();

            object[] features = swDoc.FeatureManager.GetFeatures(true) as object[];

            foreach (Feature feature in features)
            {

                object[] faces = (object[])feature.GetFaces();

                if (faces != null)
                {
                    foreach (Face face in faces)
                    {
                        result.Add(face);
                    }

                }
            }

            return new HashSet<Face>(result.Cast<Face>());
        }

        // Before start open SolidWorks and <3d Part document>
        //  with "Simulation" AddIn (NOT "Flow Simulation")
        public static void DoResearch()
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
        }
    }
}
