﻿using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Sodu.Controls;
using Windows.UI.Xaml;
using Sodu.Services;

namespace Sodu.ViewModel
{
    public class MainPageViewModel : BaseViewModel, IViewModel
    {

        #region 属性，字段

        //private Frame m_ContentFrame = new Frame();
        ///// <summary>
        //// 右侧内容区域的page
        ///// </summary>
        //public Frame ContentFrame
        //{
        //    get
        //    {
        //        return m_ContentFrame;
        //    }
        //    set
        //    {
        //        //m_ContentFrame = value;
        //        this.SetProperty(ref this.m_ContentFrame, value);
        //        //RaisePropertyChanged("ContentFrame");
        //    }
        //}

        private bool m_IsLeftPanelOpen;
        /// <summary>
        /// 左侧控制面板是否显示
        /// </summary>
        public bool IsLeftPanelOpen
        {
            get

            {
                return m_IsLeftPanelOpen;
            }
            set
            {
                this.SetProperty(ref this.m_IsLeftPanelOpen, value);
            }
        }

        private bool m_IsLoading = false;
        /// <summary>
        /// 是否正在加载
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return m_IsLoading;
            }
            set
            {
                if (m_IsLoading != value)
                {
                    this.SetProperty(ref this.m_IsLoading, value);
                }
            }
        }


        private string m_ContentTitle;
        /// <summary>
        /// 右侧内容区域的标题
        /// </summary>
        public string ContentTitle
        {
            get { return m_ContentTitle; }
            set
            {
                this.SetProperty(ref this.m_ContentTitle, value);
            }
        }


        private string m_SearchPara;
        /// <summary>
        /// 搜索参数
        /// </summary>
        public string SearchPara
        {
            get { return m_SearchPara; }
            set
            {
                this.SetProperty(ref this.m_SearchPara, value);
            }
        }

        private ObservableCollection<MenuModel> m_CurrentMenuList;
        public ObservableCollection<MenuModel> CurrentMenuList
        {
            get
            {
                return m_CurrentMenuList;
            }
            set
            {
                if (this.m_CurrentMenuList != value)
                {
                    this.SetProperty(ref this.m_CurrentMenuList, value);
                }
            }
        }

        public ObservableCollection<MenuModel> LoadMenuList
        {
            get
            {
                return Constants.ConstantValue.LoadMenuList;
            }
        }
        public ObservableCollection<MenuModel> UnloadMenuList
        {
            get
            {
                return Constants.ConstantValue.UnloadMenuList;
            }
        }

        private MenuModel m_CurrentMenu;

        public MenuModel CurrentMenu
        {
            get { return m_CurrentMenu; }
            set
            {
                this.SetProperty(ref this.m_CurrentMenu, value);
                if (CurrentMenu != null)
                {
                    NavigateToPage(m_CurrentMenu, NavigatePara);
                }
            }
        }


        /// <summary>
        /// 导航传递的参数
        /// </summary>
        public object NavigatePara { get; set; }

        #endregion

        #region 构造函数
        public MainPageViewModel()
        {
            InitAppSettingData();
        }

        #endregion

        #region 方法

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            IsLoading = false;
        }


        public void InitAppSettingData()
        {
            try
            {

                if (!ViewModelInstance.Instance.IsLogin)
                {
                    this.CurrentMenuList = this.UnloadMenuList;
                }
                else
                {
                    this.CurrentMenuList = this.LoadMenuList;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void ChangeLoginState(bool isLogin)
        {
            ViewModelInstance.Instance.IsLogin = isLogin;
            if (isLogin)
            {
                this.CurrentMenuList = this.LoadMenuList;
            }
            else
            {
                this.CurrentMenuList = this.UnloadMenuList;
            }
            this.CurrentMenu = this.CurrentMenuList[0];
        }

        /// <summary>
        /// 导航到选择的页面
        /// </summary>
        /// <param name="menu"></param>
        public void NavigateToPage(MenuModel menu, object para = null)
        {
            try
            {
                if (menu != null)
                {
                    this.IsLeftPanelOpen = false;
                    NavigationService.NavigateTo(menu, para);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void SetCurrentMenu(string menuName)
        {
            try
            {
                this.CurrentMenu = this.CurrentMenuList.ToList().Find(p => p.MenuName == menuName);

            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("未知错误");
            }
        }
        public void RefreshData(object obj = null, bool IsRefresh = false)
        {
            //throw new NotImplementedException();
        }

        public void StartLoading()
        {
            IsLoading = true;
        }
        public void StopLoading()
        {
            IsLoading = false;
        }

        #endregion


        #region 命令 +命令方法


        /// <summary>
        /// 打开或关闭左侧控制面板
        /// </summary>
        public ICommand IsLeftPanelOpenCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    IsLeftPanelOpen = !IsLeftPanelOpen;
                });
            }
        }


        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        public BaseCommand BookItemSelectedChangedCommand
        {
            get
            {
                return new BaseCommand(OnBookItemSelectedChangedCommand);
            }
        }
        public void OnBookItemSelectedChangedCommand(object obj)
        {
            //SetCurrentMenu("0");
            BookEntity entity = obj as BookEntity;

            if (entity != null)
            {
                MenuModel menu = new MenuModel() { MenuName = entity.BookName, MenuType = typeof(UpdateChapterPage) };
                ViewModelInstance.Instance.EverReadBookPageViewModelInstance.AddToHistoryList(entity);

                if (ViewModelInstance.Instance.IsLogin && ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutAddToShelf)
                {
                    Task.Run(async () =>
                   {
                       if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                       {
                           string html = await (new HttpHelper()).WebRequestGet(string.Format(PageUrl.AddToShelfPage, entity.BookID));
                           if (html.Contains("{\"success\":true}"))
                           {
                               ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Add(entity);
                           }
                       }

                   });
                }
                NavigateToPage(menu, entity);
            }
            else
            {
                CommonMethod.ShowMessage("数据有误，请重新尝试");
            }

        }
        /// <summary>
        /// 选中相应的chapteritem
        /// </summary>
        public BaseCommand BookChapterSelectedChangedCommand
        {
            get
            {
                return new BaseCommand(OnBookChapterSelectedChangedCommand);
            }
        }
        public void OnBookChapterSelectedChangedCommand(object obj)
        {
            //SetCurrentMenu("0");
            BookEntity entity = obj as BookEntity;

            if (entity != null)
            {
                if (entity.LyWeb.Equals("起点中文网")) return;
                MenuModel menu = new MenuModel() { MenuName = entity.ChapterName, MenuType = typeof(BookContentPage) };
                //0 表示从更新列表跳转 1 表示从目录跳转
                NavigateToPage(menu, new object[] { "0", entity });
            }
            else
            {
                CommonMethod.ShowMessage("数据有误，请重新尝试");
            }

        }

        /// <summary>
        /// 注销
        /// </summary>
        public ICommand LogoutCommand
        {
            get
            {
                return new RelayCommand<bool>(
                    async (str) =>
                {
                    try
                    {
                        IsLeftPanelOpen = false;
                        HttpHelper http = new HttpHelper();
                        string html = await http.WebRequestGet(PageUrl.LogoutPage);
                        if (html != null && html.Contains("to delete public domains' cookies"))
                        {
                            ChangeLoginState(false);
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonMethod.ShowMessage("注销失败，请重新操作");
                    }
                });
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                return new RelayCommand<bool>(
                    async (str) =>
                    {
                        var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定退出？") { Title = "退出" };
                        msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                        {
                            App.Current.Exit();
                        }));
                        msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                        {
                            return;
                        }));
                        await msgDialog.ShowAsync();
                    });
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        public ICommand SettingCommand
        {
            get
            {
                return new RelayCommand<bool>(
                      (str) =>
                    {
                        MenuModel menu = new MenuModel() { MenuName = "设置", MenuType = typeof(CodingPage) };
                        NavigateToPage(menu, null);
                    });
            }
        }

        /// <summary>
        /// 本地图书
        /// </summary>
        public ICommand LocalBooksCommand
        {
            get
            {
                return new RelayCommand<bool>(
                      (str) =>
                    {
                        MenuModel menu = new MenuModel() { MenuName = "本地图书", MenuType = typeof(CodingPage) };
                        NavigateToPage(menu, null);
                    });
            }
        }

        /// <summary>
        /// 阅读记录
        /// </summary>
        public ICommand ReadHistoryCommand
        {
            get
            {
                return new RelayCommand<bool>(
                      (str) =>
                      {
                          MenuModel menu = new MenuModel() { MenuName = "阅读记录", MenuType = typeof(EverReadPage) };
                          NavigateToPage(menu, null);
                      });
            }
        }


        #endregion
    }
}
