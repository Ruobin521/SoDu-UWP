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
using Windows.UI.Core;
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

        public void InitData(object obj = null)
        {
            //  如果正在加载，或者提示不需要刷新 或者 obj为空 说明是从左侧菜单列表项从而导致刷新，这时候不需要刷新了
            if (IsLoading || this.BookList.Count > 0)
            {
                return;
            }
            SetData(1);
        }

        private void SetData(int pageindex)
        {
            Task.Run(async () =>
            {
                string url = null;
                if (pageindex == 1)
                {
                    url = PageUrl.BookRankListPage;
                }
                else
                {
                    url = string.Format(PageUrl.BookRankListPage2, pageindex);
                }
                string html = await GetHtmlData(url);
                return html;
            }).ContinueWith(async (result) =>
            {
                if (result.Result != null)
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       bool rs = SetBookList(result.Result.ToString(), pageindex);
                       if (!rs)
                       {
                           ToastHeplper.ShowMessage(" 第" + pageindex + "页数据加载失败");
                       }
                       else
                       {
                           ToastHeplper.ShowMessage("已加载第" + pageindex + "页，共8页");
                       }
                   });
                }
            });
        }

        private async Task<string> GetHtmlData(string url)
        {
            string html = null;
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            try
            {
                html = await http.WebRequestGet(url, true);
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

        public bool SetBookList(string html, int pageIndex)
        {

            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> arrary = AnalysisSoduService.GetRankListFromHtml(html);
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
                    foreach (var item in arrary)
                    {
                        this.BookList.Add(item);
                    }
                    this.PageIndex = pageIndex;
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
                    if (PageIndex == 1)
                    {
                        ToastHeplper.ShowMessage("已经是第一页");
                        return;
                    }
                    SetData(1);
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
                    if (PageIndex == PageCount)
                    {
                        ToastHeplper.ShowMessage("已经是最后一页");
                        return;
                    }
                    SetData(PageCount);
                });
            }
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
                SetData(PageIndex);
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
                ToastHeplper.ShowMessage("已经是最后一页");
                return;
            }
            SetData(PageIndex + 1);
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
                ToastHeplper.ShowMessage("已经是第一页");
                return;
            }
            SetData(PageIndex - 1);
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
            NavigationService.GoBack();
        }

        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        public BaseCommand BookItemSelectedChangedCommand
        {
            get
            {
                return new BaseCommand(OnBookItemSelectedChangedCommand);
            }
        }

        private void OnBookItemSelectedChangedCommand(object obj)
        {
            ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
        }
    }
}

