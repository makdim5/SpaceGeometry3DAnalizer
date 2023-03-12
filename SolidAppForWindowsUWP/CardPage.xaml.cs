using System;
using System.Collections.Generic;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace SolidAppForWindowsUWP
{
    public sealed partial class CardPage : Page
    {
        public CardPage()
        {
            this.InitializeComponent();
            processShower.Visibility= Visibility.Collapsed;
        }

        
    }
}
