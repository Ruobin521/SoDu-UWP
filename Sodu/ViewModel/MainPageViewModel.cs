using GalaSoft.MvvmLight.Command;
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
        private bool m_IsButtonVisiable = true;
        /// <summary>
        ///  控制按钮是否显示
        /// </summary>
        public bool IsButtonVisiable
        {
            get

            {
                return m_IsButtonVisiable;
            }
            set
            {
                this.SetProperty(ref this.m_IsButtonVisiable, value);
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

        public void SetLeftControlButtonVisiablity(bool value)
        {
            IsButtonVisiable = value;
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
            NavigationService.ContentFrame.BackStack.Clear();
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
            catch (Exception ex)
            {

            }
        }

        public void SetCurrentMenu(Type type)
        {
            try
            {
                this.CurrentMenu = this.CurrentMenuList.ToList().FirstOrDefault(p => p.MenuType == type);
            }
            catch (Exception ex)
            {
                ToastHeplper.ShowMessage("未知错误");
            }
        }
        public void InitData(object obj = null)
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
            BookEntity entity = obj as BookEntity;

            if (entity != null)
            {
                MenuModel menu = new MenuModel() { MenuName = entity.BookName, MenuType = typeof(UpdateChapterPage) };


                //添加小说到历史记录
                ViewModelInstance.Instance.EverReadBookPageViewModelInstance.AddToHistoryList(entity);

                //判断是否自动添加书到收藏
                if (ViewModelInstance.Instance.IsLogin && ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutAddToShelf)
                {
                    Task.Run(async () =>
                   {
                       if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                       {
                           string html = await (new HttpHelper()).WebRequestGet(string.Format(PageUrl.AddToShelfPage, entity.BookID));
                           if (html.Contains("{\"success\":true}"))
                           {
                               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                               {
                                   if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                                   {
                                       ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Insert(0, entity);
                                   }
                               });
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
        public ICommand LogoutCommand
        {
            get
            {
                return new RelayCommand<bool>(
                      (str) =>
                {
                    IsLeftPanelOpen = false;
                    NavigationService.NavigateTo(typeof(LogoutPage), null);
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
                        IsLeftPanelOpen = false;

                        NavigationService.NavigateTo(typeof(SettingPage), null);
                    });
            }
        }

        /// <summary>
        /// 下载中心
        /// </summary>
        public ICommand DownLoadCenterCommadn
        {
            get
            {
                return new RelayCommand<bool>(
                      (str) =>
                      {
                          IsLeftPanelOpen = false;
                          NavigationService.NavigateTo(typeof(DownLoadCenterPage), null);
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
                        IsLeftPanelOpen = false;
                        NavigationService.NavigateTo(typeof(LocalBookPage), null);
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
                          IsLeftPanelOpen = false;
                          NavigationService.NavigateTo(typeof(EverReadPage), null);
                      });
            }
        }

        #endregion
    }
}
