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

        public static void DefineSolidWorksApp()
        {
            comList = RotManager.GetRunningInstances("SldWorks.Application");

            if (comList.Count == 0)
            {
                throw new NotSWAppFoundException();
            }

            app = (SldWorks)comList[0];

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

