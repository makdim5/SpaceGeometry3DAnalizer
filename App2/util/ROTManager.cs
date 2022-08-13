using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace App2.util
{
    public static class RotManager
    {
        // Requires Using System.Runtime.InteropServices.ComTypes
        // Get all running instance by querying ROT for matching progId.
        public static List<object> GetRunningInstances(string progId)
        {
            return GetRunningInstances(new string[] { progId }, out List<string> _);
        }

        // Requires Using System.Runtime.InteropServices.ComTypes
        // Get all running instance by querying ROT for matching progIds.
        public static List<object> GetRunningInstances(string[] progIds)
        {
            return GetRunningInstances(progIds, out List<string> _);
        }

        // Requires Using System.Runtime.InteropServices.ComTypes
        // Get all running instance and their displayNames by the querying ROT for matching progIds.
        public static List<object> GetRunningInstances(string[] progIds, out List<string> displayNames)
        {
            List<string> clsIds = new List<string>();

            // get the app clsid
            foreach (string progId in progIds)
            {
                Type type = Type.GetTypeFromProgID(progId);
                if (type != null)
                    clsIds.Add(type.GUID.ToString().ToUpper());
            }

            // get Running Object Table ...
            GetRunningObjectTable(0, out IRunningObjectTable Rot);
            if (Rot == null)
            {
                displayNames = null;
                return null;
            }

            // get enumerator for ROT entries
            Rot.EnumRunning(out IEnumMoniker monikerEnumerator);

            if (monikerEnumerator == null)
            {
                displayNames = null;
                return null;
            }

            // go through all entries and identifies unique app instances
            List<object> instances = new List<object>();
            List<string> names = new List<string>();

            monikerEnumerator.Reset();
            IntPtr pNumFetched = new IntPtr();
            IMoniker[] monikers = new IMoniker[1];

            while (monikerEnumerator.Next(1, monikers, pNumFetched) == 0)
            {
                CreateBindCtx(0, out IBindCtx bindCtx);
                if (bindCtx == null)
                    continue;

                string displayName;
                monikers[0].GetDisplayName(bindCtx, null, out displayName);

                foreach (string clsId in clsIds)
                {
                    if (displayName.ToUpper().IndexOf(clsId) > 0)
                    {
                        Rot.GetObject(monikers[0], out object ComObject);
                        if (ComObject == null)
                            continue;
                        instances.Add(ComObject);
                        names.Add(displayName);
                        break;
                    }
                }
            }
            displayNames = names;
            return instances;
        }

        #region OLE Methods

        /// Returns a pointer to an implementation of IBindCtx (a bind context object).
        /// This object stores information about a particular moniker-binding operation.

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        /// Returns a pointer to the IRunningObjectTable
        /// interface on the local running object table (ROT).
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved,
           out IRunningObjectTable pprot);

        #endregion OLE Methods
    }
}
