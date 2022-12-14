using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using App2.Simulation.Study;
using App2.SolidWorksPackage.Simulation.FeatureFace;
using App2.SolidWorksPackage.Simulation.MaterialWorker;
using App2.SolidWorksPackage.Simulation.MeshWorker;
using App2.SolidWorksPackage.Simulation.Study;
using System;

using ConsoleApp1.SolidWorksPackage.NodeWork;

using ConsoleApp1.util;
using System.Windows.Forms;

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
        // with "Simulation" AddIn (NOT "Flow Simulation")
        public static void DoResearch()
        {
            try
            {
                SolidWorksAppWorker.DefineSolidWorksApp();
                var activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
                
                Console.WriteLine("Приложение SW и документ определены!\n");

                Dictionary<int, List<Feature>> features = new();

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
                    criticalValue= 0.2 * MaterialManager.GetMaterials()[study.GetMaterialName()].physicalProperties["SIGXT"], 
                    maxvalue = stressValues["max"]*0.1;


                Console.WriteLine($" минимальное напряжение " +
                    $"VON =  {minvalue} // " +
                    $"максимальное напряжение по VON {stressValues["max"]}" +
                    $"  // предел прочности при растяжении = {MaterialManager.GetMaterials()[study.GetMaterialName()].physicalProperties["SIGXT"]}" +
                    $" критическое > максимальное по VON : {criticalValue > stressValues["max"]} критическое значение:{criticalValue}" 
                    );

                
                var areas = new List<ElementArea>();
                Console.WriteLine("Начало поиска областей");
                var cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, criticalValue, areas);
                Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");


                var newEmptyDoc = SolidWorksAppWorker.CreateNewDocument();
                int counter = 0;
                while (cutElementAreas.Count() != 0)
                {
                    counter++;
                    features.Add(counter, new());
                    Console.WriteLine("Начало выреза областей ...");
                    foreach (var area in cutElementAreas)
                    {
                        areas.Add(area);
                        features[counter].AddRange(ElementAreaWorker.DrawElementArea(activeDoc, null, area));
                        Console.WriteLine("Конец выреза промежуточной области");

                    }
                    Console.WriteLine("Конец выреза областей");
                    Console.WriteLine("Повторное исследование ");
                    study.CreateDefaultMesh();

                    study.RunStudy();
                    Console.WriteLine("Повторные результаты и поиск элементов!");
                    studyResults = study.GetResult();

                    Console.WriteLine("Начало поиска областей");
                    cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, criticalValue, areas);
                    Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}\n" +
                    $"Сами области:{cutElementAreas}");


                }

                var crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue, studyResults.DefineMinMaxStressValues(param)["max"]);
                NodeElementAreaWorker.DefineAreaElementDistances(areas, crashNodes);
                Console.WriteLine("Вывод графика расстояний критических узлов и областей. ...");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormChart(NodeElementAreaWorker.area_distances, areas));

                // отброс последней итерации
                if (counter != 0)
                {
                    Console.WriteLine("Удаление вырезов последней итерации ...");
                    SolidWorksDrawer.UndoFeaturesCuts(activeDoc, features[counter]);
                }


                Console.WriteLine("Выполнение программы завершено. Закройте окно для завершения!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex);
            }
        }

        public static void DoTest()
        {
            try
            {

                SolidWorksAppWorker.DefineSolidWorksApp();
                var doc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();

                Console.WriteLine("Приложение SW и документ определены!\n");

                double unit = 1000;
                doc.ClearSelection();
                doc.SketchManager.Insert3DSketch(true);

                var sketchPoint1 = doc.SketchManager.CreatePoint(-10 / unit, 0 / unit, 10 / unit);
                var sketchPoint2 = doc.SketchManager.CreatePoint(10 / unit, 10 / unit, 10 / unit);
                var sketchPoint3 = doc.SketchManager.CreatePoint(0 / unit, 0 / unit, 0 / unit);

                doc.SketchManager.Insert3DSketch(false);
                doc.ClearSelection();


                sketchPoint1.Select2(true, 1);
                sketchPoint2.Select2(true, 1);
                sketchPoint3.Select2(true, 1);

                Feature swPlane = doc.CreatePlaneThru3Points3(true);
                swPlane.Select2(true, 1);
                swPlane.Name = "plane1";


                doc.InsertSketch2(true);

                Sketch actSketch = doc.SketchManager.ActiveSketch;
                
               
                //doc.SketchManager.CreateCornerRectangle(5/unit, 0, 0, 10/unit, 7/unit, 0);
                doc.SketchManager.CreateCircleByRadius(0 / unit, 2 / unit, 0, 20/unit);
                doc.SketchManager.CreateCircleByRadius(40/ unit, 2 / unit, 0, 10 / unit);
                doc.SketchManager.CreateCircleByRadius(60 / unit, 2 / unit, 0, 5 / unit);
                //doc.InsertSketch2(false);



                doc.FeatureManager.FeatureExtrusion2(true, false, true, 0, 0, 0.01, 0.01, false, 
                    false, false, false, 1.74532925199433E-02, 1.74532925199433E-02,
                    false, false, false, false, true, true, true, 0, 0, false);


                Console.WriteLine("Выполнение программы завершено. Закройте окно для завершения!");
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
            faceManager.DefineFace("Грань 2", FaceType.ForceLoad, 100);
            var loadFaces = faceManager.GetFacesPerType(FaceType.ForceLoad);


            return new StaticStudyRecord(0, material, fixFaces, loadFaces, mesh);

        }


    }
}
