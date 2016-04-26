using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Services;
using Sodu.Util;
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
        public bool IsNeedRefresh { get; set; } = true;

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

        public bool IsLoading
        {
            get; set;
        }

        private BitmapImage m_ImageSource = new BitmapImage() { CreateOptions = BitmapCreateOptions.IgnoreImageCache, UriSource = new Uri(PageUrl.VerificationCodePage, UriKind.Absolute) };
        public BitmapImage ImageSource
        {
            get
            {
                return m_ImageSource;
            }
            set
            {
                SetProperty(ref this.m_ImageSource, value);
            }
        }

        private string m_UserName = "aaa";
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

        private string m_PassWord = "123456";
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
        private string m_EmailAddress = "123@qq.com";
        public string EmailAddress
        {
            get
            {
                return m_EmailAddress;
            }
            set
            {
                SetProperty(ref this.m_EmailAddress, value);
            }
        }

        private string m_VerificationCode;
        public string VerificationCode
        {
            get
            {
                return m_VerificationCode;
            }
            set
            {
                SetProperty(ref this.m_VerificationCode, value);
            }
        }

        HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void RefreshData(object obj = null, bool IsRefresh = false)
        {
            IsLoading = false;
        }


        async void RegisterMethod()
        {
            try
            {
                //  如果正在加载，或者提示不需要刷新 
                if (IsLoading)
                {
                    return;
                }
                string uri = PageUrl.RegisterPostPage;
                string postData = "username=" + ChineseGBKConverter.Utf8ToGb2312(UserName) + "&password1=" + PassWord + "&email=" + EmailAddress + "&yzm=" + VerificationCode + "&B1=%C8%B7%C8%CF%D7%A2%B2%E1&postcheck=true";
                string html = await http.HttpClientPostRequest(uri, postData);
                if (html.Contains("验证码输入错误，请重新注册"))
                {
                    CommonMethod.ShowMessage("验证码输入错误，请重新注册");
                }

                else if (html.Contains("您已经注册过了"))
                {
                    CommonMethod.ShowMessage("该账户已存在！");
                }
                else if (html.Contains("注册成功"))
                {
                    CommonMethod.ShowMessage("注册成功,请登录");
                    ViewModelInstance.Instance.MainPageViewModelInstance.SetCurrentMenu("登陆");
                }
                else
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("获取数据有误，请重新尝试");

            }
            finally
            {
                IsLoading = false;
            }
        }


        ///点击确定
        /// </summary>
        public RelayCommand<object> ConfirmCommand
        {
            get
            {
                return new RelayCommand<object>(OnConfirmCommand);
            }
        }
        private void OnConfirmCommand(object obj)
        {
            if (string.IsNullOrEmpty(this.UserName))
            {
                CommonMethod.ShowMessage("请输入用户名");
                return;
            }

            if (string.IsNullOrEmpty(this.PassWord))
            {
                CommonMethod.ShowMessage("请输入密码");
                return;
            }

            if (string.IsNullOrEmpty(this.EmailAddress))
            {
                CommonMethod.ShowMessage("请输入邮箱地址");
                return;
            }

            if (string.IsNullOrEmpty(this.VerificationCode))
            {
                CommonMethod.ShowMessage("请输入验证码");
                return;
            }

            RegisterMethod();
        }

        ///刷新验证码
        /// </summary>
        public RelayCommand<object> RefreshVCCommand
        {
            get
            {
                return new RelayCommand<object>(OnRefreshVCCommand);
            }
        }
        private void OnRefreshVCCommand(object obj)
        {
            this.ImageSource = null;
            this.ImageSource = new BitmapImage() { CreateOptions = BitmapCreateOptions.IgnoreImageCache, UriSource = new Uri(PageUrl.VerificationCodePage, UriKind.Absolute) };
        }
    }
}
