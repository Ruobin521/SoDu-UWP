﻿using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
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
using Windows.UI.Core;

namespace Sodu.ViewModel
{
    public class LocalBookPageViewModel : BaseViewModel, IViewModel
    {

        private static readonly object obj = new object();

        private ObservableCollection<BookEntity> m_LocalBookList;
        ///本地图书列表
        public ObservableCollection<BookEntity> LocalBookList
        {
            get
            {
                if (m_LocalBookList == null)
                {
                    m_LocalBookList = new ObservableCollection<BookEntity>();
                }
                return m_LocalBookList;
            }
            set
            {
                this.SetProperty(ref this.m_LocalBookList, value);
            }
        }
        private bool m_IsEditing;
        public bool IsEditing
        {
            get
            {
                return m_IsEditing;
            }
            set
            {
                this.SetProperty(ref this.m_IsEditing, value);
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
                this.SetProperty(ref this.m_IsLoading, value);
            }
        }

        private bool m_IsUpdating;
        public bool IsUpdating
        {
            get
            {
                return m_IsUpdating;
            }
            set
            {
                this.SetProperty(ref this.m_IsUpdating, value);
            }
        }


        private bool m_IsChecking;
        public bool IsChecking
        {
            get
            {
                return m_IsChecking;
            }
            set
            {
                this.SetProperty(ref this.m_IsChecking, value);
            }
        }


        private string m_ContentTitle = "本地小说";
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
        private string m_ProcessMessage;
        public string ProcessMessage
        {
            get
            {
                return m_ProcessMessage;
            }

            set
            {
                SetProperty(ref m_ProcessMessage, value);
            }
        }


        public void CancleHttpRequest()
        {

        }

        public void InitData(object obj = null)
        {
            if (IsLoading || IsChecking || IsUpdating)
            {
                return;
            }
            GetLocalBook();
        }

