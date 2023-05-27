using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
using Windows.UI.Xaml.Navigation;

namespace SpaceOptimizerUWP.Views
{
    public sealed partial class RunPage : Page
    {
        int counter = 0;
        ResearchDbModel model;
        CutConfig cutConfig;
        List<Area> areas;
        public RunPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                List<object> params_ = (List<object>)e.Parameter;
                model = (ResearchDbModel)params_[0];
                if ((int)params_[1] == 0)
                {
                    if (model.Areas == null)
                    {
                        model.Areas = new List<Area>();
                    }
                    
                    nextBtn.Click += nextBtn_Click;

                }
                else if ((int)params_[1] == 1)
                {
                    areas = model.Areas;
                    nextBtn.Click += NextBtn2_Click;
                }
            }
            else
            {
                Message.Show("Возникли проблемы с запуском исследования, вернитесь назад!", Frame.XamlRoot);
            }
            
        }

        private async void NextBtn2_Click(object sender, RoutedEventArgs e)
        {
            nextBtn.IsEnabled = false;
            bool res;
            try
            {
                if (counter == 0)
                {
                    nextBtn.IsEnabled = true;
                    cutConfig = new CutConfig(CutType.NODE_WAY, NodeCutWay.FIGURE_WAY, FigureCutType.RECTANGLE);

                    cutConfig.cutType = CutType.NAMES[cutTypeBox.SelectedIndex];

                    if (cutTypeBox.SelectedIndex == 0)
                    {

                        cutConfig.nodeCutWay = NodeCutWay.NAMES[nodeCutWayBox.SelectedIndex];
                        cutConfig.figureType = FigureCutType.NAMES[figureCutBox.SelectedIndex];
                    }

                    cutConfigPanel.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    headerTb.Text = "Создаём менеджера исследований согласно конфигурации ...";

                    await Task.Delay(500);
                    res = BackConnectorService.CreateResearchManagerInBack(model.Research, cutConfig);
                    var res2 = BackConnectorService.LoadAreas(model.Areas);
                    if (!(res))
                    {
                        Message.Show("Создать менеджера не удалось", Frame.XamlRoot);
                        Frame.Navigate(typeof(ResearchProjectPage), model);
                    } else if (!(res2))
                    {
                        Message.Show("Области не подгрузились!", Frame.XamlRoot);
                        Frame.Navigate(typeof(ResearchProjectPage), model);
                    }
                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    infoTextBlock.Text = "Менеджер исследований определен успешно, области подгружены ,нажмите далее ...";
                    counter++;
                    nextBtn.IsEnabled = true;
                }
                else if (counter == 1)
                {
                    headerTb.Text = "Выполняем вырез областей ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Collapsed;
                    cutRunPanel.Visibility = Visibility.Visible;
                    await Task.Delay(500);

                    for (int i = 0; i < areas.Count; i++)
                    {
                        cutCounterTb.Text = $"Вырезано {i} из {areas.Count} областей ...";
                        progressBar.Value = i * 100 / areas.Count;
                        await Task.Delay(500);
                        BackConnectorService.CutArea(i);

                    }
                    cutCounterTb.Text = $"Области вырезаны!";
                    progressBar.Visibility = Visibility.Collapsed;

                    nextBtn.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                Message.Show(ex.Message, Frame.XamlRoot);
                Frame.Navigate(typeof(ResearchProjectPage), model);
            }
        }

        private async void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            nextBtn.IsEnabled = false;
            bool res;
            try
            {
                if (counter == 0)
                {
                    nextBtn.IsEnabled = true;
                    cutConfig = new CutConfig(CutType.NODE_WAY, NodeCutWay.FIGURE_WAY, FigureCutType.RECTANGLE);
                    
                    cutConfig.cutType = CutType.NAMES[cutTypeBox.SelectedIndex];

                    if (cutTypeBox.SelectedIndex == 0)
                    {
                       
                        cutConfig.nodeCutWay = NodeCutWay.NAMES[nodeCutWayBox.SelectedIndex];
                        cutConfig.figureType = FigureCutType.NAMES[figureCutBox.SelectedIndex];
                    }

                    cutConfigPanel.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    headerTb.Text = "Создаём менеджера исследований согласно конфигурации ...";

                    await Task.Delay(500);
                    res = BackConnectorService.CreateResearchManagerInBack(model.Research, cutConfig);
                    if (!(res))
                    {
                        Message.Show("Создать менеджера не удалось", Frame.XamlRoot);
                        Frame.Navigate(typeof(ResearchProjectPage), model);
                    }
                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    infoTextBlock.Text = "Менеджер исследований определен успешно, нажмите далее ...";
                    counter++;
                    nextBtn.IsEnabled = true;
                } else if (counter == 1)
                {
                    headerTb.Text = "Определяем результаты исследований ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                    var msg = BackConnectorService.DetermineResearchResults();
                    
                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    infoTextBlock.Text = msg;
                    counter++;
                    nextBtn.IsEnabled = true;
                }
                else if (counter == 2)
                {
                    headerTb.Text = "Ищем критические узлы ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                    var msg = BackConnectorService.DetermineCriticalNodes();

                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    infoTextBlock.Text = msg;
                    counter++;
                    nextBtn.IsEnabled = true;
                }
                else if (counter == 3)
                {
                    headerTb.Text = "Выполняем поиск областей ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                    areas = BackConnectorService.DetermineAreas();
                    model.Areas.AddRange(areas);
                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    infoTextBlock.Text = $"Определены области в количестве {areas.Count()}!";
                    counter++;
                    nextBtn.IsEnabled = true;
                }
                else if (counter == 4)
                {
                    headerTb.Text = "Выполняем вырез областей ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Collapsed;
                    cutRunPanel.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                   
                    for (int i = 0; i < areas.Count; i++)
                    {
                        cutCounterTb.Text = $"Вырезано {i} из {areas.Count} областей ...";
                        progressBar.Value = i * 100 / areas.Count;
                        await Task.Delay(500);
                        BackConnectorService.CutArea(i);

                    }
                    cutCounterTb.Text = $"Области вырезаны!";
                    progressBar.Visibility = Visibility.Collapsed;
                    
                    counter++;
                    nextBtn.IsEnabled = true;
                }
                else if (counter == 5)
                {
                    headerTb.Text = "Выполняем повторное исследование ...";
                    infoTextBlock.Visibility = Visibility.Collapsed;
                    cutRunPanel.Visibility = Visibility.Collapsed;
                    progressRing.Visibility = Visibility.Visible;
                    await Task.Delay(500);
                    res = BackConnectorService.RunStudy();
                    progressRing.Visibility = Visibility.Collapsed;
                    infoTextBlock.Visibility = Visibility.Visible;
                    if (!(res))
                    {
                        infoTextBlock.Text = "Исследование провести не удалось!";
                    }
                    
                    infoTextBlock.Text = $"Повторное исследование проведено успешно!";
                    counter = 1;
                    nextBtn.IsEnabled = true;
                }


            } catch (Exception ex)
            {
                Message.Show(ex.Message, Frame.XamlRoot);
                Frame.Navigate(typeof(ResearchProjectPage), model);
            }
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new ResearchDbContext())
            {
                db.Researchs.Update(model);
                db.SaveChanges();
            }
            Frame.Navigate(typeof(ResearchProjectPage), model);
        }

        private void cutTypeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cutTypeBox.SelectedIndex == 0)
            {
                figureCutBox.Visibility = Visibility.Visible;
                nodeCutWayBox.Visibility = Visibility.Visible;
            }
            else
            {
                figureCutBox.Visibility = Visibility.Collapsed;
                nodeCutWayBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}
