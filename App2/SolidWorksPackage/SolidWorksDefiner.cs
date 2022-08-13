using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.exceptions;
using App2.util;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace App2
{
    internal class SolidWorksDefiner
    {
        public static SldWorks app;
        public static List<object> comList;
        private const string APP_NAME = "SldWorks.Application";

        public static void DefineSolidWorksApp()
        {
            comList = RotManager.GetRunningInstances(APP_NAME);

            if (comList.Count == 0)
            {
                throw new NotSWAppFoundException();
            }

            app = (SldWorks)comList[0];

        }

        public static void OpenSolidWorksApp()
        {
            comList = RotManager.GetRunningInstances(APP_NAME);
            if (comList.Count == 0)
            {
                app = Activator.CreateInstance(Type.GetTypeFromProgID(APP_NAME)) as SldWorks;
                app.Visible = true;
                
            }
 
        }

        public static void CloseSolidWorksApp()
        {
            
            if (app != null)
            {
                app.ExitApp();

            }

        }


        public static ModelDoc2 DefineActiveSolidWorksDocument()
        {
            DefineSolidWorksApp();
            ModelDoc2 swDoc = (ModelDoc2)app.GetFirstDocument();
            if (swDoc == null)
            {
                throw new NotSWDocumentFoundException();

            }

            int pref_toggle = (int)swUserPreferenceToggle_e.swInputDimValOnCreate;

            app.SetUserPreferenceToggle(pref_toggle, false);

            return swDoc;
        }

    }       
}

