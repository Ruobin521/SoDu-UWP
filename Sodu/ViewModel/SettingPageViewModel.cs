using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.SettingHelper;
using Sodu.Model;
using Sodu.Services;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
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
        public static string n_IsPreLoad = "IsPreLoad";
        public static string n_ContentBackColor = "ContentBackColor";




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

        private ElementTheme m_Theme = ElementTheme.Default;
        public ElementTheme Theme
        {
            get
            {
                return m_Theme;
            }
            set
            {
                this.SetProperty(ref this.m_Theme, value);
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

        private bool m_IsPreLoad = false;
        /// <summary>
        /// 预读下一章
        /// </summary>
        public bool IsPreLoad
        {

            get
            {
                return m_IsPreLoad;
            }
            set
            {

                if (value == m_IsPreLoad)
                {
                    return;
                }
                SetProperty(ref m_IsPreLoad, value);
                SetPreLoad(value);
            }
        }

        /// <summary>
        ///  
        /// </summary>
        [IgnoreDataMember]
        public List<SolidColorBrush> ColorList
        {
            get
            {
                return ConstantValue.BackColorList;
            }
        }

        private SolidColorBrush m_ContentBackColor;
        public SolidColorBrush ContentBackColor
        {
            get
            {
                return m_ContentBackColor;
            }
            set
            {
                if (value == m_ContentBackColor)
                {
                    return;
                }
                SetProperty(ref m_ContentBackColor, value);
                SetBackColor(value);
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
            try
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


                //设置预读
                if (!SettingHelper.CheckKeyExist(n_IsPreLoad))
                {
                    SettingHelper.SetValue(n_IsPreLoad, true);
                    IsPreLoad = true;
                }
                else
                {
                    var value = (bool)SettingHelper.GetValue(n_IsPreLoad);
                    IsPreLoad = value;
                }

                //设置背景色
                if (!SettingHelper.CheckKeyExist(n_ContentBackColor))
                {
                    SettingHelper.SetValue(n_ContentBackColor, this.ColorList[0].Color.ToString());
                    ContentBackColor = this.ColorList[0];
                }
                else
                {
                    var value = SettingHelper.GetValue(n_ContentBackColor);
                    ContentBackColor = ColorBrushHelper.ConverterFromString(value.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                SetDefaultSetting();
            }
        }




        private void SetDefaultSetting()
        {
            SettingHelper.SetValue(n_IsAutoLogin, true);
            SettingHelper.SetValue(n_IfAutAddToShelf, true);
            SettingHelper.SetValue(n_IfDownloadInWAAN, false);
            SettingHelper.SetValue(n_IsNightModel, false);
            SettingHelper.SetValue(n_IsLandscape, false);
            SettingHelper.SetValue(n_TextFontSzie, "20");
            SettingHelper.SetValue(n_IsPreLoad, true);
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

            if (IsNightModel)
            {
                this.Theme = ElementTheme.Dark;
            }
            else
            {
                this.Theme = ElementTheme.Light;
            }
        }


        public bool GetNightMode()
        {
            var value = (bool)SettingHelper.GetValue(n_IsNightModel);
            return value;
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
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            }
        }

        public void SetPreLoad(bool value)
        {
            SettingHelper.SetValue(n_IsPreLoad, value);
            IsPreLoad = value;
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


        public void SetBackColor(SolidColorBrush value)
        {
            SettingHelper.SetValue(n_ContentBackColor, value.Color.ToString());
            ContentBackColor = value;
        }



    }
}