        public void GetLocalBook()
        {
            this.LocalBookList.Clear();
            Task.Run(async () =>
        {
            var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
            if (result != null)
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in result)
                    {
                        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, item.BookID + ".db");
                        if (File.Exists(path))
                        {
                            item.IsLocal = true;
                            this.LocalBookList.Add(item);

                            var list = Database.DBBookCatalog.SelectBookCatalogs(AppDataPath.GetBookDBPath(item.BookID), item.BookID);
                            if (list != null)
                            {
                                foreach (var catalog in list)
                                {
                                    if (item.CatalogList == null)
                                    {
                                        item.CatalogList = new ObservableCollection<BookCatalog>();
                                    }
                                    item.CatalogList.Add(catalog);
                                }
                            }
                        }
                        else
                        {
                            Database.DBLocalBook.DeleteLocalBookByBookID(Constants.AppDataPath.GetLocalBookDBPath(), item.BookID);
                        }
                    }
                });
            }
        }).ContinueWith(obj =>
        {
            CheckUpdate();
        });
        }

        private void CheckUpdate()
        {
            if (IsChecking) return;
            if (this.LocalBookList.Count < 1) return;

            foreach (var item in LocalBookList)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.CatalogListUrl))
                        {
                            var list = await Services.AnalysisBookCatalogList.GetCatalogList(item.CatalogListUrl, item.BookID, new HttpHelper());
                            if (list != null)
                            {
                                var last = Database.DBBookCatalog.SelectBookCatalogs(AppDataPath.GetBookDBPath(item.BookID), item.BookID);

                                var undownLoad = list.FindAll(p => last.FirstOrDefault(m => m.CatalogUrl == p.CatalogUrl) == null);

                                if (undownLoad != null && undownLoad.Count > 0)
                                {
                                    item.UnDownloadCatalogList = new ObservableCollection<BookCatalog>();
                                    undownLoad.ForEach(p => item.UnDownloadCatalogList.Add(p));
                                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                        item.UnReadCountData = "  " + undownLoad.Count.ToString();
                                    });
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }

        public LocalBookPageViewModel()
        {

        }


        /// <summary>
        /// 全选，全不选
        /// </summary>
        public RelayCommand<object> EditCommand
        {
            get
            {
                return new RelayCommand<object>(
                 (obj) =>
                 {

                     if (IsLoading) return;
                     OnEditCommand();
                 }
                    );
            }
        }

        public void OnEditCommand()
        {
            if (IsLoading) return;

            if (this.LocalBookList == null || this.LocalBookList.Count < 1)
            {
                IsEditing = false;
                return;
            }
            if (!IsEditing)
            {
                IsEditing = true;
                foreach (var item in LocalBookList)
                {
                    item.IsInEdit = true;
                }
            }
            else
            {
                foreach (var item in LocalBookList)
                {
                    item.IsInEdit = false;
                    item.IsSelected = false;
                }
                IsEditing = false;
            }
        }


        /// <summary>
        /// 全选，全不选
        /// </summary>
        public RelayCommand<object> UpdateCommand
        {
            get
            {
                return new RelayCommand<object>(
                 (obj) =>
                 {

                     if (IsUpdating) return;
                     OnUpdateCommand();
                 }
                    );
            }
        }

        private void OnUpdateCommand()
        {
            if (IsLoading) return;
            if (LocalBookList == null || LocalBookList.Count == 0) return;

            IsLoading = true;
            IsUpdating = true;

            Task[] tasks = new Task[LocalBookList.Count];

            for (int j = 0; j < LocalBookList.Count; j++)
            {
                var item = LocalBookList[j];
                tasks[j] = Task.Factory.StartNew(async () =>
                 {
                     try
                     {
                         if (item.UnDownloadCatalogList == null || item.UnDownloadCatalogList.Count == 0)
                         {
                             await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                             {
                                 item.UnReadCountData = "";
                             });
                             return;
                         }
                         var list = new List<BookCatalog>();
                         item.UnDownloadCatalogList.ToList().ForEach(p => list.Add(p));
                         for (int i = 0; i < list.Count; i++)
                         {
                             if (!IsUpdating)
                             {
                                 await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                 {
                                     item.UnReadCountData = "  " + item.UnDownloadCatalogList.Count.ToString();
                                 });
                                 break;
                             }

                             await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                             {
                                 item.UnReadCountData = "正在更新(" + "剩余" + (list.Count - i).ToString() + ")";
                             });

                             string html = await AnalysisContentService.GetHtmlContent(new HttpHelper(), list[i].CatalogUrl);
                             // item.CatalogContentGUID = item.BookID + item.Index.ToString();
                             BookCatalogContent content = new BookCatalogContent()
                             {
                                 BookID = item.BookID,
                                 CatalogUrl = list[i].CatalogUrl,
                                 Content = html
                             };
                             await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                             {
                                 item.NewestChapterName = list[i].CatalogName;
                                 item.NewestChapterUrl = list[i].CatalogUrl;
                                 item.UpdateTime = DateTime.Now.ToString();
                                 item.CatalogList.Add(list[i]);
                             });

                             Database.DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetBookDBPath(item.BookID), list[i]);
                             Database.DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetBookDBPath(item.BookID), content);
                             Database.DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), item);
                             item.UnDownloadCatalogList.Remove(list[i]);

                             if (i == list.Count - 1)
                             {
                                 await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                 {
                                     item.UnReadCountData = null;
                                 });
                             }
                         }
                     }
                     catch (Exception ex)
                     {

                     }
                     finally
                     {
                         await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             IsUpdating = false;
                             IsLoading = false;
                         });
                     }
                 });

            }
        }

        /// <summary>
        /// 下架
        /// </summary>
        public RelayCommand<object> RemoveBookCommand
        {
            get
            {
                return new RelayCommand<object>(OnRemoveBookFromShelfCommand);
            }
        }
        private async void OnRemoveBookFromShelfCommand(object obj)
        {
            if (IsLoading) return;
            if (!IsEditing)
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog(" \n请点击编辑按钮，并选择需要删除的小说。") { Title = "删除本地缓存" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                {
                    return;
                }));
                await msgDialog.ShowAsync();
            }
            else
            {

                List<BookEntity> removeList = new List<BookEntity>();

                foreach (var item in LocalBookList)
                {
                    if (item.IsSelected == true)
                    {
                        removeList.Add(item);
                    }
                }

                if (removeList.Count > 0)
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定删除" + removeList.Count + "本小说？") { Title = "删除本地缓存" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        RemoveBookList(removeList);
                    }));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();

                }
                else
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n请勾选需要删除的小说。") { Title = "删除本地缓存" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
            }
        }

        private async void RemoveBookList(List<BookEntity> removeList)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            await Task.Run(async () =>
             {
                 foreach (var item in removeList)
                 {
                     try
                     {
                         System.IO.File.Delete(Constants.AppDataPath.GetBookDBPath(item.BookID));

                         await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             this.LocalBookList.Remove(item);
                         });
                     }
                     catch (Exception)
                     {
                         ToastHeplper.ShowMessage(item.BookName + "删除失败，请重新尝试");
                         continue;
                     }
                 }
             });
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = false;
                OnEditCommand();
            });


        }

        public RelayCommand<object> BookItemSelectedCommand
        {
            get
            {
                return new RelayCommand<object>(OnBookItemSelectedCommand);
            }
        }

        private void OnBookItemSelectedCommand(object obj)
        {
            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                ToastHeplper.ShowMessage("获取数据有误");
                return;
            }

            if (IsEditing)
            {
                entity.IsSelected = !entity.IsSelected;
                return;
            }
            else
            {
                if (string.IsNullOrEmpty(entity.LastReadChapterUrl))
                {
                    var item = entity.CatalogList.FirstOrDefault();
                    entity.LastReadChapterUrl = item.CatalogUrl; ;
                    entity.LastReadChapterName = item.CatalogName;
                }
                NavigationService.NavigateTo(typeof(BookContentPage), entity);
            }
        }

        public RelayCommand<object> CancleUpdateCommand
        {
            get
            {
                return new RelayCommand<object>(OnCancleUpdateCommand);
            }
        }
        private async void OnCancleUpdateCommand(object obj)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                this.IsUpdating = false;
                IsLoading = false;
            });
        }
    }
}
