﻿using System;
using SolidServer.SolidWorksApplicationPackage;
using SolidWorks.Interop.cosworks;

namespace SolidServer.SolidWorksPackage.ResearchPackage
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
            if (study == null)
                throw new Exception("Исследование не удалось получить!");
           
            if (study.Results == null)
                throw new Exception("Результаты исследования не удалось получить!");

            return new StaticStudy(study);
            
            
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

        public StaticStudyRecord CreateSimpleRecord(string materialName)
        {
            // Задание сетки и материала
            Material material = MaterialManager.GetMaterials()[materialName];
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
