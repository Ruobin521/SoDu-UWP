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
using Windows.Web.Http;

namespace Sodu.Constants
{
    public class ConstantValue
    {
        public static ObservableCollection<MenuModel> UnloadMenuList = new ObservableCollection<MenuModel>()
        {
            new MenuModel() { MenuName= "日点击排行榜",MenuIcon="",MenuType=typeof(RankListPage)},
            new MenuModel() { MenuName= "热门小说",MenuIcon="",MenuType=typeof(HotPage) },
            new MenuModel() { MenuName= "推荐阅读",MenuIcon="",MenuType=typeof(HomePage) },
            new MenuModel() { MenuName= "搜索",MenuIcon="",MenuType=typeof(SearchResultPage)},
            new MenuModel() { MenuName= "注册",MenuIcon="",MenuType=typeof(RegisterPage)},
            new MenuModel() { MenuName= "登陆",MenuIcon="",MenuType=typeof(LoginPage)},

        };

        public static ObservableCollection<MenuModel> LoadMenuList = new ObservableCollection<MenuModel>()
        {
            new MenuModel() { MenuName= "个人书架",MenuIcon="",MenuType=typeof(MyBookShelfPage)},
            new MenuModel() { MenuName= "日点击排行榜",MenuIcon="",MenuType=typeof(RankListPage)},
            new MenuModel() { MenuName= "热门小说",MenuIcon="",MenuType=typeof(HotPage) },
            new MenuModel() { MenuName= "推荐阅读",MenuIcon="",MenuType=typeof(HomePage) },
            new MenuModel() { MenuName= "搜索",MenuIcon="",MenuType=typeof(SearchResultPage)},
        };
    }

}
