using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookCatalogPageViewModel : BaseViewModel, IViewModel
    {
        private bool IsLocal { get; set; }

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
        public string CatalogPageUrl
        {
            get; set;
        }

        public BookEntity CurrentBookEntity { get; set; }


        private ObservableCollection<BookCatalog> m_CatalogList;
        /// <summary>
        /// 目录集合
        /// </summary>
        public ObservableCollection<BookCatalog> CatalogList
        {
            get
            {
                if (m_CatalogList == null)
                {
                    m_CatalogList = new ObservableCollection<BookCatalog>();
                }
                return m_CatalogList;
            }
            set
            {
                this.SetProperty(ref this.m_CatalogList, value);
            }
        }

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

        HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void RefreshData(object obj = null)
        {
            object[] para = obj as object[];
            if (para[0] != null && para[0].ToString().Equals(this.CatalogPageUrl))
            {
                return;
            }
            this.CatalogList.Clear();

            this.CatalogPageUrl = para[0].ToString();
            BookEntity temp = para[1] as BookEntity;
            this.CurrentBookEntity = new BookEntity()
            {
                BookID = temp.BookID,
                BookName = temp.BookName,
                ChapterName = temp.ChapterName,
                ChapterUrl = temp.ChapterUrl,
                CatalogUrl = temp.CatalogUrl,
                LyWeb = temp.LyWeb,
                UpdateTime = temp.UpdateTime,
                CatalogList = temp.CatalogList,
            };

            this.ContentTitle = CurrentBookEntity.BookName + "  目录";
            if (this.CurrentBookEntity.CatalogList != null && this.CurrentBookEntity.CatalogList.Count > 0)
            {
                int i = 0;
                foreach (var item in this.CurrentBookEntity.CatalogList)
                {
                    item.Index = i;
                    i++;
                    item.CatalogUrl = Path.Combine(CatalogPageUrl, item.CatalogUrl);
                    this.CatalogList.Add(item);
                }
            }
            else
            {
                SetData(CatalogPageUrl);
            }
        }

        private void SetData(string url)
        {
            Task.Run(async () =>
            {
                string html = await GetHtmlData(url);
                return html;
            }).ContinueWith(async (result) =>
           {
               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 if (result.Result != null && SetCatalogList(result.Result.ToString(), url))
                 {
                     CommonMethod.ShowMessage("已加载目录数据");
                 }
                 else
                 {
                     CommonMethod.ShowMessage("未能获取目录数据");
                 }
             });
           });
        }

        public async Task<string> GetHtmlData(string url)
        {
            string html = string.Empty;

            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            try
            {
                html = await http.WebRequestGet(url, true);
            }
            catch (Exception ex)
            {
                html = null;
            }
            finally
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = false;
                });
            }

            return html;
        }


        private bool SetCatalogList(string html, string url)
        {
            var result = AnalysisBookCatalogList.GetCatalogListByHtml(html, url);
            if (result != null)
            {
                if (this.CatalogList != null)
                {
                    this.CatalogList.Clear();
                }
                int i = 0;
                foreach (var item in result)
                {
                    item.Index = i;
                    i++;
                    item.CatalogUrl = Path.Combine(CatalogPageUrl, item.CatalogUrl);
                    item.BookID = this.CurrentBookEntity.BookID;
                    this.CatalogList.Add(item);
                }
                this.CurrentBookEntity.CatalogList = this.CatalogList.ToList();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 选中目录
        /// </summary>
        public ICommand CatalogSelectedCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
                {
                    if (IsLoading) return;

                    if (str != null)
                    {
                        try
                        {
                            BookCatalog catalog = str as BookCatalog;
                            if (catalog == null) return;

                            CurrentBookEntity.ChapterUrl = catalog.CatalogUrl;
                            CurrentBookEntity.ChapterName = catalog.CatalogName;

                            ViewModelInstance.Instance.BookContentPageViewModelInstance.BookEntity = CurrentBookEntity;
                            //ViewModelInstance.Instance.BookContentPageViewModelInstance.CurrentCatalog = catalog;
                            //ViewModelInstance.Instance.BookContentPageViewModelInstance.CatalogList = this.CatalogList;

                            //ViewModelInstance.Instance.BookContentPageViewModelInstance.SetData(catalog);
                            //// NavigationService.GoBack(null, null);
                            MenuModel menu = new MenuModel() { MenuName = catalog.CatalogName, MenuType = typeof(BookContentPage) };

                            ViewModelInstance.Instance.MainPageViewModelInstance.NavigateToPage(menu, CurrentBookEntity);

                            //   NavigationService.NavigateTo(menu, new object[] { "1", CurrentBookEntity, this.CatalogList, catalog });

                            //MenuModel menu = new MenuModel() { MenuName = catalog.CatalogName, MenuType = typeof(BookContentPage) };

                            //ViewModelInstance.Instance.MainPageViewModelInstance.NavigateToPage(menu, CurrentBookEntity);
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }
        }


        /// </summary>
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return new RelayCommand<object>(OnRefreshCommand);
            }
        }

        private async void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                SetData(CatalogPageUrl);
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




        /// </summary>
        public RelayCommand<object> DwonLoadhCommand
        {
            get
            {
                return new RelayCommand<object>(OnDwonLoadhCommandd);
            }
        }


        private void OnDwonLoadhCommandd(object obj)
        {
            if (IsLoading) return;
            if (this.CurrentBookEntity != null && this.CurrentBookEntity.CatalogList != null && this.CurrentBookEntity.CatalogList.Count > 0)
            {
                Services.CommonMethod.ShowMessage("开始下载图书，请耐心等待。");
                ViewModelInstance.Instance.DownLoadCenterViewModelInstance.AddNewDownloadItem(this.CurrentBookEntity);
            }

        }
    }
}
