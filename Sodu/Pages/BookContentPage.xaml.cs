﻿using Sodu.Core.Util;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookContentPage : Page
    {
        double x = 0;//用来接收手势水平滑动的长度

        public BookContentPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;


            ManipulationCompleted += The_ManipulationCompleted;//订阅手势滑动结束后的事件
            ManipulationStarted += BookContentPage_ManipulationStarted;   //订阅手势滑动结束后的事件
            ManipulationDelta += The_ManipulationDelta;//订阅手势滑动事件

            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                this.Loaded -= BookContentPage_Loaded;
                this.Loaded += BookContentPage_Loaded;
                this.grid.Tapped -= grid_Tapped;
                this.grid.Tapped += grid_Tapped;
            }
        }



        private void BookContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollviewer = GetVisualChildCollection<ScrollViewer>(this.listview)[0];
            scrollviewer.ViewChanging -= Scrollviewer_ViewChanging;
            scrollviewer.ViewChanging += Scrollviewer_ViewChanging;
        }

        private void Scrollviewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (this.commandbar.ClosedDisplayMode == AppBarClosedDisplayMode.Compact)
            {
                SetFullScreen(true);
            }
        }

        private void BookContentPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                if (this.commandbar.ClosedDisplayMode == AppBarClosedDisplayMode.Compact)
                {
                    this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
                }
            }

            x = 0;
        }

        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            x += e.Delta.Translation.X;//将滑动的值赋给x
        }

        private void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (x > 85)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("0");
            }
            else if (x < -85)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("1");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetFullScreen(true);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SetFullScreen(false);
        }

        private void SetFullScreen(bool value)
        {
            if (value)
            {
                ViewModel.ViewModelInstance.Instance.MainPageViewModelInstance.SetLeftControlButtonVisiablity(false);

                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsPC)
                {
                    this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                }
                else if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                {
                    this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;

                    //StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    //await statusBar.HideAsync();
                }
            }
            else
            {
                ViewModel.ViewModelInstance.Instance.MainPageViewModelInstance.SetLeftControlButtonVisiablity(true);
                this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;

                //if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                //{
                //    StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                //    await statusBar.ShowAsync();
                //}
            }
        }


        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        public static void GetVisualChildCollection<T>(DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    visualCollection.Add(child as T);
                else if (child != null)
                    GetVisualChildCollection(child, visualCollection);
            }
        }


        private void grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PlatformHelper.GetPlatform() != PlatformHelper.Platform.IsMobile)
            {
                e.Handled = true;
                return;
            }

            if (this.commandbar.ClosedDisplayMode == AppBarClosedDisplayMode.Hidden)
            {
                this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
            }
            else if (this.commandbar.ClosedDisplayMode == AppBarClosedDisplayMode.Compact)
            {
                this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
            }
        }

    }
}
