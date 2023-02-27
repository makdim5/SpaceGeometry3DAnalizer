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

        public bool isPlane;

        public Point3D point1;
        public Point3D point2;
        public Point3D point3;

        public FacePlane() { }

        public FacePlane(Face face, ModelDoc2 doc)
        {
            object[] e = face.GetEdges() as object[];
            List<Edge> edges = new();

            foreach (Edge edge in e)
            {
                edges.Add(edge);
            }

            int i = 0, j = 2;
            if (edges[i].GetStartVertex() != null)
            {
                point1 = new(edges[i].GetStartVertex().GetPoint()[0]*1000, edges[i].GetStartVertex().GetPoint()[1] * 1000, edges[i].GetStartVertex().GetPoint()[2] * 1000);
                point2 = new(edges[i].GetEndVertex().GetPoint()[0] * 1000, edges[i].GetEndVertex().GetPoint()[1] * 1000, edges[i].GetEndVertex().GetPoint()[2] * 1000);
                point3 = new(edges[j].GetEndVertex().GetPoint()[0] * 1000, edges[j].GetEndVertex().GetPoint()[1] * 1000, edges[j].GetEndVertex().GetPoint()[2] * 1000);
                DefinePlaneCoffs(point1, point2, point3);

               
                isPlane = true;
            }
            else
            {
                isPlane= false;
                Console.WriteLine($"Данная грань не является плоскостью : {edges}");
            }

        }


        public void Draw(ModelDoc2 doc)
        {
          
            doc.SketchManager.CreatePoint(point1.x/1000, point1.y/1000, point1.z / 1000);
            doc.SketchManager.CreatePoint(point2.x/1000, point2.y/ 1000, point2.z / 1000);
            doc.SketchManager.CreatePoint(point3.x/1000, point3.y / 1000, point3.z / 1000 );
           
        }


    }
}
