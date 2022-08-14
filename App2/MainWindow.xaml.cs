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

namespace App2
{

    public sealed partial class MainWindow : Window
    {
        Dictionary<string, string> localSettings;
        
        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += On_Closed;
            RestoreSettings();


        }

        public async void RestoreSettings()
        {
            localSettings = JsonWorker.LoadData();
            try
            {
                if (localSettings != null)
                {
                    swloader_combobox.SelectedIndex = Int16.Parse(localSettings["swloader_combobox"]);

                    swdocloader_combobox.SelectedIndex = Int16.Parse(localSettings["swdocloader_combobox"]);
                }
            }
            catch (KeyNotFoundException ex)
            {
                ;
            }

            if (swloader_combobox.SelectedIndex == 0)
            {

                await Task.Run(()=> SolidWorksDefiner.OpenSolidWorksApp());

            }
            else if (swloader_combobox.SelectedIndex == 1)
            {
                return;

            }

            if (swdocloader_combobox.SelectedIndex == 0)
            {
                Message.ProgressShow(SolidWorksDefiner.CreateNewDocument,
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
                        localSettings["filepath"] = filepath;
                    }
                }

                if (swdocloader_combobox.SelectedIndex == 2)
                {
                    filepath = await FileBrowserWorker.GetFilePathAsync(this);
                    localSettings["filepath"] = filepath;
                }


                if (filepath != null)
                {
                    Message.ProgressShow( () => SolidWorksDefiner.OpenDocument(filepath),
                        this.Content.XamlRoot, "Открытие файла");
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
            SolidWorksDefiner.CloseSolidWorksApp();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var activeDoc = SolidWorksDefiner.DefineActiveSolidWorksDocument();
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
                 SolidWorksDefiner.DefineActiveSolidWorksDocument());

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
            Message.ProgressShow(SolidWorksDefiner.OpenSolidWorksApp,
                        this.Content.XamlRoot);
           

        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SolidWorksDefiner.CloseSolidWorksApp();
        }        

    }
}
