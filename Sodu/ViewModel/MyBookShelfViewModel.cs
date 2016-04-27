using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class MyBookShelfViewModel : BaseViewModel, IViewModel
    {
        public bool IsNeedRefresh { get; set; } = true;


        private string _ContentTitle = "个人书架";
        public string ContentTitle
        {
            get
            {
                return _ContentTitle;

            }

            set
            {
                SetProperty(ref _ContentTitle, value);
            }
        }

        public bool IsEditing = false;


        private bool m_IsShow = false;

        public bool IsShow
        {
            get
            {
                return m_IsShow;
            }
            set
            {
                SetProperty(ref this.m_IsShow, value);
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

        private ObservableCollection<BookEntity> m_ShelfBookList;
        //我的书架
        public ObservableCollection<BookEntity> ShelfBookList
        {
            get
            {
                if (m_ShelfBookList == null)
                {
                    m_ShelfBookList = new ObservableCollection<BookEntity>();
                }
                return m_ShelfBookList;
            }
            set
            {
                this.SetProperty(ref this.m_ShelfBookList, value);
            }
        }


        HttpHelper http = new HttpHelper();

        public MyBookShelfViewModel()
        {

        }

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }


        public async Task<bool> BackpressedHandler()
        {
            if (IsEditing)
            {
                OnEditCommand();
                return true;
            }

            if (IsLoading)
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    CancleHttpRequest();
                });
                return true;
            }
            return false;
        }
        public void RefreshData(object obj = null, bool IsRefresh = true)
        {
            if (!IsNeedRefresh) return;
            this.IsShow = false;

            SetData();
        }

        public void SetData()
        {
            Task.Run(async () =>
            {
                string html = await GetHtmlData();
                return html;
            }).ContinueWith(async (result) =>
            {
                if (result.Result != null)
                {
                    string html = result.Result;
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (html.Contains("退出") && html.Contains("站长留言") && html.Contains("我的书架") && !html.Contains("注册"))
                        {
                            SetBookList(html);
                        }
                        else if (html.Contains("您还没有登录"))
                        {
                            CommonMethod.ShowMessage("您还没有登录");
                            ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                        }
                    });
                }
            });
        }


        private async Task<string> GetHtmlData()
        {
            string html = null;

            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            try
            {
                html = await http.WebRequestGet(PageUrl.BookShelfPage, true);
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

        public async void SetBookList(string html)
        {
            if (this.ShelfBookList != null)
            {
                this.ShelfBookList.Clear();
            }
            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> list = GetBookListMethod.GetBookShelftListFromHtml(html);

                if (list == null)
                {
                    this.IsShow = true;
                    return;
                }
                else
                {
                    this.IsShow = false;
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            this.ShelfBookList.Add(item);
                            await Task.Delay(1);
                        }
                    }
                    CommonMethod.ShowMessage("个人收藏已更新");
                }

            }
        }

        public async void RemoveBook(List<BookEntity> removeBookList)
        {
            string html = string.Empty;
            try
            {
                IsLoading = true;

                foreach (var item in removeBookList)
                {
                    string url = PageUrl.BookShelfPage + "?id=" + item.BookID;
                    html = await http.WebRequestGet(url);
                    if (html.Contains("取消收藏成功"))
                    {
                        ShelfBookList.Remove(item);
                        //CommonMethod.ShowMessage(item.BookName + " 取消收藏成功");
                    }
                    else
                    {
                        CommonMethod.ShowMessage(item.BookName + "取消收藏失败，请重新操作");
                    }
                }
                CommonMethod.ShowMessage("操作成功");
                removeBookList.Clear();
            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("操作失败，请重新尝试");
            }
            finally
            {
                IsLoading = false;
                OnEditCommand();
            }
        }
        public async void RemoveBookList(List<BookEntity> removeBookList)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            Task.Run(async () =>
            {
                bool result = true;
                foreach (var item in removeBookList)
                {
                    if (!IsLoading)
                    {
                        result = false;
                        break;
                    }
                    string url = PageUrl.BookShelfPage + "?id=" + item.BookID;
                    string html = await http.WebRequestGet(url);
                    if (html.Contains("取消收藏成功"))
                    {
                        await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            ShelfBookList.Remove(item);
                        });
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;

            }).ContinueWith(async (result) =>
           {
               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
               {
                   if (!result.Result)
                   {
                       CommonMethod.ShowMessage("操作完毕，但有部分图书没有成功移除");
                   }
                   else
                   {
                       CommonMethod.ShowMessage("操作完毕");
                       removeBookList.Clear();
                   }
                   IsLoading = false;
               }
               );
           });
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

        private async void OnEditCommand()
        {
            if (IsLoading) return;

            if (this.ShelfBookList == null || this.ShelfBookList.Count < 1) return;
            if (!IsEditing)
            {
                IsEditing = true;
                foreach (var item in ShelfBookList)
                {
                    item.IfBookshelf = true;
                    //try
                    //{
                    //    if (ShelfBookList.IndexOf(item) % 5 == 0)
                    //    {
                    //        await Task.Delay(1);
                    //    }
                    //}
                    //catch (Exception ex)
                    //{

                    //}
                }
            }
            else
            {
                foreach (var item in ShelfBookList)
                {
                    item.IfBookshelf = false;
                    item.IsSelected = false;
                    await Task.Delay(1);
                }
                IsEditing = false;
            }
        }



        /// <summary>
        /// 全选，全不选
        /// </summary>
        public RelayCommand<object> SelectAllCommand
        {
            get
            {
                return new RelayCommand<object>(OnSelectAllCommand);
            }
        }
        public void OnSelectAllCommand(object obj)
        {
            if (!IsEditing) return;
            if (obj != null && obj.ToString().Equals("0"))
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = true;
                }
            }
            else if (obj != null && obj.ToString().Equals("1"))
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = false;
                }
            }
        }


        /// <summary>
        /// 下架
        /// </summary>
        public RelayCommand<object> RemoveBookFromShelfCommand
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
                var msgDialog = new Windows.UI.Popups.MessageDialog(" \n请点击编辑按钮，并选择需要取消收藏的小说。") { Title = "取消收藏" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                {
                    return;
                }));
                await msgDialog.ShowAsync();
            }
            else
            {

                List<BookEntity> removeList = new List<BookEntity>();

                foreach (var item in ShelfBookList)
                {
                    if (item.IsSelected == true)
                    {
                        removeList.Add(item);
                    }
                }

                if (removeList.Count > 0)
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定取消收藏" + removeList.Count + "本小说？") { Title = "取消收藏" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        RemoveBook(removeList);
                    }));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();

                }
                else
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n请勾选需要取消收藏的小说。") { Title = "取消收藏" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
            }
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
            if (IsLoading) return;
            BookEntity entity = obj as BookEntity;
            if (entity != null)
            {
                if (IsEditing)
                {
                    entity.IsSelected = !entity.IsSelected;
                }
                else
                {
                    ViewModelInstance.Instance.UpdataChapterPageViewModelInstance.IsNeedRefresh = true;
                    // this.IsNeedRefresh = false;
                    ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);

                    if (entity.UnReadCountData != null)
                    {
                        entity.UnReadCountData = string.Empty;
                    }
                }
            }
        }

        #region  上拉刷新,下拉加载

        ///跳转到相应页数
        /// </summary>
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return new RelayCommand<object>(OnRefreshCommand);
            }
        }

        private void OnRefreshCommand(object obj)
        {

            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                SetData();
            }
        }

        #endregion


    }
}
