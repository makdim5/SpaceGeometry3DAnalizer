//using System;
//using System.Text.RegularExpressions;
//using Windows.Storage.Streams;
//using Windows.UI.WindowManagement;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Media.Imaging;

//namespace App2.util
//{
//    public class WindowShower
//    {
//        public static async void Show<pageToShow, backPage>(Frame backPageFrame, object parametr = null)
//        {

//            // Create a new window.
//            AppWindow appWindow = await AppWindow.TryCreateAsync();

//            // Create a Frame and navigate to the Page you want to show in the new window.
//            Frame appWindowContentFrame = new Frame();
//            appWindowContentFrame.Navigate(typeof(pageToShow), parametr);

//            // Attach the XAML content to the window.
//            Windows.UI.Xaml.Hosting.ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);


//            // When the window is closed, be sure to release
//            // XAML resources and the reference to the window.
//            appWindow.Closed += delegate
//            {
//                appWindowContentFrame.Content = null;
//                appWindow = null;
//                backPageFrame.Navigate(typeof(backPage));
//            };

//            // Show the window.
//            await appWindow.TryShowAsync();
//        }

//    //    public static async void ShowSaleP<pageToShow>(AddSalePage backPageFrame, object parametr = null)
//    //    {

//    //        // Create a new window.
//    //        AppWindow appWindow = await AppWindow.TryCreateAsync();

//    //        // Create a Frame and navigate to the Page you want to show in the new window.
//    //        Frame appWindowContentFrame = new Frame();
//    //        appWindowContentFrame.Navigate(typeof(pageToShow), parametr);

//    //        // Attach the XAML content to the window.
//    //        Windows.UI.Xaml.Hosting.ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowContentFrame);

//    //        backPageFrame.IsEnabled = false;

//    //        // When the window is closed, be sure to release
//    //        // XAML resources and the reference to the window.
//    //        appWindow.Closed += delegate
//    //        {
//    //            appWindowContentFrame.Content = null;
//    //            appWindow = null;
//    //            backPageFrame.UpdateCompts();
//    //            backPageFrame.IsEnabled = true;
//    //        };

//    //        // Show the window.
//    //        await appWindow.TryShowAsync();
//    //    }

//    //    public static BitmapImage SetImage(string urlText)
//    //    {
//    //        var bitm = new BitmapImage();
//    //        if (Regex.Match(urlText, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"] != null)
//    //        {
//    //            var base64Data = Regex.Match(urlText, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
//    //            var binData = Convert.FromBase64String(base64Data);

//    //            using (var ms = new InMemoryRandomAccessStream())
//    //            {

//    //                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
//    //                {
//    //                    writer.WriteBytes((byte[])binData);
//    //                    writer.StoreAsync().GetResults();
//    //                }

//    //                bitm.SetSource(ms);
                    
//    //            }
//    //        }

//    //        if (urlText.Contains("https://"))
//    //            bitm = new BitmapImage(new Uri(urlText));

//    //        return bitm;
//    //    }
//    //}
//}
