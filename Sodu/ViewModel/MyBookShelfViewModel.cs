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
                //if (!ViewModelInstance.Instance.NeedSelfShelfRefresh)
                //{
                //    return;
                //}
                // ViewModelInstance.Instance.NeedSelfShelfRefresh = false;
                IsLoading = true;
                html = await HttpHelper.HttpClientGetRequest(PageUrl.HomePage);

                if (string.IsNullOrEmpty(html))
                {
                    return;
                }
                if (html.Contains("注销") && html.Contains("站长留言") && html.Contains("我的永久书架"))
                {
                    SetBookList(html);
                    if (this.ShelfBookList == null || this.ShelfBookList.Count < 1)
                    {
                        this.IsShow = true;
                    }
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
                //ObservableCollection<BookEntity> list = GetBookListMethod.GetBookShelftListFromHtml(html);
                ObservableCollection<BookEntity>[] ArrayList = GetBookListMethod.GetHomePageBookList(html);

                if (ArrayList == null)
                {
                    return;
                }
                else
                {
                    this.ShelfBookList.Clear();
                    if (ArrayList[1].Count > 0)
                    {
                        foreach (var item in ArrayList[1])
                        {
                            this.ShelfBookList.Add(item);
                            await Task.Delay(1);
                        }
                    }
                    ViewModelInstance.Instance.HomePageViewModelInstance.UpdateBookList = ArrayList[2];
                }

            }
        }

        public async void RemoveBook(List<BookEntity> removeBookList)
        {
            string html = string.Empty;
            try
            {
                string postData = "";
                foreach (var item in removeBookList)
                {
                    postData = postData + "bookid=" + item.BookID + "&";
                }
                postData = postData + "hello=%CF%C2%BC%DC";
                IsLoading = true;
                html = await HttpHelper.HttpClientPostRequest(PageUrl.RemoveBooktPage, postData);
                if (html.Contains("永久书架小说下架成功"))
                {
                    foreach (var item in removeBookList)
                    {
                        ShelfBookList.Remove(item);
                    }
                    CommonMethod.ShowMessage(removeBookList.Count + "本小说成功被移除");
                }
                else/* if (html.Contains("您还没有登录"))*/
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("操作失败，请重新尝试");
            }
            finally
            {
                IsLoading = false;
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
            if (!IsEditing)
            {
                foreach (var item in ShelfBookList)
                {
                    item.IfBookshelf = true;

                    await Task.Delay(1);
                }
                IsEditing = true;
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
                var msgDialog = new Windows.UI.Popups.MessageDialog(" \n请点击编辑按钮，并选择需要下架的小说。") { Title = "下架小说" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                {
                    return;
                }));
                await msgDialog.ShowAsync();
            }

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
                var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定下架" + removeList.Count + "本小说？") { Title = "下架小说" };
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
