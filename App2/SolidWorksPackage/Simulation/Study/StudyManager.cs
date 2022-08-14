using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolidWorks.Interop.cosworks;

namespace App2.Simulation.Study
{
    public class StudyManager
    {
        private dynamic COSMOSWORKS;

        private CWModelDoc actDoc;
        private ICWStudyManager studyMgr;

        public StudyManager(dynamic COSMOSWORKS) {

            this.COSMOSWORKS = COSMOSWORKS;

            actDoc = COSMOSWORKS.ActiveDoc;
            studyMgr = actDoc.StudyManager;
        }

        public StaticStudy CreateStudy(StaticStudyRecord record) 
        {
            return new StaticStudy(CreateStaticStudy(record.text), record);
        }

        public bool TryGetStudyFromSolidworks(out StaticStudy study)
        {
            study = null;

            CWStudy s = studyMgr.GetStudy(studyMgr.ActiveStudy);

            if (s == null)
                return false;

            study = new StaticStudy(s);

            return study != null;
        }

        public CWStudy CreateStaticStudy(string name) {

            actDoc = COSMOSWORKS.ActiveDoc;
            studyMgr = actDoc.StudyManager;

            ClearAllStudy();

            int error = 0;

            CWStudy study = studyMgr.CreateNewStudy3(
                name,
                (int)swsAnalysisStudyType_e.swsAnalysisStudyTypeStatic,
                (int)swsMeshType_e.swsMeshTypeMixed, 
                out error);

            if (error != 0) {
                throw new Exception("Создание нового исследования провалилось!\nКод ошибки :"+ error +"\n");
            }

            return study;

        }

        public void ClearAllStudy() {
            ClearAllStudy(COSMOSWORKS);
        }
        
        private static void ClearAllStudy(dynamic COSMOSWORKS) {
            CWModelDoc actDoc = COSMOSWORKS.ActiveDoc;

            ICWStudyManager studyMgr = actDoc.StudyManager;

            if (studyMgr.StudyCount > 0)
            {

                for (int i = 0; i < studyMgr.StudyCount; i++)
                {
                    studyMgr.DeleteStudy(studyMgr.GetStudy(i).Name);
                }
            }
        }
    }
}
