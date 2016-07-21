using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.SettingHelper;
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
using Windows.Graphics.Display;
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
        public static string n_IsNightModel = "IsNightModel";
        public static string n_IsLandscape = "IsLandscape";


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

                if (value == m_IfAutoLogin)
                {
                    return;
                }
                SetProperty(ref m_IfAutoLogin, value);
                SetAutoLogin(value);
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

        private bool m_IsNightModel = false;
        /// <summary>
        /// 是否开启夜晚模式
        /// </summary>
        public bool IsNightModel
        {

            get
            {
                return m_IsNightModel;
            }
            set
            {

                if (value == m_IsNightModel)
                {
                    return;
                }
                SetProperty(ref m_IsNightModel, value);
                SetNightMode(value);
            }
        }

        private bool m_IsLandscape = false;
        /// <summary>
        /// 是否开启横向模式
        /// </summary>
        public bool IsLandscape
        {

            get
            {
                return m_IsLandscape;
            }
            set
            {

                if (value == m_IsLandscape)
                {
                    return;
                }
                SetProperty(ref m_IsLandscape, value);
                SetLandscape(value);
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
            if (!SettingHelper.CheckKeyExist(n_IsAutoLogin))
            {
                SettingHelper.SetValue(n_IsAutoLogin, true);
                IfAutoLogin = true;
            }
            else
            {
                var value = (bool)SettingHelper.GetValue(n_IsAutoLogin);

                if (value)
                {
                    IfAutoLogin = true;
                }
                else
                {
                    IfAutoLogin = false;
                }
            }


            //自动添加书架
            if (!SettingHelper.CheckKeyExist(n_IfAutAddToShelf))
            {
                SettingHelper.SetValue(n_IfAutAddToShelf, true);
                IfAutAddToShelf = true;
            }
            else
            {
                var value = (bool)SettingHelper.GetValue(n_IfAutAddToShelf);

                if (value)
                {
                    IfAutAddToShelf = true;
                }
                else
                {
                    IfAutAddToShelf = false;
                }
            }


            //在流量下下载
            if (!SettingHelper.CheckKeyExist(n_IfDownloadInWAAN))
            {
                SettingHelper.SetValue(n_IfDownloadInWAAN, false);
                IfDownloadInWAAN = false;
            }
            else
            {
                var value = (bool)SettingHelper.GetValue(n_IfDownloadInWAAN);
                if (value)
                {
                    IfDownloadInWAAN = true;
                }
                else
                {
                    IfDownloadInWAAN = false;
                }
            }


            //正文字体大小
            if (!SettingHelper.CheckKeyExist(n_TextFontSzie))
            {
                SettingHelper.SetValue(n_TextFontSzie, "20");
                TextFontSzie = 20;
            }
            else
            {
                string value = SettingHelper.GetValue(n_TextFontSzie).ToString();
                int size = Convert.ToInt32(value);
                if (size % 2 != 0)
                {
                    size = size - 1;
                    SettingHelper.SetValue(n_TextFontSzie, size);

                }

                TextFontSzie = size;
            }

            //设置夜间模式
            if (!SettingHelper.CheckKeyExist(n_IsNightModel))
            {
                SettingHelper.SetValue(n_IsNightModel, false);
                IsNightModel = false;
            }
            else
            {
                var value = (bool)SettingHelper.GetValue(n_IsNightModel);
                IsNightModel = value;
            }


            //设置横向模式
            if (!SettingHelper.CheckKeyExist(n_IsLandscape))
            {
                SettingHelper.SetValue(n_IsLandscape, false);
                IsLandscape = false;
            }
            else
            {
                var value = (bool)SettingHelper.GetValue(n_IsLandscape);
                IsLandscape = value;
            }
            SetLandscape(IsLandscape);

        }


        public void SetAutoLogin(bool value)
        {
            if (value)
            {
                SettingHelper.SetValue(n_IsAutoLogin, true);
            }
            else
            {
                SettingHelper.SetValue(n_IsAutoLogin, false);
            }
            IfAutoLogin = value;
        }

        public void SetAutoAddToShelf(bool value, bool isShowMessage = false)
        {
            if (value)
            {
                SettingHelper.SetValue(n_IfAutAddToShelf, true);
            }
            else
            {
                SettingHelper.SetValue(n_IfAutAddToShelf, false);
            }
            IfAutAddToShelf = value;
        }

        public void SetDownLoadInWAAN(bool value, bool isShowMessage = true)
        {
            SettingHelper.SetValue(n_IfDownloadInWAAN, value);
            IfDownloadInWAAN = value;
        }

        public void SetNightMode(bool value)
        {
            SettingHelper.SetValue(n_IsNightModel, value);
            IsNightModel = value;
        }

        public void SetLandscape(bool value)
        {
            SettingHelper.SetValue(n_IsLandscape, value);
            IsLandscape = value;
            if (IsLandscape)
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            else
            {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }
        }


        public void SetTextSize(int value, bool isShowMessage = false)
        {
            if (value > 28 || value < 16)
            {
                return;
            }
            SettingHelper.SetValue(n_TextFontSzie, value.ToString());
            TextFontSzie = value;
        }



        [IgnoreDataMember]
        private RelayCommand<object> m_SaveCommand;
        public RelayCommand<object> SaveCommand
        {
            get
            {
                return m_SaveCommand ?? (m_SaveCommand = new RelayCommand<object>((obj) =>
                  {
                      SaveSetting();
                  }));
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
