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
        protected string param = "VON";
        protected double minvalue, maxvalue, criticalValue;
        protected List<ElementArea> areas;
        protected HashSet<Node> wholeNodes;
        public IEnumerable<object> cutAreas;
        public BaseResearchManager()
        {
            cutAreas = new List<object>();
            crashNodes = new List<Node>();
            facePlanes = new List<FacePlane>();
            areas = new List<ElementArea>();
            activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
            studyManager = new StudyManager();
            Console.WriteLine("Приложение SolidWorks и документ определены!\n");
        }

        public void RunInLoop()
        {
            try
            {
                GetCompletedStudyResults();
                DefineCriticalNodes();
                DetermineCutAreas();
                while (crashNodes.Count() == 0)
                {
                    CutAreas();
                    RunStudy();
                    GetCompletedStudyResults();
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
            minvalue = stressValues["min"];
            criticalValue = 0.2 * materialInfo.physicalProperties["SIGXT"];
            maxvalue = stressValues["max"] * 0.1;

            string msg = ($"\nМинимальное напряжение {param} =  {minvalue}" +
                $"\nмаксимальное напряжение по {param} {stressValues["max"]}" +
                $"\nпредел прочности при растяжении = " +
                $"{materialInfo.physicalProperties["SIGXT"]}" +
                $"\nкритическое > максимальное по {param} : {criticalValue > stressValues["max"]}" +
                $"\nкритическое значение:{criticalValue}\n"
                );
            if (studyResults.DefineMinMaxStressValues(param)["max"] >= criticalValue)
            {
                crashNodes = studyResults.DefineNodesPerStressParam(param, criticalValue, studyResults.DefineMinMaxStressValues(param)["max"]);
                msg += ($"Были найдены узлы с превышенной нагрузкой в количестве : {crashNodes.Count()}");

            }
            Console.WriteLine(msg);
            return new Dictionary<string, object>() { { "msg", msg } };
        }

        public Dictionary<string, object> DetermineCutAreas()
        {
            if (crashNodes.Count() > 0)
            {
                return new Dictionary<string, object>();
            }
            Console.WriteLine("Начало поиска областей");
            facePlanes = FeatureFaceManager.DefineFacePlanes(activeDoc);
            //var cutNodes = ElementAreaWorker.ExceptInsideNodes(
            // studyResults.DefineNodesPerStressParam(param, minvalue, maxvalue), areas);
            wholeNodes = new HashSet<Node>(studyResults.DefineNodesPerStressParam(param, minvalue, maxvalue));
            wholeNodes.ExceptWith(ElementAreaWorker.ExceptFaceClosestNodes(wholeNodes, facePlanes));
            string msg = $"Количество узлов для выявления областей" +
                $": {wholeNodes.Count()}\n";

            var result = DefineAreas();

            Console.WriteLine(msg + $"Окончание поиска областей. Их общее количество - {result["cutElementAreasCount"]}");
            return result;

        }
        public abstract Dictionary<string, object> DefineAreas();
        public abstract void CutArea(int index);
        public void CutAreas()
        {
            Console.WriteLine("Начало выреза областей ...");
            for (int i = 0; i < cutAreas.Count(); i++)
            {
                CutArea(i);
            }
            cutAreas = new List<ElementArea>();
            Console.WriteLine("Конец выреза областей");

        }
        public void RunStudy()
        {
            study.CreateDefaultMesh();
            study.RunStudy();
        }

    }
}
