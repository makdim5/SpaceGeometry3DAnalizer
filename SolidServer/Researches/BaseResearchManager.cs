using Newtonsoft.Json;
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
        protected IEnumerable<Node> crashNodes;
        protected StudyManager studyManager;
        protected StaticStudy study;
        protected StaticStudyResults studyResults;
        protected string param;
        protected double minvalue, maxvalue, criticalValue;
        protected List<Area> areas;
        protected HashSet<Node> wholeNodes;
        public List<Area> cutAreas;
        protected Dictionary<string, string> cutConfiguration;
        protected Dictionary<string, object> managerConfiguration;
        public BaseResearchManager(Dictionary<string, object> clasteringConfiguration, Dictionary<string, string> cutConfiguration)
        {
            this.cutConfiguration = cutConfiguration;
            cutAreas = new List<Area>();
            crashNodes = new List<Node>();
            facePlanes = new List<FacePlane>();
            areas = new List<Area>();
            activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
            studyManager = new StudyManager();
            Console.WriteLine("Приложение SolidWorks и документ определены!\n");
            this.managerConfiguration = clasteringConfiguration;
            param = managerConfiguration["filterParam"] as String;
        }

        public void RunInLoop()
        {
            try
            {
                GetCompletedStudyResults(); // получить результаты выполненного исследования
                DefineCriticalNodes(); // определение критических точек
                DetermineCutAreas(); // опре
                while (crashNodes.Count() == 0 && cutAreas.Count() > 0)
                {
                    CutAreas();
                    RunStudy();
                    GetCompletedStudyResults(); // получить результаты выполненного исследования
                    DefineCriticalNodes();
                    DetermineCutAreas();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public Dictionary<string, object> GetCompletedStudyResults()
        {
            study = studyManager.GetExistingCompletedStudy();
            studyResults = study.GetResult();
            string msg = ("Загружено активное исследование!" +
            $"\nРезультаты исследования: кол-во элементов - {studyResults.meshElements.Count()}, узлов - {studyResults.nodes.Count()}");

            Console.WriteLine(msg);
            return new Dictionary<string, object>() { { "msg", msg } };

        }

        public Dictionary<string, object> DefineCriticalNodes()
        {
            var stressValues = studyResults.DefineMinMaxStressValues(param);
            var materialInfo = MaterialManager.GetMaterials()[study.MaterialName];
            var materialParam = managerConfiguration["materialParam"] as String;
            minvalue = stressValues["min"];
            criticalValue = Convert.ToDouble(managerConfiguration["coef1"]) * materialInfo.physicalProperties[materialParam];
            maxvalue = stressValues["max"] * Convert.ToDouble(managerConfiguration["coef2"]);

            string msg = ($"\nМинимальное напряжение {param} =  {minvalue}" +
                $"\nмаксимальное напряжение по {param} {stressValues["max"]}" +
                $"\nпредел прочности при растяжении = " +
                $"{materialInfo.physicalProperties[materialParam]}" +
                $"\nкритическое > максимальное по {param} : {criticalValue > stressValues["max"]}" +
                $"\nкритическое значение:{criticalValue}\n"
                );
            if (studyResults.DefineMinMaxStressValues(param)["max"] >= criticalValue)
            {
                crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue, studyResults.DefineMinMaxStressValues(param)["max"]);
                msg += ($"Были найдены узлы с превышенной нагрузкой в количестве : {crashNodes.Count()}");
            }
            Console.WriteLine(msg);
            return new Dictionary<string, object>() { { "msg", msg }, { "crashNodes", JsonConvert.SerializeObject(crashNodes) } };
        }

        public Dictionary<string, object> DetermineCutAreas()
        {
            // Если существуют критические узлы, то области не формируем
            if (crashNodes.Count() > 0)
            {
                return new Dictionary<string, object>();
            }
            Console.WriteLine("Начало поиска областей");
            facePlanes = FeatureFaceManager.DefineFacePlanes(activeDoc); // определение граней детали
            wholeNodes = new HashSet<Node>(studyResults.DefineNodesPerStressParam(param, minvalue, maxvalue)); // 
            wholeNodes.ExceptWith(AreaWorker.ExceptFaceClosestNodes(wholeNodes, facePlanes)); // 
            string msg = $"Количество узлов для выявления областей : {wholeNodes.Count()}\n";

            var result = DefineAreas();

            Console.WriteLine(msg + $"Окончание поиска областей. Их общее количество - {cutAreas.Count()}");
            return result;

        }
        public abstract Dictionary<string, object> DefineAreas();
        public void CutArea(int index, Dictionary<string, string> cutConfiguration)
        {
            AreaWorker.DrawAreaPerConfiguration(activeDoc, cutAreas.ElementAt(index) as Area, studyResults, cutConfiguration);
            Console.WriteLine($"Конец выреза промежуточной области - {index + 1}");
        }

        public void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");
            for (int i = 0; i < cutAreas.Count(); i++)
            {
                CutArea(i, cutConfiguration);
            }
            cutAreas = new List<Area>();
            Console.WriteLine("Конец выреза областей");
        }

        public void RunStudy()
        {
            var meshConf = managerConfiguration["meshParams"] as Dictionary<string, string>;
            study.CreateDefaultMesh(
                Convert.ToInt32(meshConf["Quality"]),
                Convert.ToInt32(meshConf["UseJacobianCheck"]),
                Convert.ToInt32(meshConf["MesherType"]),
                Convert.ToInt32(meshConf["MinElementsInCircle"]),
                Convert.ToDouble(meshConf["GrowthRatio"]),
                Convert.ToInt32(meshConf["SaveSettingsWithoutMeshing"]),
                Convert.ToInt32(meshConf["Unit"])
                );
            study.RunStudy();
        }
    }
}
