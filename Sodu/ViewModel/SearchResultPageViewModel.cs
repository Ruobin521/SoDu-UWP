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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class SearchResultPageViewModel : BaseViewModel, IViewModel
    {
        public bool IsNeedRefresh { get; set; } = true;

        private string _ContentTitle = "搜索";
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


        private string m_SearchPara;
        /// <summary>
        /// 搜索参数
        /// </summary>
        public string SearchPara
        {
            get { return m_SearchPara; }
            set
            {
                this.SetProperty(ref this.m_SearchPara, value);
            }
        }

        private ObservableCollection<BookEntity> m_SearchResultList;
        /// <summary>
        /// 搜索结果
        /// </summary>
        public ObservableCollection<BookEntity> SearchResultList
        {
            get
            {
                return m_SearchResultList;
            }
            set
            {
                this.SetProperty(ref this.m_SearchResultList, value);
            }
        }


        private int m_CurrentPageIndex = 1;
        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return m_CurrentPageIndex;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentPageIndex, value);
            }
        }

        private int m_MaxPageIndex = 1;
        /// <summary>
        /// 共多少页
        /// </summary>
        public int MaxPageIndex
        {
            get
            {
                return m_MaxPageIndex;
            }
            set
            {
                this.SetProperty(ref this.m_MaxPageIndex, value);
            }
        }
        public bool IsLoading
        {
            get; set;
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
        private bool m_IsShow2 = true;

        public bool IsShow2
        {
            get
            {
                return m_IsShow2;
            }
            set
            {
                SetProperty(ref this.m_IsShow2, value);
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

        public async void RefreshData(object obj = null, bool IsRefresh = false)
        {
            try
            {
                //  如果正在加载，或者提示不需要刷新 或者obj为空说明是从主要左侧列表项从而导致刷新，这时候不需要刷新了
                if (IsLoading || !IsRefresh || string.IsNullOrEmpty(SearchPara))
                {
                    return;
                }

                if (obj != null)
                {
                    if (Convert.ToInt32(obj) > this.MaxPageIndex)
                    {
                        CommonMethod.ShowMessage("已经是最后一页");
                        return;

                    }
                    if (Convert.ToInt32(obj) < 1)
                    {
                        CommonMethod.ShowMessage("已经是第一页");
                        return;
                    }
                    else
                    {
                        this.CurrentPageIndex = Convert.ToInt32(obj);
                    }
                }

                if (SearchResultList != null)
                {
                    SearchResultList.Clear();
                }
                IsLoading = true;
                string uri = string.Format(PageUrl.BookSearchPage, ChineseGBKConverter.Utf8ToGb2312(SearchPara), CurrentPageIndex);

                string html = await http.WebRequestGet(uri);
                this.MaxPageIndex = GetMaxPageIndex(html);
                SearchResultList = GetBookListMethod.GetSearchResultkListFromHtml(html);
                IsShow = true;
                IsShow2 = false;
                CommonMethod.ShowMessage("第" + CurrentPageIndex + "页，共" + MaxPageIndex + "页");
            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("获取数据有误，请重新尝试");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// 获取尾页 页码
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private int GetMaxPageIndex(string html)
        {
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match pageString = Regex.Match(html, "(?<=<a href.*?page=).*?(?=\">尾页</a>)", RegexOptions.RightToLeft);
            int pageIndex = Convert.ToInt32(pageString.ToString());
            return pageIndex;
        }


        /// <summary>
        /// 搜索数据
        /// </summary>
        public RelayCommand<object> SearchCommand
        {
            get
            {
                return new RelayCommand<object>(OnSearchCommand);
            }
        }
        private void OnSearchCommand(object obj)
        {
            RefreshData(1, true);
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
            if (string.IsNullOrEmpty(this.SearchPara))
            {
                CommonMethod.ShowMessage("请输入搜索条件");
                return;
            }
            RefreshData(CurrentPageIndex, true);
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
            RefreshData(this.CurrentPageIndex + 1, true);
        }

        public RelayCommand<object> PrePageCommand
        {
            get
            {
                return new RelayCommand<object>(OnPrePageCommand);
            }
        }

        private void OnPrePageCommand(object obj)
        {
            RefreshData(this.CurrentPageIndex - 1, true);
        }


        public RelayCommand<object> LastPageCommand
        {
            get
            {
                return new RelayCommand<object>(OnLastPageCommand);
            }
        }

        private void OnLastPageCommand(object obj)
        {
            RefreshData(this.MaxPageIndex, true);
        }


        #endregion

    }
}
