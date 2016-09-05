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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;

namespace Sodu.ViewModel
{
    public class BookContentPageViewModel : BaseViewModel, IViewModel
    {
        #region 属性，字段

        private HttpHelper htmlHttp = new HttpHelper();
        private HttpHelper preHtmlHttp = new HttpHelper();
        private HttpHelper catalogsHttp = new HttpHelper();


        /// <summary>
        /// 是否点击了目录按钮
        /// </summary>
        private bool isClickCtalog = false;

        /// <summary>
        /// 是否正在预加载
        /// </summary>
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

        private bool m_CanDownLoad;
        public bool CanDownLoad
        {
            get
            {
                return m_CanDownLoad;
            }
            set
            {
                SetProperty(ref m_CanDownLoad, value);
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



        public string m_CurrentPageContent;
        /// <summary>
        /// 当前分页内容
        /// </summary>
        public string CurrentPageContent
        {
            get
            {
                return m_CurrentPageContent;
            }
            set
            {
                SetProperty(ref m_CurrentPageContent, value);
            }
        }

        public string m_PrePageContent;
        /// <summary>
        ///  上一页内容
        /// </summary>
        public string PrePageContent
        {
            get
            {
                return m_PrePageContent;
            }
            set
            {
                SetProperty(ref m_PrePageContent, value);
            }
        }

        public string m_NextPageContent;
        /// <summary>
        /// 下一页内容
        /// </summary>
        public string NextPageContent
        {
            get
            {
                return m_NextPageContent;
            }
            set
            {
                SetProperty(ref m_NextPageContent, value);
            }
        }

        public int m_CurrentPagIndex;
        public int CurrentPagIndex
        {
            get
            {
                return m_CurrentPagIndex;
            }
            set
            {
                SetProperty(ref m_CurrentPagIndex, value);
            }
        }

        public int m_TotalPagCount;
        /// <summary>
        /// 当前章节总页数
        /// </summary>
        public int TotalPagCount
        {
            get
            {
                return m_TotalPagCount;
            }
            set
            {
                SetProperty(ref m_TotalPagCount, value);
            }
        }

        public int m_CurrentCatalogIndex;
        /// <summary>
        /// 当前章节索引
        /// </summary>
        public int CurrentCatalogIndex
        {
            get
            {
                return m_CurrentCatalogIndex;
            }
            set
            {
                SetProperty(ref m_CurrentCatalogIndex, value);
            }
        }

        public int m_TotalCatalogCount;
        /// <summary>
        /// 总章数
        /// </summary>
        public int TotalCatalogCount
        {
            get
            {
                return m_TotalCatalogCount;
            }
            set
            {
                SetProperty(ref m_TotalCatalogCount, value);
            }
        }

        public string m_CurrentCatalogContent;
        /// <summary>
        /// 当前章节html
        /// </summary>
        public string CurrentCatalogContent
        {
            get
            {
                return m_CurrentCatalogContent;
            }
            set
            {
                SetProperty(ref m_CurrentCatalogContent, value);
            }
        }


        public string m_NextCatalogContent;
        /// <summary>
        /// 下一章html
        /// </summary>
        public string NextCatalogContent
        {
            get
            {
                return m_NextCatalogContent;
            }
            set
            {
                SetProperty(ref m_NextCatalogContent, value);
            }
        }


        public ObservableCollection<string> m_CurrentCatalogParagraphs;
        public ObservableCollection<string> CurrentCatalogParagraphs
        {
            get
            {
                if (m_CurrentCatalogParagraphs == null)
                {
                    m_CurrentCatalogParagraphs = new ObservableCollection<string>();
                }
                return m_CurrentCatalogParagraphs;
            }
            set
            {
                SetProperty(ref m_CurrentCatalogParagraphs, value);
            }
        }

        public ObservableCollection<string> m_CurrentCatalogPages;
        /// <summary>
        /// 内容分页
        /// </summary>
        public ObservableCollection<string> CurrentCatalogPages
        {
            get
            {
                if (m_CurrentCatalogPages == null)
                {
                    m_CurrentCatalogPages = new ObservableCollection<string>();
                }
                return m_CurrentCatalogPages;
            }
            set
            {
                SetProperty(ref m_CurrentCatalogPages, value);
            }
        }

        public ObservableCollection<string> m_PerContentPages;
        /// <summary>
        /// 上一章内容分页
        /// </summary>
        public ObservableCollection<string> PreContentPages
        {
            get
            {
                if (m_PerContentPages == null)
                {
                    m_PerContentPages = new ObservableCollection<string>();
                }
                return m_PerContentPages;
            }
            set
            {
                SetProperty(ref m_PerContentPages, value);
            }
        }

        public ObservableCollection<string> m_NextContentPages;
        /// <summary>
        /// 下一章内容分页
        /// </summary>
        public ObservableCollection<string> NextContentPages
        {
            get
            {
                if (m_NextContentPages == null)
                {
                    m_NextContentPages = new ObservableCollection<string>();
                }
                return m_NextContentPages;
            }
            set
            {
                SetProperty(ref m_NextContentPages, value);
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
        /// 当前章节
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

        private BookCatalog m_NextCatalog;
        /// <summary>
        /// 下个章节
        /// </summary>
        public BookCatalog NextCatalog
        {
            get
            {
                return m_NextCatalog;
            }
            set
            {
                this.SetProperty(ref this.m_NextCatalog, value);
            }
        }

        private BookEntity m_BookEntity;
        /// <summary>
        /// 当前小说
        /// </summary>
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

        private double ContentContainerWith { get; set; }
        private double ContentContainerHeitht { get; set; }

        private double PerTextHeight { get; set; }
        private double PerTextWidth { get; set; }

        #endregion

        #region 构造函数
        public BookContentPageViewModel()
        {

        }

        #endregion

        #region 方法

        public bool SetSize(double containerWith, double containerHtght, double txtWidth, double txtHeight)
        {
            if (containerWith != ContentContainerWith || containerHtght != ContentContainerHeitht ||
                PerTextWidth != txtWidth || PerTextHeight != txtHeight)
            {
                ContentContainerWith = containerWith;
                ContentContainerHeitht = containerHtght;
                PerTextWidth = txtWidth;
                PerTextHeight = txtHeight;

                return true;
            }
            return false;
        }

        public void InitData(object obj)
        {
            CancleHttpRequest();
            ResetData();

            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                return;
            }
            this.BookEntity = entity.Clone();

            if (!entity.IsLocal)
            {
                this.IsSwitchButtonShow = false;
            }
            else
            {
                this.IsSwitchButtonShow = true;
            }


            if (CheckIfLoacalExist())
            {
                CanDownLoad = false;
            }
            else
            {
                CanDownLoad = true;
            }

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

        public void ResetData()
        {
            isClickCtalog = false;
            isPreLoading = false;
            IsSwitchButtonShow = false;

            BookEntity = null;
            NextCatalog = null;


            CurrentCatalogIndex = 1;
            TotalPagCount = 1;

            CurrentPagIndex = 1;
            CurrentPageContent = "";

            NextPageContent = "";
            PrePageContent = "";
            CurrentPageContent = "";

            NextCatalogContent = "";
            CurrentCatalogContent = "";



            PreContentPages.Clear();
            NextContentPages.Clear();
            CurrentCatalogParagraphs.Clear();
            CurrentCatalogPages.Clear();
        }

        private bool CheckIfLoacalExist()
        {
            bool rs = false;
            var result = DBLocalBook.GetAllLocalBookList(AppDataPath.GetLocalBookDBPath());

            if (result != null && result.FirstOrDefault(p => p.BookID == this.BookEntity.BookID) != null)
            {
                rs = true;
            }

            return rs;
        }


        public async void SetData(BookCatalog catalog)
        {
            if (IsLoading)
            {
                return;
            }

            IsLoading = true;
            preHtmlHttp.HttpClientCancleRequest();
            this.NextPageContent = "";
            this.PrePageContent = "";

            try
            {
                string html = await GetHtmlData(catalog, htmlHttp);
                if (html != null)
                {
                    SetCurrentContent(catalog, html);
                }
                else
                {
                    if (NavigationService.ContentFrame.Content is BookContentPage)
                    {
                        ToastHeplper.ShowMessage("未能获取正文内容");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        public void SetCurrentContent(BookCatalog catalog, string html)
        {
            if (catalog == null)
            {
                return;
            }
            this.CurrentCatalogContent = html;
            this.CurrentCatalog = catalog;

            this.BookEntity.LastReadChapterName = catalog.CatalogName;
            this.BookEntity.LastReadChapterUrl = catalog.CatalogUrl;

            if (BookEntity.CatalogList != null && BookEntity.CatalogList.Count > 0)
            {
                this.CurrentCatalogIndex = BookEntity.CatalogList.IndexOf(BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == catalog.CatalogUrl)) + 1;
                this.NextCatalog = GetNextCatalog();
            }
            this.ContentTitle = BookEntity.LastReadChapterName;

            if (!this.BookEntity.IsLocal)
            {
                //添加小说到历史记录
                ViewModelInstance.Instance.EverReadBookPageViewModelInstance.AddToHistoryList(BookEntity);
                // ToastHeplper.ShowMessage("正文加载完毕");
            }
            else
            {
                DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), BookEntity);
            }

            if (string.IsNullOrEmpty(NextCatalogContent) && ViewModelInstance.Instance.SettingPageViewModelInstance.IsPreLoad)
            {
                GetNextCatalogHtmlData();
            }
            if (CurrentCatalogParagraphs != null)
            {
                this.CurrentCatalogParagraphs.Clear();
            }
            this.CurrentCatalogParagraphs = SplitString(html);
            this.CurrentCatalogPages = GetContentPages(this.CurrentCatalogParagraphs);
            SetContentPage();
        }


        private async Task<string> GetHtmlData(BookCatalog catalog, HttpHelper http)
        {
            string resultHtml = null;
            if (catalog == null) return null;
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


        /// <summary>
        /// 获取下一章
        /// </summary>
        /// <returns></returns>
        private BookCatalog GetNextCatalog()
        {
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
            return tempcatalog;
        }


        /// <summary>
        /// 获取上一章
        /// </summary>
        /// <returns></returns>
        private BookCatalog GetPreCatalog()
        {
            if (this.BookEntity.CatalogList == null || this.BookEntity.CatalogList.Count == 0)
            {
                return null;
            }

            preHtmlHttp.HttpClientCancleRequest();

            var tempcatalog = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == this.CurrentCatalog.CatalogUrl);
            int index = this.BookEntity.CatalogList.IndexOf(tempcatalog);

            if (tempcatalog != null && this.BookEntity.CatalogList.Count > 1)
            {
                tempcatalog = this.BookEntity.CatalogList[this.BookEntity.CatalogList.IndexOf(tempcatalog) - 1];
            }

            return tempcatalog;
        }


        public void GetNextCatalogHtmlData()
        {
            Task.Run(async () =>
            {
                string resultHtml = null;
                try
                {
                    if (isPreLoading) return;
                    isPreLoading = true;
                    resultHtml = await GetHtmlData(GetNextCatalog(), preHtmlHttp);
                }
                catch (Exception)
                {
                    resultHtml = null;
                }
                finally
                {
                    isPreLoading = false;

                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        NextCatalogContent = resultHtml;
                    });
                }
            });
        }


        private void SetBookCatalogList()
        {
            Task.Run(async () =>
           {
               try
               {
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
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

                       var result = await Services.AnalysisBookCatalogList.GetCatalogList(BookEntity.CatalogListUrl, this.BookEntity.BookID, catalogsHttp);

                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
                       {
                           this.BookEntity.Description = result.Item2;
                           this.BookEntity.Cover = result.Item3;
                       });
                       List<BookCatalog> list = result.Item1;
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

                       }
                   }
               }
               catch (Exception ex)
               {
                   Debug.WriteLine(ex.Message);
               }
               finally
               {
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
                 {
                     try
                     {
                         if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                         {
                             this.TotalCatalogCount = this.BookEntity.CatalogList.Count;
                             this.CurrentCatalogIndex = BookEntity.CatalogList.IndexOf(BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == CurrentCatalog.CatalogUrl)) + 1;

                             IsSwitchButtonShow = true;
                             IsLoadingCatalogList = false;

                             if (isClickCtalog && IsSwitchButtonShow)
                             {
                                 IsLoading = false;
                                 NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                                 isClickCtalog = false;
                             }
                         }
                     }
                     catch (Exception ex)
                     {

                     }
                 });
               }
           }).ContinueWith((obj) =>
         {
             if (string.IsNullOrEmpty(NextCatalogContent) && ViewModelInstance.Instance.SettingPageViewModelInstance.IsPreLoad)
             {
                 if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                 {
                     NextCatalog = GetNextCatalog();
                     NextCatalogContent = null;
                     GetNextCatalogHtmlData();
                 }
             }
         });
        }

        private string SetBookCataologListUrl(string catalogUrl, string html = null)
        {
            var catalogListUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogListUrl(catalogUrl);

            return catalogListUrl;
        }


        public ObservableCollection<string> GetContentPages(ObservableCollection<string> list)
        {
            if (list == null || list.Count == 0)
            {
                return null;
            }
            var paragraphs = list;

            int linesCount = (int)(ContentContainerHeitht / PerTextHeight);
            int perLineCount = (int)(ContentContainerWith / PerTextWidth);


            if ((ContentContainerHeitht % PerTextHeight) / PerTextHeight > 0.8)
            {
                linesCount = linesCount + 1;
            }
            ObservableCollection<string> pages = new ObservableCollection<string>();

            try
            {
                int i = 0;
                string tempPageContent = string.Empty;
                foreach (var str in paragraphs)
                {
                    string lineStr = string.Empty;
                    var chars = str.ToArray();
                    var tempList = chars.ToList();
                    foreach (var word in tempList)
                    {
                        lineStr += word;
                        if (lineStr.Length == perLineCount)
                        {
                            tempPageContent += lineStr + "\r";
                            lineStr = string.Empty;
                            i++;
                            if (i == linesCount)
                            {
                                pages.Add(tempPageContent);
                                tempPageContent = string.Empty;
                                i = 0;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(lineStr))
                    {
                        tempPageContent += lineStr + "\r";
                        lineStr = string.Empty;
                        i++;
                        if (i == linesCount)
                        {
                            pages.Add(tempPageContent);
                            tempPageContent = string.Empty;
                            i = 0;
                        }
                    }
                    if (paragraphs.IndexOf(str) == paragraphs.Count - 1 && !string.IsNullOrEmpty(tempPageContent))
                    {
                        pages.Add(tempPageContent);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return pages;
        }

        public void SetContentPage(int index = 0, int count = 0)
        {
            try
            {
                if (CurrentCatalogPages != null && CurrentCatalogPages.Count > 0)
                {
                    if (index != 0 && count != 0)
                    {
                        this.CurrentPagIndex = (int)((double)index / (double)count * CurrentCatalogPages.Count);
                    }
                    else
                    {
                        this.CurrentPagIndex = 1;
                    }
                    if (this.CurrentPagIndex < 1 || this.CurrentPagIndex > CurrentCatalogPages.Count)
                    {
                        this.CurrentPagIndex = 1;
                    }

                    this.CurrentPageContent = CurrentCatalogPages[this.CurrentPagIndex - 1];
                    this.PrePageContent = null;
                    this.NextPageContent = CurrentCatalogPages.Count >= CurrentPagIndex + 1 ? CurrentCatalogPages[CurrentPagIndex] : null;
                    TotalPagCount = CurrentCatalogPages.Count;
                }
                else
                {
                    this.CurrentPagIndex = 1;
                    TotalPagCount = 1;
                    this.CurrentPageContent = null;
                    this.PrePageContent = null;
                    this.NextPageContent = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                this.CurrentPagIndex = 1;
                TotalPagCount = CurrentCatalogPages.Count;
                this.CurrentPageContent = CurrentCatalogPages[this.CurrentPagIndex - 1];
                this.PrePageContent = null;
                this.NextPageContent = null;
            }
        }


        public ObservableCollection<string> SplitString(string str)
        {
            ObservableCollection<string> strList = new ObservableCollection<string>();
            string[] lists = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lists.Count(); i++)
            {
                if (!string.IsNullOrEmpty(lists[i]))
                {
                    strList.Add("　　" + lists[i].Trim());
                }
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
            await Task.Run(() =>
          {
              try
              {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
        /// 目录
        /// </summary>
        private ICommand m_CatalogCommand;
        public ICommand CatalogCommand
        {
            get
            {
                return m_CatalogCommand ?? (m_CatalogCommand = new RelayCommand<bool>((str) =>
               {
                   try
                   {
                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
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

                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"> 0  上一章  1 下一章</param>
        public void OnSwtichCommand(object str, bool page = false)
        {
            if (IsLoading) return;
            try
            {
                if (this.BookEntity.CatalogList == null || this.BookEntity.CatalogList.Count <= 1)
                {
                    ToastHeplper.ShowMessage("暂未获取到章节列表，无法切换");
                    return;
                }

                BookCatalog temp = null;
                //上一章
                if (str.ToString().Equals("0"))
                {
                    temp = GetPreCatalog();
                    this.NextCatalogContent = this.CurrentCatalogContent;
                }
                //下一章
                else if (str.ToString().Equals("1"))
                {
                    if (!string.IsNullOrEmpty(NextCatalogContent))
                    {
                        string tempStr = NextCatalogContent;
                        NextCatalogContent = null;
                        SetCurrentContent(NextCatalog, tempStr);
                        return;
                    }
                    else
                    {
                        temp = GetNextCatalog();
                    }
                }
                else if (str is BookCatalog)
                {
                    temp = str as BookCatalog;
                    this.NextCatalogContent = null;
                }

                if (temp != null && temp.CatalogName != null && temp.CatalogUrl != null)
                {
                    SetData(temp);
                }
                else
                {
                    NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                    return;
                }
            }
            catch (Exception)
            {
                NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
            }
        }

        /// </summary>
        private RelayCommand<object> m_DwonLoadhCommand;
        public RelayCommand<object> DwonLoadhCommand
        {
            get
            {
                return m_DwonLoadhCommand ?? (m_DwonLoadhCommand = new RelayCommand<object>(OnDwonLoadhCommand));
            }
        }

        private void OnDwonLoadhCommand(object obj)
        {
            if (IsLoading) return;
            if (this.BookEntity != null && this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
            {
                var result = DBLocalBook.GetAllLocalBookList(AppDataPath.GetLocalBookDBPath());
                if (result != null && result.FirstOrDefault(p => p.BookID == this.BookEntity.BookID) != null)
                {
                    ToastHeplper.ShowMessage("该图书已经下载过");
                }

                ViewModelInstance.Instance.DownLoadCenterViewModelInstance.AddNewDownloadItem(this.BookEntity);
            }
            else
            {
                ToastHeplper.ShowMessage("该图书暂时无法下载");
            }
        }
        #endregion
    }
}
