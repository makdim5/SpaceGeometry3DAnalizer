using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using App2.exceptions;
using App2.SolidWorksPackage;
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
            
            try
            {
                app = Marshal.GetActiveObject("SldWorks.Application") as SldWorks;
            } catch (COMException)
            {
                throw new NotSWAppFoundException();
            }

        }

        public static void OpenSolidWorksApp()
        {
            comList = RotManager.GetRunningInstances(APP_NAME);
            if (comList.Count == 0)
            {
                app = Activator.CreateInstance(Type.GetTypeFromProgID(APP_NAME)) as SldWorks;
                app.Visible = true;

                app.LoadAddIn(app.GetExecutablePath() + @"\Simulation\cosworks.dll");

            
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
            ModelDoc2 swDoc = (ModelDoc2)app.ActiveDoc;
            if (swDoc == null)
            {
                throw new NotSWDocumentFoundException();

            }

            int pref_toggle = (int)swUserPreferenceToggle_e.swInputDimValOnCreate;

            app.SetUserPreferenceToggle(pref_toggle, false);

            return swDoc;
        }



        public static ModelDoc2 OpenDocument(string path)
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

            return swDoc;
        }

        public static ModelDoc2 CreateNewDocument()
        {
            return app.NewPart();
        }


        //Получить путь к файлам с материалами
        public static string[] GetPathsMaterialDataBase()
        {
            if (app == null)
            {
                DefineSolidWorksApp();
            }
            object[] result = app.GetMaterialDatabases() as object[];

            return result.Cast<string>().ToArray();
        }

        public static dynamic GetSimulation()
        {
            if (app == null)
            {
                DefineSolidWorksApp();
            }
            
            int swVersion = Convert.ToInt32(app.RevisionNumber().Substring(0, 2));

            
            dynamic COSMOSObject = app.GetAddInObject($"SldWorks.Simulation.{swVersion - 15}");

            return COSMOSObject == null ? null : COSMOSObject.CosmosWorks;
        }

    }
}

