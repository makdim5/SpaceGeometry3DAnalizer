using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using SpaceOptimizerUWP.Models;
using SpaceOptimizerUWP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
        List<Area> areas;
        HashSet<Node> nodes;
        public ResearchProjectPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            model = (ResearchDbModel)e.Parameter;
            areas = model.Areas;
            areasList.ItemsSource = areas;
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

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Message.Show(model.Research.ToString(), Frame.XamlRoot, "Параметры исследования");
        }

        private void areasList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Area clickedArea = (Area)e.ClickedItem;
            nodes = clickedArea.nodes;
            nodesList.ItemsSource = nodes;
        }

        private void nodesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Node clickedNode = (Node)e.ClickedItem;
            Message.Show(clickedNode.ToString(), Frame.XamlRoot, $"Информация об узле #{clickedNode.number}");
        }

        private async void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (nodesList.Visibility == Visibility.Visible)
            {
                chartBtn.Label = "Показать список";
                areasChartWebView.Visibility = Visibility.Visible;
                nodesList.Visibility = Visibility.Collapsed;
                string pointsData = JsonConvert.SerializeObject(areas.Select(area => area.GetCoordsToChart()).ToList());
                string html = "<html><body>  <div id=\"my_dataviz\" style=\" width: 100vw;\r\n  height: 100vh;\" >\r\n</div><script src=\"https://cdn.plot.ly/plotly-2.20.0.min.js\" charset=\"utf-8\"></script>\r\n    <script>\r\n        var points =\r\n        " + pointsData + "\r\n\r\n        var data = points.map(function (item) {\r\n            return {\r\n                x: item[0], y: item[1], z: item[2],\r\n                mode: 'markers',\r\n                marker: {\r\n                    opacity: 0.8\r\n                },\r\n                type: 'scatter3d'\r\n            }\r\n        });\r\n        var layout = {\r\n            margin: {\r\n                l: 0,\r\n                r: 0,\r\n                b: 0,\r\n                t: 0\r\n            }\r\n        };\r\n        Plotly.newPlot('my_dataviz', data, layout);</script></body></html>";
                await areasChartWebView.EnsureCoreWebView2Async();
                areasChartWebView.NavigateToString(html);
            } else if (areasChartWebView.Visibility == Visibility.Visible)
            {
                chartBtn.Label = "Показать график";
                nodesList.Visibility = Visibility.Visible;
                areasChartWebView.Visibility = Visibility.Collapsed;
            }
            
        }

        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            Message.ProgressShow(() => BackConnectorService.OpenDocument(model.FilePath), Frame.XamlRoot);
           
        }
    }
}
