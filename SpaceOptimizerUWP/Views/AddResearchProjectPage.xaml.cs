using Microsoft.Toolkit.Uwp.UI;
using Newtonsoft.Json.Linq;
using SpaceOptimizerUWP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpaceOptimizerUWP.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class AddResearchProjectPage : Page
    {
        int counter = 0;
        const int maxCounterValue = 4;
        string researchType;

        ResearchDbModel model = new();
        MeshParams meshParams;
        public AddResearchProjectPage()
        {
            this.InitializeComponent();
            SetFrame(counter);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
        }

        private async void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (counter == 0)
            {
                if (titleTb.Text == "" && filePathTb.Text == "")
                {
                    return;
                }
                model.Title = titleTb.Text;
            } else if (counter == 1)
            {

                meshParams = new MeshParams(QualityBox.Value.ToString(),
                    UseJacobianCheckBox.Value.ToString(),
                    MesherTypeBox.Value.ToString(),
                    MinElementsInCircleBox.Value.ToString(),
                    GrowthRatioBox.Value.ToString(),
                    SaveSettingsWithoutMeshingBox.Value.ToString(),
                    UnitBox.Value.ToString());
                try
                {
                    meshParams.CheckIsRightAttributes();
                } catch (ArgumentException ex)
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Ошибка ввода!";
                    dialog.PrimaryButtonText = "OK";
                    dialog.DefaultButton = ContentDialogButton.Primary;
                    dialog.Content = new TextBlock() { Text = ex.Message };
                    await dialog.ShowAsync();
                    return;
                }
            } else if (counter == 2)
            {
                model.Research = new BaseResearch();
                model.Research.coef1 = Coef1Box.Value.ToString();
                model.Research.coef2 = Coef2Box.Value.ToString();
                if (filterParamBox.SelectedValue != null && materialParamBox.SelectedValue != null && researchTypeBox.SelectedValue != null)
                {
                    model.Research.filterParam = filterParamBox.SelectedValue.ToString();
                    model.Research.materialParam = materialParamBox.SelectedValue.ToString();

                    if (researchTypeBox.SelectedIndex == 0)
                    {
                        researchType = ResearchType.DBSCAN;
                        epsBox.Visibility = Visibility.Visible;
                        minSamplesBox.Visibility = Visibility.Visible;
                        squeezeCoefBox.Visibility = Visibility.Collapsed;
                        nodesIntersectionAmountBox.Visibility = Visibility.Collapsed;
                    } else if (researchTypeBox.SelectedIndex == 1)
                    {
                        researchType = ResearchType.ADJACMENT;
                        epsBox.Visibility = Visibility.Collapsed;
                        minSamplesBox.Visibility = Visibility.Collapsed;
                        squeezeCoefBox.Visibility = Visibility.Visible;
                        nodesIntersectionAmountBox.Visibility = Visibility.Visible;
                    }

                    nextBtn.Content = "Завершить!";
                }
                else
                {
                    return;
                }
            } else if (counter == 3)
            {
                if (researchType == ResearchType.ADJACMENT)
                {
                    model.Research.eps = "";
                    model.Research.minSamples = "";
                    model.Research.squeezeCoef = squeezeCoefBox.Value.ToString();
                    model.Research.nodesIntersectionAmount = nodesIntersectionAmountBox.Value.ToString();
                } else if (researchType == ResearchType.DBSCAN)
                {
                    model.Research.eps = epsBox.Value.ToString();
                    model.Research.minSamples = minSamplesBox.Value.ToString();
                    model.Research.squeezeCoef = "";
                    model.Research.nodesIntersectionAmount = "";
                }
                else
                {
                    return;
                }
                model.Research.meshParams = meshParams;

                try
                {
                    model.Research.CheckIsRightAttributes();
                }
                catch (ArgumentException ex)
                {
                    ContentDialog dialog1 = new ContentDialog();
                    dialog1.Title = "Ошибка ввода!";
                    dialog1.PrimaryButtonText = "OK";
                    dialog1.DefaultButton = ContentDialogButton.Primary;
                    dialog1.Content = new TextBlock() { Text = ex.Message };
                    await dialog1.ShowAsync();
                    return;
                }

                using (var db = new ResearchDbContext())
                {
                    db.Researchs.Add(model);
                    db.SaveChanges();
                }

                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Cообщение";
                dialog.PrimaryButtonText = "OK";
                dialog.DefaultButton = ContentDialogButton.Primary;
                dialog.Content = new TextBlock() { Text ="Создание исследования прошло успешно!" };
                await dialog.ShowAsync();

                this.Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
                return;
            }
            
            prevBtn.Visibility = Visibility.Visible;
            if (counter + 1 < maxCounterValue)
            {
                counter++;
                SetFrame(counter);
            }

        }

        private void SetFrame(int counter)
        {
            if (counter == 0)
            {
                firstFrame.Visibility = Visibility.Visible;
                secondFrame.Visibility = Visibility.Collapsed;
                thirdFrame.Visibility = Visibility.Collapsed;
                forthFrame.Visibility = Visibility.Collapsed;
            }
            else if (counter == 1)
            {
                firstFrame.Visibility = Visibility.Collapsed;
                secondFrame.Visibility = Visibility.Visible;
                thirdFrame.Visibility = Visibility.Collapsed;
                forthFrame.Visibility = Visibility.Collapsed;
            } else if (counter == 2)
            {
                firstFrame.Visibility = Visibility.Collapsed;
                secondFrame.Visibility = Visibility.Collapsed;
                thirdFrame.Visibility = Visibility.Visible;
                forthFrame.Visibility = Visibility.Collapsed;
            } else if (counter == 3)
            {
                firstFrame.Visibility = Visibility.Collapsed;
                secondFrame.Visibility = Visibility.Collapsed;
                thirdFrame.Visibility = Visibility.Collapsed;
                forthFrame.Visibility = Visibility.Visible;
            }
        }

        private void prevBtn_Click(object sender, RoutedEventArgs e)
        {
            counter--;
            nextBtn.Content = "Далее";
            SetFrame(counter);
            if (counter == 0)
            {
                prevBtn.Visibility = Visibility.Collapsed;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            openPicker.CommitButtonText = "Открыть";
            openPicker.FileTypeFilter.Add(".SLDPRT");
            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                filePathTb.Text = file.Path;
                model.FilePath = file.Path;
            }
        }
    }
}
