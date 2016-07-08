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
        }

        private void BookContentPage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
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

        private async void SetFullScreen(bool value)
        {
            if (value)
            {
                ViewModel.ViewModelInstance.Instance.MainPageViewModelInstance.SetLeftControlButtonVisiablity(false);
                this.commandbar.ClosedDisplayMode = AppBarClosedDisplayMode.Minimal;
            }
            else
            {
                ViewModel.ViewModelInstance.Instance.MainPageViewModelInstance.SetLeftControlButtonVisiablity(true);
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


        Point _startPint;
        private void listview_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _startPint = e.GetCurrentPoint(this.listview).Position;
        }



        private void listview_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            Point endPoint = e.GetCurrentPoint(this.listview).Position;


        }

        private void listview_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            Point endPoint = e.GetCurrentPoint(this.listview).Position;

            if (_startPint.X - endPoint.X > 20)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("1");
            }
            else if (endPoint.X - _startPint.X > 20)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("0");
            }
        }

        private void listview_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            Point endPoint = e.GetCurrentPoint(this.listview).Position;

            if (_startPint.X - endPoint.X > 20)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("1");
            }
            else if (endPoint.X - _startPint.X > 20)
            {
                (this.DataContext as BookContentPageViewModel).OnSwtichCommand("0");
            }
        }
    }
}
