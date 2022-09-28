using App2.SolidWorksPackage.Cells;
using App2.util.mathutils;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App2.SolidWorksPackage
{
    internal class SolidWorksDrawer
    {
        public const double RADIUS = 0.01;

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

        public static Feature DrawPyramid(ModelDoc2 doc, PyramidFourVertexArea area)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(false);
            var sketchPoint = doc.SketchManager.CreatePoint(area.vertex1.x / unit, area.vertex1.y / unit, area.vertex1.z / unit);
            doc.SketchManager.Insert3DSketch(true);


            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(false);

            var sketchSegments = new SketchSegment[] {

                    doc.SketchManager.CreateLine(
                    area.vertex2.x / unit, area.vertex2.y / unit, area.vertex2.z / unit,
                    area.vertex3.x / unit, area.vertex3.y / unit, area.vertex3.z / unit),

                    doc.SketchManager.CreateLine(
                    area.vertex3.x / unit, area.vertex3.y / unit, area.vertex3.z / unit,
                    area.vertex4.x / unit, area.vertex4.y / unit, area.vertex4.z / unit),

                    doc.SketchManager.CreateLine(
                    area.vertex4.x / unit, area.vertex4.y / unit, area.vertex4.z / unit,
                    area.vertex2.x / unit, area.vertex2.y / unit, area.vertex2.z / unit)
                };

            bool canDraw = sketchSegments[0] != null && sketchSegments[1] != null && sketchSegments[2] != null;

            if (!canDraw)
                throw new Exception("Не возможно нарисовать пирамиду!");

            sketchSegments[0].GetSketch().MergePoints(0.000001);

            doc.SketchManager.Insert3DSketch(true);

            Feature cut = null;

            if (sketchSegments != null && sketchPoint != null)
            {

                doc.ClearSelection();
                sketchPoint.Select2(true, 1);
                ((Feature)sketchSegments[0].GetSketch()).Select2(true, 1);
                cut = doc.FeatureManager.InsertCutBlend(false, true, false, 1, 0, 0, false, 0, 0, 0, true, true);

            }
            doc.ClearSelection();

            return cut;
        }


        public static void DrawSphere(ModelDoc2 doc, Point3D centr, double radius)
        {
            Point3D center = new Point3D ( centr.x / 1000, centr.y / 1000, centr.z / 1000 );
            double radious = radius / 1000;

            Point3D startArcPoint = new(center.x, (center.y + radious), 0);
            Point3D endArcPoint = new Point3D(center.x, (center.y - radious), 0);

            doc.Extension.SelectByID2("Ñïåðåäè", "PLANE", center.x, center.y, center.z, false, 0, null, 0);

            doc.SketchManager.InsertSketch(true);

            doc.SketchManager.CreateArc(center.x, center.y, center.z,
                startArcPoint.x, startArcPoint.y, startArcPoint.z,
                endArcPoint.x, endArcPoint.y, endArcPoint.z, -1);

            doc.SketchManager.CreateLine(endArcPoint.x, endArcPoint.y, endArcPoint.z,
                startArcPoint.x, startArcPoint.y, startArcPoint.z);

            doc.Extension.SelectByID2("Line1", "SKETCHSEGMENT", endArcPoint.x,
                endArcPoint.y, endArcPoint.z, true, 0, null, 0);
            doc.ShowNamedView2("*Òðèìåòðèÿ", 8);
            doc.ViewZoomtofit2();
            doc.ClearSelection2(true);
            doc.Extension.SelectByID2("Line1", "SKETCHSEGMENT", endArcPoint.x, endArcPoint.y
                , endArcPoint.z, false, 16, null, 0);
            doc.FeatureManager.FeatureRevolve2(true, true, false, false, false, false, 0, 0, 6.2831853071796, 0, false, false, radious, radious, 0, 0, 0, true, true, true);
            doc.SelectionManager.EnableContourSelection = false;
        }


        public static void UndoFeaturesCuts (ModelDoc2 doc, List<Feature> features)
        {
            foreach(var item in features)
            {
                doc.Extension.SelectByID2(item.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                doc.EditDelete();
            }
            
        }

    }
}
