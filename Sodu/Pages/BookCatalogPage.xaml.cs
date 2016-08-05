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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookCatalogPage : Page
    {
        public BookCatalogPage()
        {
            this.InitializeComponent();
            //this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                return;
            }
          (this.DataContext as IViewModel)?.InitData(e.Parameter);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.listview.Items != null && this.listview.Items.Count > 0)
            {
                if (this.direction.Label.Equals("回到顶部"))
                {
                    this.listview.ScrollIntoView(this.listview.Items[0]);
                    this.direction.Label = "转到底部";
                }

                else if (this.direction.Label.Equals("转到底部"))
                {
                    this.listview.ScrollIntoView(this.listview.Items[this.listview.Items.Count - 1]);
                    this.direction.Label = "回到顶部";
                }
            }
        }

    }
}
