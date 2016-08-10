using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Pages;
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
using Windows.UI.Xaml;
using Sodu.Services;
using Windows.System;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Model;
using SoDu.Core.Util;
using Sodu.Core.Util;

namespace Sodu.ViewModel
{
    public class MainPageViewModel : BaseViewModel, IViewModel
    {

        #region 属性，字段



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
                if (m_CurrentMenu == value) return;
                if (m_CurrentMenu != null)
                {
                    m_CurrentMenu.IsSelected = false;
                }

                this.SetProperty(ref this.m_CurrentMenu, value);
                if (m_CurrentMenu != null)
                {
                    m_CurrentMenu.IsSelected = true;
                }
            }
        }


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
            catch (Exception)
            {

            }
        }


        public void ChangeLoginState(bool isLogin)
        {
            ViewModelInstance.Instance.IsLogin = isLogin;
            this.CurrentMenuList = new ObservableCollection<MenuModel>();

            if (isLogin)
            {
                this.LoadMenuList.ToList().ForEach(p => this.CurrentMenuList.Add(p));
            }
            else
            {
                this.UnloadMenuList.ToList().ForEach(p => this.CurrentMenuList.Add(p));
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
                    NavigationService.NavigateTo(menu.MenuType, para);
                }
            }
            catch (Exception)
            {
                ToastHeplper.ShowMessage("导航出现异常");
            }
        }
        public void NavigateToPage(Type pageType, object para = null)
        {
            try
            {
                if (pageType != null)
                {
                    this.IsLeftPanelOpen = false;
                    NavigationService.NavigateTo(pageType, para);
                }
            }
            catch (Exception)
            {
                ToastHeplper.ShowMessage("导航出现异常");
            }
        }


        public void SetCurrentMenu(Type type)
        {
            try
            {
                this.CurrentMenu = this.CurrentMenuList.ToList().FirstOrDefault(p => p.MenuType == type);
            }
            catch (Exception)
            {
                ToastHeplper.ShowMessage("导航出现异常");
            }
        }
        public void InitData(object obj = null)
        {
            return;
        }

        #endregion

        #region 命令 +命令方法
        public ICommand m_IsLeftPanelOpenCommand;
        /// <summary>
        /// 打开或关闭左侧控制面板
        /// </summary>
        public ICommand IsLeftPanelOpenCommand
        {
            get
            {
                return m_IsLeftPanelOpenCommand ??
             (m_IsLeftPanelOpenCommand = new RelayCommand<bool>((str) =>
        {
            IsLeftPanelOpen = !IsLeftPanelOpen;
        }));
            }
        }


        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        private RelayCommand<object> m_BookItemSelectedChangedCommand;
        public RelayCommand<object> BookItemSelectedChangedCommand
        {
            get
            {
                return m_BookItemSelectedChangedCommand ??
                    (m_BookItemSelectedChangedCommand = new RelayCommand<object>(OnBookItemSelectedChangedCommand));
            }
        }
        public void OnBookItemSelectedChangedCommand(object obj)
        {
            BookEntity entity = obj as BookEntity;

            if (entity != null)
            {
                MenuModel menu = new MenuModel() { MenuName = entity.BookName, MenuType = typeof(UpdateChapterPage) };


                //判断是否自动添加书到收藏
                if (ViewModelInstance.Instance.IsLogin && ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutAddToShelf)
                {
                    Task.Run(async () =>
                   {
                       if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                       {
                           string html = await (new HttpHelper()).WebRequestGet(string.Format(ViewModelInstance.Instance.UrlService.GetAddToShelfPage(), entity.BookID));
                           if (html.Contains("{\"success\":true}"))
                           {
                                DispatcherHelper.CheckBeginInvokeOnUI( () =>
                               {
                                   if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                                   {
                                       ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Insert(0, new BookEntity()
                                       {
                                           AuthorName = entity.AuthorName,
                                           BookID = entity.BookID,
                                           BookName = entity.BookName,
                                           CatalogListUrl = entity.CatalogListUrl,
                                           LastReadChapterName = entity.LastReadChapterName,
                                           LastReadChapterUrl = entity.LastReadChapterUrl,
                                           NewestChapterName = entity.NewestChapterName,
                                           NewestChapterUrl = entity.NewestChapterUrl,
                                           UnReadCountData = entity.UnReadCountData,
                                           UpdateTime = entity.UpdateTime,
                                           UpdateCatalogUrl = entity.UpdateCatalogUrl,
                                           LyWeb = entity.LyWeb
                                       });
                                   }
                               });
                           }
                           else
                           {

                           }
                       }
                   });
                }
                NavigationService.NavigateTo(typeof(UpdateChapterPage), entity);
            }
            else
            {
                ToastHeplper.ShowMessage("数据有误，请重新尝试");
            }
        }

        /// 注销
        /// </summary>
        private ICommand m_LogoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return m_LogoutCommand ?? (
                 m_LogoutCommand = new RelayCommand<bool>(
                      (str) =>
                {
                    NavigateToPage(typeof(LogoutPage));
                }));
            }
        }

        /// 登陆
        /// </summary>
        private ICommand m_LoginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return m_LoginCommand ?? (
                 m_LoginCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(LoginPage));
                      }));
            }
        }



        /// 注册
        /// </summary>
        private ICommand m_RegisterCommand;
        public ICommand RegisterCommand
        {
            get
            {
                return m_RegisterCommand ?? (
                 m_RegisterCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(RegisterPage));
                      }));
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        private ICommand m_ExitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return m_ExitCommand ??
                    (
                m_ExitCommand = new RelayCommand<bool>(
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
                    }));
            }
        }

        /// <summary>
        /// 设置
        /// </summary>
        private ICommand m_SettingCommand;
        public ICommand SettingCommand
        {
            get
            {
                return m_SettingCommand ??
                    (
               m_SettingCommand = new RelayCommand<bool>(
                      (str) =>
                    {
                        NavigateToPage(typeof(SettingPage));
                    }));
            }
        }

        /// <summary>
        /// 使用帮助
        /// </summary>
        private ICommand m_HelpCommand;
        public ICommand HelpCommand
        {
            get
            {
                return m_HelpCommand ??
                    (
               m_HelpCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(AboutPage));
                      }));
            }
        }
        /// <summary>
        /// 首页
        /// </summary>
        private ICommand m_HomePageCommand;
        public ICommand HomePageCommand
        {
            get
            {
                return m_HomePageCommand ??
                    (m_HomePageCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(HomePage));
                      }));
            }
        }


        /// <summary>
        ///  搜索
        /// </summary>
        private ICommand m_SearchCommand;
        public ICommand SearchCommand
        {
            get
            {
                return m_SearchCommand ??
                    (m_SearchCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(SearchResultPage));
                      }));
            }
        }


        /// <summary>
        /// 下载中心
        /// </summary>
        private ICommand m_DownLoadCenterCommand;
        public ICommand DownLoadCenterCommand
        {
            get
            {
                return m_DownLoadCenterCommand ??
                    (m_DownLoadCenterCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(DownLoadCenterPage));
                      }));
            }
        }

        /// <summary>
        /// 本地图书
        /// </summary>
        private ICommand m_LocalBooksCommand;
        public ICommand LocalBooksCommand
        {
            get
            {
                return m_LocalBooksCommand ?? (m_LocalBooksCommand = new RelayCommand<bool>(
                      (str) =>
                    {
                        NavigateToPage(typeof(LocalBookPage));
                    }));
            }
        }

        /// <summary>
        /// 应用商店评价
        /// </summary>
        private ICommand m_EvaluateCommand;
        public ICommand EvaluateCommand
        {
            get
            {
                return m_EvaluateCommand ?? (m_EvaluateCommand = new RelayCommand<bool>(
                     async (str) =>
                      {
                          await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9nblggh4sk4v"));
                      }));
            }
        }

        /// <summary>
        /// 阅读记录
        /// </summary>
        private ICommand m_ReadHistoryCommand;
        public ICommand ReadHistoryCommand
        {
            get
            {
                return m_ReadHistoryCommand ?? (m_ReadHistoryCommand = new RelayCommand<bool>(
                      (str) =>
                      {
                          NavigateToPage(typeof(HistoryPage));
                      }));
            }
        }

        #endregion
    }
}
