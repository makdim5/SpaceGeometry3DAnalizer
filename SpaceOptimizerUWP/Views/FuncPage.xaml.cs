using Microsoft.Toolkit;
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
        public FuncPage()
        {
            this.InitializeComponent();
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
                return new HttpDataService("http://127.0.0.1:8005/").PostAsJsonAsync<Dictionary<string, object>>("", "setmngr", data);

            });
            task.Wait();
            outTextBlock.Text = task.GetResultOrDefault();
        }
    }
}
