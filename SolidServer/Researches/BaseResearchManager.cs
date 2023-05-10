using SolidServer.AreaWorkPackage;
using SolidServer.SolidWorksApplicationPackage;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidServer.Researches
{
    public abstract class BaseResearchManager
    {
        protected ModelDoc2 activeDoc;
        protected Dictionary<int, List<Feature>> features;
        protected List<FacePlane> facePlanes;
        protected StudyManager studyManager;
        protected StaticStudy study;
        protected StaticStudyResults studyResults;
        protected string param = "VON";
        protected string material = "AISI 1035 Steel (SS)";// Сталь - Steel
        protected double minvalue, maxvalue, criticalValue;
        protected List<ElementArea> areas;
        public IEnumerable<ElementArea> cutElementAreas;

        public BaseResearchManager()
        {
            facePlanes = new List<FacePlane>();
            areas = new List<ElementArea>();
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

        public abstract void DefineAreas();

        public abstract void CutAreas();

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

            // Определение нагруженных граней с силой в 100 Н
            faceManager.DefineFace("Грань 2", FaceType.ForceLoad, 100);
            var loadFaces = faceManager.GetFacesPerType(FaceType.ForceLoad);


            return new StaticStudyRecord(0, material, fixFaces, loadFaces, mesh);

        }
    }
}
