using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.Web.Http;

namespace Sodu.ViewModel
{
    public class SettingPageViewModel : BaseViewModel
    {
        [IgnoreDataMember]
        private bool IsLoading { get; set; }

        [IgnoreDataMember]
        private bool HasChanged { get; set; }

        private int m_TextFontSzie = 20;
        /// <summary>
        /// 阅读显示字体大小  14-26
        /// </summary>
        public int TextFontSzie
        {
            get
            {
                return m_TextFontSzie;
            }
            set
            {
                SetProperty(ref m_TextFontSzie, value);
                // SaveSetting();
            }
        }

        private List<int> m_FontSzieList;
        /// <summary>
        /// 阅读显示字体大小  14-26
        /// </summary>
        [IgnoreDataMember]
        public List<int> FontSzieList
        {
            get
            {
                return m_FontSzieList;
            }
            set
            {
                SetProperty(ref m_FontSzieList, value);
            }
        }

        private bool m_IfAutoLogin = true;
        /// <summary>
        /// 是否自动登陆
        /// </summary>
        public bool IfAutoLogin
        {

            get
            {
                return m_IfAutoLogin;
            }
            set
            {
                if (value == m_IfAutoLogin) return;
                SetProperty(ref m_IfAutoLogin, value);
                // SaveSetting();

            }

        }

        private bool m_IfAutAddToShelf;
        /// <summary>
        /// 是否自动添加点击的小说到个人收藏
        /// </summary>
        public bool IfAutAddToShelf
        {
            get
            {
                return m_IfAutAddToShelf;
            }
            set
            {
                SetProperty(ref m_IfAutAddToShelf, value);
                // SaveSetting();
            }

        }

        private UserCookie m_UserCookie;
        public UserCookie UserCookie
        {
            get { return m_UserCookie; }
            set
            {
                SetProperty(ref m_UserCookie, value);
                //  SaveSetting();
            }
        }

        public SettingPageViewModel()
        {
            this.FontSzieList = new List<int>()
            {
               16,18,20,22,24,26,28
            };
        }



        [IgnoreDataMember]
        public RelayCommand<object> SaveCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    SaveSetting();
                });
            }
        }

        public async void SaveSetting(bool showMessage = true)
        {
            try
            {
                if (!IsLoading)
                {
                    string fileName = ConstantValue.XmlCacheFileNameDic[typeof(SettingPageViewModel)];
                    bool result = await SerializeHelper.WriteAsync(this, fileName);
                    if (showMessage)
                    {
                        CommonMethod.ShowMessage("已保存设置更改");
                    }
                    if (HasChanged)
                    {
                        SaveSetting();
                        HasChanged = false;
                    }
                    IsLoading = false;
                }
                else
                {
                    HasChanged = true;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                HasChanged = false;
                IsLoading = false;
            }
        }


        public void SetFontSize(bool isAdd, bool isShowMessage = true)
        {
            if (isAdd)
            {
                if (this.TextFontSzie < 28)
                {

                    this.TextFontSzie += 2;
                    SaveSetting(isShowMessage);
                }

            }
            else
            {
                if (this.TextFontSzie > 16)
                {
                    this.TextFontSzie -= 2;
                    SaveSetting(isShowMessage);
                }
            }
        }
    }


    public class UserCookie
    {
        //
        // 摘要:
        //     获取 HttpCookie 对其有效的域。
        //
        // 返回结果:
        //     HttpCookie 对其有效的域。
        public System.String Domain { get; set; }
        //
        // 摘要:
        //     获取或设置 HttpCookie 的到期日期和时间。
        //
        // 返回结果:
        //     HttpCookie 的过期日期和时间。
        [IgnoreDataMember]
        public DateTimeOffset? Expires { get; set; }
        //
        // 摘要:
        //     获取或设置一个值，该值控制脚本或其他活动内容是否可访问此 HttpCookie。
        //
        // 返回结果:
        //     脚本或其他活动内容是否可以访问此 HttpCookie。如果脚本或其他活动内容无法访问此 HTTP Cookie，则为 true；否则为 false。默认为
        //     false。
        public System.Boolean HttpOnly { get; set; }
        //
        // 摘要:
        //     获取表示 HttpCookie 名称的标记。
        //
        // 返回结果:
        //     表示 HttpCookie 名称的标记。
        public System.String Name { get; set; }
        //
        // 摘要:
        //     获取应用 HttpCookie 的 URI 路径部分。
        //
        // 返回结果:
        //     应用 HttpCookie 的 URI 路径部分。
        public System.String Path { get; set; }
        //
        // 摘要:
        //     获取或设置 HttpCookie 的安全级别。
        //
        // 返回结果:
        //     HttpCookie 的安全级别。如果客户端仅在使用 HTTPS 的后续请求中返回 Cookie，则为 true；否则为 false。默认为 false。
        public System.Boolean Secure { get; set; }
        //
        // 摘要:
        //     获取或设置 HttpCookie 的值。
        //
        // 返回结果:
        //     HttpCookie 的值。
        public System.String Value { get; set; }

    }
}
