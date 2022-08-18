using System;
using App2.Simulation.Study;

using SolidWorks.Interop.cosworks;

namespace App2.SolidWorksPackage.Simulation.Study
{
    public class StudyManager
    {
        private dynamic COSMOSWORKS; // api name of Simulation AddIn
        private ICWStudyManager studyMgr;

        public StudyManager()
        {

            COSMOSWORKS = SolidWorksAppWorker.GetSimulation();

        }

        public StaticStudy CreateStudy(StaticStudyRecord record)
        {
            studyMgr = COSMOSWORKS.ActiveDoc.StudyManager;

            int error;

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

        public StaticStudy GetExistingCompletedStudy()
        {
            studyMgr = COSMOSWORKS.ActiveDoc.StudyManager;
            CWStudy study = studyMgr.GetStudy(0);
            if (study != null)
            {
                return study.Results != null ? new StaticStudy(study) : null;
            }
            return null;
        }

        public void ClearAllStudy()
        {
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
