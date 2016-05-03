using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
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
        public bool IsLocal { get; set; }

        private IconElement m_RefreshIcon = new SymbolIcon(Symbol.Refresh);
        public IconElement RefreshIcon
        {
            get
            {
                return m_RefreshIcon;
            }
            set
            {
                SetProperty(ref m_RefreshIcon, value);
            }
        }

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
                if (m_IsLoading == false)
                {
                    this.RefreshIcon = new SymbolIcon(Symbol.Refresh);
                }
                else
                {
                    this.RefreshIcon = new SymbolIcon(Symbol.Cancel);
                }
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

        public string CatalogListUrl
        {
            get; set;
        }


        public bool m_IsCatalogMenuShow = true;
        public bool IsCatalogMenuShow
        {
            get
            {
                return m_IsCatalogMenuShow;
            }
            set
            {
                SetProperty(ref m_IsCatalogMenuShow, value);
            }
        }

        public StringBuilder m_TextContent;
        public StringBuilder TextContent
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




        public bool m_DirectionArrowShow;
        public bool DirectionArrowShow
        {
            get
            {
                return m_DirectionArrowShow;
            }
            set
            {
                SetProperty(ref m_DirectionArrowShow, value);
            }
        }



        public int m_FontSize;
        public int ContentFontSzie
        {
            get
            {
                return m_FontSize;
            }
            set
            {
                SetProperty(ref m_FontSize, value);
            }
        }

        //private ObservableCollection<BookCatalog> m_CatalogList;
        ///// <summary>
        ///// 目录集合
        ///// </summary>
        //public ObservableCollection<BookCatalog> CatalogList
        //{
        //    get
        //    {
        //        return m_CatalogList;
        //    }
        //    set
        //    {
        //        this.SetProperty(ref this.m_CatalogList, value);
        //        //if (CatalogList != null && CatalogList.Count > 0)
        //        //{
        //        //    this.IsCatalogMenuShow = true;
        //        //    this.DirectionArrowShow = true;
        //        //}
        //    }
        //}

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
                if (this.m_BookEntity != null)
                {
                    this.ContentTitle = this.m_BookEntity.ChapterName;
                }
            }
        }


        public BookContentPageViewModel()
        {
            this.ContentFontSzie = ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie;
        }

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }
        HttpHelper http = new HttpHelper();


        public void RefreshData(object obj)
        {

            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                return;
            }

            this.ContentTitle = entity.BookName + "_" + entity.ChapterName;

            this.TextContent = null;

            this.IsCatalogMenuShow = false;
            this.DirectionArrowShow = false;


            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }

            this.BookEntity = entity;

            this.CurrentCatalog = new BookCatalog() { BookID = entity.BookID, CatalogName = entity.ChapterName, CatalogUrl = entity.ChapterUrl, LyWeb = new Uri(entity.ChapterUrl).Authority };

            SetData(CurrentCatalog);
        }

        public void SetData(BookCatalog catalog)
        {
            Task.Run(async () =>
           {
               if (IsLocal)
               {
                   string html = await GetCatafromDatabase(catalog);
                   return html;
               }
               else
               {
                   string html = await GetHtmlData(catalog.CatalogUrl);
                   return html;
               }

           }).ContinueWith(async (result) =>
          {
              string html = result.Result;
              await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  if (html != null)
                  {
                      SetTextContent(html);
                      Services.CommonMethod.ShowMessage("正文加载完毕");
                  }
                  else
                  {
                      Services.CommonMethod.ShowMessage("未能获取正文内容");
                  }
              });
          });
        }


        private async void SetBookCataologListUrl(string html, string catalogUrl)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                     {
                         IsCatalogMenuShow = true;
                         DirectionArrowShow = true;
                     }
                     else
                     {
                         string catalogListUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogListUrl(html, catalogUrl);

                         if (string.IsNullOrEmpty(catalogListUrl))
                         {
                             IsCatalogMenuShow = false;
                         }
                         else
                         {
                             CatalogListUrl = catalogListUrl;
                             IsCatalogMenuShow = true;
                         }
                     }
                 });
        }

        private void SetTextContent(string html)
        {
            this.ContentTitle = this.BookEntity.BookName + "_" + this.CurrentCatalog.CatalogName;

            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            int textCount = ((int)(width / this.ContentFontSzie)) * ((int)height / this.ContentFontSzie);
            int count = (int)(html.Length / textCount) + 1;

            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }
            List<string> strList = SplitString(html, count);
            for (int i = 0; i < strList.Count; i++)
            {
                this.ContentListt.Add(strList[i]);
            }
        }

        private List<string> SplitString(string str, int count = 20)
        {
            List<string> strList = new List<string>();
            if (str.Length < 1000)
            {
                strList.Add(str);
            }
            else
            {
                int perCount = str.Length / count + 1;
                for (int i = 0; i < count; i++)
                {
                    string tempStr = null;
                    if (i == count - 1)
                    {
                        tempStr = str.Substring(perCount * i, str.Length - perCount * i);
                    }
                    else
                    {
                        tempStr = str.Substring(perCount * i, perCount);
                    }
                    strList.Add(tempStr);
                }
            }

            return strList;
        }


        /// <summary>
        /// 网络请求数据
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public async Task<string> GetHtmlData(string catalogUrl)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });

            string html = null;
            string content = null;
            try
            {
                html = await http.WebRequestGet(catalogUrl, false);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                SetBookCataologListUrl(html, catalogUrl);
                content = Services.AnalysisContentHtmlService.AnalysisContentHtml(html, catalogUrl);
                if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }
                List<string> lists = AnalysisPagingUrlFromUrl.GetPagingUrlListFromUrl(html, catalogUrl);
                if (lists != null)
                {
                    foreach (var url in lists)
                    {
                        string temp = await http.WebRequestGet(url, false);
                        temp = await http.WebRequestGet(url, false);
                        if (temp != null)
                        {
                            temp = Services.AnalysisContentHtmlService.AnalysisContentHtml(temp, url);
                        }
                        if (temp != null)
                        {
                            content += temp.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                content = null;
            }
            finally
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = false;
                });
            }
            return content;
        }




        /// <summary>
        /// 从数据库获取数据
        /// <param name="catalog"></param>
        /// <param name="isBackGround"></param>
        /// <returns></returns>
        private async Task<string> GetCatafromDatabase(BookCatalog catalog, bool isBackGround = true)
        {
            await Task.Delay(1000);
            return null;
        }

        /// <summary>
        /// 字体增大
        /// </summary>
        public ICommand FontIncreaseCommand
        {
            get
            {
                return new RelayCommand<bool>(async (str) =>
               {
                   var list = new ObservableCollection<string>();
                   foreach (var item in this.ContentListt)
                   {
                       list.Add(item);
                   }
                   this.ContentListt.Clear();
                   ViewModelInstance.Instance.SettingPageViewModelInstance.SetFontSize(true, false);
                   // this.ContentFontSzie = ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie;
                   foreach (var item in list)
                   {
                       this.ContentListt.Add(item);
                       await Task.Delay(5);
                   }
               });
            }
        }

        /// <summary>
        /// 字体减小
        /// </summary>
        public ICommand FontDecreaseCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
             {
                 var list = new ObservableCollection<string>();
                 foreach (var item in this.ContentListt)
                 {
                     list.Add(item);
                 }
                 this.ContentListt.Clear();
                 ViewModelInstance.Instance.SettingPageViewModelInstance.SetFontSize(false, false);
                 foreach (var item in list)
                 {
                     this.ContentListt.Add(item);
                 }
             });
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    if (IsLoading)
                    {
                        CancleHttpRequest();
                    }
                    else
                    {
                        SetData(CurrentCatalog);
                    }
                });
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    NavigationService.GoBack(null, null);
                });
            }
        }


        /// <summary>
        /// 目录
        /// </summary>
        public ICommand CatalogCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    // this.IsNeedRefresh = false;
                    MenuModel menu = new MenuModel() { MenuName = BookEntity.BookName, MenuType = typeof(BookCatalogPage) };
                    ViewModelInstance.Instance.MainPageViewModelInstance.NavigateToPage(menu, new object[] { this.CatalogListUrl, this.BookEntity });
                });
            }
        }

        /// <summary>
        /// 切换章节
        /// </summary>
        public ICommand SwitchCatalogCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
                {
                    OnSwtichCommand(str);
                });
            }
        }

        private void OnSwtichCommand(object str)
        {
            if (IsLoading) return;

            //上一章
            if (str.ToString().Equals("0"))
            {
                if (this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) == 0)
                {
                    Services.CommonMethod.ShowMessage("已经是第一章。");
                    return;
                }
                if (this.BookEntity.CatalogList.Count < 2)
                {
                    return;
                }
                this.CurrentCatalog = this.BookEntity.CatalogList[this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) - 1];
            }
            //下一章
            else /*if (str.ToString().Equals("01"))*/
            {
                if (this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) == this.BookEntity.CatalogList.Count - 1)
                {
                    Services.CommonMethod.ShowMessage("已经是最后一章。");
                    return;
                }
                if (this.BookEntity.CatalogList.Count < 2)
                {
                    return;
                }
                this.CurrentCatalog = this.BookEntity.CatalogList[this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) + 1];
            }

            if (this.CurrentCatalog != null && CurrentCatalog.CatalogName != null && this.CurrentCatalog.CatalogUrl != null)
            {
                this.BookEntity.ChapterName = this.CurrentCatalog.CatalogName;
                this.BookEntity.ChapterUrl = this.CurrentCatalog.CatalogUrl;
                SetData(CurrentCatalog);
            }
        }


    }
}
