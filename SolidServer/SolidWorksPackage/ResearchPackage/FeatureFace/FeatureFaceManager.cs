﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using SolidWorks.Interop.sldworks;
using System.Threading;

namespace SolidServer.SolidWorksPackage.ResearchPackage
{
    public class FeatureFaceManager
    {
        public HashSet<FeatureFace> freeFaces;

        private static Random rand = new Random(unchecked((int)(DateTime.Now.Ticks)));

        public FeatureFaceManager()
        {
            freeFaces = new HashSet<FeatureFace>();
        }

        public FeatureFaceManager(ModelDoc2 swDoc)
        {
            freeFaces = GetFeatureFaces(GetFaces(swDoc));
            swDoc.GraphicsRedraw2();
        }

        public FeatureFaceManager(IEnumerable<Face> faces)
        {
            freeFaces = GetFeatureFaces(faces);
        }

        public FeatureFaceManager(IEnumerable<FeatureFace> faces)
        {
            freeFaces = faces.ToHashSet();
        }

        public FeatureFace GetFeatureFacePerName(string name)
        {
            return freeFaces.Where(x => x.name.Equals(name)).FirstOrDefault();
        }
        public void DefineFace(string name, FaceType type, double loadValue = 0)
        {
            var face = GetFeatureFacePerName(name);
            face.type = type;
            face.force = loadValue;
        }
        public IEnumerable<FeatureFace> GetFacesPerType(FaceType type)
        {
            return from face in freeFaces
                   where face.type.Equals(type)
                   select face;
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

        public static HashSet<Face> GetFaces(ModelDoc2 swDoc)
        {
            HashSet<object> result = new HashSet<object>();
            object[] features = swDoc.FeatureManager.GetFeatures(true) as object[];

            foreach (Feature feature in features)
            {
                object[] faces = (object[])feature.GetFaces();

                if (faces != null)
                {
                    foreach (Face face in faces)
                    {
                        result.Add(face);
                    }
                }
            }
            return new HashSet<Face>(result.Cast<Face>());
        }
        public static List<FacePlane> DefineFacePlanes(ModelDoc2 activeDoc)
        {
            List<FacePlane> facePlanes = new();
            List<Thread> threads = new List<Thread>();
            List<HashSet<Node>> notCloseNodes = new();
            foreach (var face in GetFaces(activeDoc))
            {
                var thread = new Thread(() => {
                    var plane = new FacePlane(face);
                    if (plane.isPlane)
                        facePlanes.Add(plane);
                });
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            return facePlanes;
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
    }
}