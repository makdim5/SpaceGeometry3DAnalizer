using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace App2.util
{
    public class Message
    {
        private static Action action;
        public static async void Show(string msg, XamlRoot xamlRoot, string title = "Сообщение")
        {

            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = "ОК",
                XamlRoot = xamlRoot,
            };

            await dialog.ShowAsync();
        }

        public static async void ProgressShow(Action act, XamlRoot xamlRoot, string title = "Подождите несколько секунд ...")
        {

            var prBar = new ProgressBar();
            prBar.IsIndeterminate = true;
            var dialog = new ContentDialog
            {
                Content = prBar,
                Title = title,
                XamlRoot = xamlRoot,

            };
            action = act;
            dialog.Opened += ProgressContentDialog_Opened;

            await dialog.ShowAsync();
        }

        private static async void ProgressContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {

            await Task.Run(() =>
            {
                action();
                
            });

            sender.Title = "Завершено!";
            await Task.Delay(500);
            sender.Hide();

        }


    }
}
