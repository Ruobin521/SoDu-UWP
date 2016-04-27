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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookContentPage : Page
    {
        public BookContentPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;

        }
        public static List<T> GetVisualChildCollection<T>(object parent) where T : UIElement
        {
            List<T> visualCollection = new List<T>();
            GetVisualChildCollection(parent as DependencyObject, visualCollection);
            return visualCollection;
        }

        private void directionAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrllviewer = GetVisualChildCollection<ScrollViewer>(this.listview).FirstOrDefault();

            if (scrllviewer == null)

                return;

            if (this.listview.Items != null && this.listview.Items.Count > 0)
            {
                if (this.direction.Label.Equals("回到顶部"))
                {
                    scrllviewer.ScrollToVerticalOffset(0);
                    this.direction.Label = "转到底部";
                }

                else if (this.direction.Label.Equals("转到底部"))
                {
                    //this.listview.ScrollIntoView(this.listview.Items[this.listview.Items.Count - 1]);
                    scrllviewer.ScrollToVerticalOffset(scrllviewer.ScrollableHeight);
                    this.direction.Label = "回到顶部";
                }
            }
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


        //private void txtContent_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        //{
        //    this.scrollViewer.ScrollToVerticalOffset(0);
        //}
    }
}
