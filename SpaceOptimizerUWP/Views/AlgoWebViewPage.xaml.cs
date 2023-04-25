using System;

using SpaceOptimizerUWP.ViewModels;

using Windows.UI.Xaml.Controls;

namespace SpaceOptimizerUWP.Views
{
    public sealed partial class AlgoWebViewPage : Page
    {
        public AlgoWebViewViewModel ViewModel { get; } = new AlgoWebViewViewModel();

        public AlgoWebViewPage()
        {
            InitializeComponent();
            ViewModel.Initialize(webView);
        }
    }
}
