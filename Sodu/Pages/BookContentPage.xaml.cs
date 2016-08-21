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
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
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
using Sodu.UC;
using SoDu.Core.Extension;

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
        private DispatcherTimer timer;


        private object para = null;
        private NavigationMode mode;

        private CompositeTransform _currentContentPageTransform;
        public BookContentPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.Loaded -= BookContentPage_Loaded;
            this.Loaded += BookContentPage_Loaded;

            this.SizeChanged -= BookContentPage_SizeChanged;
            this.SizeChanged += BookContentPage_SizeChanged;

            //this.ContentGrid.KeyUp -= BookContentPage_KeyUp;
            //this.ContentGrid.KeyUp += BookContentPage_KeyUp;

            ManipulationCompleted += The_ManipulationCompleted;//订阅手势滑动结束后的事件
            ManipulationStarted += BookContentPage_ManipulationStarted;   //订阅手势滑动结束后的事件
            ManipulationDelta += The_ManipulationDelta;//订阅手势滑动事件

            commandbar.Visibility = Visibility.Collapsed;
            this.ColorPanel.Closed -= ColorPanel_Closed;
            this.ColorPanel.Closed += ColorPanel_Closed;

            this.ColorPanel.FontSizeChanged -= ColorPanel_FontSizeChanged;
            this.ColorPanel.FontSizeChanged += ColorPanel_FontSizeChanged;

            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                InitBattery();
                this.grid.Holding -= this.Grid_OnHolding;
                this.grid.Holding += this.Grid_OnHolding;
            }
            else
            {
                BattaryStatus.Visibility = Visibility.Collapsed;
                this.grid.RightTapped -= this.Grid_OnRightTapped;
                this.grid.RightTapped += this.Grid_OnRightTapped;
            }

            InitTimer();
        }

        private void BookContentPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = ((this.DataContext as IViewModel) as BookContentPageViewModel)?.SetSize(ContentGrid.ActualWidth, ContentGrid.ActualHeight, txtTest.ActualWidth / 4, txtTest.ActualHeight);
            var hasChnaged = size != null && (bool)size;
            if (hasChnaged)
            {
                ((this.DataContext as IViewModel) as BookContentPageViewModel)?.SetContentPage();
            }

        }

        private async void ColorPanel_FontSizeChanged(double value)
        {
            this.txtTest.FontSize = value;
            await Task.Delay(10);
            BookContentPage_SizeChanged(null, null);
        }
        private void InitBattery()
        {
            BattaryStatus.Visibility = Visibility.Visible;
            _battery = Battery.GetDefault();
            _battery.RemainingChargePercentChanged += _battery_RemainingChargePercentChanged;
            this.TextBattery.Text = string.Format("{0}", _battery.RemainingChargePercent);
        }

        private void InitTimer()
        {
            this.TextTime.Text = DateTime.Now.ToString("HH:mm");
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, object e)
        {
            this.TextTime.Text = DateTime.Now.ToString("HH:mm");
        }

        private void ColorPanel_Closed()
        {
            SetCommandBarMode(false);
        }

        private void BookContentPage_Loaded(object sender, RoutedEventArgs e)
        {

            // this.ContentGrid.Focus(FocusState.Pointer);
            MenuOpiton(false);

            if (mode == NavigationMode.New)
            {
                (this.DataContext as IViewModel)?.InitData(para);
            }

            ((this.DataContext as IViewModel) as BookContentPageViewModel)?.SetSize(ContentGrid.ActualWidth, ContentGrid.ActualHeight, txtTest.ActualWidth / 4, txtTest.ActualHeight);


        }

        private void _battery_RemainingChargePercentChanged(object sender, object e)
        {

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.TextBattery.Text = string.Format("{0}", _battery.RemainingChargePercent);
            }
                );

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.mode = e.NavigationMode;
            this.para = e.Parameter;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            MenuOpiton(false);
            (this.DataContext as BookContentPageViewModel)?.CancleHttpRequest();
        }


        private void BookContentPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var vm = this.DataContext as BookContentPageViewModel;
            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                MenuOpiton(false);
            }
            x = 0;


        }

        private bool CanNextSwithcPage()
        {
            var vm = this.DataContext as BookContentPageViewModel;
            if (vm.CurrentPagIndex == vm.TotalPagCount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CanPreSwithcPage()
        {
            var vm = this.DataContext as BookContentPageViewModel;
            if (vm.CurrentPagIndex == 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void The_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var vm = this.DataContext as BookContentPageViewModel;

            if (vm.IsLoading)
            {
                return;
            }

            x += e.Delta.Translation.X;
        }

        private void The_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //上一章
            if (x > 85)
            {
                OnSwitch("0");

            }
            // 下一章
            else if (x < -85)
            {
                OnSwitch("1");
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



        private void MenuOpiton(bool value)
        {
            if (!value)
            {
                if (this.ColorPanel.Visibility != Visibility.Visible) return;

                SetCommandBarMode(false);
                this.ColorPanel.Close();
            }
            else
            {
                if (this.ColorPanel.Visibility != Visibility.Collapsed) return;

                SetCommandBarMode(true);
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

        private void Grid_OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            e.Handled = true;
            if (e.HoldingState == HoldingState.Started)
            {
                MenuOpiton(this.ColorPanel.Visibility != Visibility.Visible);
            }
        }

        private void Grid_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            e.Handled = true;
            MenuOpiton(this.ColorPanel.Visibility != Visibility.Visible);
        }


        private void Grid_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            e.Handled = true;
            var point = e.GetPosition(this.ContentGrid);
            if (this.ColorPanel.Visibility == Visibility.Visible)
            {
                MenuOpiton(false);
                return;
            }
            //上一章
            if (point.X > this.ContentGrid.ActualWidth / 3 && point.X < this.ContentGrid.ActualWidth / 3 * 2)
            {
                MenuOpiton(true);
            }
            else
            {
                OnTapped(point);
            }
        }

        private void grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            e.Handled = true;
            var point = e.GetPosition(this.ContentGrid);

            if (this.ColorPanel.Visibility == Visibility.Visible)
            {
                MenuOpiton(false);
                return;
            }

            OnTapped(point);
        }

        private void OnTapped(Point point)
        {
            //上一章
            if (point.X < this.ContentGrid.ActualWidth / 3)
            {
                OnSwitch("0");

            }
            //下一章
            else if (point.X >= this.ContentGrid.ActualWidth / 3 * 2)
            {
                OnSwitch("1");
            }
        }


        private void OnSwitch(string option, bool animation = false)
        {
            var vm = this.DataContext as BookContentPageViewModel;

            if (vm.IsLoading) return;

            if (option.Equals("0"))
            {
                if (vm.CurrentPagIndex == 1 || !ViewModelInstance.Instance.SettingPageViewModelInstance.IsReadByPageMode)
                {
                    vm.OnSwtichCommand("0");
                }
                else if (vm.CurrentPagIndex > 1)
                {
                    vm.CurrentPagIndex = vm.CurrentPagIndex - 1;

                    vm.NextPageContent = vm.CurrentPageContent;

                    vm.CurrentPageContent = vm.ContentPages[vm.CurrentPagIndex - 1];

                    vm.PrePageContent = vm.CurrentPagIndex >= 2 ? vm.ContentPages[vm.CurrentPagIndex - 2] : null;

                    if (ViewModelInstance.Instance.SettingPageViewModelInstance.SwitchAnimation)
                    {
                        this.NextPage.Text = (this.DataContext as BookContentPageViewModel).NextPageContent;
                        this.NextPage.StartToRight();
                    }
                }
            }
            else if (option.Equals("1"))
            {
                if (vm.CurrentPagIndex == vm.ContentPages.Count ||
                  !ViewModelInstance.Instance.SettingPageViewModelInstance.IsReadByPageMode)
                {
                    vm.OnSwtichCommand("1");
                }

                else if (vm.CurrentPagIndex < vm.ContentPages.Count)
                {

                    vm.CurrentPagIndex = vm.CurrentPagIndex + 1;

                    vm.PrePageContent = vm.CurrentPageContent;

                    vm.CurrentPageContent = vm.ContentPages[vm.CurrentPagIndex - 1];

                    vm.NextPageContent = vm.CurrentPagIndex < vm.ContentPages.Count
                        ? vm.ContentPages[vm.CurrentPagIndex]
                        : null;


                    if (ViewModelInstance.Instance.SettingPageViewModelInstance.SwitchAnimation)
                    {
                        this.NextPage.Text = (this.DataContext as BookContentPageViewModel).PrePageContent;
                        this.NextPage.StartToLeft();
                    }

                }
            }

        }


    }
}
