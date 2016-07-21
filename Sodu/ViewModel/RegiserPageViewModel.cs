using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Util;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Sodu.ViewModel
{
    public class RegiserPageViewModel : BaseViewModel, IViewModel
    {


        private string _ContentTitle = "用户注册";
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

        //private BitmapImage m_ImageSource = new BitmapImage() { CreateOptions = BitmapCreateOptions.IgnoreImageCache, UriSource = new Uri(PageUrl.VerificationCodePage, UriKind.Absolute) };
        //public BitmapImage ImageSource
        //{
        //    get
        //    {
        //        return m_ImageSource;
        //    }
        //    set
        //    {
        //        SetProperty(ref this.m_ImageSource, value);
        //    }
        //}

        private string m_UserName;
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                SetProperty(ref this.m_UserName, value);
            }
        }

        private string m_PassWord;
        public string PassWord
        {
            get
            {
                return m_PassWord;
            }
            set
            {
                SetProperty(ref this.m_PassWord, value);
            }
        }
        private string m_PassWord2;
        public string PassWord2
        {
            get
            {
                return m_PassWord2;
            }
            set
            {
                SetProperty(ref this.m_PassWord2, value);
            }
        }

        HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void InitData(object obj = null)
        {

        }

        private async void RegisterMethod()
        {
            try
            {
                //  如果正在加载，或者提示不需要刷新 
                if (IsLoading)
                {
                    return;
                }
                IsLoading = true;
                string html = null;
                await Task.Run(async () =>
                 {
                     string uri = ViewModelInstance.Instance.UrlService.GetRegisterPostPage();
                     string postData = "username=" + ChineseGBKConverter.Utf8ToGb2312(UserName) + "&userpass=" + PassWord;
                     html = await http.HttpClientPostRequest(uri, postData);
                 });
                IsLoading = false;
                if (html.Contains("{\"success\":true}"))
                {
                    ToastHeplper.ShowMessage("注册成功");
                    ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(true);
                }
                else if (html.Contains("{\"success\":false}"))
                {
                    throw new Exception("注册失败，该用户可能已经注册过");
                }
                else
                {
                    throw new Exception("注册失败");
                }

            }
            catch (Exception ex)
            {
                ToastHeplper.ShowMessage(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }


        ///点击确定
        /// </summary>
        private RelayCommand<object> m_ConfirmCommand;
        public RelayCommand<object> ConfirmCommand
        {
            get
            {
                return m_ConfirmCommand ?? (m_ConfirmCommand = new RelayCommand<object>(OnConfirmCommand));
            }
        }
        private void OnConfirmCommand(object obj)
        {
            if (CheckInput())
            {
                RegisterMethod();
            }
        }

        private bool CheckInput()
        {
            bool result = true;

            if (string.IsNullOrEmpty(this.UserName))
            {
                ToastHeplper.ShowMessage("请输入用户名");
                return false;
            }

            if (string.IsNullOrEmpty(this.PassWord))
            {
                ToastHeplper.ShowMessage("请输入密码");
                return false;

            }

            if (string.IsNullOrEmpty(this.PassWord2))
            {
                ToastHeplper.ShowMessage("请输入确认密码");
                return false;

            }
            if (!this.PassWord2.Equals(this.PassWord))
            {
                ToastHeplper.ShowMessage("两次密码不一致");
                return false;
            }
            return result;
        }

        ///刷新验证码
        /// </summary>
        private RelayCommand<object> m_CancleCommand;
        public RelayCommand<object> CancleCommand
        {
            get
            {
                return m_CancleCommand ?? (m_CancleCommand = new RelayCommand<object>(OnCancleCommand));
            }
        }
        private void OnCancleCommand(object obj)
        {
            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}
