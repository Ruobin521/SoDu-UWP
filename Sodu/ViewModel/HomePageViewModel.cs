using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;

using SoDu.Core.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sodu.ViewModel
{
    public class HomePageViewModel : BaseViewModel, IViewModel
    {
        #region 属性 字段
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
        public ObservableCollection<BookEntity> RecommendBookList
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

        private ObservableCollection<BookEntity> m_HotBookList;
        public ObservableCollection<BookEntity> HotBookList
        {
            get
            {
                if (m_HotBookList == null)
                {
                    m_HotBookList = new ObservableCollection<BookEntity>();
                }
                return m_HotBookList;
            }
            set
            {
                this.SetProperty(ref this.m_HotBookList, value);
            }
        }


        private string _ContentTitle = "";
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
        public HomePageViewModel()
        {
        }


        HttpHelper http = null;

        #endregion

        #region 方法

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
            if (IsLoading) return;

            CancleHttpRequest();

            if (this.RecommendBookList.Count > 0 && this.HotBookList.Count > 0)
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
           }).ContinueWith(async (result) =>
          {
              await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 if (result != null && SetBookList(result.Result.ToString()))
                 {
                     ToastHeplper.ShowMessage("已更新" + RecommendBookList.Count + "条数据");
                 }
                 else
                 {
                     if (NavigationService.ContentFrame.Content is Pages.RecommendPage)
                     {
                         ToastHeplper.ShowMessage("未能获取推荐阅读数据");
                     }
                 }
             });
          });
        }


        public async Task<string> GetHtmlData()
        {
            string html = null;

            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
           {
               IsLoading = true;
           });
            try
            {
                http = new HttpHelper();
                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetHomePage(), true);
            }
            catch (Exception)
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
            if (!string.IsNullOrEmpty(html))
            {

                ObservableCollection<BookEntity>[] arraryList = AnalysisSoduService.GetHomePageBookList(html);
                if (arraryList == null)
                {
                    return false;
                }
                else
                {
                    if (arraryList[1] == null)
                    {
                        return false;
                    }
                    this.RecommendBookList.Clear();
                    foreach (var item in arraryList[1])
                    {
                        this.RecommendBookList.Add(item);
                    }

                    this.HotBookList.Clear();
                    foreach (var item in arraryList[2])
                    {
                        this.HotBookList.Add(item);
                    }
                    result = true;

                    return result;
                }
            }
            else
            {
                return result;
            }
        }

        #endregion


        #region 命令

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
                SetData();
            }
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
            if (IsLoading) return;
            //InitData(PageIndex + 1);            
            InitData(1);
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


        /// <summary>
        /// 选中相应的bookitem
        /// </summary>
        private RelayCommand<object> m_BookItemSelectedChangedCommand;
        public RelayCommand<object> BookItemSelectedChangedCommand
        {
            get
            {
                return m_BookItemSelectedChangedCommand ?? (m_BookItemSelectedChangedCommand = new RelayCommand<object>((obj) =>
                  {
                      if (!IsLoading)
                      {
                          ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
                      }
                  }));
            }
        }

        #endregion

    }
}
