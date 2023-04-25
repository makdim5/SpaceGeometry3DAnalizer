using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SpaceOptimizerUWP.Services;
using SpaceOptimizerUWP.ViewModels;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace SpaceOptimizerUWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Task.Run( async() => await new HttpDataService("http://127.0.0.1:8005/").GetAsync<string>("", "opensw"));

        }

        private void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Task.Run(async () => await new HttpDataService("http://127.0.0.1:8005/").GetAsync<string>("", "closesw"));

        }

        private async void Button_Click_2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.CommitButtonText = "Открыть";
            openPicker.FileTypeFilter.Add(".SLDPRT");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var data = new Dictionary<string, string>() { { "docPath", file.Path } };
                await Task.Run(async () => await new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync<Dictionary<string, string>>("", "opendoc", data));

            }

        }
    }
}
