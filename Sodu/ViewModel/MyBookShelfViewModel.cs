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
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class MyBookShelfViewModel : BaseViewModel, IViewModel
    {

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

        private bool IsEditing = false;


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


        private ObservableCollection<BookEntity> m_ShelfBookList = new ObservableCollection<BookEntity>();
        //我的书架
        public ObservableCollection<BookEntity> ShelfBookList
        {
            get
            {
                return m_ShelfBookList;
            }
            set
            {
                this.SetProperty(ref this.m_ShelfBookList, value);

            }
        }


        HttpHelper HttpHelper = new HttpHelper();

        public MyBookShelfViewModel()
        {

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
        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            HttpHelper.HttpClientCancleRequest();
            IsLoading = false;
        }


        public bool BackpressedHandler()
        {
            if (IsEditing)
            {
                OnEditCommand();
                return true;
            }
            return false;
        }
        public async void RefreshData(object obj = null, bool IsRefresh = true)
        {
            string html = string.Empty;
            try
            {
                //  如果正在加载，或者提示不需要刷新 或者 obj为空 说明是从左侧菜单列表项从而导致刷新，这时候不需要刷新了
                if (IsLoading || !IsRefresh || (obj == null && this.ShelfBookList.Count > 0))
                {
                    return;
                }

                IsLoading = true;
                html = await HttpHelper.WebRequestGet(PageUrl.BookShelfPage);

                if (string.IsNullOrEmpty(html))
                {
                    return;
                }
                if (html.Contains("退出") && html.Contains("站长留言") && html.Contains("我的书架") && !html.Contains("注册"))
                {
                    SetBookList(html);

                }
                else if (html.Contains("您还没有登录"))
                {
                    throw new Exception("您还没有登录");
                }
                else
                {
                    throw new Exception("请求数据出错");
                }
            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage(ex.Message);
                ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void SetBookList(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> list = GetBookListMethod.GetBookShelftListFromHtml(html);

                if (list == null)
                {
                    return;
                }
                else
                {
                    if (this.ShelfBookList != null)
                    {
                        this.ShelfBookList.Clear();
                    }
                    else
                    {
                        this.ShelfBookList = new ObservableCollection<BookEntity>();
                    }
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            this.ShelfBookList.Add(item);
                            await Task.Delay(1);
                        }
                    }

                    if (this.ShelfBookList == null || this.ShelfBookList.Count < 1)
                    {
                        this.IsShow = true;
                    }
                    //ViewModelInstance.Instance.HomePageViewModelInstance = ArrayList[2];
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
                    html = await HttpHelper.WebRequestGet(url);
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
        private void OnSelectAllCommand(object obj)
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
            }
        }

        /// <summary>
        /// 全选，全不选
        /// </summary>
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
                    ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
                    ViewModelInstance.Instance.NeedSelfShelfRefresh = true;
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
                RefreshData(1, true);
            }
        }

        #endregion


    }
}
