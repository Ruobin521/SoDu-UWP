﻿using Sodu.Core.Util;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.Devices.Power;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Sodu.Services;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookContentPage : Page
    {
        double x = 0;//用来接收手势水平滑动的长度

        //电池信息
        private Battery _battery = null;

        public BookContentPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;


            ManipulationCompleted += The_ManipulationCompleted;//订阅手势滑动结束后的事件
            ManipulationStarted += BookContentPage_ManipulationStarted;   //订阅手势滑动结束后的事件
            ManipulationDelta += The_ManipulationDelta;//订阅手势滑动事件

            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                this.ColorPanel.Closed -= ColorPanel_Closed;
                this.ColorPanel.Closed += ColorPanel_Closed;

                BattaryStatus.Visibility = Visibility.Visible;
                _battery = Battery.GetDefault();
                _battery.RemainingChargePercentChanged += _battery_RemainingChargePercentChanged;
                this.TextBattary.Text = string.Format("{0} %", _battery.RemainingChargePercent);
            }
            else
            {
                BattaryStatus.Visibility = Visibility.Collapsed;
            }

            this.Loaded -= BookContentPage_Loaded;
            this.Loaded += BookContentPage_Loaded;


        }

        private void ColorPanel_Closed()
        {
            SetCommandBarMode(false);
        }

        private void BookContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            MenuOpiton(false);

            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                var scrollviewer = GetVisualChildCollection<ScrollViewer>(this.listview)[0];
                scrollviewer.ViewChanging -= Scrollviewer_ViewChanging;
                scrollviewer.ViewChanging += Scrollviewer_ViewChanging;
            }
        }

        private void _battery_RemainingChargePercentChanged(object sender, object e)
        {

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.TextBattary.Text = string.Format("{0} %", _battery.RemainingChargePercent);
            }
                );

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            (this.DataContext as IViewModel)?.InitData(e.Parameter);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            MenuOpiton(false);
        }


        private void Scrollviewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            MenuOpiton(false);
        }

        private void BookContentPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                MenuOpiton(false);
            }

            x = 0;
        }

        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            x += e.Delta.Translation.X;//将滑动的值赋给x
        }

        private void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //上一章
            if (x > 85)
            {
                (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("0");
            }
            // 下一章
            else if (x < -85)
            {
                (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("1");
            }
        }

        //public int GetPerLineCount()
        //{
        //    var width = this.txtTest.ActualWidth;
        //    int count1 = (int)(this.ContentGrid.ActualWidth / width);
        //    return count1;
        //}

        //public int GetTotalLineCount()
        //{
        //    int height = 36;
        //    int count2 = (int)(this.ContentGrid.ActualHeight / height);
        //    return count2;
        //}


        //public double GetTxtTestActualHeight()
        //{
        //    return this.txtTest.ActualHeight;
        //}


        public double GetContentAreatActualHeight()
        {
            return this.ContentGrid.ActualHeight;
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
            e.Handled = true;

            var point = e.GetPosition(this.ContentGrid);
            if (point.X > this.ContentGrid.ActualWidth / 3 && point.X < this.ContentGrid.ActualWidth / 3 * 2)
            {
                MenuOpiton(this.ColorPanel.Visibility != Visibility.Visible);
            }

            //上一章
            else if (point.X <= this.ContentGrid.ActualWidth / 3)
            {
                if (this.ColorPanel.Visibility == Visibility.Visible)
                {
                    MenuOpiton(false);
                    return;
                }
                (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("0");
            }
            else if (point.X >= this.ContentGrid.ActualWidth / 3 * 2)
            {
                if (this.ColorPanel.Visibility == Visibility.Visible)
                {
                    MenuOpiton(false);
                    return;
                }
               (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("1");
            }


        }

        private void MenuOpiton(bool value)
        {
            if (!value)
            {
                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                {
                    SetCommandBarMode(false);
                }
                this.ColorPanel.Close();
            }
            else
            {
                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                {
                    SetCommandBarMode(true);
                }
                this.ColorPanel.Show();
            }
        }


        private void SetCommandBarMode(bool value)
        {
            this.commandbar.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MenuOpiton(this.ColorPanel.Visibility != Visibility.Visible);
        }

        private void AppBar_Click(object sender, RoutedEventArgs e)
        {
            MenuOpiton(false);
        }
    }
}
