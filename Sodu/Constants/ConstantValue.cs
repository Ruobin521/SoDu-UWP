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
            //new MenuModel() { MenuName= "阅读记录",MenuIcon="",MenuType=typeof(EverReadPage)},
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
            //new MenuModel() { MenuName= "阅读记录",MenuIcon="",MenuType=typeof(EverReadPage)},
            new MenuModel() { MenuName= "搜索",MenuIcon="",MenuType=typeof(SearchResultPage)},
        };
    }

    public class PageUrl
    {
        /// <summary>
        /// <summary>
        /// 首页地址
        /// </summary>
        //public const string HomePage = "http://www.soduso.com";
        public const string HomePage = "http://www.sodu.cc/";

        /// <summary>
        /// 登录页
        /// </summary>
        //public const string LoginPostPage = "http://www.soduso.com/checklogin.ashx";
        public const string LoginPostPage = "http://www.sodu.cc/handler/login.html";

        /// <summary>
        /// 注销页面
        /// </summary>
        //public const string LogoutPage = "http://www.soduso.com/logout.ashx";
        public const string LogoutPage = "http://www.sodu.cc/logout.html?callback=http://www.sodu.cc/home.html";


        //注册页面post
        //public const string RegisterPostPage = "http://www.soduso.com/reg.ashx ";
        public const string RegisterPostPage = "http://www.sodu.cc/handler/reg.html";

        /// <summary>
        /// 验证码地址
        /// </summary>
        public const string VerificationCodePage = "http://www.soduso.com/verifyCode.ashx";


        /// <summary>
        /// 我的书架
        /// </summary>
        //public const string BookShelfPage = "http://www.soduso.com/myshujia.aspx";
        public const string BookShelfPage = "http://www.sodu.cc/home.html";

        /// <summary>
        /// 搜索地址
        /// </summary>
       // public const string BookSearchPage = "http://www.soduso.com/search/index.aspx?key={0}&page={1}";
        public const string BookSearchPage = "http://www.sodu.cc/result.html?searchstr={0}";

        /// <summary>
        /// 排行榜地址
        /// </summary>
        //public const string BookRankListPage = "http://www.soduso.com/top_{0}.html";
        public const string BookRankListPage = "http://www.sodu.cc/top.html";
        public const string BookRankListPage2 = "http://www.sodu.cc/top_{0}.html";

        /// <summary>
        /// 看过的书
        /// </summary>
        public const string EverReadPage = "http://www.soduso.com/mybook.aspx?how=1";

        /// <summary>
        /// 最新更新地址
        /// </summary>
        public const string UpdatetPage = "http://www.soduso.com/more_{0}.html";

        // <summary>
        /// 移除书架
        /// </summary>
        //public const string RemoveBooktPage = "http://www.soduso.com/shujia.aspx?action=yongjiudel";

        ///添加至书架
        public const string AddToShelfPage = "http://www.sodu.cc/handler/home.html?bid={0}";

    }

    public class SeetingData
    {

        public static string IsAutoLogin = "IsAutoLogin";


    }


}
