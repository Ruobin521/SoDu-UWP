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
        public HotPageViewModel()
        {
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
        public void RefreshData(object obj = null)
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
                         CommonMethod.ShowMessage("热门小说数据已更新");
                     }
                     else
                     {
                         CommonMethod.ShowMessage("未能获取热门小说数据");
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
                html = await http.WebRequestGet(PageUrl.HomePage, true);
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

                    ObservableCollection<BookEntity>[] arraryList = GetBookListMethod.GetHomePageBookList(html);
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
            //RefreshData(PageIndex + 1);            
            RefreshData(1);
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
