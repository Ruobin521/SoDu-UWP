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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using GalaSoft.MvvmLight.Threading;

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
            IsLoading = true;
            this.LocalBookList.Clear();
            Task.Run(() =>
      {
          try
          {
              var result = DBLocalBook.GetAllLocalBookList(AppDataPath.GetLocalBookDBPath());
              if (result != null)
              {
                  foreach (var item in result)
                  {
                      string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, AppDataPath.LocalBookFolderName, item.BookID + ".db");

                      if (File.Exists(path))
                      {
                          var list = DBBookCatalog.SelectBookCatalogs(AppDataPath.GetBookDBPath(item.BookID), item.BookID);
                          if (list != null && list.Count > 0)
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
                          DispatcherHelper.CheckBeginInvokeOnUI(() =>
                          {
                              item.NewestChapterName = list.LastOrDefault().CatalogName;
                              item.NewestChapterUrl = list.LastOrDefault().CatalogUrl;
                              item.IsLocal = true;
                              this.LocalBookList.Add(item);
                          });
                      }
                      else
                      {
                          DBLocalBook.DeleteLocalBookByBookID(AppDataPath.GetLocalBookDBPath(), item.BookID);
                      }
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

      }).ContinueWith(obj =>
      {
          CheckUpdate();
      });
        }

        private void CheckUpdate()
        {
            if (IsChecking)
            {
                return;
            }
            if (this.LocalBookList.Count < 1) return;

            foreach (var item in LocalBookList)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(item.CatalogListUrl))
                        {
                            var result = await AnalysisBookCatalogList.GetCatalogList(item.CatalogListUrl, item.BookID, new HttpHelper());
                            if (result.Item1 != null)
                            {
                                var list = result.Item1;
                                var last = DBBookCatalog.SelectBookCatalogs(AppDataPath.GetBookDBPath(item.BookID), item.BookID);

                                var undownLoad = list.FindAll(p => last.FirstOrDefault(m => m.CatalogUrl == p.CatalogUrl) == null);

                                if (undownLoad != null && undownLoad.Count > 0)
                                {
                                    item.UnDownloadCatalogList = new ObservableCollection<BookCatalog>();
                                    undownLoad.ForEach(p => item.UnDownloadCatalogList.Add(p));
                                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                                  {
                                      item.UnReadCountData = "  " + undownLoad.Count.ToString();
                                  });
                                }

                            }
                        }
                    }
                    catch (Exception)
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
                     if (IsLoading) return;
                     if (LocalBookList == null || LocalBookList.Count == 0) return;
                     OnUpdateCommand();
                 }
                    );
            }
        }

        private void OnUpdateCommand()
        {
            var reslut = LocalBookList.FirstOrDefault(p => !string.IsNullOrEmpty(p.UnReadCountData));
            if (reslut != null)
            {
                ToastHeplper.ShowMessage("开始更新。");
            }
            else
            {
                ToastHeplper.ShowMessage("无更新，请稍后再试。");
                IsUpdating = false;
                return;
            }

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
                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                              {
                                  item.UnReadCountData = "  " + item.UnDownloadCatalogList.Count.ToString();
                              });
                                break;
                            }

                            DispatcherHelper.CheckBeginInvokeOnUI(() =>
                          {
                              item.UnReadCountData = "正在更新(" + "剩余" + (list.Count - i).ToString() + ")";
                          });

                            string html = await AnalysisContentService.GetHtmlContent(new HttpHelper(), list[i].CatalogUrl);
                            if (!string.IsNullOrEmpty(html))
                            {
                                BookCatalogContent content = new BookCatalogContent()
                                {
                                    BookID = item.BookID,
                                    CatalogUrl = list[i].CatalogUrl,
                                    Content = html
                                };
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                              {
                                  item.NewestChapterName = list[i].CatalogName;
                                  item.NewestChapterUrl = list[i].CatalogUrl;
                                  item.UpdateTime = DateTime.Now.ToString();
                                  item.CatalogList.Add(list[i]);
                              });

                                lock (obj)
                                {
                                    DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetBookDBPath(item.BookID), list[i]);
                                    DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetBookDBPath(item.BookID), content);
                                    DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), item);
                                }
                            }

                            item.UnDownloadCatalogList.Remove(list[i]);

                            if (i == list.Count - 1)
                            {
                                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                              {
                                  item.UnReadCountData = null;
                              });
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }

                    await Task.Factory.ContinueWhenAll(tasks, completedTasks =>
                      {
                          IsUpdating = false;
                      });
                });

            }
        }

        /// <summary>
        /// 下架
        /// </summary>
        private RelayCommand<object> m_RemoveBookCommand;
        public RelayCommand<object> RemoveBookCommand
        {
            get
            {
                return m_RemoveBookCommand ?? (m_RemoveBookCommand = new RelayCommand<object>(OnRemoveBookFromShelfCommand));
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
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
          {
              IsLoading = true;
          });
            await Task.Run(() =>
           {
               foreach (var item in removeList)
               {
                   try
                   {
                       string path = Path.Combine(AppDataPath.GetLocalBookFolderPath(), item.BookID + ".db");
                       if (File.Exists(path))
                       {
                           File.Delete(path);
                       }
                       DBLocalBook.DeleteLocalBookByBookID(AppDataPath.GetLocalBookDBPath(), item.BookID);

                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
                     {
                         DBLocalBook.DeleteLocalBookByBookID(AppDataPath.GetLocalBookDBPath(), item.BookID);
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
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
          {
              IsLoading = false;
              OnEditCommand();
          });


        }

        private RelayCommand<object> m_BookItemSelectedCommand;
        public RelayCommand<object> BookItemSelectedCommand
        {
            get
            {
                return m_BookItemSelectedCommand ?? (m_BookItemSelectedCommand = new RelayCommand<object>(OnBookItemSelectedCommand));
            }
        }

        private void OnBookItemSelectedCommand(object obj)
        {
            BookEntity entity = obj as BookEntity;

            if (IsEditing)
            {
                entity.IsSelected = !entity.IsSelected;
                return;
            }
            else
            {
                if (entity == null || entity.CatalogList == null || entity.CatalogList.Count == 0)
                {
                    ToastHeplper.ShowMessage("获取数据有误");
                    return;
                }

                if (string.IsNullOrEmpty(entity.LastReadChapterUrl))
                {
                    var item = entity.CatalogList.FirstOrDefault();
                    entity.LastReadChapterUrl = item.CatalogUrl; ;
                    entity.LastReadChapterName = item.CatalogName;
                }
                NavigationService.NavigateTo(typeof(BookContentPage), entity);
            }
        }

        private RelayCommand<object> m_CancleUpdateCommand;
        public RelayCommand<object> CancleUpdateCommand
        {
            get
            {
                return m_CancleUpdateCommand ?? (m_CancleUpdateCommand = new RelayCommand<object>(OnCancleUpdateCommand));
            }
        }
        private void OnCancleUpdateCommand(object obj)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
          {
              this.IsUpdating = false;
              IsLoading = false;
          });
        }

        private RelayCommand<object> m_RefreshCommand;
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return m_RefreshCommand ?? (m_RefreshCommand = new RelayCommand<object>(OnRefreshCommand));
            }
        }

        private void OnRefreshCommand(object obj)
        {
            GetLocalBook();
        }
    }
}
