using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Pages;
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
    public class UpdateChapterViewModel : BaseViewModel, IViewModel
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

        public bool m_IsAddBtnShow;
        public bool IsAddBtnShow
        {
            get
            {
                return m_IsAddBtnShow;
            }
            set
            {
                SetProperty(ref m_IsAddBtnShow, value);
            }
        }

        private BookEntity m_CurrentEntity;
        public BookEntity CurrentEntity
        {
            get
            {
                return m_CurrentEntity;
            }
            set
            {
                SetProperty(ref this.m_CurrentEntity, value);
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


        private ObservableCollection<BookEntity> m_ChapterList = new ObservableCollection<BookEntity>();
        //更新列表
        public ObservableCollection<BookEntity> ChapterList
        {
            get
            {
                return m_ChapterList;
            }
            set
            {
                this.SetProperty(ref this.m_ChapterList, value);

            }
        }

        private string m_ContentTitle;
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
        public string CurrentPageUrl { get; set; }


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

            if (CurrentEntity == obj as BookEntity)
            {
                return;
            }

            PageCount = 1;
            PageIndex = 1;
            CurrentEntity = (obj as BookEntity);
            CurrentPageUrl = (obj as BookEntity).UpdateCatalogUrl;
            ContentTitle = (obj as BookEntity).BookName;

            if (this.ChapterList != null)
            {
                ChapterList.Clear();
            }
            if (!ViewModelInstance.Instance.IsLogin)
            {
                IsAddBtnShow = false;
            }
            else if (ViewModelInstance.Instance.IsLogin && ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutAddToShelf)
            {
                IsAddBtnShow = false;
            }
            else if (ViewModelInstance.Instance.IsLogin && !ViewModelInstance.Instance.SettingPageViewModelInstance.IfAutAddToShelf && ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == CurrentEntity.BookID) == null)
            {
                IsAddBtnShow = true;
            }
            else
            {
                IsAddBtnShow = false;
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

                string url = null;
                if (nextPageIndex == 1)
                {
                    url = CurrentPageUrl;
                }
                else
                {
                    url = CurrentPageUrl.Insert(CurrentPageUrl.Length - 5, "_" + nextPageIndex);
                }
                http = new HttpHelper();
                html = await http.WebRequestGet(url, true);
                if (html == null) return null;
                var html1 = html;
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
              {
                  if (PageCount == 1)
                  {
                      Match match = Regex.Match(html1, @"(?<=总计.*?记录.*?共).*?(?=页)");
                      if (match != null)
                      {
                          try
                          {
                              PageCount = Convert.ToInt32(match.ToString().Trim());
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
                List<BookEntity> arrary = AnalysisSoduService.GetBookUpdateChapterList(html);
                if (arrary == null)
                {
                    return false;
                }
                else
                {

                    if (this.ChapterList != null)
                    {
                        this.ChapterList.Clear();
                    }
                    else
                    {
                        this.ChapterList = new ObservableCollection<BookEntity>();
                    }
                    this.PageIndex = pageIndex;

                    foreach (var item in arrary)
                    {
                        item.BookName = this.ContentTitle;
                        item.BookID = this.CurrentEntity.BookID;
                        this.ChapterList.Add(item);
                    }
                    return true;
                }
            }

            return false;
        }



        /// </summary>
        private RelayCommand<object> m_AddToShelfCommand;
        public RelayCommand<object> AddToShelfCommand
        {
            get
            {
                return m_AddToShelfCommand ?? (m_AddToShelfCommand = new RelayCommand<object>(async (obj) =>
                   {
                       try
                       {
                           IsLoading = true;
                           if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == CurrentEntity.BookID) == null)
                           {
                               string html = await (new HttpHelper()).WebRequestGet(string.Format(ViewModelInstance.Instance.UrlService.GetAddToShelfPage(), CurrentEntity.BookID));
                               if (html.Contains("{\"success\":true}"))
                               {
                                   ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Add(CurrentEntity);
                                   IsAddBtnShow = false;
                                   ToastHeplper.ShowMessage("收藏成功");
                               }
                           }
                           else
                           {
                               ToastHeplper.ShowMessage("您已收藏过本书");
                           }
                       }
                       catch (Exception)
                       {
                           ToastHeplper.ShowMessage("收藏失败，请重新操作");
                       }
                       finally
                       {
                           IsLoading = false;
                       }
                   }));
            }
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

        private async void OnRequestCommand(object obj)
        {
            if (IsLoading) return;
            await LoadPageDataByIndex(PageIndex + 1);
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
