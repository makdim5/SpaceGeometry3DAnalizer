using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using App2.Simulation.Study;
using App2.SolidWorksPackage.Simulation.FeatureFace;
using App2.SolidWorksPackage.Simulation.MaterialWorker;
using App2.SolidWorksPackage.Simulation.MeshWorker;
using App2.SolidWorksPackage.Simulation.Study;
using System;
using App2.SolidWorksPackage.Cells;

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
            try
            {
                SolidWorksAppWorker.DefineSolidWorksApp();
                var doc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
                Console.WriteLine("Приложение SW и документ определены!\n");

                var studyManager = new StudyManager();

                var study = studyManager.GetExistingCompletedStudy();

                if (study == null)
                {
                    Console.WriteLine("Создание исследования запущено ...");
                    study = studyManager.CreateStudy(CreateSimpleRecord());
                    Console.WriteLine("Создание исследования завершено. Проведение исследования начато ...");
                    study.RunStudy();
                    Console.WriteLine("Проведение исследования завершено успешно!");
                }
                else
                {
                    Console.WriteLine("Загружено активное исследование!");
                }


                var studyResults = study.GetResult();


                Console.WriteLine($" Результаты исследования: кол-во элементов: {studyResults.meshElements.Count()}, узлов: {studyResults.nodes.Count()}");

               
                string param = "VON";
                //var strainValues = studyResults.DefineMinMaxStrainValues("ESTRN");
                var stressValues = studyResults.DefineMinMaxStressValues(param);
                double minvalue = stressValues["min"],
                    maxvalue= 2 * MaterialManager.GetMaterials()[study.MaterialName].physicalProperties["SIGYLD"];

                Console.WriteLine($" минимальное напряжение " +
                    $"VON =  {minvalue} // " +
                    $"максимальное напряжение по VON {stressValues["max"]}" +
                    $"  // предел текущести = {maxvalue}");


                var cutElements = studyResults.DetermineCutElements(param, minvalue, maxvalue);
                while (cutElements.Count() != 0)
                {
                    Console.WriteLine($"Вырез элементов {cutElements.Count()}");
                    int i = 0;
                    foreach (var element in cutElements)
                    {
                        if (i > 5)
                            break;
                        var elementPyramid = new PyramidFourVertexArea(element.GetDrawingVertexes(0.2));
                        SolidWorksDrawer.DrawPyramid(doc, elementPyramid);
                        i++;
                    }
                    Console.WriteLine("Повторное исследование ");
                    study.CreateDefaultMesh();

                    study.RunStudy();
                    Console.WriteLine("Повторные результаты и поиск элементов!");
                    studyResults = study.GetResult();
                    cutElements = studyResults.DetermineCutElements(param, minvalue, maxvalue);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex);
            }
        }

        public static StaticStudyRecord CreateSimpleRecord() 
        {
            // Задание сетки и материала
            Material material = MaterialManager.GetMaterials()["Медь"];
            var mesh = new Mesh();

            FeatureFaceManager faceManager = new FeatureFaceManager(
                SolidWorksAppWorker.DefineActiveSolidWorksDocument());

            // Определение фиксированных граней
            faceManager.DefineFace("Грань 1", FaceType.Fixed);
            var fixFaces = faceManager.GetFacesPerType(FaceType.Fixed);

            // Определение нагруженных граней с силой в 10000000 Н
            faceManager.DefineFace("Грань 2", FaceType.ForceLoad, 9900000);
            var loadFaces = faceManager.GetFacesPerType(FaceType.ForceLoad);


            return new StaticStudyRecord(0, material, fixFaces, loadFaces, mesh);

        }


    }
}
