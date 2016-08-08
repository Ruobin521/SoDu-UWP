using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Model;
using Sodu.Services;

using SoDu.Core.Util;
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
using Microsoft.Practices.Unity;
using SoDu.Core.API;
using Sodu.Core.Util;

namespace Sodu.ViewModel
{
    public class BookShelfPageViewModel : BaseViewModel, IViewModel
    {


        #region  属性 字段
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
        private bool m_IsAllSelected;
        public bool IsAllSelected
        {
            get
            {
                return m_IsAllSelected;
            }
            set
            {
                SetProperty(ref this.m_IsAllSelected, value);
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
                SetProperty(ref this.m_IsEditing, value);
            }
        }

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

        #endregion

        public BookShelfPageViewModel()
        {

        }

        #region 方法

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            try
            {
                if (http != null)
                {
                    http.HttpClientCancleRequest();
                }
                IsLoading = false;
            }
            catch (Exception)
            {

            }

        }

        public bool BackpressedHandler()
        {
            CancleHttpRequest();

            if (IsEditing)
            {
                OnEditCommand();
                return true;
            }

            if (IsLoading)
            {
                CancleHttpRequest();
                return true;
            }

            return false;
        }

        public void InitData(object obj = null)
        {
            this.IsShow = false;
            if (this.ShelfBookList.Count > 0)
            {
                return;
            }
            ShelfBookList.Clear();
            SetData();
        }

        public void SetData()
        {
            //设置编辑模式为false
            IsEditing = false;
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
                            ToastHeplper.ShowMessage("您还没有登录");
                            ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                        }
                        else
                        {
                            ToastHeplper.ShowMessage("获取数据失败");
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
                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetBookShelfPage(), true);
            }
            catch (Exception)
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
            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> list = AnalysisSoduService.GetBookShelftListFromHtml(html);

                if (this.ShelfBookList != null)
                {
                    this.ShelfBookList.Clear();
                    await Task.Delay(1);
                }

                if (list == null)
                {
                    this.IsShow = true;
                    return;
                }
                else
                {
                    this.IsShow = false;
                    if (list != null && list.Count > 0)
                    {
                        var temp = list.OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
                        foreach (var item in temp)
                        {
                            this.ShelfBookList.Add(item);
                        }
                    }
                    ToastHeplper.ShowMessage("个人书架已更新");
                }

            }
        }

        public void RemoveBookList(List<BookEntity> removeBookList)
        {
            Task.Run(async () =>
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            bool result = true;
            foreach (var item in removeBookList)
            {
                if (!IsLoading)
                {
                    result = false;
                    break;
                }
                string url = ViewModelInstance.Instance.UrlService.GetBookShelfPage() + "?id=" + item.BookID;
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
               IsLoading = false;
               if (result.Result)
               {
                   ToastHeplper.ShowMessage("操作完毕");
                   removeBookList.Clear();
                   OnEditCommand();
               }
           }
           );
       });
        }


        #endregion


        #region  命令

        /// <summary>
        /// 全选，全不选
        /// </summary>
        private RelayCommand<object> m_EditCommand;
        public RelayCommand<object> EditCommand
        {
            get
            {
                return m_EditCommand ?? (m_EditCommand = new RelayCommand<object>(
                 (obj) =>
                 {

                     if (IsLoading) return;
                     OnEditCommand();
                 }
                    ));
            }
        }

        public void OnEditCommand()
        {
            if (IsLoading) return;

            if (this.ShelfBookList == null || this.ShelfBookList.Count < 1)
            {
                IsEditing = false;
                return;
            }

            SetBookShelfEditStatus(!IsEditing);
        }

        public void SetBookShelfEditStatus(bool vale)
        {
            IsEditing = vale;

            if (!vale)
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsInEdit = false;
                    item.IsSelected = false;
                }
                IsEditing = false;
                IsAllSelected = false;
            }
            else
            {
                IsEditing = true;
                foreach (var item in ShelfBookList)
                {
                    item.IsInEdit = true;
                }
            }
        }


        /// <summary>
        /// 全选，全不选
        /// </summary>
        private RelayCommand<object> m_SelectAllCommand;
        public RelayCommand<object> SelectAllCommand
        {
            get
            {
                return m_SelectAllCommand ?? (m_SelectAllCommand = new RelayCommand<object>(OnSelectAllCommand));
            }
        }
        public void OnSelectAllCommand(object obj)
        {
            if (!IsEditing) return;

            if (!IsAllSelected)
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = true;
                }
                IsAllSelected = true;
            }
            else
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = false;
                }
                IsAllSelected = false;
            }
        }


        /// <summary>
        /// 下架
        /// </summary>
        private RelayCommand<object> m_RemoveBookFromShelfCommand;
        public RelayCommand<object> RemoveBookFromShelfCommand
        {
            get
            {
                return m_RemoveBookFromShelfCommand ?? (m_RemoveBookFromShelfCommand = new RelayCommand<object>(OnRemoveBookFromShelfCommand));
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
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n请勾选需要取消收藏的小说。") { Title = "取消收藏" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
            }
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
            if (IsLoading) return;
            BookEntity entity = obj as BookEntity;
            if (entity != null)
            {
                if (IsEditing)
                {
                    entity.IsSelected = !entity.IsSelected;

                    IsAllSelected = true;
                    foreach (var item in ShelfBookList)
                    {
                        if (!item.IsSelected)
                        {
                            IsAllSelected = false;
                            break;
                        }
                    }
                }
                else
                {
                    ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);

                    if (entity.UnReadCountData != null)
                    {
                        entity.UnReadCountData = string.Empty;
                    }
                }
            }
        }


        ///跳转到相应页数
        /// </summary>
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
