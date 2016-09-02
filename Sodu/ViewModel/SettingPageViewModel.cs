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
using System.Windows.Input;
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
        public static string n_LineHeight = "LineHeight";
        public static string n_Cookie = "Cookie";
        public static string n_IsNightModel = "IsNightModel";
        public static string n_IsLandscape = "IsLandscape";
        public static string n_IsPreLoad = "IsPreLoad";
        public static string n_ContentBackColor = "ContentBackColor";
        public static string n_LightValue = "LightValue";
        public static string n_IsReadByPageMode = "ReadByPageMode";
        public static string n_SwitchAnimation = "SwitchAnimation";
        public static string n_AppVersion = "AppVersion";



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
                SetTextSize(value);
            }
        }


        private int m_LineHeight = 32;
        /// <summary>
        /// 阅读显示字体大小  14-26
        /// </summary>
        public int LineHeight
        {
            get
            {
                return m_LineHeight;
            }
            set
            {
                if (value == m_LineHeight)
                {
                    return;
                }
                SetProperty(ref m_LineHeight, value);
                SetLineHeight(value, true);
            }
        }


        private double m_LightValue = 0;
        /// <summary>
        /// 阅读界面亮度
        /// </summary>
        public double LightValue
        {
            get
            {
                return m_LightValue;
            }
            set
            {
                if (value == m_LightValue)
                {
                    return;
                }
                SetProperty(ref m_LightValue, value);
                SetLightValue(value, true);
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


        private bool m_IsReadByPageMode = true;
        /// <summary>
        /// 是否开启分页阅读模式
        /// </summary>
        public bool IsReadByPageMode
        {

            get
            {
                return m_IsReadByPageMode;
            }
            set
            {

                if (value == m_IsReadByPageMode)
                {
                    return;
                }
                SetProperty(ref m_IsReadByPageMode, value);
                SetIsReadByPageMode(value);
            }
        }

        private bool m_SwitchAnimation = true;
        /// <summary>
        /// 是否开启切换动画
        /// </summary>
        public bool SwitchAnimation
        {

            get
            {
                return m_SwitchAnimation;
            }
            set
            {

                if (value == m_SwitchAnimation)
                {
                    return;
                }
                SetProperty(ref m_SwitchAnimation, value);
                SetSwitchAnimatione(value);
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

        }


        public void InitSettingData()
        {
            try
            {
                #region    //自动添加书架
                try
                {
                    if (!SettingHelper.CheckKeyExist(n_IfAutAddToShelf))
                    {
                        SettingHelper.SetValue(n_IfAutAddToShelf, true);
                        m_IfAutAddToShelf = true;
                    }
                    else
                    {
                        m_IfAutAddToShelf = (bool)SettingHelper.GetValue(n_IfAutAddToShelf);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IfAutAddToShelf, true);
                    m_IfAutAddToShelf = true;
                }

                #endregion

                #region  //在流量下下载
                try
                {
                    if (!SettingHelper.CheckKeyExist(n_IfDownloadInWAAN))
                    {
                        SettingHelper.SetValue(n_IfDownloadInWAAN, false);
                        m_IfDownloadInWAAN = false;
                    }
                    else
                    {
                        m_IfDownloadInWAAN = (bool)SettingHelper.GetValue(n_IfDownloadInWAAN);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IfDownloadInWAAN, false);
                    m_IfDownloadInWAAN = false;
                }

                #endregion

                #region  //正文字体大小
                try
                {

                    if (!SettingHelper.CheckKeyExist(n_TextFontSzie))
                    {
                        SettingHelper.SetValue(n_TextFontSzie, "20");
                        m_TextFontSzie = 20;
                    }
                    else
                    {
                        string value = SettingHelper.GetValue(n_TextFontSzie).ToString();
                        m_TextFontSzie = Convert.ToInt32(value);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_TextFontSzie, "20");
                    m_TextFontSzie = 20;
                }

                #endregion

                #region  //正文行间距
                try
                {
                    if (!SettingHelper.CheckKeyExist(n_LineHeight))
                    {
                        SettingHelper.SetValue(n_LineHeight, 35);
                        m_LineHeight = 35;
                    }
                    else
                    {
                        string value = SettingHelper.GetValue(n_LineHeight).ToString();
                        m_LineHeight = Convert.ToInt32(value);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_LineHeight, 35);
                    m_LineHeight = 35;
                }

                #endregion

                #region //设置夜间模式

                try
                {

                    if (!SettingHelper.CheckKeyExist(n_IsNightModel))
                    {
                        SettingHelper.SetValue(n_IsNightModel, false);
                        m_IsNightModel = false;
                    }
                    else
                    {
                        m_IsNightModel = (bool)SettingHelper.GetValue(n_IsNightModel);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IsNightModel, false);
                    m_IsNightModel = false;
                }
                finally
                {
                    SetNightMode(m_IsNightModel);
                }

                #endregion

                #region   //设置横向模式
                try
                {

                    if (!SettingHelper.CheckKeyExist(n_IsLandscape))
                    {
                        SettingHelper.SetValue(n_IsLandscape, false);
                        m_IsLandscape = false;
                    }
                    else
                    {
                        var value = (bool)SettingHelper.GetValue(n_IsLandscape);
                        m_IsLandscape = value;
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IsLandscape, false);
                    m_IsLandscape = false;
                }
                finally
                {
                    SetLandscape(m_IsLandscape);
                }

                #endregion

                #region   //设置预读
                try
                {

                    if (!SettingHelper.CheckKeyExist(n_IsPreLoad))
                    {
                        SettingHelper.SetValue(n_IsPreLoad, true);
                        m_IsPreLoad = true;
                    }
                    else
                    {
                        var value = (bool)SettingHelper.GetValue(n_IsPreLoad);
                        m_IsPreLoad = value;
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IsPreLoad, true);
                    m_IsPreLoad = true;
                }
                #endregion

                #region   //设置阅读背景色
                try
                {

                    if (!SettingHelper.CheckKeyExist(n_ContentBackColor))
                    {
                        SettingHelper.SetValue(n_ContentBackColor, this.ColorList[0].Color.ToString());
                        m_ContentBackColor = this.ColorList[0];
                    }
                    else
                    {
                        var value = SettingHelper.GetValue(n_ContentBackColor);
                        m_ContentBackColor = this.ColorList.ToList().FirstOrDefault(p => p.Color.ToString().Equals(value)) ??
                                           this.ColorList[0];
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_ContentBackColor, this.ColorList[0].Color.ToString());
                    m_ContentBackColor = this.ColorList[0];
                }
                #endregion

                #region //亮度
                try
                {

                    if (!SettingHelper.CheckKeyExist(n_LightValue))
                    {
                        SettingHelper.SetValue(n_LightValue, 100);
                        m_LightValue = 100;
                    }
                    else
                    {
                        string value = SettingHelper.GetValue(n_LightValue).ToString();
                        m_LightValue = Convert.ToDouble(value) < 20 ? 20 : Convert.ToDouble(value);
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_LightValue, 100);
                    m_LightValue = 100;
                }
                #endregion

                #region  //是否分页阅读
                try
                {
                    if (!SettingHelper.CheckKeyExist(n_IsReadByPageMode))
                    {
                        SettingHelper.SetValue(n_IsReadByPageMode, true);
                        m_IsReadByPageMode = true;
                    }
                    else
                    {
                        var value = (bool)SettingHelper.GetValue(n_IsReadByPageMode);
                        m_IsReadByPageMode = value;
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_IsReadByPageMode, true);
                    m_IsReadByPageMode = true;
                }

                #endregion

                #region  //是否开启切换动画
                try
                {
                    if (!SettingHelper.CheckKeyExist(n_SwitchAnimation))
                    {
                        SettingHelper.SetValue(n_SwitchAnimation, true);
                        m_SwitchAnimation = true;
                    }
                    else
                    {
                        var value = (bool)SettingHelper.GetValue(n_SwitchAnimation);
                        m_SwitchAnimation = value;
                    }
                }
                catch (Exception)
                {
                    SettingHelper.SetValue(n_SwitchAnimation, true);
                    m_SwitchAnimation = true;
                }

                #endregion

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
            SettingHelper.SetValue(n_ContentBackColor, this.ColorList[0].Color.ToString());
            SettingHelper.SetValue(n_LightValue, 1);
            SettingHelper.SetValue(n_IsReadByPageMode, true);
            SettingHelper.SetValue(n_SwitchAnimation, true);
            SettingHelper.SetValue(n_LineHeight, 35);
        }


        public string GetAppVersion()
        {
            if (!SettingHelper.CheckKeyExist(n_AppVersion))
            {
                return null;
            }
            else
            {
                var value = SettingHelper.GetValue(n_AppVersion).ToString();
                return value;
            }
        }

        public void SetAppVersion(string value)
        {
            SettingHelper.SetValue(n_AppVersion, value);
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

        public void SetSwitchAnimatione(bool value)
        {
            SettingHelper.SetValue(n_SwitchAnimation, value);
            SwitchAnimation = value;
        }

        public void SetIsReadByPageMode(bool value)
        {
            SettingHelper.SetValue(n_IsReadByPageMode, value);
            IsReadByPageMode = value;
        }

        public void SetNightMode(bool value)
        {
            SettingHelper.SetValue(n_IsNightModel, value);
            IsNightModel = value;
            this.Theme = IsNightModel ? ElementTheme.Dark : ElementTheme.Light;
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
            DisplayInformation.AutoRotationPreferences = IsLandscape ? DisplayOrientations.Landscape : DisplayOrientations.None;
        }

        public void SetPreLoad(bool value)
        {
            SettingHelper.SetValue(n_IsPreLoad, value);
            IsPreLoad = value;
        }


        public void SetTextSize(int value)
        {
            SettingHelper.SetValue(n_TextFontSzie, value.ToString());
            TextFontSzie = value;
        }

        private void SetLineHeight(int value, bool v)
        {
            SettingHelper.SetValue(n_LineHeight, value.ToString());
            LineHeight = value;
        }


        public void SetBackColor(SolidColorBrush value)
        {
            SettingHelper.SetValue(n_ContentBackColor, value.Color.ToString());
            ContentBackColor = value;
        }
        private void SetLightValue(double value, bool v)
        {
            SettingHelper.SetValue(n_LightValue, value);
            LightValue = value;
        }

        #region 命令

        #endregion
    }
}
