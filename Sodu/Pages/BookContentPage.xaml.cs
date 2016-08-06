using Sodu.Core.Util;
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
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
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
            }

            this.Loaded -= BookContentPage_Loaded;
            this.Loaded += BookContentPage_Loaded;

        }

        private void ColorPanel_Closed()
        {
            this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
        }

        private void BookContentPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                var scrollviewer = GetVisualChildCollection<ScrollViewer>(this.listview)[0];
                scrollviewer.ViewChanging -= Scrollviewer_ViewChanging;
                scrollviewer.ViewChanging += Scrollviewer_ViewChanging;
            }
        }

        private void Scrollviewer_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (this.commandbar.ClosedDisplayMode == AppBarClosedDisplayMode.Compact)
            {
                this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
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
                (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("0");
            }
            else if (x < -85)
            {
                (this.DataContext as BookContentPageViewModel)?.OnSwtichCommand("1");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
            (this.DataContext as IViewModel)?.InitData(e.Parameter);
        }

        public int GetPerPageCount()
        {
            var width = this.txtTest.ActualWidth;
            int height = 36;
            int count1 = (int)(this.ContentGrid.ActualWidth / width);
            int count2 = (int)(this.ContentGrid.ActualHeight / height);
            return count1 * count2;
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
            if (this.ColorPanel.Visibility == Visibility.Visible)
            {
                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                {
                    this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Hidden;
                }
                this.ColorPanel.Close();
            }
            else
            {
                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
                {
                    this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Compact;
                }

                this.ColorPanel.Show();
            }
        }

    }
}
