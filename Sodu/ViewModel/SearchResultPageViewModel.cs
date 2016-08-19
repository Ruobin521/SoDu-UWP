using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Services;

using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;

namespace Sodu.ViewModel
{
    public class SearchResultPageViewModel : BaseViewModel, IViewModel
    {

        #region 字段，属性
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
                if (m_SearchResultList == null)
                {
                    m_SearchResultList = new ObservableCollection<BookEntity>();
                }
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

        HttpHelper http = new HttpHelper();

        #endregion

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            if (http != null)
            {
                http.HttpClientCancleRequest();
            }
            IsLoading = false;
        }

        public void InitData(object obj = null)
        {
            CancleHttpRequest();
            SearchResultList.Clear();
            this.SearchPara = string.Empty;
        }

        public void SetData(string para)
        {

            SearchResultList.Clear();
            this.SearchPara = para;
            Task.Run(async () =>
            {
                string uri = string.Format(ViewModelInstance.Instance.UrlService.GetSearchPage(), System.Net.WebUtility.UrlEncode(para));
                string html = await GetHtmlData(uri);
                return html;
            }).ContinueWith((resultHtml) =>
          {
              DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (resultHtml.Result != null && SetBookList(resultHtml.Result.ToString()))
                {
                    ToastHeplper.ShowMessage("共返回" + this.SearchResultList.Count + "条结果");
                }
                else
                {
                    ToastHeplper.ShowMessage("无搜索结果");
                }
            });

          });
        }

        public bool SetBookList(string html)
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(html))
                {
                    ObservableCollection<BookEntity> arraryList = AnalysisSoduService.GetRankListFromHtml(html);
                    if (arraryList == null)
                    {
                        return false;
                    }
                    else
                    {
                        this.SearchResultList.Clear();
                        if (arraryList.Count > 0)
                        {
                            foreach (var item in arraryList)
                            {
                                this.SearchResultList.Add(item);
                            }
                        }
                        result = true;
                    }
                }
                else
                {
                    return result;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> GetHtmlData(string url)
        {
            string html = string.Empty;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
          {
              IsLoading = true;
          });
            try
            {
                html = await http.WebRequestGet(url, false);
            }
            catch (Exception)
            {
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
              {
                  IsLoading = false;
              });
            }

            return html;
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
        private RelayCommand<object> m_SearchCommand;
        public RelayCommand<object> SearchCommand
        {
            get
            {
                return m_SearchCommand ?? (m_SearchCommand = new RelayCommand<object>(OnSearchCommand));
            }
        }
        private void OnSearchCommand(object obj)
        {
            this.SearchResultList.Clear();
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
            {
                ToastHeplper.ShowMessage("请输入搜索条件");
                return;
            }
            SetData(obj.ToString());
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
                return;
            }

            if (string.IsNullOrEmpty(SearchPara))
            {
                ToastHeplper.ShowMessage("请输入搜索条件");
                return;
            }
            SetData(SearchPara);
        }

        /// <summary>
        ///跳转到相应页数
        /// </summary>
        private RelayCommand<object> m_RequestCommand;
        public RelayCommand<object> RequestCommand
        {
            get
            {
                return m_RequestCommand ?? (m_RequestCommand = new RelayCommand<object>(OnRequestCommand));
            }
        }

        private void OnRequestCommand(object obj)
        {
            //  InitData(this.CurrentPageIndex);
        }

        private RelayCommand<object> m_PrePageCommand;
        public RelayCommand<object> PrePageCommand
        {
            get
            {
                return m_PrePageCommand ?? (m_PrePageCommand = new RelayCommand<object>(OnPrePageCommand));
            }
        }

        private void OnPrePageCommand(object obj)
        {
            //InitData(this.CurrentPageIndex - 1);
        }


        private RelayCommand<object> m_LastPageCommand;
        public RelayCommand<object> LastPageCommand
        {
            get
            {
                return m_LastPageCommand ?? (m_LastPageCommand = new RelayCommand<object>(OnLastPageCommand));
            }
        }

        private void OnLastPageCommand(object obj)
        {
            // InitData(this.MaxPageIndex);
        }


        private void OnBackCommand(object obj)
        {
            NavigationService.GoBack();
        }


        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        public RelayCommand<object> m_bookItemSelectedChangedCommand;
        public RelayCommand<object> BookItemSelectedChangedCommand
        {
            get
            {
                return m_bookItemSelectedChangedCommand ?? (m_bookItemSelectedChangedCommand = new RelayCommand<object>((obj) =>
                       {
                           if (!IsLoading)
                           {
                               // this.IsNeedRefresh = false;
                               ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
                           }
                       }));
            }
        }

    }
}
