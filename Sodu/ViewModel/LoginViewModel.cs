using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Sodu.ViewModel
{
    public class LoginViewModel : BaseViewModel, IViewModel
    {
        #region  属性


        private string _ContentTitle = "用户登录";
        public string ContentTitle
        {
            get
            {
                return _ContentTitle;

            }

            set
            {
                SetProperty(ref _ContentTitle, value);
            }
        }

        private string m_Password;
        public string Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                SetProperty(ref m_Password, value);
            }
        }

        private string m_UserName;
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                SetProperty(ref m_UserName, value);
            }
        }

        private bool m_IsLoading;
        public bool IsLoading
        {
            get
            {
                return m_IsLoading;
            }
            set
            {
                SetProperty(ref m_IsLoading, value);
            }
        }

        private bool m_IsChecked;
        public bool IsChecked
        {
            get
            {
                return m_IsChecked;
            }
            set
            {
                SetProperty(ref m_IsChecked, value);
            }
        }


        private bool m_IsAutoLogin = true;
        public bool IsAutoLogin
        {
            get
            {
                return m_IsAutoLogin;
            }
            set
            {
                SetProperty(ref m_IsAutoLogin, value);
                ViewModelInstance.Instance.SettingPageViewModelInstance.SetAutoLogin(value);
            }
        }



        public HttpHelper HttpHelper = new HttpHelper();

        #endregion

        #region  构造函数
        public LoginViewModel()
        {

        }
        #endregion


        #region  方法

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            HttpHelper.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void SetCookie(string url, bool ifAutoLogin)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(url));
            foreach (var cookieItem in cookieCollection)
            {
                if (cookieItem.Name == "sodu_user")
                {
                    if (ifAutoLogin)
                    {
                        ///设置cookie存活时间，如果为null，则表示只在一个会话中生效。
                        cookieItem.Expires = new DateTimeOffset(DateTime.Now.AddDays(365));
                        // ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.UserCookie =
                        //     new ViewModel.UserCookie()
                        //     {
                        //         Expires = cookieItem.Expires,
                        //         Value = cookieItem.Value,
                        //         Name = cookieItem.Name,
                        //         Path = cookieItem.Path,
                        //         Domain = cookieItem.Domain,
                        //     };
                        //ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.SaveSetting();
                    }
                    else
                    {
                        cookieItem.Expires = null;
                    }
                    filter.CookieManager.SetCookie(cookieItem, false);
                }
            }
        }

        public void InitData(object obj = null)
        {

        }
        #endregion


        #region  命令&命令方法

        /// <summary>
        /// 
        /// </summary>
        public RelayCommand<object> LoginCommand
        {
            get
            {
                return new RelayCommand<object>(OnLoginCommand);
            }
        }

        private async void OnLoginCommand(object obj)
        {
            if (IsLoading)
            {
                return;
            }
            else
            {
                IsLoading = true;
                string postdata = string.Empty;
                string html = string.Empty;
                try
                {

                    if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.Password))
                    {
#if DEBUG
                        postdata = "username=918201&userpass=8166450";
#endif

#if  !DEBUG
                        ToastHeplper.ShowMessage("用户名和密码不能为空。");
                        return;
#endif
                    }
                    else
                    {
                        postdata = "username=" + this.UserName + "&userpass=" + this.Password;
                    }

                    html = await HttpHelper.HttpClientPostRequest(ViewModelInstance.Instance.UrlService.GetLoginPage(), postdata);
                    if (html.Contains("{\"success\":true}"))
                    {
                        ToastHeplper.ShowMessage("登陆成功");

                        ViewModelInstance.Instance.SettingPageViewModelInstance.SetAutoLogin(IsAutoLogin);
                        SetCookie(ViewModelInstance.Instance.UrlService.GetLoginPage(), IsAutoLogin);

                        ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(true);
                    }
                    else
                    {
                        ToastHeplper.ShowMessage("账号或密码错误，请重新输入。");
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        public RelayCommand<object> CancleCommand
        {
            get
            {
                return new RelayCommand<object>(OnCancleCommand);
            }
        }
        private void OnCancleCommand(object obj)
        {
            CancleHttpRequest();
        }


        #endregion


    }
}
