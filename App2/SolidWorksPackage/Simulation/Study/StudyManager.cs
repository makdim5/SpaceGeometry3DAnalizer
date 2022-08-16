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
        private ICWStudyManager studyMgr;

        public StudyManager() {

            this.COSMOSWORKS = SolidWorksAppWorker.GetSimulation();

        }

        public StaticStudy CreateStudy(StaticStudyRecord record) 
        {
            studyMgr = COSMOSWORKS.ActiveDoc.StudyManager;

            ClearAllStudy();

            int error = 0;

            CWStudy study = studyMgr.CreateNewStudy3(
                record.text,
                (int)swsAnalysisStudyType_e.swsAnalysisStudyTypeStatic,
                (int)swsMeshType_e.swsMeshTypeMixed,
                out error);

            if (error != 0)
            {
                throw new Exception("Создание нового исследования провалилось!\nКод ошибки :" + error + "\n");
            }
            return new StaticStudy(study, record);
        }

        
        public void ClearAllStudy() {
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
