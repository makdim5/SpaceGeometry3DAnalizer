﻿using SolidAppForWindowsUWP.util;
using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.Generic;
using Newtonsoft.Json;
using Windows.Storage.Pickers;
using Windows.Storage;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace SolidAppForWindowsUWP
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Dictionary<string, string> dataToSend;
        public MainPage()
        {
            dataToSend= new Dictionary<string, string>();
            this.InitializeComponent();

        }

        //public async void RestoreSettings()
        //{
        //    localSettings = JsonWorker.LoadData();
        //    try
        //    {
        //        if (localSettings != null)
        //        {
        //            swloader_combobox.SelectedIndex = Int16.Parse(localSettings["swloader_combobox"]);


        //        }
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        ;
        //    }

        //    try
        //    {
        //        if (localSettings != null)
        //        {

        //            swdocloader_combobox.SelectedIndex = Int16.Parse(localSettings["swdocloader_combobox"]);
        //        }
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        ;
        //    }

        //    if (swloader_combobox.SelectedIndex == 0)
        //    {

        //        //await Task.Run(() => SolidWorksAppWorker.OpenSolidWorksApp());

        //    }
        //    else if (swloader_combobox.SelectedIndex == 1)
        //    {
        //        return;

        //    }

        //    if (swdocloader_combobox.SelectedIndex == 0)
        //    {
        //        //Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
        //        //        this.Content.XamlRoot, "Создание нового документа");

        //    }
        //    else
        //    {
        //        string filepath = null;
        //        if (swdocloader_combobox.SelectedIndex == 1)
        //        {
        //            try
        //            {
        //                filepath = localSettings["filepath"];
        //            }
        //            catch (KeyNotFoundException)
        //            {
        //                var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //                picker.FileTypeFilter.Add(".json");
        //                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
        //                filepath = file.Path;

        //            }
        //        }

        //        if (swdocloader_combobox.SelectedIndex == 2)
        //        {
        //            var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //            picker.FileTypeFilter.Add(".json");
        //            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
        //            filepath = file.Path;

        //        }


        //        if (filepath != null)
        //        {
        //            //Message.ProgressShow(() => SolidWorksAppWorker.OpenDocument(filepath),
        //            //    this.Content.XamlRoot, "Открытие файла");
        //            //localSettings["filepath"] = filepath;
        //        }
        //        else
        //        {
        //            Message.Show("File has not been chosen!", this.Content.XamlRoot, "Mistake!");
        //        }

        //    }
        //}

        private void On_Closed(object sender, CoreWindowEventArgs args)
        {

            ClientSocketUtil.FinishCommandServer();
        }

        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string answer = "";
        //    try
        //    {
        //        answer = ClientSocketUtil.SendMsgToServer("Hello");
        //    }
        //    catch (Exception)
        //    {
        //        answer = "Соединение разорвано!";
        //    }

        //    Message.Show(answer, this.Content.XamlRoot);
        //}

        //private void myButton2_Click(object sender, RoutedEventArgs e)
        //{



        //}





        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    Message.ProgressShow(SolidWorksAppWorker.OpenSolidWorksApp,
        //                this.Content.XamlRoot);


        //}
        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    SolidWorksAppWorker.CloseSolidWorksApp();
        //}

        //private void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    Message.ProgressShow(SolidWorksAppWorker.CreateNewDocument,
        //                this.Content.XamlRoot, "Открытие файла");
        //}



        private void Button_Click_5(object sender, RoutedEventArgs e)
        {

            SmokeGrid.Visibility = Visibility.Visible;
            smokeHeader.Text = "Мастер исследований";
            simpleFrame.Navigate(typeof(CardPage), null, new DrillInNavigationTransitionInfo());
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {

            SmokeGrid.Visibility = Visibility.Collapsed;

        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Параметры";
            dialog.PrimaryButtonText = "Закрыть";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new SettingsPage();

            await dialog.ShowAsync();
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "O программе SolidSpaceAnalizer";
            dialog.PrimaryButtonText = "OK";
            dialog.DefaultButton = ContentDialogButton.Primary;
            dialog.Content = new TextBlock()
            {
                Text = "Версия 1.0\nПрограмма для топологической оптимизации" +
                "\nмонолитных деталей с помощью SolidWorks."
            };

            await dialog.ShowAsync();

        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is FrameworkElement rootElement)
            {
                if (rootElement.RequestedTheme == ElementTheme.Dark)
                {
                    rootElement.RequestedTheme = ElementTheme.Light;
                }
                else
                {
                    rootElement.RequestedTheme = ElementTheme.Dark;
                };
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new ToastContentBuilder()
                .AddText("Send a message.")
                .Show();
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                dataToSend["command"] = "opensw";
                ClientSocketUtil.SendMsgToServer(JsonConvert.SerializeObject(dataToSend));
                Message.ShowAsNotification("Solidworks открыт!");
            }
            catch
            {
                Message.Show("Нет соединения с сервером!", XamlRoot);
            }

        }

        private void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                dataToSend["command"] = "closesw";
                ClientSocketUtil.SendMsgToServer(JsonConvert.SerializeObject(dataToSend));
                Message.ShowAsNotification("Solidworks закрыт!");
            }
            catch
            {

                Message.Show("Нет соединения с сервером!", XamlRoot);
            }
        }
        private async void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.FileTypeFilter.Add(".SLDPRT");
           
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                try
                {
                    dataToSend["command"] = "opendoc";
                    dataToSend["docPath"] = file.Path;
                    ClientSocketUtil.SendMsgToServer(JsonConvert.SerializeObject(dataToSend));
                    Message.ShowAsNotification("Simulation закрыт!");
                }
                catch
                {

                    Message.Show("Нет соединения с сервером!", XamlRoot);
                }
            }
            
            
        }
    }
}
