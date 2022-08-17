using App2.Simulation.Study;
using SolidWorksSimulationManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage.Simulation.Study
{
    public class ResearchController
    {
        public static StaticStudyResults RunResearch(StudyManager studyManager, StaticStudyRecord studyRecord = null)
        {
            StaticStudyResults result = null;

            //try
            //{
                StaticStudy study = studyManager.CreateStudy(studyRecord);
                int error = study.RunStudy();

            //var ty = study.GetResult();
                //result = error == 0
                //    ? study.GetResult()
                //    : throw new Exception(
                //        String.Format("Создание исследования провалилось!",
                //        error,
                //        studyRecord)
                //        );
                //}
                //catch (Exception e)
                //{
                //    throw e;
                //}
                //finally
                //{
                //    studyManager.ClearAllStudy();
                //}

            return result;
        }

        public static StaticStudyResults RunResearch(StudyManager studyManager, StaticStudy study)
        {
            StaticStudyResults result = null;

            try
            {
                int error = study.RunStudy();

                result = error == 0
                    ? study.GetResult()
                    : throw new Exception(
                        String.Format("Создание исследования провалилось!",
                        error)
                        );
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }




    }
}
