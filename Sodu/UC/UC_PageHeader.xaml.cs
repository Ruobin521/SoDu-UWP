using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sodu.Pages;
using Sodu.Services;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UC
{
    public sealed partial class UC_PageHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(UC_PageHeader), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }


        public static readonly DependencyProperty SearchButtonVisibilityProperty = DependencyProperty.Register(
            "SearchButtonVisibility", typeof(Visibility), typeof(UC_PageHeader), new PropertyMetadata(Windows.UI.Xaml.Visibility.Collapsed));

        public Visibility SearchButtonVisibility
        {
            get { return (Visibility)GetValue(SearchButtonVisibilityProperty); }
            set { SetValue(SearchButtonVisibilityProperty, value); }
        }
        public UC_PageHeader()
        {
            this.InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(typeof(SearchResultPage));
        }
    }
}
