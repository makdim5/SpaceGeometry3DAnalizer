using System;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using App2.util;
using App2.exceptions;
using App2.SolidWorksPackage;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Threading;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition.SystemBackdrops;
using WinRT;
using System.Runtime.InteropServices;
using App2.SolidWorksPackage.Simulation.FeatureFace;
using App2.Simulation.Study;
using App2.SolidWorksPackage.Simulation.MaterialWorker;
using App2.SolidWorksPackage.Simulation.Study;
using App2.SolidWorksPackage.Cells;

namespace App2
{

    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

        object m_dispatcherQueueController = null;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
            {
                // one already exists, so we'll just use it.
                return;
            }

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }

    public sealed partial class MainWindow : Window
    {
        Dictionary<string, string> localSettings;

        WindowsSystemDispatcherQueueHelper m_wsdqHelper; // See separate sample below for implementation
        Microsoft.UI.Composition.SystemBackdrops.MicaController m_micaController;
        Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController m_acrylicController;
        Microsoft.UI.Composition.SystemBackdrops.SystemBackdropConfiguration m_configurationSource;



        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += On_Closed;
            RestoreSettings();

            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            m_acrylicController = new DesktopAcrylicController();
            m_configurationSource = new SystemBackdropConfiguration();

            m_configurationSource.IsInputActive = true;
            m_configurationSource.Theme = SystemBackdropTheme.Default;
            m_acrylicController.TintOpacity = 0.0f;
            m_acrylicController.LuminosityOpacity = 0.0f;
            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_acrylicController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_acrylicController.SetSystemBackdropConfiguration(m_configurationSource);



        }

        public async void RestoreSettings()
        {
            localSettings = JsonWorker.LoadData();
            try
            {
                if (localSettings != null)
                {
                    swloader_combobox.SelectedIndex = Int16.Parse(localSettings["swloader_combobox"]);


                }
            }
            catch (KeyNotFoundException ex)
            {
                ;
            }

            try
            {
                if (localSettings != null)
                {

                    swdocloader_combobox.SelectedIndex = Int16.Parse(localSettings["swdocloader_combobox"]);
                }
            }
            catch (KeyNotFoundException ex)
            {
                ;
            }

            if (swloader_combobox.SelectedIndex == 0)
            {

                await Task.Run(() => SolidWorksAppWorker.OpenSolidWorksApp());

            }
            else if (swloader_combobox.SelectedIndex == 1)
            {
                return;

            }

            if (swdocloader_combobox.SelectedIndex == 0)
            {
                Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
                        this.Content.XamlRoot, "Создание нового документа");

            }
            else
            {
                string filepath = null;
                if (swdocloader_combobox.SelectedIndex == 1)
                {
                    try
                    {
                        filepath = localSettings["filepath"];
                    }
                    catch (KeyNotFoundException)
                    {
                        filepath = await FileBrowserWorker.GetFilePathAsync(this);

                    }
                }

                if (swdocloader_combobox.SelectedIndex == 2)
                {
                    filepath = await FileBrowserWorker.GetFilePathAsync(this);

                }


                if (filepath != null)
                {
                    Message.ProgressShow(() => SolidWorksAppWorker.OpenDocument(filepath),
                        this.Content.XamlRoot, "Открытие файла");
                    localSettings["filepath"] = filepath;
                }
                else
                {
                    Message.Show("File has not been chosen!", this.Content.XamlRoot, "Mistake!");
                }

            }
        }

        private void On_Closed(object sender, WindowEventArgs args)
        {
            JsonWorker.SaveData(localSettings);
            SolidWorksAppWorker.CloseSolidWorksApp();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activeDoc = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
                textBlock.Text = "Подключение к SolidWorks выполнено! Документ найден.";

            }
            catch (Exception ex) when (ex is NotSWDocumentFoundException
            || ex is NotSWAppFoundException)
            {
                Message.Show(ex + "", this.Content.XamlRoot, "Mistake!");
            }
        }

        private void myButton2_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                SolidWorksDrawerModels.DrawSimpleTestModel(
                 SolidWorksAppWorker.DefineActiveSolidWorksDocument());

            }
            catch (Exception ex) when (ex is NotSWDocumentFoundException
            || ex is NotSWAppFoundException)
            {
                Message.Show(ex + "", this.Content.XamlRoot, "Mistake!");
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SWLoadTip.IsOpen = true;

        }

        private void swloader_combobox_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (localSettings != null)
                localSettings["swloader_combobox"] = swloader_combobox.SelectedIndex + "";


        }

        private void swdocloader_combobox_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            if (localSettings != null)
                localSettings["swdocloader_combobox"] = swdocloader_combobox.SelectedIndex + "";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SWLoadDocTip.IsOpen = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Message.ProgressShow(SolidWorksAppWorker.OpenSolidWorksApp,
                        this.Content.XamlRoot);


        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SolidWorksAppWorker.CloseSolidWorksApp();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
                        this.Content.XamlRoot, "Открытие файла");
        }

        private async void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var filepath = await FileBrowserWorker.GetFilePathAsync(this);
            if (filepath != null)
            {
                Message.ProgressShow(() => SolidWorksAppWorker.OpenDocument(filepath),
                        this.Content.XamlRoot, "Открытие файла");
                localSettings["filepath"] = filepath;
            }
            else
            {
                Message.Show("File has not been chosen!", this.Content.XamlRoot, "Mistake!");
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var faceManager = new FeatureFaceManager(SolidWorksAppWorker.DefineActiveSolidWorksDocument());

            string t = "";
            foreach (var item in faceManager.nameFaces)
            {
                t += $"faceManager.nameFaces[{item.Key}] = {item.Value}";
            }

            textBlock.Text = t;
        }

        private async void Button_Click_7(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Создание исследования запущено ...";
            
            Material material = MaterialManager.GetMaterials()["Медь"];

            double averageGlobalElementSize = 3.19728742, tolerance = 0.15986437;
            var mesh = new StudyMesh(averageGlobalElementSize, tolerance);

            //mesh.GetNodes() as object[];

            var document = SolidWorksAppWorker.DefineActiveSolidWorksDocument();
            FeatureFaceManager FFM = new FeatureFaceManager(document);
            FeatureFaceManager FFM_LoadFace = new FeatureFaceManager(document);
            FeatureFaceManager FFM_FixFace = new FeatureFaceManager(document);


            FFM.ReleaseFeatureFaces(FFM.busyFaces);
            // Set fixed faces

            HashSet<FeatureFace> selectedFixedFaces = new HashSet<FeatureFace>();
            selectedFixedFaces.Add(FFM.nameFaces["Грань 1"]);

            FFM.LoadFeatureFaces(selectedFixedFaces);

            FFM_FixFace.SetFaces(selectedFixedFaces);

            var fixFaces = new HashSet<StudyFace>();

            foreach (var face in FFM_FixFace.freeFaces)
            {
                fixFaces.Add(new StudyFace(face, 0));
            }

            // Set loaded faces
            var FFM_listFace = new FeatureFaceManager();

            var FFM_listForce = new FeatureFaceManager();
            HashSet<FeatureFace> selectedLoadedFaces = new HashSet<FeatureFace>();
            selectedLoadedFaces.Add(FFM.nameFaces["Грань 2"]);

            FFM_listFace.AddFaces(selectedLoadedFaces);

            FFM.LoadFeatureFaces(selectedLoadedFaces);

            foreach (FeatureFace face in selectedLoadedFaces)
            {
                // add force value
                face.name += "-100";
            }

            FFM_listForce.AddFaces(FFM_listFace.freeFaces);
            FFM_listFace.LoadFeatureFaces(FFM_listFace.freeFaces);

            var tempLoad = FFM_listForce.freeFaces;

            FFM_LoadFace.SetFaces(tempLoad);

            FFM.LoadFeatureFaces(tempLoad);



            var loadFaces = new HashSet<StudyFace>();

            foreach (var face in FFM_LoadFace.freeFaces)
            {
                string text = face.name.Split('-')[0];

                double value = Convert.ToDouble(face.name.Split('-')[1]);

                face.name = text;
                loadFaces.Add(new StudyFace(face, value));
            }

            StaticStudyRecord studyRecord = new StaticStudyRecord(0, material, fixFaces, loadFaces, mesh);

            textBlock.Text = "Создание исследования завершено";

            await Task.Delay(2000);

            textBlock.Text = "Проведение исследования начато ...";

            try
            {
                var studyManager = new StudyManager(SolidWorksAppWorker.GetSimulation());
                StaticStudy study = studyManager.CreateStudy(studyRecord);
                study.RunStudy();
                textBlock.Text = "Проведение исследования завершено успешно ...";
            }
            catch (Exception)
            {
                textBlock.Text = "Проведение исследования не удалось! :( ...";
            }

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            var pyramid = new PyramidFourVertexArea(
                    new util.mathutils.Point3D(100, 0, 0),
                    new util.mathutils.Point3D(200, 0, 0),
                    new util.mathutils.Point3D(150, 300, 0),
                    new util.mathutils.Point3D(150, 150, 200)
                );

            SolidWorksDrawerModels.DrawPyramid(
                SolidWorksAppWorker.DefineActiveSolidWorksDocument(), pyramid);
        }
    }
}
