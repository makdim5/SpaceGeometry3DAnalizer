using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace SpaceOptimizerUWP.Services
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
            var dialog = new ContentDialog
            {
                Content = new ProgressRing() { IsActive = true },
                Title = title,
                XamlRoot = xamlRoot,

            };
            action = act;
            dialog.Opened += ProgressContentDialog_Opened;

            await dialog.ShowAsync();
        }

        private static async void ProgressContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            var msg = "";
            await Task.Run(() =>
            {
                try
                {
                    action();
                } catch(Exception ex)
                {
                    msg = ex.Message;
                }
                

            });
            if (msg != "")
            {
                sender.Title = msg;
                await Task.Delay(1500);
            }
           
            sender.Title = "Завершено!";
            await Task.Delay(500);
            sender.Hide();

        }

    }
}
