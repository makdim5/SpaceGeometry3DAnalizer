using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidServer.util.mathutils;

namespace SolidServer.SolidWorksPackage.Simulation.FeatureFace
{
    public class FeatureFace
    {

        public string name;
        public double force;
        public readonly Face face;

        public FaceType type;
        private Entity entity;

        public Color color;

        public Vector3D GetNormal()
        {

            double[] normal = (double[]) face.Normal;

            return new Vector3D(normal[0], normal[1], normal[2]);
        }

        public Point3D GetCenter()
        {
            double[] UVBounds = (double[])face.GetUVBounds();

            double centerU = (UVBounds[0] + UVBounds[1]) / 2;
            double centerV = (UVBounds[2] + UVBounds[3]) / 2;

            Surface surface = face.GetSurface() as Surface;

            double[] param = (double[]) surface.Evaluate(centerU, centerV, 0, 0);


            return new Point3D(param[0] * 1000, param[1] * 1000, param[2] * 1000);
        }

        public FeatureFace()
        {
            this.entity = face as Entity;
            type = FaceType.NoneType;
            
        }
        public FeatureFace(Face face, string name) :base()
        {
            
            this.name = name;
            this.face = face;
            
            this.color = GetColor();

        }

        public FeatureFace(Face face, string name, Color color) : this(face, name)
        {
            
            this.color = color;

            SetColor(color);


        }

        public void SetColor(Color color)
        {

            double[] param = face.MaterialPropertyValues as double[];

            if (param == null)
            {
                param = new double[9] {
                    0, 0, 0,
                    1, 1, 0.5,
                    0.4, 0, 0
                };
            }

            param[0] = color.R / 255f;
            param[1] = color.G / 255f;
            param[2] = color.B / 255f;

            face.MaterialPropertyValues = param;

        }

        public Color GetColor()
        {

            double[] param = face.MaterialPropertyValues as double[];

            if (param == null)
            {
                param = new double[3];
            }

            Color color = Color.FromArgb(
                Convert.ToInt32(param[0] * 255),
                Convert.ToInt32(param[1] * 255),
                Convert.ToInt32(param[2] * 255)
                );

            return color;
        }

        public Point3D[] GetVertixs()
        {

            object[] edges = face.GetEdges() as object[];

            HashSet<Point3D> vertices = new HashSet<Point3D>();

            if (edges != null)
            {
                foreach (Edge edge in edges)
                {

                    double[] start = edge.IGetStartVertex().GetPoint() as double[];
                    double[] end = edge.IGetEndVertex().GetPoint() as double[];

                    vertices.Add(new Point3D(start[0] * 1000, start[1] * 1000, start[2] * 1000));
                    vertices.Add(new Point3D(end[0] * 1000, end[1] * 1000, end[2] * 1000));

                }

            }

            return vertices.ToArray();

        }

        public void Select(bool append = false)
        {
            entity.Select(append);
        }

        public void DeSelect()
        {
            entity.DeSelect();
        }

        public override string ToString()
        {
            return name;
        }
    }
}