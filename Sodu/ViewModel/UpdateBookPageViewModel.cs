using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Pages;
using Sodu.Services;
using SoDu.Core.Util;

namespace Sodu.ViewModel
{
    public class UpdateBookPageViewModel : BaseViewModel, IViewModel
    {
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


        private int m_PageCount;
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

        private ObservableCollection<BookEntity> m_BookList;
        public ObservableCollection<BookEntity> BookList
        {
            get
            {
                return m_BookList ?? (m_BookList = new ObservableCollection<BookEntity>());
            }
            set
            {
                this.SetProperty(ref this.m_BookList, value);
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

        private string m_ContentTitle = "最新更新";
        public string ContentTitle
        {
            get
            {
                return m_ContentTitle;
            }
            set
            {
                this.SetProperty(ref this.m_ContentTitle, value);

            }
        }

        HttpHelper http;
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

            PageCount = 1;
            PageIndex = 1;

            if (this.BookList != null)
            {
                BookList.Clear();
            }
            SetData(1);
        }

        private void SetData(int pageIndex)
        {
            if (IsLoading) return;

            Task.Run(async () =>
            {
                string html = await LoadPageDataByIndex(pageIndex);
                return html;

            }).ContinueWith((result) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (result.Result != null && SetBookList(result.Result.ToString(), pageIndex))
                    {
                        ToastHeplper.ShowMessage("已加载第" + PageIndex + "页，共" + PageCount + "页");
                    }
                    else
                    {
                        ToastHeplper.ShowMessage("未能获取最新章节数据");
                    }
                });
            });
        }

        async Task<string> LoadPageDataByIndex(int nextPageIndex)
        {
            string html = string.Empty;
            try
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    IsLoading = true;
                });

                string url = ViewModelInstance.Instance.UrlService.GetLastUpdateBookListPage(nextPageIndex.ToString());

                http = new HttpHelper();
                html = await http.WebRequestGet(url, true);
                if (html == null) return null;
                var html1 = html;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    if (PageCount == 1)
                    {
                        Match match = Regex.Match(html1, "<a href=\"/map_(.*?).html\" title=\"尾页\">尾页</a>", RegexOptions.RightToLeft);
                        if (match != null)
                        {
                            try
                            {
                                PageCount = Convert.ToInt32(match.Groups[1].ToString().Trim());
                            }
                            catch (Exception)
                            {
                                PageCount = 1;
                            }
                        }
                    }
                });

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

        public bool SetBookList(string html, int pageIndex)
        {
            if (!string.IsNullOrEmpty(html))
            {
                var arrary = AnalysisSoduService.GetRankListFromHtml(html);
                if (arrary == null)
                {
                    return false;
                }
                else
                {
                    if (this.BookList != null)
                    {
                        this.BookList.Clear();
                    }
                    this.PageIndex = pageIndex;
                    foreach (var item in arrary)
                    {
                        this.BookList.Add(item);
                    }
                    return true;
                }
            }

            return false;
        }



        private RelayCommand<object> m_FirstPageCommand;
        public RelayCommand<object> FirstPageCommand
        {
            get
            {
                return m_FirstPageCommand ?? (m_FirstPageCommand = new RelayCommand<object>((obj) =>
                {
                    if (IsLoading) return;
                    if (PageIndex == 1)
                    {
                        ToastHeplper.ShowMessage("已经是第一页");
                        return;
                    }
                    SetData(1);
                }));
            }
        }

        private RelayCommand<object> m_LastPageCommand;
        public RelayCommand<object> LastPageCommand
        {
            get
            {
                return m_LastPageCommand ?? (m_LastPageCommand = new RelayCommand<object>((obj) =>
                {
                    if (IsLoading) return;
                    if (PageIndex == PageCount)
                    {
                        ToastHeplper.ShowMessage("已经是最后一页");
                        return;
                    }
                    SetData(PageCount);
                }));
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
                SetData(1);
            }
        }

      
        private RelayCommand<object> m_NextPageCommand;
        public RelayCommand<object> NextPageCommand
        {
            get
            {
                return m_NextPageCommand ?? (m_NextPageCommand = new RelayCommand<object>(OnNextPageCommand));
            }
        }

        private void OnNextPageCommand(object obj)
        {
            if (IsLoading) return;

            if (PageIndex == PageCount)
            {
                ToastHeplper.ShowMessage("已经是最后一页");
                return;
            }
            SetData(PageIndex + 1);
        }

        private RelayCommand<object> m_PreviousPageCommand;
        public RelayCommand<object> PreviousPageCommand
        {
            get
            {
                return m_PreviousPageCommand ?? (m_PreviousPageCommand = new RelayCommand<object>(OnPreviousPageCommand));
            }
        }

        private void OnPreviousPageCommand(object obj)
        {
            if (IsLoading) return;

            if (PageIndex == 1)
            {
                ToastHeplper.ShowMessage("已经是第一页");
                return;
            }
            SetData(PageIndex - 1);
        }

        private RelayCommand<object> m_BackCommand;
        public RelayCommand<object> BackCommand
        {
            get
            {
                return m_BackCommand ?? (m_BackCommand = new RelayCommand<object>(OnBackCommand));
            }
        }

        private void OnBackCommand(object obj)
        {
            NavigationService.GoBack();
        }

        private RelayCommand<object> m_BookChapterSelectedChangedCommand;
        public RelayCommand<object> BookChapterSelectedChangedCommand
        {
            get
            {
                return m_BookChapterSelectedChangedCommand ?? (m_BookChapterSelectedChangedCommand = new RelayCommand<object>((obj) =>
                {
                    if (!IsLoading)
                    {
                        BookEntity entity = obj as BookEntity;
                        if (entity != null)
                        {
                            NavigationService.NavigateTo(typeof(BookContentPage), entity);
                        }
                    }
                }
                ));
            }
        }

    }
}
