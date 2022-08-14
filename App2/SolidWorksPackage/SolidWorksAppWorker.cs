using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App2.exceptions;
using App2.util;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace App2
{
    internal class SolidWorksAppWorker
    {
        public static SldWorks app;
        public static List<object> comList;
        public const string APP_NAME = "SldWorks.Application";

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
            foreach (Process proc in Process.GetProcessesByName("sldProcMon"))
            {
                proc.Kill();
            }

            foreach (Process proc in Process.GetProcessesByName("SLDWORKS"))
            {
                proc.Kill();
            }


        }


        public static ModelDoc2 DefineActiveSolidWorksDocument()
        {
            if (app == null)
            {
                throw new NotSWAppFoundException();
            }
            ModelDoc2 swDoc = (ModelDoc2)app.GetFirstDocument();
            if (swDoc == null)
            {
                throw new NotSWDocumentFoundException();

            }

            int pref_toggle = (int)swUserPreferenceToggle_e.swInputDimValOnCreate;

            app.SetUserPreferenceToggle(pref_toggle, false);

            return swDoc;
        }



        public static void OpenDocument(string path)
        {


            int fileError = default(int);
            int fileWarning = default(int);

            //Open doc
            ModelDoc2 swDoc = app.OpenDoc6(
                path,
                (int)swDocumentTypes_e.swDocPART,
                (int)swOpenDocOptions_e.swOpenDocOptions_Silent,
                null,
                ref fileError,
                ref fileWarning
            );


            if (swDoc != null)
            {
                //Set the working directory to the document directory
                string pathName = swDoc.GetPathName();
                app.SetCurrentWorkingDirectory(pathName.Substring(0, pathName.LastIndexOf("\\")));
            }


        }

        public static void CreateNewDocument()
        {
            if (app != null)
                app.NewPart();

        }

    }
}

