using SpaceOptimizerUWP.Models;
using SpaceOptimizerUWP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpaceOptimizerUWP.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class ResearchProjectPage : Page
    {
        ResearchDbModel model;
        public ResearchProjectPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            model = (ResearchDbModel)e.Parameter;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RunPage), model);

        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            dialog.Title = "Вы действительно хотите удалить проект?";
            dialog.PrimaryButtonText = "Да";
            dialog.SecondaryButtonText = "Отмена";
            
            dialog.DefaultButton = ContentDialogButton.Primary;


            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                using (var db = new ResearchDbContext())
                {
                    db.Researchs.Remove(model);
                    db.SaveChanges();
                }

                Frame.Navigate(typeof(MainPage));
            }

        }
    }
}
