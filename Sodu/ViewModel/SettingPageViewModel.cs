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
                SetProperty(ref m_IfAutoLogin, value);
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
            }

        }

        private string m_UserName;
        /// <summary>
        /// 自动登录的用户名
        /// </summary>
        public string UserName
        {
            get { return m_UserName; }
            set
            {
                SetProperty(ref m_UserName, value);
            }
        }

    }



}
