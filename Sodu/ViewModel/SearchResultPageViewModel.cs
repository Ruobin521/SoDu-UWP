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
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class SearchResultPageViewModel : BaseViewModel, IViewModel
    {
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

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void InitData(object obj = null)
        {
            try
            {
                if (obj == null)
                {
                    SearchPara = "";
                    this.SearchResultList.Clear();
                    return;
                }

                if (SearchResultList != null)
                {
                    SearchResultList.Clear();
                }
                SetData(obj.ToString());
            }
            catch (Exception ex)
            {
                ToastHeplper.ShowMessage("获取数据有误，请重新尝试");
            }
        }


        public void SetData(string para)
        {
            Task.Run(async () =>
            {

                string uri = string.Format(PageUrl.BookSearchPage, System.Net.WebUtility.UrlEncode(para));
                string html = await GetHtmlData(uri);
                return html;
            }).ContinueWith(async (resultHtml) =>
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public async Task<string> GetHtmlData(string url)
        {
            string html = string.Empty;

            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            try
            {
                html = await http.WebRequestGet(url, false);
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
            InitData(obj.ToString());
        }



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
                if (obj == null) return;
                InitData(obj.ToString());
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
            //  InitData(this.CurrentPageIndex);
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
            //InitData(this.CurrentPageIndex - 1);
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
            // InitData(this.MaxPageIndex);
        }


        private void OnBackCommand(object obj)
        {
            NavigationService.GoBack();
        }


        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        public BaseCommand BookItemSelectedChangedCommand
        {
            get
            {
                return new BaseCommand((obj) =>
                {
                    if (!IsLoading)
                    {
                        // this.IsNeedRefresh = false;
                        ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
                    }
                });
            }
        }



    }
}
