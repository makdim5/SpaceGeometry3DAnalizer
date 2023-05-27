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
                var items = db.Researchs.Include(u => u.Research).ToList();
                researchesList.ItemsSource = items;
            }
        }

        //private async void Button_Click_2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    FileOpenPicker openPicker = new FileOpenPicker();
        //    openPicker.ViewMode = PickerViewMode.Thumbnail;
        //    openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        //    openPicker.CommitButtonText = "Открыть";
        //    openPicker.FileTypeFilter.Add(".SLDPRT");
        //    var file = await openPicker.PickSingleFileAsync();

        //    if (file != null)
        //    {
        //        var data = new Dictionary<string, string>() { { "docPath", file.Path } };
        //        await Task.Run(async () => await new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync<Dictionary<string, string>>("", "opendoc", data));

        //    }

        //}

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            //var words = await httpHelper.GetAsync<List<Word>>("words");
            //if (words == null)
            //{
            //    return;
            //}
            //if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            //{
            //    var suitableItems = new List<Word>();
            //    var splittext = sender.Text.ToLower().Split(" ");
            //    foreach (var word in words)
            //    {
            //        var found = splittext.All((key) =>
            //        {
            //            return word.word_title.ToLower().Contains(key);
            //        });
            //        if (found)
            //        {
            //            suitableItems.Add(word);
            //        }
            //    }
            //    if (suitableItems.Count == 0)
            //    {
            //        suitableItems.Add(new Word(0, "no results found", ""));
            //    }
            //    companiesList.ItemsSource = suitableItems;
            //}

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
