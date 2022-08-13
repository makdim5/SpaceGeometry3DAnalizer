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
    }
}
