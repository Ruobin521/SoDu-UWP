using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
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
        private bool IsEditing;

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

        public void GetLocalBook()
        {
            this.LocalBookList.Clear();
            Task.Run(async () =>
        {
            var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
            foreach (var item in result)
            {
                var list = Database.DBBookCatalog.SelectBookCatalogs(Constants.AppDataPath.GetLocalBookDBPath(), item.BookID);
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
            if (result != null)
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {

                    foreach (var item in result)
                    {
                        this.LocalBookList.Add(item);
                    }
                });
            }
        });
        }

        public void RefreshData(object obj = null)
        {
            GetLocalBook();
        }

        public void CancleHttpRequest()
        {

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

            if (this.LocalBookList == null || this.LocalBookList.Count < 1) return;
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
                             Services.CommonMethod.ShowMessage(item.BookName + "删除失败，请重新尝试");
                         }
                     }
                     catch (Exception ex)
                     {
                         Services.CommonMethod.ShowMessage(item.BookName + "删除失败，请重新尝试");
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
                CommonMethod.ShowMessage("获取数据有误");
                return;
            }

            if (IsEditing)
            {
                entity.IsSelected = !entity.IsSelected;
                return;
            }
            else
            {
                if (entity.CatalogList == null || entity.CatalogList.Count < 1)
                {
                    CommonMethod.ShowMessage("获取章节数据有误");
                    return;
                }

                if (string.IsNullOrEmpty(entity.LastReadChapterUrl))
                {
                    var item = entity.CatalogList.FirstOrDefault();
                    if (item != null)
                    {
                        entity.LastReadChapterUrl = item.CatalogUrl; ;
                        entity.LastReadChapterName = item.CatalogName;
                    }
                }

                ViewModelInstance.Instance.BookContentPageViewModelInstance.IsLocal = true;
                NavigationService.NavigateTo(typeof(BookContentPage), entity);

            }
        }
    }
}
