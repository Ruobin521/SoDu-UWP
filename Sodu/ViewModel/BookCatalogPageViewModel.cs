﻿using GalaSoft.MvvmLight.Command;
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

        private BookEntity m_CurrentBookEntity;
        public BookEntity CurrentBookEntity
        {
            get
            {
                return m_CurrentBookEntity;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentBookEntity, value);
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


        HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void RefreshData(object obj = null)
        {
            BookEntity temp = obj as BookEntity;
            if (temp == null)
            {
                return;
            }

            this.CurrentBookEntity = new BookEntity()
            {
                BookID = temp.BookID,
                BookName = temp.BookName,
                NewestChapterName = temp.NewestChapterName,
                NewestChapterUrl = temp.NewestChapterUrl,
                CatalogListUrl = temp.CatalogListUrl,
                UpdateCatalogUrl = temp.UpdateCatalogUrl,
                LyWeb = temp.LyWeb,
                UpdateTime = temp.UpdateTime,
                CatalogList = temp.CatalogList,
            };

            this.ContentTitle = CurrentBookEntity.BookName + "  目录";

            if (CheckIfLoacalExist())
            {
                CanDownLoad = false;
            }
            else
            {
                CanDownLoad = true;
            }
        }

        private bool CheckIfLoacalExist()
        {
            bool rs = false;
            var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
            if (result.FirstOrDefault(p => p.BookID == this.CurrentBookEntity.BookID) != null)
            {
                rs = true;
            }

            return rs;
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

                            CurrentBookEntity.LastReadChapterUrl = catalog.CatalogUrl;
                            CurrentBookEntity.LastReadChapterName = catalog.CatalogName;


                            NavigationService.GoBack();

                            ViewModelInstance.Instance.BookContentPageViewModelInstance.OnSwtichCommand(catalog);
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
                SetCatalogData(CurrentBookEntity.CatalogListUrl);
            }
        }
        private async void SetCatalogData(string url)
        {
            bool rs = true;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            await Task.Run(async () =>
            {
                string html = await GetCatalogHtmlData(url);
                if (html != null && await SetCatalogList(html, url))
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        rs = true;
                    });
                }
            });
        }

        public async Task<string> GetCatalogHtmlData(string url)
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
        private async Task<bool> SetCatalogList(string html, string url)
        {
            bool rs = false;
            var result = AnalysisBookCatalogList.GetCatalogListByHtml(html, url);
            if (result != null)
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (this.CurrentBookEntity.CatalogList != null)
                    {
                        this.CurrentBookEntity.CatalogList.Clear();
                    }
                    else
                    {
                        this.CurrentBookEntity.CatalogList = new ObservableCollection<BookCatalog>();
                    }
                    foreach (var item in result)
                    {
                        item.BookID = this.CurrentBookEntity.BookID;
                        item.CatalogContentGUID = item.BookID + item.Index.ToString();
                        this.CurrentBookEntity.CatalogList.Add(item);
                    }
                    rs = true;
                });
            }
            return rs;
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
                    NavigationService.GoBack();
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
                var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
                if (result.FirstOrDefault(p => p.BookID == this.CurrentBookEntity.BookID) != null)
                {
                    Services.CommonMethod.ShowMessage("该图书已经下载过");
                }

                ViewModelInstance.Instance.DownLoadCenterViewModelInstance.AddNewDownloadItem(this.CurrentBookEntity);
            }

        }
    }
}
