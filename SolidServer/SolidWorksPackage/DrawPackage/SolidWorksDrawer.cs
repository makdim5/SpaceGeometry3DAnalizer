using SolidServer.SolidWorksPackage.Cells;
using SolidServer.SolidWorksPackage.ResearchPackage;
using SolidServer.Utitlites;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;

namespace SolidServer.SolidWorksPackage
{
    internal class SolidWorksDrawer
    {
        public static void ColorFaces(HashSet<Face> faces, int r, int g, int b)
        {
            foreach (var face in faces)
            {
                double[] param = face.MaterialPropertyValues;

                if (param == null)
                {
                    param = new double[9] {
                        0, 0, 0,
                        1, 1, 0.5,
                        0.4, 0, 0
                    };
                }

                param[0] = r / 255f;
                param[1] = g / 255f;
                param[2] = b / 255f;

                face.MaterialPropertyValues = param;
            }
        }
        public static void DrawNodes(ModelDoc2 doc, IEnumerable<Node> nodes)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);

            foreach (var node in nodes)
            {

                doc.SketchManager.CreatePoint(node.point.x / unit, node.point.y / unit, node.point.z / unit);

            }

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();
        }
        public static Feature DrawPyramid(ModelDoc2 doc, PyramidFourVertexArea area, int mode = 1)
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
                if (mode == 1)
                {
                    // вырез
                    cut = doc.FeatureManager.InsertCutBlend(false, true, false, 1, 0, 0, false, 0, 0, 0, true, true);
                }
                else
                {
                    // выдавливание
                    doc.FeatureManager.SetNetBlendCurveData(0, 0, 0, 0, 1, true);
                    doc.FeatureManager.SetNetBlendCurveData(0, 1, 0, 0, 1, true);
                    doc.FeatureManager.SetNetBlendDirectionData(0, 32, 0, false, false);
                    doc.FeatureManager.SetNetBlendDirectionData(1, 32, 0, false, false);
                    cut = doc.FeatureManager.InsertNetBlend2(0, 2, 0, false, 0.0001, true, true, true, true, false,
                        -1, -1, false, -1, false, false, -1, false, -1, false, false);
                }
            }
            doc.ClearSelection();
            return cut;
        }

        public static Feature DoTruba(ModelDoc2 doc, double[] points, double rad = 0.005)
        {
            doc.ClearSelection();

            doc.SketchManager.Insert3DSketch(true);
            var spline = doc.SketchManager.CreateSpline(points);
            spline.Select2(true, 1);

            var swFeatData = doc.FeatureManager.CreateDefinition((int)swFeatureNameID_e.swFmSweep);
            swFeatData.AdvancedSmoothing = false;
            swFeatData.AlignWithEndFaces = 0;
            swFeatData.AutoSelect = true;
            swFeatData.CircularProfile = true;
            swFeatData.CircularProfileDiameter = rad * 2;
            swFeatData.D1ReverseTwistDir = false;
            swFeatData.EndTangencyType = 0;
            swFeatData.FeatureScope = true;
            swFeatData.MaintainTangency = false;
            swFeatData.Merge = true;
            swFeatData.MergeSmoothFaces = true;
            swFeatData.PathAlignmentType = 10;
            swFeatData.StartTangencyType = 0;
            swFeatData.ThinFeature = false;
            swFeatData.ThinWallType = 0;
            swFeatData.TwistControlType = 0;
            swFeatData.SetTwistAngle(0);
            swFeatData.SetWallThickness(true, 0);
            var cut = doc.FeatureManager.CreateFeature(swFeatData);

            doc.ClearSelection();
            return cut;
        }

        public static Feature CutEllipsoid(ModelDoc2 doc, double xc, double yc, double zc, double radA = 0.005, double radB = 0.001, double radC = 0.005)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);
            var sketchPoint1 = doc.SketchManager.CreatePoint(xc / unit, (yc + 20) / unit, zc / unit);

            var sketchPoint2 = doc.SketchManager.CreatePoint(xc / unit, (yc - 20) / unit, zc / unit);
            var sketchPoint3 = doc.SketchManager.CreatePoint((xc + 20) / unit, yc / unit, zc / unit);

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();

            sketchPoint1.Select2(true, 1);
            sketchPoint2.Select2(true, 1);
            sketchPoint3.Select2(true, 1);

            Feature swPlane = doc.CreatePlaneThru3Points3(true);
            swPlane.Select2(true, 1);
            doc.Extension.EditRebuildAll();
            doc.InsertSketch2(true);

            Point3D center = new Point3D(xc / unit, yc / unit, zc / unit);
            double radious = radB;

            Point3D startArcPoint = new Point3D(center.x, (center.y + radious), center.z);
            Point3D endArcPoint = new Point3D(center.x, (center.y - radious), center.z);

            doc.SketchManager.CreateEllipticalArc(center.x, center.y, center.z,
                center.x + radA, center.y, center.z,
                center.x, center.y + radB, center.z,
                center.x, center.y - radB, center.z,
                center.x, center.y + radB, center.z, -1);

            var line = doc.SketchManager.CreateLine(endArcPoint.x, endArcPoint.y, endArcPoint.z,
                startArcPoint.x, startArcPoint.y, startArcPoint.z);

            line.Select2(false, 16);
            var cut = doc.FeatureManager.FeatureRevolve2(true, true, false, true, false, false, 0, 0, 6.2831853071796, 0, false, false, radious, radious, 0, 0, 0, true, true, true);
            doc.SelectionManager.EnableContourSelection = false;
            doc.InsertSketch2(false);
            doc.ClearSelection();
            return cut;
        }

        public static Feature CutSphiere(ModelDoc2 doc, Sphere sphere)
        {
           return CutSphiere(doc, sphere.center.x, sphere.center.y, sphere.center.z, sphere.radious);
        }
        public static Feature CutSphiere(ModelDoc2 doc, double xc, double yc, double zc, double rad = 0.005)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);
            var sketchPoint1 = doc.SketchManager.CreatePoint(xc / unit, (yc + 20) / unit, zc / unit);

            var sketchPoint2 = doc.SketchManager.CreatePoint(xc / unit, (yc - 20) / unit, zc / unit);
            var sketchPoint3 = doc.SketchManager.CreatePoint((xc + 20) / unit, yc / unit, zc / unit);

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();

            sketchPoint1.Select2(true, 1);
            sketchPoint2.Select2(true, 1);
            sketchPoint3.Select2(true, 1);

            Feature swPlane = doc.CreatePlaneThru3Points3(true);
            swPlane.Select2(true, 1);
            doc.Extension.EditRebuildAll();
            doc.InsertSketch2(true);

            Point3D center = new Point3D(xc / unit, yc / unit, zc / unit);
            double radious = rad;

            Point3D startArcPoint = new Point3D(center.x, (center.y + radious), center.z);
            Point3D endArcPoint = new Point3D(center.x, (center.y - radious), center.z);

            doc.SketchManager.CreateArc(center.x, center.y, center.z,
                startArcPoint.x, startArcPoint.y, startArcPoint.z,
                endArcPoint.x, endArcPoint.y, endArcPoint.z, -1);

            var line = doc.SketchManager.CreateLine(endArcPoint.x, endArcPoint.y, endArcPoint.z,
                startArcPoint.x, startArcPoint.y, startArcPoint.z);

            line.Select2(false, 16);
            var cut = doc.FeatureManager.FeatureRevolve2(true, true, false, true, false, false, 0, 0, 6.2831853071796, 0, false, false, radious, radious, 0, 0, 0, true, true, true);
            doc.SelectionManager.EnableContourSelection = false;
            doc.InsertSketch2(false);
            doc.ClearSelection();
            return cut;
        }

        public static Feature CutCilindr(ModelDoc2 doc, double xc, double yc, double zc, double height = 0.01, double radious = 0.005)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);
            var sketchPoint1 = doc.SketchManager.CreatePoint(xc / unit, (yc + 20) / unit, zc / unit);

            var sketchPoint2 = doc.SketchManager.CreatePoint(xc / unit, (yc - 20) / unit, zc / unit);
            var sketchPoint3 = doc.SketchManager.CreatePoint((xc + 20) / unit, yc / unit, zc / unit);

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();

            sketchPoint1.Select2(true, 1);
            sketchPoint2.Select2(true, 1);
            sketchPoint3.Select2(true, 1);

            Feature swPlane = doc.CreatePlaneThru3Points3(true);
            swPlane.Select2(true, 1);
            doc.Extension.EditRebuildAll();
            doc.InsertSketch2(true);

            Point3D center = new Point3D(xc / unit, yc / unit, zc / unit);

            doc.SketchManager.CreateCircleByRadius(center.x, center.y, center.z, radious);


            var cut = doc.FeatureManager.FeatureCut4(true, false, false, 0, 0, height, height, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02, false, false, false,
                false, false, true, true, true, true, false, 0, 0, false, false);

            doc.InsertSketch2(false);
            doc.ClearSelection();
            return cut;

        }

        public static Feature CutCube(ModelDoc2 doc, double xc, double yc, double zc, double h1 = 0.002)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);
            var sketchPoint1 = doc.SketchManager.CreatePoint(xc / unit, (yc + 20) / unit, zc / unit);

            var sketchPoint2 = doc.SketchManager.CreatePoint(xc / unit, (yc - 20) / unit, zc / unit);
            var sketchPoint3 = doc.SketchManager.CreatePoint((xc + 20) / unit, yc / unit, zc / unit);

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();

            sketchPoint1.Select2(true, 1);
            sketchPoint2.Select2(true, 1);
            sketchPoint3.Select2(true, 1);

            Feature swPlane = doc.CreatePlaneThru3Points3(true);
            swPlane.Select2(true, 1);
            doc.Extension.EditRebuildAll();
            doc.InsertSketch2(true);

            doc.SketchManager.CreateCornerRectangle((xc - h1 / 2) / unit, (yc + h1 / 2) / unit, zc / unit,
                (xc + h1 / 2) / unit, (yc - h1 / 2) / unit, zc / unit);


            var cut = doc.FeatureManager.FeatureCut4(true, false, false, 0, 0, h1, h1, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02, false, false, false,
                false, false, true, true, true, true, false, 0, 0, false, false);

            doc.InsertSketch2(false);
            doc.ClearSelection();
            return cut;
        }

        public static Feature CutParallelepiped(ModelDoc2 doc, Parallelepiped parallelepiped)
        {
            return CutParallelepiped(doc, parallelepiped.minX, parallelepiped.maxX, parallelepiped.minY,
                parallelepiped.maxY, parallelepiped.minZ, parallelepiped.maxZ);
        }
        public static Feature CutParallelepiped(ModelDoc2 doc, double x1, double x2, double y1, double y2, double z1, double z2)
        {
            double unit = 1000;
            doc.ClearSelection();
            doc.SketchManager.Insert3DSketch(true);
            var sketchPoint1 = doc.SketchManager.CreatePoint(x1 / unit, y1 / unit, z1 / unit);

            var sketchPoint2 = doc.SketchManager.CreatePoint((x1 + 10) / unit, y2 / unit, z1 / unit);
            var sketchPoint3 = doc.SketchManager.CreatePoint(x1 / unit, (y1 + 10) / unit, z1 / unit);

            doc.SketchManager.Insert3DSketch(false);
            doc.ClearSelection();

            sketchPoint1.Select2(true, 1);
            sketchPoint2.Select2(true, 1);
            sketchPoint3.Select2(true, 1);

            Feature swPlane = doc.CreatePlaneThru3Points3(true);
            swPlane.Select2(true, 1);
            doc.Extension.EditRebuildAll();
            doc.InsertSketch2(true);

            doc.SketchManager.CreateCornerRectangle(x1 / unit, y1 / unit, z1 / unit,
                x2 / unit, y2 / unit, z1 / unit);

            var cut = doc.FeatureManager.FeatureCut4(true, false, false, 0, 0, (z2 - z1) / unit, (z2 - z1) / unit, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02, false, false, false,
                false, false, true, true, true, true, false, 0, 0, false, false);

            doc.InsertSketch2(false);
            doc.ClearSelection();

            return cut;
        }

        public static void UndoFeaturesCuts(ModelDoc2 doc, List<Feature> features)
        {
            foreach (var item in features)
            {
                doc.Extension.SelectByID2(item.Name, "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
                doc.EditDelete();
            }
        }
    }
}
