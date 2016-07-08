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
using Windows.UI.Core;
using Windows.Web.Http;

namespace Sodu.ViewModel
{
    public class SettingPageViewModel : BaseViewModel
    {
        public static string n_IsAutoLogin = "IsAutoLogin";
        public static string n_IfAutAddToShelf = "IfAutAddToShelf";
        public static string n_IfDownloadInWAAN = "IfDownloadInWAAN";
        public static string n_IsFullScreen = "IsFullScreen";
        public static string n_TextFontSzie = "TextFontSzie";
        public static string n_Cookie = "Cookie";


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
                if (value == m_TextFontSzie)
                {
                    return;
                }
                SetProperty(ref m_TextFontSzie, value);
                SetTextSize(value, true);
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

                if (value == m_IfAutAddToShelf)
                {
                    return;
                }
                SetProperty(ref m_IfAutoLogin, value);
                SetAutoLogin(value, true);
            }
        }

        private bool m_IfAutAddToShelf = true;
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
                if (value == m_IfAutAddToShelf)
                {
                    return;
                }
                SetProperty(ref m_IfAutAddToShelf, value);
                SetAutoAddToShelf(value, true);
            }

        }

        private bool m_IfDownloadInWAAN = false;
        /// <summary>
        /// 是否在流量下下载小说
        /// </summary>
        public bool IfDownloadInWAAN
        {
            get
            {
                return m_IfDownloadInWAAN;
            }
            set
            {
                if (value == m_IfDownloadInWAAN)
                {
                    return;
                }
                SetProperty(ref m_IfDownloadInWAAN, value);
                SetDownLoadInWAAN(value, true);
            }

        }



        private UserCookie m_UserCookie;
        public UserCookie UserCookie
        {
            get
            {
                return m_UserCookie;
            }
            set
            {
                SetProperty(ref m_UserCookie, value);
            }
        }

        public SettingPageViewModel()
        {
            this.FontSzieList = new List<int>()
            {
               16,18,20,22,24,26,28
            };

        }


        public void InitSettingData()
        {
            //自动登录
            if (!SettingService.CheckKeyExist(n_IsAutoLogin))
            {
                SettingService.SetSetting(n_IsAutoLogin, "true");
                IfAutoLogin = true;
            }
            else
            {
                string value = SettingService.GetSetting(n_IsAutoLogin);

                if (value.Equals("true"))
                {
                    IfAutoLogin = true;
                }
                else
                {
                    IfAutoLogin = false;
                }
            }


            //自动添加书架
            if (!SettingService.CheckKeyExist(n_IfAutAddToShelf))
            {
                SettingService.SetSetting(n_IfAutAddToShelf, "true");
                IfAutAddToShelf = true;
            }
            else
            {
                string value = SettingService.GetSetting(n_IfAutAddToShelf);

                if (value.Equals("true"))
                {
                    IfAutAddToShelf = true;
                }
                else
                {
                    IfAutAddToShelf = false;
                }
            }

            //在流量下下载
            if (!SettingService.CheckKeyExist(n_IfDownloadInWAAN))
            {
                SettingService.SetSetting(n_IfDownloadInWAAN, "false");
                IfDownloadInWAAN = false;
            }
            else
            {
                string value = SettingService.GetSetting(n_IfDownloadInWAAN);
                if (value.Equals("true"))
                {
                    IfDownloadInWAAN = true;
                }
                else
                {
                    IfDownloadInWAAN = false;
                }
            }


            //正文字体大小
            if (!SettingService.CheckKeyExist(n_TextFontSzie))
            {
                SettingService.SetSetting(n_TextFontSzie, "20");
                TextFontSzie = 20;
            }
            else
            {
                string value = SettingService.GetSetting(n_TextFontSzie);
                TextFontSzie = Convert.ToInt32(value);
            }
        }


        public void SetAutoLogin(bool value, bool isShowMessage = true)
        {
            if (value)
            {
                SettingService.SetSetting(n_IsAutoLogin, "true");
            }
            else
            {
                SettingService.SetSetting(n_IsAutoLogin, "false");
            }
            IfAutoLogin = value;
        }

        public void SetAutoAddToShelf(bool value, bool isShowMessage = false)
        {
            if (value)
            {
                SettingService.SetSetting(n_IfAutAddToShelf, "true");
            }
            else
            {
                SettingService.SetSetting(n_IfAutAddToShelf, "false");
            }
            IfAutAddToShelf = value;

        }

        public void SetDownLoadInWAAN(bool value, bool isShowMessage = true)
        {
            if (value)
            {
                SettingService.SetSetting(n_IfDownloadInWAAN, "true");
            }
            else
            {
                SettingService.SetSetting(n_IfDownloadInWAAN, "false");
            }

            IfDownloadInWAAN = value;

        }


        public void SetTextSize(int value, bool isShowMessage = false)
        {
            SettingService.SetSetting(n_TextFontSzie, value);
            TextFontSzie = value;
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

        private void SaveSetting()
        {
            throw new NotImplementedException();
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
