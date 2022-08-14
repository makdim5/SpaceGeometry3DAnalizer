using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Drawing;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace App2.SolidWorksPackage.Simulation.FeatureFace
{
    public class FeatureFaceManager
    {

        public Dictionary<string, FeatureFace> nameFaces;

        public HashSet<FeatureFace> freeFaces;

        public HashSet<FeatureFace> busyFaces;

        private static Random rand = new Random(unchecked((int)(DateTime.Now.Ticks)));

        public FeatureFaceManager()
        {

            freeFaces = new HashSet<FeatureFace>();

            busyFaces = new HashSet<FeatureFace>();

            nameFaces = new Dictionary<string, FeatureFace>();

        }

        public FeatureFaceManager(ModelDoc2 swDoc)
        {

            freeFaces = GetFeatureFaces(GetFaces(swDoc));

            busyFaces = new HashSet<FeatureFace>();

            nameFaces = GetDictNameFaces(freeFaces);

            swDoc.GraphicsRedraw2();

        }

        public FeatureFaceManager(IEnumerable<Face> faces)
        {

            freeFaces = GetFeatureFaces(faces);

            busyFaces = new HashSet<FeatureFace>();

            nameFaces = GetDictNameFaces(freeFaces);

        }

        public FeatureFaceManager(IEnumerable<FeatureFace> faces)
        {

            freeFaces = faces.ToHashSet();

            busyFaces = new HashSet<FeatureFace>();

            nameFaces = GetDictNameFaces(freeFaces);

        }

        public void AddFaces(IEnumerable<FeatureFace> faces)
        {

            freeFaces = freeFaces.Concat(faces).ToHashSet();

            nameFaces = GetDictNameFaces(freeFaces);
        }

        public void SetFaces(IEnumerable<FeatureFace> faces)
        {

            freeFaces = faces.ToHashSet();

            busyFaces = new HashSet<FeatureFace>();

            nameFaces = GetDictNameFaces(freeFaces);
        }

        public void ClearFreeFaces()
        {

            freeFaces.Clear();
            nameFaces = GetDictNameFaces(busyFaces);
        }

        public void RemoveFreeFaces(IEnumerable<FeatureFace> faces)
        {

            if (faces == null) { return; }

            HashSet<FeatureFace> intersect = this.freeFaces.Intersect(faces).ToHashSet();

            this.freeFaces.ExceptWith(intersect);

            foreach (FeatureFace face in intersect)
            {

                nameFaces.Remove(face.name);

            }


        }

        private static string GetName(int index)
        {

            return "Грань " + index;

        }

        private static Color GetColor()
        {

            int r = rand.Next(100, 255);
            int g = rand.Next(0, 255);
            int b = rand.Next(55, 100);

            return Color.FromArgb(r, g, b);

        }

        private static HashSet<Face> GetFaces(ModelDoc2 swDoc)
        {

            HashSet<object> result = new HashSet<object>();

            Feature[] features = (Feature[])swDoc.FeatureManager.GetFeatures(true);

            foreach (Feature feature in features)
            {

                Face[] faces = (Face[]) feature.GetFaces();

                if (faces != null)
                {
                    foreach (object face in faces)
                    {
                        result.Add(face);
                    }

                }
            }

            return new HashSet<Face>(result.Cast<Face>());
        }

        private static HashSet<FeatureFace> GetFeatureFaces(IEnumerable<Face> faces)
        {

            HashSet<FeatureFace> result = new HashSet<FeatureFace>();

            int index = 1;

            foreach (Face face in faces)
            {
                string faceName = GetName(index);

                Color faceColor = GetColor();

                FeatureFace featureFace = new FeatureFace(face, faceName, faceColor);

                result.Add(featureFace);

                index++;

            }

            return result;
        }

        private static Dictionary<string, FeatureFace> GetDictNameFaces(IEnumerable<FeatureFace> faces)
        {

            Dictionary<string, FeatureFace> result = new Dictionary<string, FeatureFace>();

            foreach (FeatureFace face in faces)
            {

                result.Add(face.name, face);

            }

            return result;

        }

        public void ReleaseFeatureFaces(IEnumerable<FeatureFace> busyFaces)
        {

            if (busyFaces == null) { return; }

            HashSet<FeatureFace> intersect = this.busyFaces.Intersect(busyFaces).ToHashSet();

            this.busyFaces.ExceptWith(intersect);

            this.freeFaces = this.freeFaces.Concat(intersect).ToHashSet();

        }

        public void LoadFeatureFaces(IEnumerable<FeatureFace> freeFaces)
        {
            if (freeFaces == null) { return; }

            HashSet<FeatureFace> intersect = this.freeFaces.Intersect(freeFaces).ToHashSet();

            this.freeFaces.ExceptWith(intersect);

            this.busyFaces = this.busyFaces.Concat(intersect).ToHashSet();
        }
    }
}