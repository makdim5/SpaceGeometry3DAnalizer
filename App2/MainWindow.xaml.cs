using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using App2.util;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App2
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {

        

        public MainWindow()
        {
            this.InitializeComponent();
        }

        ModelDoc2 swModel;
        SketchManager sm;
        FeatureManager fm;
        SldWorks app;
       

        private void myButton_Click(object sender, RoutedEventArgs e)
        {


            List<object> comList = RotManager.GetRunningInstances("SldWorks.Application");

            if (comList.Count == 0)
            {
                Message.Show("Подключение к SolidWorks не выполнено! ", this.Content.XamlRoot);
                return;
            }

            app = (SldWorks)comList[0];
            textBlock.Text = ("Подключение к SolidWorks выполнено!");

            swModel = (ModelDoc2)app.GetFirstDocument();
            if (swModel == null)
            {
                textBlock.Text += ("Документ не найден.");
                return;

            }

            int pref_toggle = (int)swUserPreferenceToggle_e.swInputDimValOnCreate;

            app.SetUserPreferenceToggle(pref_toggle, false);

            sm = swModel.SketchManager;
            fm = swModel.FeatureManager;

            textBlock.Text += ("Документ найден.");
            Message.Show(textBlock.Text, this.Content.XamlRoot);




        }

        private void myButton2_Click(object sender, RoutedEventArgs e)
        {
            if (swModel is null)
            {
                return;
            }

            double x0 = 0.05, y0 = 0.05, z0 = 0.05;

            // рисуем параллелепипед

            // выбор передней плоскости
            swModel.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            swModel.SketchManager.InsertSketch(true);

            // рисование на плоскости
            swModel.SketchManager.CreateCornerRectangle(x0, y0, z0, x0 + 0.135, y0 + 0.09, z0);

            // вытягивание
            swModel.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.082, 0.082, false, false, false, false, z0, z0, false, false, false, false, true, true, true, 0, 0, false);

            // выбор передней плоскости
            swModel.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            swModel.SketchManager.InsertSketch(true);



            swModel.SketchManager.CreateArc(x0 + 0.135 / 2 - 0.076 / 2 + 0.038, y0 + 0.082 - 0.05, z0, x0 + (0.135 - 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.082 - 0.05, z0, -1);
            swModel.SketchManager.CreateLine(x0 + (0.135 - 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 - 0.076) / 2, y0 + 0.025, z0);
            swModel.SketchManager.CreateLine(x0 + (0.135 + 0.076) / 2, y0 + 0.082 - 0.05, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.025, z0);
            swModel.SketchManager.CreateLine(x0 + (0.135 - 0.076) / 2, y0 + 0.025, z0, x0 + (0.135 + 0.076) / 2, y0 + 0.025, z0);

            swModel.SketchManager.CreateCornerRectangle(x0, y0 + 0.018, z0, x0 + (0.135 - 0.104) / 2, y0 + 0.09, z0);
            swModel.SketchManager.CreateCornerRectangle(x0 + (0.135 - 0.104) / 2 + 0.104, y0 + 0.018, z0, x0 + 0.135, y0 + 0.09, z0);
            swModel.FeatureManager.FeatureCut3(false, false, false, 1, 0, 0.09, 0.09, false, false, false, false, 1.74532925199433E-02, 1.74532925199433E-02,
                false, false, false, false, false, true, true, true, true, false, 0, 0, false);

            swModel.ClearSelection2(true);


        }


    


}
}
