using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDu.Core.API
{
    public class URLService_CC : IURLService
    {
        /// <summary>
        /// <summary>
        /// 首页地址
        /// </summary>
        private const string HomePage = "http://www.sodu.cc/";

        /// <summary>
        /// 登录页
        /// </summary>
        private const string LoginPostPage = "http://www.sodu.cc/handler/login.html";

        /// <summary>
        /// 注销页面
        /// </summary>
        private const string LogoutPage = "http://www.sodu.cc/logout.html?callback=http://www.sodu.cc/home.html";

        //注册页面post
        private const string RegisterPostPage = "http://www.sodu.cc/handler/reg.html";

        /// <summary>
        /// 我的书架
        /// </summary>
        private const string BookShelfPage = "http://www.sodu.cc/home.html";

        /// <summary>
        /// 搜索地址
        /// </summary>
        private const string BookSearchPage = "http://www.sodu.cc/result.html?searchstr={0}";

        /// <summary>
        /// 排行榜地址
        /// </summary>
        private const string BookRankListPage = "http://www.sodu.cc/top.html";
        private const string BookRankListPage2 = "http://www.sodu.cc/top_{0}.html";


        ///添加至书架
        public const string AddToShelfPage = "http://www.sodu.cc/handler/home.html?bid={0}";


        public string GetHomePage()
        {
            return HomePage;
        }

        public string GetLoginPage()
        {
            return LoginPostPage;

        }

        public string GetLogoutPage()
        {
            return LogoutPage;

        }

        public string GetRegisterPostPage()
        {
            return RegisterPostPage;
        }

        public string GetBookShelfPage()
        {
            return BookShelfPage;
        }
        public string GetSearchPage()
        {
            return BookSearchPage;
        }

        public string GetRankListPage(string pageIndex = null)
        {
            if (pageIndex == null)
            {
                return BookRankListPage;
            }
            else
            {
                return BookRankListPage2;
            }
        }

        public string GetAddToShelfPage()
        {
            return AddToShelfPage;
        }
    }
}
