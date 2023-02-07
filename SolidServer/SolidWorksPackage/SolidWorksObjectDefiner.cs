﻿using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using SolidServer.Simulation.Study;
using SolidServer.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.SolidWorksPackage.Simulation.MaterialWorker;
using SolidServer.SolidWorksPackage.Simulation.MeshWorker;
using SolidServer.SolidWorksPackage.Simulation.Study;
using System;

using ConsoleApp1.SolidWorksPackage.NodeWork;

using ConsoleApp1.util;
using System.Windows.Forms;
using SolidWorks.Interop.swconst;

namespace SolidServer.SolidWorksPackage
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
                string material = "AISI 1035 Steel (SS)";// Сталь - Steel
                //var strainValues = studyResults.DefineMinMaxStrainValues("ESTRN");
                var stressValues = studyResults.DefineMinMaxStressValues(param);
                double minvalue = stressValues["min"],
                    criticalValue= 0.2 * MaterialManager.GetMaterials()[material].physicalProperties["SIGXT"], 
                    maxvalue = stressValues["max"]*0.1;


                Console.WriteLine($" минимальное напряжение " +
                    $"VON =  {minvalue} // " +
                    $"максимальное напряжение по VON {stressValues["max"]}" +
                    $"  // предел прочности при растяжении = {MaterialManager.GetMaterials()[material].physicalProperties["SIGXT"]}" +
                    $" критическое > максимальное по VON : {criticalValue > stressValues["max"]} критическое значение:{criticalValue}" 
                    );

                
                var areas = new List<ElementArea>();
                Console.WriteLine("Начало поиска областей");
                var cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, criticalValue, areas, activeDoc);
                Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");


                int counter = 0;
                int surfCounter = 1;
                while (cutElementAreas.Count() != 0)
                {
                    counter++;
                    features.Add(counter, new());
                    Console.WriteLine("Начало выреза областей ...");
                    
                    foreach (var area in cutElementAreas)
                    {
                        
                        areas.Add(area);
                        features[counter].AddRange(
                            ElementAreaWorker.DrawElementArea(
                                activeDoc, null,
                            ElementAreaWorker.SqueezeArea( area)));
                        Console.WriteLine("Конец выреза промежуточной области");

                    }

                    // cut
                    surfCounter += cutElementAreas.Count();

                    string mainBodyName;
                    if (counter == 1){
                        mainBodyName = "Вырез-Вытянуть2";
                        
                    }
                    else
                    {
                        mainBodyName = $"Соединить{counter-1}";
                    }

                    activeDoc.Extension.SelectByID2(mainBodyName, "SOLIDBODY", 0, 0, 0, false, 1, null, 0);

                    for (int i = surfCounter - cutElementAreas.Count(); i <= surfCounter; i++)
                    {
                        activeDoc.Extension.SelectByID2($"Imported{i}", "SOLIDBODY", 0, 0, 0, true, 2, null, 0); //Imported in engl

                    }

                    activeDoc.FeatureManager.InsertCombineFeature(
                        (int)swBodyOperationType_e.SWBODYCUT, null, null);

                    var comb = activeDoc.FeatureManager.InsertCombineFeature((int)swBodyOperationType_e.SWBODYCUT, null, Array.Empty<object>());
                    if (comb != null)
                    {
                        var swCombineBodiesFeatureData = (CombineBodiesFeatureData)comb.GetDefinition();

                        swCombineBodiesFeatureData.AccessSelections(activeDoc, null);
                        swCombineBodiesFeatureData.ReleaseSelectionAccess();

                    }

                    activeDoc.ClearSelection2(true);

                    Console.WriteLine("Конец выреза областей");
                    Console.WriteLine("Повторное исследование ");
                    study.CreateDefaultMesh();

                    study.RunStudy();
                    Console.WriteLine("Повторные результаты и поиск элементов!");
                    studyResults = study.GetResult();

                    Console.WriteLine("Начало поиска областей");
                    cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, criticalValue, areas, activeDoc);
                    Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}\n" +
                    $"Сами области:{cutElementAreas}");


                }

                var crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue, studyResults.DefineMinMaxStressValues(param)["max"]);
                if (crashNodes.Count() != 0)
                {
                    NodeElementAreaWorker.DefineAreaElementDistances(areas, crashNodes);
                    Console.WriteLine("Вывод графика расстояний критических узлов и областей. ...");
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FormChart(NodeElementAreaWorker.area_distances, areas));
                }
                

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

        public static StaticStudyRecord CreateSimpleRecord() 
        {
            // Задание сетки и материала
            Material material = MaterialManager.GetMaterials()["Copper"];
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
