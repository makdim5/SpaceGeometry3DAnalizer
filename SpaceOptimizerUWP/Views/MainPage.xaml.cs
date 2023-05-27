using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpaceOptimizerUWP.Models;
using SpaceOptimizerUWP.Services;
using SpaceOptimizerUWP.ViewModels;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.EntityFrameworkCore;

namespace SpaceOptimizerUWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        List<ResearchDbModel> projects;
        public MainPage()
        {
            InitializeComponent();

            using (var db = new ResearchDbContext())
            {
                db.MeshConfigurations.ToList();
                db.StrainNodeParameters.ToList();
                db.StressNodeParameters.ToList();
                db.Areas.ToList();
                db.Points.ToList();
                db.Nodes.ToList();
                projects = db.Researchs.Include(u => u.Research).ToList();
                researchesList.ItemsSource = projects;
            }
        }

      
        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
           
            if (projects == null | projects.Count() == 0)
            {
                return;
            }
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<ResearchDbModel> suitableItems = new();
                var splittext = sender.Text.ToLower().Split(" ");
                foreach (var project in projects)
                {
                    var found = splittext.All((key) =>
                    {
                        return project.Title.ToLower().Contains(key) || project.FilePath.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        suitableItems.Add(project);
                    }
                }
                if (suitableItems.Count > 0)
                {
                    researchesList.ItemsSource = suitableItems;
                }
               
            }

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AddResearchProjectPage), null, new EntranceNavigationTransitionInfo());
        }

        private void researchesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (ResearchDbModel)e.ClickedItem;
            Frame.Navigate(typeof(ResearchProjectPage), item, new EntranceNavigationTransitionInfo());
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Message.ProgressShow(() => { BackConnectorService.OpenSolidWorks(); },
                     this.Frame.XamlRoot);

            }
            catch (Exception ex)
            {
                ContentDialog dialog1 = new ContentDialog();
                dialog1.Title = "Произошла ошибка";
                dialog1.PrimaryButtonText = "OK";
                dialog1.DefaultButton = ContentDialogButton.Primary;
                dialog1.Content = new TextBlock() { Text = ex.Message };
                await dialog1.ShowAsync();
            }

        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                BackConnectorService.CloseSolidWorks();
            }
            catch (Exception ex)
            {
                ContentDialog dialog1 = new ContentDialog();
                dialog1.Title = "Произошла ошибка";
                dialog1.PrimaryButtonText = "OK";
                dialog1.DefaultButton = ContentDialogButton.Primary;
                dialog1.Content = new TextBlock() { Text = ex.Message };
                await dialog1.ShowAsync();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
