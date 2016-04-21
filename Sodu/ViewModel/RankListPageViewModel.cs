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
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class RankListPageViewModel : BaseViewModel, IViewModel
    {

        private string _ContentTitle = "点击排行榜";
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

        private int m_PageIndex = 1;
        public int PageIndex
        {
            get
            {
                return m_PageIndex;
            }
            set
            {
                SetProperty(ref this.m_PageIndex, value);
            }
        }

        private int m_PageCount = 8;
        public int PageCount
        {
            get
            {
                return m_PageCount;
            }
            set
            {
                SetProperty(ref this.m_PageCount, value);
            }
        }
        private ObservableCollection<BookEntity> m_BookList = new ObservableCollection<BookEntity>();
        public ObservableCollection<BookEntity> BookList
        {
            get
            {
                return m_BookList;
            }
            set
            {
                this.SetProperty(ref this.m_BookList, value);
            }
        }

        HttpHelper http = new HttpHelper();

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public async void RefreshData(object obj = null, bool IsRefresh = true)
        {
            string html = string.Empty; ;
            try
            {
                if (Convert.ToInt32(obj) > 8)
                {
                    CommonMethod.ShowMessage("已加载所有");
                    return;
                }
                //  如果正在加载，或者提示不需要刷新 或者 obj为空 说明是从左侧菜单列表项从而导致刷新，这时候不需要刷新了
                if (IsLoading || !IsRefresh || (obj == null && this.BookList.Count > 0))
                {
                    return;
                }

                IsLoading = true;
                int pageindex;
                if (obj == null)
                {
                    pageindex = 1;
                }
                else
                {
                    pageindex = Convert.ToInt32(obj);

                }
                string url = string.Empty;

                if (pageindex == 1)
                {
                    url = PageUrl.BookRankListPage;
                }
                else
                {
                    url = string.Format(PageUrl.BookRankListPage2, pageindex);
                }
                html = await http.WebRequestGet(url);

                bool result = await SetBookList(html, pageindex);
                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                html = string.Empty;
                CommonMethod.ShowMessage("获取数据失败，请重新尝试");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> SetBookList(string html, int pageIndex)
        {
            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> arrary = GetBookListMethod.GetRankListFromHtml(html);
                if (arrary == null)
                {
                    return false;
                }
                else
                {
                    //if (this.PageIndex == 1)
                    //{
                    // this.BookList.Clear();
                    //}
                    CommonMethod.ShowMessage("已加载" + PageIndex + "页，共8页");
                    this.PageIndex = pageIndex;
                    if (this.BookList == null)
                    {
                        this.BookList = new ObservableCollection<BookEntity>();
                    }
                    else
                    {
                        this.BookList.Clear();
                    }
                    foreach (var item in arrary)
                    {
                        this.BookList.Add(item);
                        await Task.Delay(1);
                    }

                    return true;
                }
            }

            return false;
        }

        public RelayCommand<object> FirstPageCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    if (IsLoading) return;
                    RefreshData(1);
                });
            }
        }

        public RelayCommand<object> LastPageCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    if (IsLoading) return;
                    RefreshData(PageCount);
                });
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
                RefreshData(1);
            }
        }


        /// <summary>
        ///跳转到相应页数
        /// </summary>
        public RelayCommand<object> RequestCommand
        {
            get
            {
                return new RelayCommand<object>(OnRequestCommand);
            }
        }

        private void OnRequestCommand(object obj)
        {
            if (IsLoading) return;
            if (PageIndex == PageCount)
            {
                CommonMethod.ShowMessage("已经是最后一页");
                return;
            }
            RefreshData(PageIndex + 1);
        }


        public RelayCommand<object> PrePageCommand
        {
            get
            {
                return new RelayCommand<object>(OnPrePageCommandd);
            }
        }

        private void OnPrePageCommandd(object obj)
        {
            if (IsLoading) return;
            if (PageIndex == 1)
            {
                CommonMethod.ShowMessage("已经是第一页");
                return;
            }
            RefreshData(PageIndex - 1);
        }

        public RelayCommand<object> BackCommand
        {
            get
            {
                return new RelayCommand<object>(OnBackCommand);
            }
        }

        private void OnBackCommand(object obj)
        {
            NavigationService.GoBack(null, null);
        }

        #endregion
    }
}

