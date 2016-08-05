using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookContentPageViewModel : BaseViewModel, IViewModel
    {
        #region 属性，字段

        private HttpHelper htmlHttp = new HttpHelper();
        private HttpHelper preHtmlHttp = new HttpHelper();
        private HttpHelper catalogsHttp = new HttpHelper();
        private DispatcherTimer timer;

        private bool isClickCtalog = false;

        private bool isPreLoading = false;

        private bool m_IsLoading;
        public bool IsLoading
        {
            get
            {
                return m_IsLoading;
            }
            set
            {
                SetProperty(ref m_IsLoading, value);
            }
        }
        private bool m_IsLoadingCatalogList;
        public bool IsLoadingCatalogList
        {
            get
            {
                return m_IsLoadingCatalogList;
            }
            set
            {
                SetProperty(ref m_IsLoadingCatalogList, value);
            }
        }

        private string m_ContentTitle;
        public string ContentTitle
        {
            get
            {
                return m_ContentTitle;
            }
            set
            {
                SetProperty(ref m_ContentTitle, value);
            }
        }

        private string m_CurrentTime;
        public string CurrentTime
        {
            get
            {
                return m_CurrentTime;
            }
            set
            {
                SetProperty(ref m_CurrentTime, value);
            }
        }



        public string m_TextContent;
        public string TextContent
        {
            get
            {
                return m_TextContent;
            }
            set
            {
                SetProperty(ref m_TextContent, value);
            }
        }

        public string m_PreTextContent;
        public string PreTextContent
        {
            get
            {
                return m_PreTextContent;
            }
            set
            {
                SetProperty(ref m_PreTextContent, value);
            }
        }


        public ObservableCollection<string> m_ContentListt;
        public ObservableCollection<string> ContentListt
        {
            get
            {
                if (m_ContentListt == null)
                {
                    m_ContentListt = new ObservableCollection<string>();
                }
                return m_ContentListt;
            }
            set
            {
                SetProperty(ref m_ContentListt, value);
            }
        }




        public bool m_IsSwitchButtonShow;
        public bool IsSwitchButtonShow
        {
            get
            {
                return m_IsSwitchButtonShow;
            }
            set
            {
                SetProperty(ref m_IsSwitchButtonShow, value);
            }
        }

        private BookCatalog m_CurrentCatalog;
        /// <summary>
        /// 当前选中的
        /// </summary>
        public BookCatalog CurrentCatalog
        {
            get
            {
                return m_CurrentCatalog;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentCatalog, value);
            }
        }

        private BookEntity m_BookEntity;
        public BookEntity BookEntity
        {
            get
            {
                return m_BookEntity;
            }
            set
            {
                this.SetProperty(ref this.m_BookEntity, value);
            }
        }

        #endregion


        #region 方法

        public BookContentPageViewModel()
        {
            InitTimer();
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, object e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        public void InitData(object obj)
        {
            CancleHttpRequest();
            isClickCtalog = false;

            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                return;
            }

            this.TextContent = null;

            if (!entity.IsLocal)
            {
                this.IsSwitchButtonShow = false;
            }
            else
            {
                this.IsSwitchButtonShow = true;

            }

            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }

            this.BookEntity = entity;

            try
            {
                if (this.BookEntity.IsLocal)
                {
                    this.CurrentCatalog = entity.CatalogList.FirstOrDefault(p => p.CatalogUrl == entity.LastReadChapterUrl);
                }

                else if (this.BookEntity.IsHistory)
                {
                    this.BookEntity.CatalogList = null;
                    this.CurrentCatalog = new BookCatalog() { BookID = entity.BookID, CatalogName = entity.LastReadChapterName, CatalogUrl = entity.LastReadChapterUrl, LyWeb = new Uri(entity.NewestChapterUrl).Authority };
                }
                else
                {
                    this.BookEntity.CatalogList = null;
                    this.CurrentCatalog = new BookCatalog() { BookID = entity.BookID, CatalogName = entity.NewestChapterName, CatalogUrl = entity.NewestChapterUrl, LyWeb = new Uri(entity.NewestChapterUrl).Authority };
                }

                this.ContentTitle = CurrentCatalog.CatalogName;

                SetData(CurrentCatalog);
                BookEntity.CatalogListUrl = SetBookCataologListUrl(CurrentCatalog.CatalogUrl);
                SetBookCatalogList();
            }
            catch (Exception)
            {
                ToastHeplper.ShowMessage("正文加载失败");
            }
        }

        public void SetData(BookCatalog catalog)
        {
            IsLoading = true;

            preHtmlHttp.HttpClientCancleRequest();
            PreTextContent = null;

            Task.Run(async () =>
           {
               string resultHtml = null;
               try
               {
                   resultHtml = await GetHtmlData(catalog, htmlHttp);
               }
               catch (Exception)
               {
                   resultHtml = null;
               }
               finally
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoading = false;
                   });
               }
               return resultHtml;

           }).ContinueWith(async (result) =>
           {
               string html = result.Result;
               bool rs = false;
               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
               {
                   if (html != null)
                   {
                       SetCurrentContent(catalog, html);
                       rs = true;
                   }
                   else
                   {
                       if (NavigationService.ContentFrame.Content is BookContentPage)
                       {
                           ToastHeplper.ShowMessage("未能获取正文内容");
                       }
                       rs = false;
                   }
               });
               return rs;
           });
        }


        private async void SetCurrentContent(BookCatalog catalog, string html)
        {
            this.CurrentCatalog = catalog;

            this.BookEntity.LastReadChapterName = catalog.CatalogName;
            this.BookEntity.LastReadChapterUrl = catalog.CatalogUrl;
            SetTextContent(html);
            this.ContentTitle = BookEntity.LastReadChapterName;
            if (!this.BookEntity.IsLocal)
            {
                //添加小说到历史记录
                ViewModelInstance.Instance.EverReadBookPageViewModelInstance.AddToHistoryList(BookEntity);
                // ToastHeplper.ShowMessage("正文加载完毕");
            }
            else
            {
                try
                {
                    DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), BookEntity);
                }
                catch (Exception)
                {

                }
            }

            if (!BookEntity.IsLocal && ViewModelInstance.Instance.SettingPageViewModelInstance.IsPreLoad)
            {
                var prehtml = await GetPreHtmlData();
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    PreTextContent = prehtml;
                });
            }
        }

        private async Task<string> GetHtmlData(BookCatalog catalog, HttpHelper http)
        {
            string resultHtml = null;
            try
            {
                if (this.BookEntity.IsLocal)
                {
                    resultHtml = await GetCatafromDatabase(catalog);
                    if (!string.IsNullOrEmpty(resultHtml))
                    {
                        return resultHtml;
                    }
                }
                resultHtml = await AnalysisContentService.GetHtmlContent(http, catalog.CatalogUrl);
            }
            catch (Exception)
            {
                resultHtml = null;
            }
            return resultHtml;
        }


        private async Task<string> GetPreHtmlData()
        {
            string resultHtml = null;
            try
            {
                if (isPreLoading) return null;
                isPreLoading = true;
                if (this.BookEntity.CatalogList == null || this.BookEntity.CatalogList.Count == 0)
                {
                    return null;
                }

                preHtmlHttp.HttpClientCancleRequest();

                var tempcatalog = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == this.CurrentCatalog.CatalogUrl);
                int index = this.BookEntity.CatalogList.IndexOf(tempcatalog);
                if (this.BookEntity.CatalogList.Count < 2)
                {
                    return null;
                }
                else if (index == this.BookEntity.CatalogList.Count - 1 || index == -1)
                {
                    return null;
                }

                tempcatalog = this.BookEntity.CatalogList[index + 1];
                resultHtml = await GetHtmlData(tempcatalog, preHtmlHttp);
            }
            catch (Exception)
            {
                resultHtml = null;
            }
            finally
            {
                isPreLoading = false;
            }
            return resultHtml;
        }

        private void SetBookCatalogList()
        {
            Task.Run(async () =>
           {
               try
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoadingCatalogList = true;
                   });
                   if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                   {
                       return;
                   }
                   else
                   {
                       if (string.IsNullOrEmpty(this.BookEntity.CatalogListUrl))
                       {
                           return;
                       }

                       List<BookCatalog> list = await Services.AnalysisBookCatalogList.GetCatalogList(BookEntity.CatalogListUrl, this.BookEntity.BookID, catalogsHttp);
                       if (list != null && list.Count > 0)
                       {
                           if (this.BookEntity.CatalogList == null)
                           {
                               this.BookEntity.CatalogList = new ObservableCollection<BookCatalog>();
                           }
                           this.BookEntity.CatalogList.Clear();
                           foreach (var item in list)
                           {
                               this.BookEntity.CatalogList.Add(item);
                           }
                           await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                           {
                               IsSwitchButtonShow = true;
                           });
                       }
                   }
               }
               catch (Exception)
               {
               }
               finally
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoadingCatalogList = false;
                       IsLoading = false;
                       if (isClickCtalog && IsSwitchButtonShow)
                       {
                           NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                           isClickCtalog = false;
                       }
                   });
               }
           }).ContinueWith(async (obj) =>
           {
               if (string.IsNullOrEmpty(PreTextContent) && !BookEntity.IsLocal && ViewModelInstance.Instance.SettingPageViewModelInstance.IsPreLoad)
               {

                   PreTextContent = null;
                   var html = await GetPreHtmlData();
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       PreTextContent = html;
                   });
               }
           });

        }

        private string SetBookCataologListUrl(string catalogUrl)
        {
            var catalogListUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogListUrl(catalogUrl);

            return catalogListUrl;
        }

        private void SetTextContent(string html)
        {
            this.ContentTitle = this.BookEntity.BookName + "_" + this.CurrentCatalog.CatalogName;

            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }
            this.ContentListt = null; ;
            List<string> strList = SplitString(html);
            for (int i = 0; i < strList.Count; i++)
            {
               this.ContentListt.Add(strList[i]);
            }
        }

        private List<string> SplitString(string str)
        {
            List<string> strList = new List<string>();

            string[] lists = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lists.Count(); i++)
            {
                strList.Add(lists[i]);
            }
            return strList;
        }

        /// <summary>
        /// 从数据库获取数据
        /// <param name="catalog"></param>
        /// <param name="isBackGround"></param>
        /// <returns></returns>
        private async Task<string> GetCatafromDatabase(BookCatalog catalog, bool isBackGround = true)
        {
            string html = null;
            await Task.Run(async () =>
            {
                try
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = true;
                    });

                    var item = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == catalog.CatalogUrl);
                    if (item != null)
                    {
                        var content = DBBookCatalogContent.SelectBookCatalogContent(AppDataPath.GetBookDBPath(BookEntity.BookID), item.CatalogUrl);
                        if (content != null)
                        {
                            html = content.Content;
                        }
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = false;
                    });
                }

            });
            return html;
        }

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            if (htmlHttp != null)
            {
                htmlHttp.HttpClientCancleRequest();
            }
            if (catalogsHttp != null)
            {
                catalogsHttp.HttpClientCancleRequest();
            }
            if (preHtmlHttp != null)
            {
                preHtmlHttp.HttpClientCancleRequest();
            }
            IsLoading = false;
        }

        #endregion

        #region 命令


        /// <summary>
        /// 字体增大
        /// </summary>
        private ICommand m_FontIncreaseCommand;
        public ICommand FontIncreaseCommand
        {
            get
            {
                return m_FontIncreaseCommand ?? (m_FontIncreaseCommand = new RelayCommand<bool>((str) =>
                {
                    ViewModelInstance.Instance.SettingPageViewModelInstance.SetTextSize(ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie + 2);
                }));
            }
        }

        /// <summary>
        /// 字体减小
        /// </summary>
        private ICommand m_FontDecreaseCommand;
        public ICommand FontDecreaseCommand
        {
            get
            {
                return m_FontDecreaseCommand ?? (m_FontDecreaseCommand = new RelayCommand<bool>((str) =>
               {
                   ViewModelInstance.Instance.SettingPageViewModelInstance.SetTextSize(ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie - 2);
               }));
            }
        }

        /// <summary>
        /// 夜间模式
        /// </summary>
        public ICommand m_NightModeCommand;
        public ICommand NightModeCommand
        {
            get
            {
                return m_NightModeCommand ??
                   (m_NightModeCommand = new RelayCommand<bool>((str) =>
                 {
                     ViewModelInstance.Instance.SettingPageViewModelInstance.SetNightMode(!ViewModelInstance.Instance.SettingPageViewModelInstance.IsNightModel);
                 }));
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private ICommand m_RefreshCommand;
        public ICommand RefreshCommand
        {
            get
            {
                return m_RefreshCommand ?? (m_RefreshCommand = new RelayCommand<bool>((str) =>
                   {
                       if (IsLoading)
                       {
                           CancleHttpRequest();
                       }
                       else
                       {
                           SetData(CurrentCatalog);
                       }
                   }));
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        private ICommand m_GoBackCommand;
        public ICommand GoBackCommand
        {
            get
            {
                return m_GoBackCommand ?? (m_GoBackCommand = new RelayCommand<bool>((str) =>
                 {
                     NavigationService.GoBack();
                 }));
            }
        }


        /// <summary>
        /// 目录
        /// </summary>
        private ICommand m_CatalogCommand;
        public ICommand CatalogCommand
        {
            get
            {
                return m_CatalogCommand ?? (m_CatalogCommand = new RelayCommand<bool>(async (str) =>
                 {
                     try
                     {
                         await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             IsLoading = true;
                         });
                         if (IsLoadingCatalogList)
                         {
                             ToastHeplper.ShowMessage("正在加载目录,请稍候");
                             isClickCtalog = true;
                             return;
                         }

                         bool rs = false;
                         if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                         {
                             rs = true;
                         }
                         else
                         {

                             ToastHeplper.ShowMessage("正在加载目录,请稍候");
                             isClickCtalog = true;
                             SetBookCatalogList();
                         }
                         if (rs)
                         {
                             NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                         }
                         else
                         {
                             ToastHeplper.ShowMessage("目录加载失败");
                         }

                         await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             IsLoading = false;
                         });
                     }
                     catch (Exception)
                     {

                     }
                 }));
            }
        }

        /// <summary>
        /// 切换章节
        /// </summary>
        private ICommand m_SwitchCatalogCommand;
        public ICommand SwitchCatalogCommand
        {
            get
            {
                return m_SwitchCatalogCommand ?? (m_SwitchCatalogCommand = new RelayCommand<object>((str) =>
                  {
                      OnSwtichCommand(str);
                  }));
            }
        }

        public void OnSwtichCommand(object str)
        {
            if (IsLoading) return;
            try
            {
                if (this.BookEntity.CatalogList == null || this.BookEntity.CatalogList.Count <= 1)
                {
                    ToastHeplper.ShowMessage("暂未获取到章节列表，无法切换");
                    return;
                }
                var tempcatalog = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == this.CurrentCatalog.CatalogUrl);
                int index = this.BookEntity.CatalogList.IndexOf(tempcatalog);
                //上一章
                if (str.ToString().Equals("0"))
                {
                    if (this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) == 0)
                    {
                        ToastHeplper.ShowMessage("已经是第一章。");
                        return;
                    }
                    if (this.BookEntity.CatalogList.Count < 2)
                    {
                        return;
                    }
                    if (tempcatalog != null)
                    {
                        tempcatalog = this.BookEntity.CatalogList[this.BookEntity.CatalogList.IndexOf(tempcatalog) - 1];
                    }
                }
                //下一章
                else if (str.ToString().Equals("1"))
                {
                    if (this.BookEntity.CatalogList.Count < 2)
                    {
                        return;
                    }
                    else if (index == this.BookEntity.CatalogList.Count - 1 || index == -1)
                    {
                        NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                        return;
                    }
                    tempcatalog = this.BookEntity.CatalogList[index + 1];
                    if (!string.IsNullOrEmpty(PreTextContent))
                    {
                        SetCurrentContent(tempcatalog, PreTextContent);
                        PreTextContent = null;
                        return;
                    }
                }
                else if (str is BookCatalog)
                {
                    BookCatalog catalog = str as BookCatalog;
                    tempcatalog = catalog;
                }

                if (tempcatalog != null && tempcatalog.CatalogName != null && tempcatalog.CatalogUrl != null)
                {
                    SetData(tempcatalog);
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion
    }
}
