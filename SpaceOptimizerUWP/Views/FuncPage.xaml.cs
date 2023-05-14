using Microsoft.Toolkit;
using Newtonsoft.Json;
using SpaceOptimizerUWP.Core.Helpers;
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
    public sealed partial class FuncPage : Page
    {

        List<Area> areas;
        int counter;
        public FuncPage()
        {
            this.InitializeComponent();
            areas = new();
            counter = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> dbscanCusteringConfiguration = new Dictionary<string, object>()
            {
                { "meshParams", new Dictionary<string, string>()
                    {
                        {"Quality", "1" },
                        {"UseJacobianCheck", "2" },
                        {"MesherType", "1" },
                        {"MinElementsInCircle","8" },
                        {"GrowthRatio", "1,4" },
                        {"SaveSettingsWithoutMeshing", "0" },
                        {"Unit", "0" }
                    }
                },
                {"filterParam", "VON" },
                {"coef1", "0,2" },
                {"coef2", "0,1" },
                {"materialParam", "SIGXT" },
                {"eps", "2" },
                {"minSamples", "4"}
            };
            Dictionary<string, string> cutConfiguration = new Dictionary<string, string>()
            {
                {"cutType", "node"},
                {"nodeCutWay", "figure"},
                {"figureType", "rect" }
            };
            // всего 2 менеджера "adjacmentElements" и "dbscan"
            var task = Task.Run(() =>
            {
                var data = new Dictionary<string, object>() {
                    { "managerType", "dbscan" },
                    {"managerConfig", dbscanCusteringConfiguration },
                    {"cutConfig", cutConfiguration }
                };
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "setmngr", data);

            });
            task.Wait();
            outTextBlock.Text = task.GetResultOrDefault();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "research", "");
            });
            task.Wait();
            outTextBlock.Text = task.GetResultOrDefault();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "critical_nodes", "");
            });
            task.Wait();
            outTextBlock.Text = task.GetResultOrDefault();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "areas", "");
            });
            task.Wait();
            string res = task.GetResultOrDefault();
            areas = JsonConvert.DeserializeObject<List<Area>>(res);
            outTextBlock.Text = areas.First().ToString();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (counter < areas.Count())
            {
                var task = Task.Run(() =>
                {
                    var data = new Dictionary<string, object>() {
                    { "index", $"{counter}" }
                    };
                    return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "cut_areas", data);

                });
                task.Wait();
                counter++;
                outTextBlock.Text = $"{areas.Count() - counter} осталось"+ task.GetResultOrDefault();
            }
            else
            {
                outTextBlock.Text = "Все области вырезаны";
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var task = Task.Run(() =>
            {
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync("", "runstudy", "");
            });
            task.Wait();
            outTextBlock.Text = task.GetResultOrDefault();
            counter = 0;
        }
    }
}
