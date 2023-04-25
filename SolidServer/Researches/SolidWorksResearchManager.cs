using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.Linq;
using System;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.AreaWorkPackage;
using SolidServer.SolidWorksApplicationPackage;

// Before start open SolidWorks and <3d Part document>
// with "Simulation" AddIn (NOT "Flow Simulation")

namespace SolidServer.Researches
{ 
    internal class SolidWorksResearchManager
    {
        private ModelDoc2 activeDoc;
        private Dictionary<int, List<Feature>> features;
        private List<FacePlane> facePlanes;
        private StudyManager studyManager;
        private StaticStudy study;
        private StaticStudyResults studyResults;
        private string param = "VON";
        private string material = "AISI 1035 Steel (SS)";// Сталь - Steel
        private string impotedConst = "Imported"; // Импортированный - Imported
        private const string startModelName = "Вырез-Вытянуть2";
        private double minvalue, maxvalue, criticalValue;
        private List<ElementArea> areas;
        public IEnumerable<ElementArea> cutElementAreas;
        private int counter;
        private int surfCounter;

        public SolidWorksResearchManager()
        {
            facePlanes = new List<FacePlane>();
            areas = new List<ElementArea>();
            counter = 0;
            surfCounter = 1;
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
            cutElementAreas = studyResults.DetermineCutAreas(param, minvalue, maxvalue,
                criticalValue, areas, activeDoc, facePlanes);
            Console.WriteLine($"Окончание поиска областей. Их общее количество - {cutElementAreas.Count()}");

        }

        public void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");

            foreach (var area in cutElementAreas)
            {

                areas.Add(area);
                ElementAreaWorker.DrawElementArea(activeDoc, area);
                Console.WriteLine("Конец выреза промежуточной области");

            }

            //// cut
            //surfCounter += cutElementAreas.Count();

            //string mainBodyName;
            //if (counter == 1)
            //{
            //    mainBodyName = startModelName;

            //}
            //else
            //{
            //    mainBodyName = $"Соединить{counter - 1}";
            //}

            //activeDoc.Extension.SelectByID2(mainBodyName, "SOLIDBODY", 0, 0, 0, false, 1, null, 0);

            //for (int i = surfCounter - cutElementAreas.Count(); i <= surfCounter; i++)
            //{
            //    activeDoc.Extension.SelectByID2($"{impotedConst}{i}", "SOLIDBODY", 0, 0, 0, true, 2, null, 0); //Imported in engl

            //}

            //activeDoc.FeatureManager.InsertCombineFeature(
            //    (int)swBodyOperationType_e.SWBODYCUT, null, null);

            //var comb = activeDoc.FeatureManager.InsertCombineFeature((int)swBodyOperationType_e.SWBODYCUT, null, Array.Empty<object>());
            //if (comb != null)
            //{
            //    var swCombineBodiesFeatureData = (CombineBodiesFeatureData)comb.GetDefinition();

            //    swCombineBodiesFeatureData.AccessSelections(activeDoc, null);
            //    swCombineBodiesFeatureData.ReleaseSelectionAccess();

            //}
            //counter++;
            //activeDoc.ClearSelection2(true);

            Console.WriteLine("Конец выреза областей");
        }

        //public void DoTest()
        //{
        //    Console.WriteLine("The number of processors " +
        //"on this computer is {0}.",
        //System.Environment.ProcessorCount);
        //    SolidWorksAppWorker.DefineSolidWorksApp();
        //    var activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();

        //    Console.WriteLine("TEST: Приложение SW и документ определены!\n");
        //    var faces = new List<FacePlane>();

        //    foreach (var face in GetFaces(activeDoc))
        //    {
        //        var plane = new FacePlane(face, activeDoc);
        //        if (plane.isPlane)
        //        {
        //            faces.Add(plane);

        //            double[] param = face.MaterialPropertyValues;

        //            if (param == null)
        //            {
        //                param = new double[9] {
        //            0, 0, 0,
        //            1, 1, 0.5,
        //            0.4, 0, 0
        //        };
        //            }

        //            param[0] = 131 / 255f;
        //            param[1] = 231 / 255f;
        //            param[2] = 111 / 255f;

        //            face.MaterialPropertyValues = param;
        //        }

        //    }

        //    Console.WriteLine("The End!");

        //}

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
