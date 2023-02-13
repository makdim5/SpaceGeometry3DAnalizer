using SolidServer.util.mathutils;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.SolidWorksPackage.Simulation.FeatureFace
{
    public class FacePlane : SimplePlane
    {
        public Face face;

        public FacePlane() { }

        public FacePlane(Face face)
        {
            object[] e = face.GetEdges() as object[];
            List<Edge> edges = new();

            foreach (Edge edge in e)
            {
                edges.Add(edge);
            }

            int i = 0;

            foreach (Edge edge in edges)
            {
                if (edge.GetStartVertex() != null && edge.GetEndVertex() != null)
                {
                    break;
                }
                i++;
            }

            int j = 0;

            foreach (Edge edge in edges)
            {
                if (edge.GetEndVertex() != null && j != i)
                {
                    break;
                }
                j++;
            }

            if (i < edges.Count && j < edges.Count)
            {
                Point3D point1 = new(edges[i].GetStartVertex().GetPoint()[0], edges[i].GetStartVertex().GetPoint()[1], edges[i].GetStartVertex().GetPoint()[2]);
                Point3D point2 = new(edges[i].GetEndVertex().GetPoint()[0], edges[i].GetEndVertex().GetPoint()[1], edges[i].GetEndVertex().GetPoint()[2]);
                Point3D point3 = new(edges[j].GetEndVertex().GetPoint()[0], edges[j].GetEndVertex().GetPoint()[1], edges[j].GetEndVertex().GetPoint()[2]);


                DefinePlaneCoffs(point1, point2, point3);
            }
            {
                Console.WriteLine("Косяк!");
            }

           

        }


    }
}
