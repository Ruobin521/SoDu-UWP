using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            if (LocalBookList.Count == 0)
            {
                GetLocalBook();
            }
            else
            {
                CheckUpdate();
            }
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
                        item.IsLocal = true;
                        this.LocalBookList.Add(item);
                    }
                });
            }
            //foreach (var item in result)
            //{
            //    var list = Database.DBBookCatalog.SelectBookCatalogs(Constants.AppDataPath.GetLocalBookDBPath(), item.BookID);
            //    if (list != null)
            //    {
            //        foreach (var catalog in list)
            //        {
            //            if (item.CatalogList == null)
            //            {
            //                item.CatalogList = new ObservableCollection<BookCatalog>();
            //            }
            //            item.CatalogList.Add(catalog);
            //        }
            //    }
            //}
        }).ContinueWith(obj =>
        {
            CheckUpdate();
        });
        }

        private void CheckUpdate()
        {
            if (IsChecking) return;
            if (this.LocalBookList.Count < 1) return;

            Task.Run(async () =>
           {
               try
               {
                   foreach (var item in LocalBookList)
                   {
                       if (!string.IsNullOrEmpty(item.CatalogListUrl))
                       {
                           var list = await Services.AnalysisBookCatalogList.GetCatalogList(item.CatalogListUrl, item.BookID, new HttpHelper());
                           if (list != null)
                           {
                               int result = 0;
                               var last = list.FirstOrDefault(p => p.CatalogUrl == item.NewestChapterUrl);

                               if (last != null)
                               {
                                   int index = list.IndexOf(last);
                                   result = list.Count - 1 - index;
                               }
                               else
                               {
                                   result = list.Count;
                               }

                               if (result > 0)
                               {
                                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                   {
                                       item.UnReadCountData = "　可用更新（" + result + ")";
                                   });
                               }

                           }
                       }
                   }
               }
               catch (Exception ex)
               {

               }
               finally
               {
                   IsChecking = false;
               }
           });
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

        private void OnEditCommand()
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
                    item.IfBookshelf = true;
                }
            }
            else
            {
                foreach (var item in LocalBookList)
                {
                    item.IfBookshelf = false;
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

            Task.Run(async () =>
          {
              await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  IsLoading = true;
                  IsUpdating = true;
              });
              try
              {
                  foreach (var item in LocalBookList)
                  {
                      if (string.IsNullOrEmpty(item.UnReadCountData))
                      {
                          continue;
                      }
                      if (!string.IsNullOrEmpty(item.CatalogListUrl))
                      {
                          var list = await Services.AnalysisBookCatalogList.GetCatalogList(item.CatalogListUrl, item.BookID, new HttpHelper());
                          if (list == null) continue;

                          var last = list.FirstOrDefault(p => p.CatalogUrl.Equals(item.NewestChapterUrl));
                          if (last != null)
                          {
                              int index = list.IndexOf(last);
                              for (int i = index + 1; i < list.Count; i++)
                              {
                                  if (!IsUpdating)
                                  {
                                      await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                      {
                                          item.UnReadCountData = "可用更新(" + (list.Count - i).ToString() + ")";
                                      });
                                      return;
                                  }
                                  else
                                  {
                                      await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                      {
                                          item.UnReadCountData = "正在更新(" + (list.Count - i).ToString() + ")";
                                      });
                                  }
                                  string html = await AnalysisContentService.GetHtmlContent(new HttpHelper(), list[i].CatalogUrl);
                                  // item.CatalogContentGUID = item.BookID + item.Index.ToString();
                                  BookCatalogContent content = new BookCatalogContent()
                                  {
                                      BookID = item.BookID,
                                      CatalogContentGUID = list[i].CatalogContentGUID,
                                      Content = html
                                  };
                                  await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                  {
                                      //item.CatalogList.Add(list[i]);
                                      item.NewestChapterName = list[i].CatalogName;
                                      item.NewestChapterUrl = list[i].CatalogUrl;

                                      item.UpdateTime = DateTime.Now.ToString();


                                  });

                                  lock (obj)
                                  {
                                      Database.DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetLocalBookDBPath(), list[i]);
                                      Database.DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetLocalBookDBPath(), content);
                                      Database.DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), item);
                                  }
                              }
                              await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                              {
                                  item.UnReadCountData = null;
                              });
                          }
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
                         var result = Database.DBLocalBook.DeleteLocalBooksDataByBookID(Constants.AppDataPath.GetLocalBookDBPath(), item.BookID);
                         if (result)
                         {
                             await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                             {
                                 this.LocalBookList.Remove(item);
                             });
                         }
                         else
                         {
                             ToastHeplper.ShowMessage(item.BookName + "删除失败，请重新尝试");
                         }
                     }
                     catch (Exception ex)
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

        private async void OnBookItemSelectedCommand(object obj)
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
                await Task.Run(async () =>
                {
                    var list = Database.DBBookCatalog.SelectBookCatalogs(Constants.AppDataPath.GetLocalBookDBPath(), entity.BookID);
                    if (list != null)
                    {
                        foreach (var catalog in list)
                        {
                            if (entity.CatalogList == null)
                            {
                                entity.CatalogList = new ObservableCollection<BookCatalog>();
                            }
                            entity.CatalogList.Add(catalog);
                        }
                    }
                    else
                    {
                        await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ToastHeplper.ShowMessage("获取章节数据有误");
                            return;
                        });
                    }
                });


                if (string.IsNullOrEmpty(entity.LastReadChapterUrl))
                {
                    var item = entity.CatalogList.FirstOrDefault();
                    if (item != null)
                    {
                        entity.LastReadChapterUrl = item.CatalogUrl; ;
                        entity.LastReadChapterName = item.CatalogName;
                    }
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
            });
        }
    }
}
