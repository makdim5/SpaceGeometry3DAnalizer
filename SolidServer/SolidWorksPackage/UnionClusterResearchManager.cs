using ConsoleApp1.SolidWorksPackage.NodeWork;
using ConsoleApp1.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.Simulation.Study;
using SolidServer.SolidWorksPackage.NodeWork;
using SolidServer.SolidWorksPackage.Simulation.FeatureFace;
using SolidServer.SolidWorksPackage.Simulation.MaterialWorker;
using SolidServer.SolidWorksPackage.Simulation.MeshWorker;
using SolidServer.SolidWorksPackage.Simulation.Study;
using SolidServer;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using SolidServer.SolidWorksPackage.Cells;
using SolidServer.SolidWorksPackage;
using Newtonsoft.Json;
using SolidServer.util;
using SolidServer.util.mathutils;

namespace ConsoleApp1.SolidWorksPackage
{
    public class UnionClusterResearchManager
    {
        private ModelDoc2 activeDoc;
        private List<FacePlane> facePlanes;
        private StudyManager studyManager;
        private StaticStudy study;
        private StaticStudyResults studyResults;
        private string param = "VON";
        private string material = "AISI 1035 Steel (SS)";// Сталь - Steel
        private double minvalue, maxvalue, criticalValue;
        private List<Sphere> sphereList;

        public UnionClusterResearchManager()
        {
            facePlanes = new();
        }

        public void DefineActiveDoc()
        {
            SolidWorksAppWorker.DefineSolidWorksApp();
            activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();


            foreach (var face in GetFaces(activeDoc))
            {
                var plane = new FacePlane(face, activeDoc);
                if (plane.isPlane)
                    facePlanes.Add(plane);
            }

            Console.WriteLine("Приложение SW и документ определены!\n");
        }

        public void RunStudy()
        {
            study.CreateDefaultMesh();
            study.RunStudy();
        }

        public string GetCompletedStudyResults()
        {
            studyManager = new StudyManager();
            study = studyManager.GetExistingCompletedStudy();
            studyResults = study.GetResult();
            string msg = ("Загружено активное исследование!" +
            $"\nРезультаты исследования: кол-во элементов: {studyResults.meshElements.Count()}, узлов: {studyResults.nodes.Count()}");

            Console.WriteLine(msg);
            return msg;

        }

        public string DefineCriticalValues()
        {

            var stressValues = studyResults.DefineMinMaxStressValues(param);
            minvalue = stressValues["min"];
            criticalValue = 0.2 * MaterialManager.GetMaterials()[material].physicalProperties["SIGXT"];
            maxvalue = stressValues["max"] * 0.1;

            string msg = ($"\nМинимальное напряжение VON =  {minvalue}" +
                $"\nмаксимальное напряжение по VON {stressValues["max"]}" +
                $"\nпредел прочности при растяжении = " +
                $"{MaterialManager.GetMaterials()[material].physicalProperties["SIGXT"]}" +
                $"\nкритическое > максимальное по VON : {criticalValue > stressValues["max"]}" +
                $"\nкритическое значение:{criticalValue}"
                );

            Console.WriteLine(msg);
            return msg;

        }

        public void DefineAreas()
        {
            Console.WriteLine("Начало поиска областей");
            var crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue,
                studyResults.DefineMinMaxStressValues(param)["max"]);

            if (studyResults.DefineMinMaxStressValues(param)["max"] >= criticalValue)
            {
                Console.WriteLine($" Были найдены узлы с превышенной нагрузкой в количестве : " +
                    $"{crashNodes.Count()}");
                return;
            }

            var findNodes = studyResults.DefineNodesPerStressParam(param, minvalue, maxvalue);
            Console.WriteLine($" Количество найденных узлов: {findNodes.Count()}");

            var sendData = SerializeNodesToJSON(findNodes);
            string spheresJson = ConnectionWorker.ConnectToUnionService(sendData);

            Dictionary<string, List<Sphere>> result = JsonConvert.DeserializeObject<Dictionary<string, List<Sphere>>>(spheresJson);
            sphereList = result["spheres"];

            Console.WriteLine($"Окончание поиска областей. Их общее количество - {sphereList.Count}");

        }

        public void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");

            foreach (var area in sphereList)
            {
                SolidWorksDrawer.CutSphiere(activeDoc, area);
                Console.WriteLine("Конец выреза промежуточной области");

            }
            Console.WriteLine("Конец выреза областей");
        }

        private string SerializeNodesToJSON(IEnumerable<Node> nodes)
        {
            List<Point3D> pointsJson = new();

            foreach (var node in nodes)
            {
                pointsJson.Add(node.point);
               
            }

            return JsonConvert.SerializeObject(pointsJson);
        }

        
        public HashSet<Face> GetFaces(ModelDoc2 swDoc)
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

        public StaticStudyRecord CreateSimpleRecord()
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
