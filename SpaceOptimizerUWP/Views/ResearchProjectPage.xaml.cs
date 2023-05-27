using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using SpaceOptimizerUWP.Models;
using SpaceOptimizerUWP.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.RemoteSystems;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;
using Syncfusion.XlsIO;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.UI;

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
            if (areas != null)
            {
                for (int i = 0; i < areas.Count; i++)
                {
                    areas[i].areaName = $"Область -  {i}";
                }
                areasList.ItemsSource = areas;

                double genVolume = 0;
                foreach (Area area in areas)
                {
                    genVolume += area.Volume;
                }
                volumeTb.Text = $"Общий объём вырезаемых областей = {Math.Round(genVolume, 2)}";
            }
            else
            {
                areasList.ItemsSource = new List<Area>() { new Area() { areaName = "Области пока не обнаружены" } };

            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RunPage), new List<object>() { model, 0 });

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
        private async void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            if (areas == null)
            {
                Message.Show("Нет доступных областей для отображения!", Frame.XamlRoot);
                return;
            }
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

        private async void AppBarButton_Click_5(object sender, RoutedEventArgs e)
        {

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                //Create a workbook with a worksheet
                IWorkbook workbook = application.Workbooks.Create(areas.Count);

               
                
                for (int i = 0; i < areas.Count; i++)
                {
                    //Access first worksheet from the workbook instance.
                    IWorksheet worksheet = workbook.Worksheets[i];
                    Assembly executingAssembly = typeof(MainPage).GetTypeInfo().Assembly;
                    worksheet.Name = $"Область {i}";

                    worksheet.Range[$"A1"].Text = "Номер узла";

                    worksheet.Range["C1:E1"].Merge();
                    worksheet.Range["C1:E1"].Text = "Координаты узла";
                    worksheet.Range[$"C2"].Text = "X";
                    worksheet.Range[$"D2"].Text = "Y";
                    worksheet.Range[$"E2"].Text = "Z";

                    worksheet.Range["G1:Q1"].Merge();
                    worksheet.Range["G1:Q1"].Text = "Значения параметров напряжения";
                    worksheet.Range[$"G2"].Text = "Sx";
                    worksheet.Range[$"H2"].Text = "Sy";
                    worksheet.Range[$"I2"].Text = "Sz";
                    worksheet.Range[$"J2"].Text = "Txy";
                    worksheet.Range[$"K2"].Text = "Tyz";
                    worksheet.Range[$"L2"].Text = "Txz";
                    worksheet.Range[$"M2"].Text = "P1";
                    worksheet.Range[$"N2"].Text = "P2";
                    worksheet.Range[$"O2"].Text = "P2";
                    worksheet.Range[$"P2"].Text = "INT";
                    worksheet.Range[$"Q2"].Text = "VON";

                    worksheet.Range["S1:AD1"].Merge();
                    worksheet.Range["S1:AD1"].Text = "Значения параметров деформации";
                    worksheet.Range[$"S2"].Text = "EPSx";
                    worksheet.Range[$"T2"].Text = "EPSy";
                    worksheet.Range[$"U2"].Text = "EPSz";
                    worksheet.Range[$"V2"].Text = "GMxy";
                    worksheet.Range[$"W2"].Text = "GMyz";
                    worksheet.Range[$"X2"].Text = "GMxz";
                    worksheet.Range[$"Y2"].Text = "ESTRN";
                    worksheet.Range[$"Z2"].Text = "SEDENS";
                    worksheet.Range[$"AA2"].Text = "ENERGY";
                    worksheet.Range[$"AB2"].Text = "E1";
                    worksheet.Range[$"AC2"].Text = "E2";
                    worksheet.Range[$"AD2"].Text = "E3";

                    for (int j = 0; j < areas[i].nodes.Count(); j++)
                    {
                        worksheet.Range[$"A{j +3}"].Number = areas[i].nodes.ElementAt(j).number;
                                               
                        worksheet.Range[$"C{j +3}"].Number = areas[i].nodes.ElementAt(j).point.x;
                        worksheet.Range[$"D{j +3}"].Number = areas[i].nodes.ElementAt(j).point.y;
                        worksheet.Range[$"E{j +3}"].Number = areas[i].nodes.ElementAt(j).point.z;
                                              
                        worksheet.Range[$"G{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Sx;
                        worksheet.Range[$"H{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Sy;
                        worksheet.Range[$"I{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Sz;
                        worksheet.Range[$"J{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Txy;
                        worksheet.Range[$"K{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Tyz;
                        worksheet.Range[$"L{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.Txz;
                        worksheet.Range[$"M{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.P1;
                        worksheet.Range[$"N{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.P2;
                        worksheet.Range[$"O{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.P3;
                        worksheet.Range[$"P{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.INT;
                        worksheet.Range[$"Q{j +3}"].Number = areas[i].nodes.ElementAt(j).stress.VON;
                                               
                        worksheet.Range[$"S{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.EPSx;
                        worksheet.Range[$"T{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.EPSy;
                        worksheet.Range[$"U{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.EPSz;
                        worksheet.Range[$"V{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.GMxy;
                        worksheet.Range[$"W{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.GMyz;
                        worksheet.Range[$"X{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.GMxz;
                        worksheet.Range[$"Y{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.ESTRN;
                        worksheet.Range[$"Z{j +3}"].Number = areas[i].nodes.ElementAt(j).strain.SEDENS;
                        worksheet.Range[$"AA{j + 3}"].Number = areas[i].nodes.ElementAt(j).strain.ENERGY;
                        worksheet.Range[$"AB{j + 3}"].Number = areas[i].nodes.ElementAt(j).strain.E1;
                        worksheet.Range[$"AC{j + 3}"].Number = areas[i].nodes.ElementAt(j).strain.E2;
                        worksheet.Range[$"AD{j + 3}"].Number = areas[i].nodes.ElementAt(j).strain.E3;
                    }
                }

               

                // Save the Workbook
                StorageFile storageFile;
                if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
                {
                    FileSavePicker savePicker = new FileSavePicker();
                    savePicker.SuggestedStartLocation = PickerLocationId.Desktop;
                    savePicker.SuggestedFileName = "Output";
                    savePicker.FileTypeChoices.Add("Excel Files", new List<string>() { ".xlsx" });
                    storageFile = await savePicker.PickSaveFileAsync();
                }
                else
                {
                    StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    storageFile = await local.CreateFileAsync("Output.xlsx", CreationCollisionOption.ReplaceExisting);
                }
                //Saving the workbook
                await workbook.SaveAsAsync(storageFile);

                // Launch the saved file
                await Windows.System.Launcher.LaunchFileAsync(storageFile);
            }
        }
        private void AppBarButton_Click_6(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RunPage), new List<object>() { model, 1 });
        }

        private async void AppBarButton_Click_7(object sender, RoutedEventArgs e)
        {
            if (areas != null)
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Вы действительно хотите удалить области исследования?";
                dialog.PrimaryButtonText = "Да";
                dialog.SecondaryButtonText = "Отмена";

                dialog.DefaultButton = ContentDialogButton.Primary;


                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                   
                    using (var db = new ResearchDbContext())
                    {
                        foreach (var area in model.Areas)
                        {
                            db.Areas.Remove(area);
                        }
                        model.Areas.Clear();
                        db.Researchs.Update(model);
                        db.SaveChanges();
                    }
                    Frame.Navigate(typeof(ResearchProjectPage), model);
                }
                
            }
        }
    }
}
