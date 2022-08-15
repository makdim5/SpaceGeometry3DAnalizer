using App2.SolidWorksPackage.Cells;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage
{
    internal class SolidWorksDrawerModels
    {
        public static void DrawSimpleTestModel(ModelDoc2 doc)
        {
            if (doc is null)
            {
                return;
            }

            double x0 = 0.05, y0 = 0.05, z0 = 0.05;

            // рисуем параллелепипед

            // выбор передней плоскости
            doc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            doc.SketchManager.InsertSketch(true);

            // рисование на плоскости
            doc.SketchManager.CreateCornerRectangle(x0, y0, z0, x0 + 0.135, y0 + 0.09, z0);

            // вытягивание
            doc.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.082, 0.082, false, false, false, false, z0, z0, false, false, false, false, true, true, true, 0, 0, false);

            // выбор передней плоскости
            doc.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            doc.SketchManager.InsertSketch(true);



            doc.SketchManager.CreateArc(x0 + 0.135 / 2 - 0.076 / 2 + 0.038, y0 + 0.082 - 0.05, z0, x0 + (0.135 - 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.082 - 0.05, z0, -1);
            doc.SketchManager.CreateLine(x0 + (0.135 - 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 - 0.076) / 2, y0 + 0.025, z0);
            doc.SketchManager.CreateLine(x0 + (0.135 + 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.025, z0);
            doc.SketchManager.CreateLine(x0 + (0.135 - 0.076) / 2, y0 + 0.025, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.025, z0);

            doc.SketchManager.CreateCornerRectangle(x0, y0 + 0.018, z0, x0 + (0.135 - 0.104) / 2, y0 + 0.09, z0);
            doc.SketchManager.CreateCornerRectangle(x0 + (0.135 - 0.104) / 2 + 0.104, y0 + 0.018, z0, x0 + 0.135, y0 + 0.09, z0);
            doc.FeatureManager.FeatureCut3(false, false, false, 1, 0, 0.09, 0.09, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02,
                false, false, false, false, false, true, true, true, true, false, 0, 0, false);

            doc.ClearSelection2(true);


        }

        public static void DrawPyramid(ModelDoc2 swDoc, PyramidFourVertexArea area)
        {
            double unit = 1000;
            swDoc.ClearSelection();
            swDoc.SketchManager.Insert3DSketch(false);
            var sketchPoint = swDoc.SketchManager.CreatePoint(area.vertex1.x / unit, area.vertex1.x / unit, area.vertex1.z / unit);
            swDoc.SketchManager.Insert3DSketch(true);


            swDoc.ClearSelection();
            swDoc.SketchManager.Insert3DSketch(false);

            var sketchSegments = new SketchSegment[] {

                    swDoc.SketchManager.CreateLine(
                    area.vertex2.x / unit, area.vertex2.y / unit, area.vertex2.z / unit,
                    area.vertex3.x / unit, area.vertex3.y / unit, area.vertex3.z / unit),

                    swDoc.SketchManager.CreateLine(
                    area.vertex3.x / unit, area.vertex3.y / unit, area.vertex3.z / unit,
                    area.vertex4.x / unit, area.vertex4.y / unit, area.vertex4.z / unit),

                    swDoc.SketchManager.CreateLine(
                    area.vertex4.x / unit, area.vertex4.y / unit, area.vertex4.z / unit,
                    area.vertex2.x / unit, area.vertex2.y / unit, area.vertex2.z / unit)
                };

            bool canDraw = sketchSegments[0] != null && sketchSegments[1] != null && sketchSegments[2] != null;

            if (!canDraw)
                return;

            sketchSegments[0].GetSketch().MergePoints(0.000001);

            swDoc.SketchManager.Insert3DSketch(true);

            if (sketchSegments != null && sketchPoint != null)
            {

                swDoc.ClearSelection();
                sketchPoint.Select2(true, 1);
                ((Feature)sketchSegments[0].GetSketch()).Select2(true, 1);
                swDoc.FeatureManager.InsertCutBlend(false, true, false, 1, 0, 0, false, 0, 0, 0, true, true);

            }
        }
    }
}
