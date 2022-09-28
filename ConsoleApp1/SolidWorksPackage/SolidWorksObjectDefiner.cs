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
using ConsoleApp1.SolidWorksPackage.NodeWork;
using App2.SolidWorksPackage.NodeWork;
using SolidWorks.Interop.swconst;

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
                var doc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
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
                    maxvalue= 2 * MaterialManager.GetMaterials()[study.MaterialName].physicalProperties["SIGYLD"];

                Console.WriteLine($" минимальное напряжение " +
                    $"VON =  {minvalue} // " +
                    $"максимальное напряжение по VON {stressValues["max"]}" +
                    $"  // предел текущести = {maxvalue}");

                var areas = new List<ElementArea>();
                Console.WriteLine("Начало поиска областей");
                var cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, areas);
                Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");

                int counter = 0;
                while (cutElementAreas.Count() != 0)
                {
                    counter++;
                    features.Add(counter, new());
                    Console.WriteLine("Начало выреза областей ...");
                    foreach (var area in cutElementAreas)
                    {
                        areas.Add(area);
                       features[counter].AddRange(ElementAreaWorker.DrawElementArea(doc, area));
                        //SolidWorksDrawer.DrawSphere(doc, area.areaCenter, area.maxRadius*0.1);
                        //SolidWorksDrawer.DrawSphere(doc, area.areaCenter ,area.maxRadius);

                    }
                    Console.WriteLine("Конец выреза областей");
                    Console.WriteLine("Повторное исследование ");
                    study.CreateDefaultMesh();

                    study.RunStudy();
                    Console.WriteLine("Повторные результаты и поиск элементов!");
                    studyResults = study.GetResult();

                    Console.WriteLine("Начало поиска областей");
                    cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue, areas);
                    Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");

                   
                }

                // отброс последней итерации

                Console.WriteLine("Удаление вырезов последней итерации ...");
                SolidWorksDrawer.UndoFeaturesCuts(doc, features[counter]);


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


                TreeControlItem tree = doc.FeatureManager.GetFeatureTreeRootItem2((int)swFeatMgrPane_e.swFeatMgrPaneBottom);

                var node = tree.GetFirstChild();

                object delObject = null;
                while(node != null)
                {
                    if (node.ObjectType is (int)swTreeControlItemType_e.swFeatureManagerItem_Feature)
                    {
                        var _featureNode = (Feature)node.Object;
                        var nodeType = _featureNode.GetTypeName();
                        var nodeName = _featureNode.Name;

                        if (nodeName == "Вырез-По сечениям5")
                        {
                            doc.Extension.SelectByID2(_featureNode.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                            doc.EditDelete();
                            break;
                        }

                        Console.WriteLine(nodeName);
                    }
      
                    node = node.GetNext();

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
