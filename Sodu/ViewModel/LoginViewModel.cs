using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Services;
using Sodu.Util;
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

        private bool m_IsAutoLogin;
        public bool IsAutoLogin
        {
            get
            {
                return m_IsAutoLogin;
            }
            set
            {
                SetProperty(ref m_IsAutoLogin, value);
                ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutoLogin = value;
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

        public void RefreshData(object obj = null, bool IsRefresh = false)
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
                        CommonMethod.ShowMessage("用户名和密码不能为空。");
#endif
                    }
                    else
                    {
                        postdata = "username=" + this.UserName + "&userpass=" + this.Password;
                    }

                    html = await HttpHelper.WebRequestPost(PageUrl.LoginPostPage, postdata);

                }
                catch (Exception ex)
                {
                    return;
                }
                finally
                {
                    IsLoading = false;
                }

                if (html.Contains("{\"success\":true}"))
                {
                    ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(true);
                }
                else
                {
                    CommonMethod.ShowMessage("账号或密码错误，请重新输入。");
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
