using Sodu.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class MyBookShelfPage : Page
    {
        public MyBookShelfPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if ((this.DataContext as ViewModel.BookShelfPageViewModel).IsEditing)
            {
                (this.DataContext as ViewModel.BookShelfPageViewModel).OnEditCommand();
            }
        }

        //protected async override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    //ObservableCollection<BookEntity> temp = new ObservableCollection<BookEntity>();
        //    //foreach (var item in (this.DataContext as ViewModel.BookShelfPageViewModel).ShelfBookList)
        //    //{
        //    //    temp.Add(item);
        //    //}

        //    //(this.DataContext as ViewModel.BookShelfPageViewModel).ShelfBookList.Clear();

        //    //foreach (var item in temp)
        //    //{
        //    //    (this.DataContext as ViewModel.BookShelfPageViewModel).ShelfBookList.Add(item);
        //    //    await Task.Delay(TimeSpan.FromSeconds(0.001));
        //    //}

        //    this.DataContext = ViewModel.ViewModelInstance.Instance.MyBookShelfViewModelInstance;
        //}
    }
}
