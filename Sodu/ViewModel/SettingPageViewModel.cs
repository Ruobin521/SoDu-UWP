using Sodu.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Web.Http;

namespace Sodu.ViewModel
{
    public class SettingPageViewModel : BaseViewModel
    {

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
            }
        }

        private bool m_IfAutoLogin;
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
                SetProperty(ref m_IfAutoLogin, value);
            }

        }


        private SoduCookie m_UserCookie;
        /// <summary>
        /// 用户自动登录时的cookie
        /// </summary>
        public SoduCookie UserCookie
        {
            get
            {
                return m_UserCookie;
            }
            set
            {
                if (m_UserCookie != value)
                {
                    SetProperty(ref m_UserCookie, value);

                }
            }
        }

    }



}
