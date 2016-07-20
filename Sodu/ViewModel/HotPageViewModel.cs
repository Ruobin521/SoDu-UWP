using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
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
    //class HotPageViewModel
    //{
    //}
    public class HotPageViewModel : BaseViewModel, IViewModel
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


        private string _ContentTitle = "热门小说";
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
        public HotPageViewModel()
        {
        }


        HttpHelper http;
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
            if (this.BookList.Count > 0)
            {
                return;
            }
            SetData();
        }

        public void SetData()
        {
            Task.Run(async () =>
            {
                string html = await GetHtmlData();
                return html;
            }).ContinueWith(async (resultHtml) =>
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                 {
                     if (resultHtml.Result != null && SetBookList(resultHtml.Result.ToString()))
                     {
                         ToastHeplper.ShowMessage("热门小说数据已更新");
                     }
                     else
                     {
                         ToastHeplper.ShowMessage("未能获取热门小说数据");
                     }
                 });

            });
        }

        public async Task<string> GetHtmlData()
        {
            string html = string.Empty;

            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                IsLoading = true;
            });
            try
            {
                http = new HttpHelper();
                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetHomePage(), true);
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

        public bool SetBookList(string html)
        {
            bool result = false;

            try
            {
                if (!string.IsNullOrEmpty(html))
                {

                    ObservableCollection<BookEntity>[] arraryList = AnalysisSoduService.GetHomePageBookList(html);
                    if (arraryList == null)
                    {
                        return false;
                    }
                    else
                    {
                        if (arraryList[2] == null)
                        {
                            return false;
                        }
                        if (this.BookList != null)
                        {
                            this.BookList.Clear();
                        }

                        foreach (var item in arraryList[2])
                        {
                            this.BookList.Add(item);
                        }
                        result = true;
                        ViewModelInstance.Instance.HomePageViewModelInstance.BookList = arraryList[1];
                        return result;
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
            //InitData(PageIndex + 1);            
            InitData(1);
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
        public RelayCommand<object> BookItemSelectedChangedCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
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
