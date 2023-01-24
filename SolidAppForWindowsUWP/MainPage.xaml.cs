using Microsoft.UI.Xaml.Controls;
using SolidAppForWindowsUWP.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MUXC = Microsoft.UI.Xaml.Controls;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace SolidAppForWindowsUWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<string, string> localSettings;
        public MainPage()
        {
            this.InitializeComponent();

            CardFrame.Navigate(typeof(CardPage));
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

                //await Task.Run(() => SolidWorksAppWorker.OpenSolidWorksApp());

            }
            else if (swloader_combobox.SelectedIndex == 1)
            {
                return;

            }

            if (swdocloader_combobox.SelectedIndex == 0)
            {
                //Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
                //        this.Content.XamlRoot, "Создание нового документа");

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
                        var picker = new Windows.Storage.Pickers.FileOpenPicker();
                        picker.FileTypeFilter.Add(".json");
                        Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                        filepath = file.Path;

                    }
                }

                if (swdocloader_combobox.SelectedIndex == 2)
                {
                    var picker = new Windows.Storage.Pickers.FileOpenPicker();
                    picker.FileTypeFilter.Add(".json");
                    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                    filepath = file.Path;

                }


                if (filepath != null)
                {
                    //Message.ProgressShow(() => SolidWorksAppWorker.OpenDocument(filepath),
                    //    this.Content.XamlRoot, "Открытие файла");
                    //localSettings["filepath"] = filepath;
                }
                else
                {
                    Message.Show("File has not been chosen!", this.Content.XamlRoot, "Mistake!");
                }

            }
        }

        private void On_Closed(object sender, CoreWindowEventArgs args)
        {
            JsonWorker.SaveData(localSettings);
            ClientSocketUtil.FinishCommandServer();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            string answer = "";
            try
            {
                answer = ClientSocketUtil.SendMsgToServer("Hello");
            }
            catch (Exception)
            {
                answer = "Соединение разорвано!";
            }

            Message.Show(answer, this.Content.XamlRoot);
        }

        private void myButton2_Click(object sender, RoutedEventArgs e)
        {



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SWLoadTip.IsOpen = true;

        }

        private void swloader_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (localSettings != null)
                localSettings["swloader_combobox"] = swloader_combobox.SelectedIndex + "";


        }

        private void swdocloader_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            //Message.ProgressShow(SolidWorksAppWorker.OpenSolidWorksApp,
            //            this.Content.XamlRoot);


        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //SolidWorksAppWorker.CloseSolidWorksApp();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
            //            this.Content.XamlRoot, "Открытие файла");
        }

        private void theme_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            switch (theme_combobox.SelectedIndex)
            {
                case 0:
                    {
                        this.Background = Resources.ThemeDictionaries["SystemControlAcrylicWindowBrush"] as Brush;
                        this.RequestedTheme = ElementTheme.Default;
                        break;
                    }
                case 1:
                    {
                        this.Background = Resources.ThemeDictionaries["SystemControlAcrylicWindowBrush"] as Brush;
                        this.RequestedTheme = ElementTheme.Dark;
                        break;
                    }
                case 2:
                    {
                        this.Background = new SolidColorBrush(Windows.UI.Colors.White);

                        this.RequestedTheme = ElementTheme.Light;
                        break;
                    }
                case 3:
                    {
                        this.Background = Resources.ThemeDictionaries["SystemControlAccentAcrylicWindowAccentMediumHighBrush"] as Brush;
                        this.RequestedTheme = ElementTheme.Default;
                        break;

                    }
            }

        }

       
    }
}
