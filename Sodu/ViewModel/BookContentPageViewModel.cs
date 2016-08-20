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



        public string m_CurrentPageContent;
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

        public int m_CatalogCount;
        public int CatalogCount
        {
            get
            {
                return m_CatalogCount;
            }
            set
            {
                SetProperty(ref m_CatalogCount, value);
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


        public ObservableCollection<string> m_ContentList;
        public ObservableCollection<string> ContentList
        {
            get
            {
                if (m_ContentList == null)
                {
                    m_ContentList = new ObservableCollection<string>();
                }
                return m_ContentList;
            }
            set
            {
                SetProperty(ref m_ContentList, value);
            }
        }

        public ObservableCollection<string> m_ContentPages;
        /// <summary>
        /// 内容分页
        /// </summary>
        public ObservableCollection<string> ContentPages
        {
            get
            {
                if (m_ContentPages == null)
                {
                    m_ContentPages = new ObservableCollection<string>();
                }
                return m_ContentPages;
            }
            set
            {
                SetProperty(ref m_ContentPages, value);
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

        private double ContentContainerWith { get; set; }
        private double ContentContainerHeitht { get; set; }

        private double PerTextHeight { get; set; }
        private double PerTextWidth { get; set; }

        #endregion


        #region 方法

        public BookContentPageViewModel()
        {

        }

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
            isClickCtalog = false;

            TotalPagCount = 1;
            CurrentPagIndex = 1;

            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                return;
            }

            this.CurrentPageContent = "";
            this.NextPageContent = "";
            this.PrePageContent = "";


            if (!entity.IsLocal)
            {
                this.IsSwitchButtonShow = false;
            }
            else
            {
                this.IsSwitchButtonShow = true;

            }

            if (this.ContentList != null)
            {
                this.ContentList.Clear();
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
                if (this.BookEntity.CatalogList == null || BookEntity.CatalogList.Count == 0)
                {
                    SetBookCatalogList();
                }
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

            this.CurrentCatalogIndex = catalog.Index + 1;
            this.NextPageContent = "";
            this.PrePageContent = "";

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
               return resultHtml;

           }).ContinueWith(async (result) =>
         {
             try
             {
                 string html = result.Result;
                 await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
           {
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
           });
             }
             catch (Exception)
             {
                 if (NavigationService.ContentFrame.Content is BookContentPage)
                 {
                     ToastHeplper.ShowMessage("未能获取正文内容");
                 }
             }
             finally
             {
                 DispatcherHelper.CheckBeginInvokeOnUI(() =>
               {
                   IsLoading = false;
               });
             }
         });
        }


        private async void SetCurrentContent(BookCatalog catalog, string html)
        {
            if (!(NavigationService.ContentFrame.Content is BookContentPage)) return;
            ;
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
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
                           DispatcherHelper.CheckBeginInvokeOnUI(() =>
                         {
                             IsSwitchButtonShow = true;
                         });
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
                     IsLoadingCatalogList = false;
                     if (isClickCtalog && IsSwitchButtonShow)
                     {
                         IsLoading = false;
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
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
                 {
                     PreTextContent = html;
                 });
               }
           });

        }

        private string SetBookCataologListUrl(string catalogUrl, string html = null)
        {
            var catalogListUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogListUrl(catalogUrl);

            return catalogListUrl;
        }

        private void SetTextContent(string html)
        {
            this.ContentTitle = this.BookEntity.BookName + "_" + this.CurrentCatalog.CatalogName;

            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            if (ContentList != null)
            {
                this.ContentList.Clear();

            }
            var strList = SplitString(html);

            foreach (string str in strList)
            {
                ContentList.Add(str);
            }

            SetContentPage();

        }

        public void SetContentPage()
        {
            if (this.ContentList == null || ContentList.Count == 0)
            {
                return; ;
            }
            var paragraphs = this.ContentList;
            int linesCount = (int)(ContentContainerHeitht / PerTextHeight);
            int perLineCount = (int)(ContentContainerWith / PerTextWidth);


            if ((ContentContainerHeitht % PerTextHeight) / PerTextHeight > 0.8)
            {
                linesCount = linesCount + 1;
            }
            List<string> pages = new List<string>();

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
                return;
            }

            ContentPages.Clear();
            foreach (string str in pages)
            {
                ContentPages.Add(str);
            }

            if (ContentPages != null && ContentPages.Count > 0)
            {
                this.CurrentPageContent = ContentPages[0];
                this.CurrentPagIndex = 1;
                TotalPagCount = ContentPages.Count;
            }
            else
            {
                this.CurrentPagIndex = 1;
                TotalPagCount = 1;
            }

        }

        private List<string> SplitString(string str)
        {
            List<string> strList = new List<string>();
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
        /// 
        /// </summary>
        /// <param name="type"> 0 上一页  1 下一页</param>
        public void SwithContent(string type)
        {
            if (type == "0")
            {
                if (CurrentPagIndex == 1 || !ViewModelInstance.Instance.SettingPageViewModelInstance.IsReadByPageMode)
                {
                    OnSwtichCommand("0");
                }
                else
                {
                    CurrentPagIndex = CurrentPagIndex - 1;
                    this.CurrentPageContent = ContentPages[CurrentPagIndex - 1];

                    this.PrePageContent = CurrentPagIndex == 1 ? "" : ContentPages[CurrentPagIndex - 2];
                    this.NextPageContent = ContentPages.Count >= CurrentPagIndex ? ContentPages[CurrentPagIndex] : "";
                }
            }
            else
            {
                if (CurrentPagIndex == this.ContentPages.Count || !ViewModelInstance.Instance.SettingPageViewModelInstance.IsReadByPageMode)
                {
                    OnSwtichCommand("1");
                }
                else
                {
                    CurrentPagIndex = CurrentPagIndex + 1;
                    this.CurrentPageContent = ContentPages[CurrentPagIndex - 1];

                    this.NextPageContent = this.CurrentPagIndex == this.ContentPages.Count ? "" : ContentPages[CurrentPagIndex];
                    this.PrePageContent = ContentPages.Count >= CurrentPagIndex ? ContentPages[CurrentPagIndex - 2] : "";

                }
            }
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

                var last = this.BookEntity.CatalogList.LastOrDefault();
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
                    else
                    {
                        tempcatalog = this.BookEntity.CatalogList.LastOrDefault();
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
                    else
                    {
                        preHtmlHttp.HttpClientCancleRequest();
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
