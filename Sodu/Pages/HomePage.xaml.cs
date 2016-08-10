using Sodu.Model;
using Sodu.ViewModel;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.pivot.SelectionChanged += pivot_SelectionChanged;
            this.Unloaded += HomePage_Unloaded;
        }



        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.pivot.ItemsSource != (this.DataContext as MainPageViewModel).CurrentMenuList)
            {
                this.pivot.ItemsSource = (this.DataContext as MainPageViewModel).CurrentMenuList;
                this.pivot.SelectedItem = (this.DataContext as MainPageViewModel).CurrentMenu;
            }
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //if (this.pivot.ItemsSource != (this.DataContext as MainPageViewModel).CurrentMenuList)
        //   {
        //       this.pivot.ItemsSource = (this.DataContext as MainPageViewModel).CurrentMenuList;
        //       this.pivot.SelectedItem = (this.DataContext as MainPageViewModel).CurrentMenu;
        //   }
        //}

        private void HomePage_Unloaded(object sender, RoutedEventArgs e)
        {
            SetBookShelfUnEditing();
        }


        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetBookShelfUnEditing();
        }

        private void SetBookShelfUnEditing()
        {
            if (ViewModelInstance.Instance.IsLogin)
            {
                if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.IsEditing)
                {
                    ViewModelInstance.Instance.MyBookShelfViewModelInstance.OnEditCommand(false);
                }
            }
        }
    }
}
