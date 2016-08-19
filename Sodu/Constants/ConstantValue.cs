using Sodu.Model;
using Sodu.Pages;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.Web.Http;

namespace Sodu.Constants
{
    public class ConstantValue
    {
        public static ObservableCollection<MenuModel> UnloadMenuList = new ObservableCollection<MenuModel>()
        {
            new MenuModel() { MenuName= "排行榜",MenuIcon="",MenuType=typeof(RankListPage)},
            new MenuModel() { MenuName= "热门小说",MenuIcon="",MenuType=typeof(HotPage) },
            new MenuModel() { MenuName= "推荐阅读",MenuIcon="",MenuType=typeof(RecommendPage) },
            //new MenuModel() { MenuName= "最新更新",MenuIcon="",MenuType=typeof(UpdateBookPage)},
            //new MenuModel() { MenuName= "注册",MenuIcon="",MenuType=typeof(RegisterPage)},
            //new MenuModel() { MenuName= "登陆",MenuIcon="",MenuType=typeof(LoginPage)},

        };

        public static ObservableCollection<MenuModel> LoadMenuList = new ObservableCollection<MenuModel>()
        {
            new MenuModel() { MenuName= "个人书架",MenuIcon="",MenuType=typeof(BookShelfPage)},
            new MenuModel() { MenuName= "排行榜",MenuIcon="",MenuType=typeof(RankListPage)},
            new MenuModel() { MenuName= "热门小说",MenuIcon="",MenuType=typeof(HotPage) },
            new MenuModel() { MenuName= "推荐阅读",MenuIcon="",MenuType=typeof(RecommendPage) },
            //new MenuModel() { MenuName= "最新更新",MenuIcon="",MenuType=typeof(UpdateBookPage)},
            //new MenuModel() { MenuName= "搜索",MenuIcon="",MenuType=typeof(SearchResultPage)},
        };


        public static List<SolidColorBrush> BackColorList = new List<SolidColorBrush>()
        {
             App.Current.Resources["BackColor1"] as SolidColorBrush,
             App.Current.Resources["BackColor2"] as SolidColorBrush,
             App.Current.Resources["BackColor3"] as SolidColorBrush,
             App.Current.Resources["BackColor4"] as SolidColorBrush,
             App.Current.Resources["BackColor5"] as SolidColorBrush,
             App.Current.Resources["BackColor6"] as SolidColorBrush,
             App.Current.Resources["BackColor7"] as SolidColorBrush,
             App.Current.Resources["BackColor8"] as SolidColorBrush,
             App.Current.Resources["BackColor9"] as SolidColorBrush,
             App.Current.Resources["BackColor10"] as SolidColorBrush,
    };

    }

}
