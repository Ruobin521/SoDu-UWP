using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Pages;
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


        HttpHelper http = new HttpHelper();
        public void CancleHttpRequest()
        {
            this.http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void RefreshData(object obj = null)
        {

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

           }).ContinueWith(async (result) =>
         {
             await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
             {
                 if (result.Result != null && await SetBookList(result.Result.ToString(), pageIndex))
                 {
                     CommonMethod.ShowMessage("已加载第" + PageIndex + "页，共" + PageCount + "页");
                 }
                 else
                 {
                     CommonMethod.ShowMessage("未能获取最新章节数据");
                 }
             });
         });
        }

        async Task<string> LoadPageDataByIndex(int nextPageIndex)
        {
            string html = string.Empty;
            try
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

                html = await http.WebRequestGet(url, true);
                if (html == null) return html;
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (PageCount == 1)
                    {
                        Match match = Regex.Match(html, @"(?<=总计.*?记录.*?共).*?(?=页)");
                        if (match != null)
                        {
                            try
                            {
                                PageCount = Convert.ToInt32(match.ToString().Trim());
                            }
                            catch (Exception ex)
                            {
                                PageCount = 1;
                            }
                        }
                    }
                });

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

        public async Task<bool> SetBookList(string html, int pageIndex)
        {
            if (!string.IsNullOrEmpty(html))
            {
                List<BookEntity> arrary = GetBookListMethod.GetBookUpdateChapterList(html);
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
        public RelayCommand<object> AddToShelfCommand
        {
            get
            {
                return new RelayCommand<object>(async (obj) =>
                {
                    try
                    {
                        IsLoading = true;
                        if (ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.ToList().Find(p => p.BookID == CurrentEntity.BookID) == null)
                        {
                            string html = await (new HttpHelper()).WebRequestGet(string.Format(PageUrl.AddToShelfPage, CurrentEntity.BookID));
                            if (html.Contains("{\"success\":true}"))
                            {
                                ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Add(CurrentEntity);
                                IsAddBtnShow = false;
                                CommonMethod.ShowMessage("收藏成功");
                            }
                        }
                        else
                        {
                            CommonMethod.ShowMessage("您已收藏过本书");
                        }
                    }
                    catch (Exception)
                    {
                        CommonMethod.ShowMessage("收藏失败，请重新操作");
                    }
                    finally
                    {
                        IsLoading = false;
                    }
                });
            }
        }

        public RelayCommand<object> FirstPageCommand
        {
            get
            {
                return new RelayCommand<object>(async (obj) =>
               {
                   if (IsLoading) return;
                   if (PageIndex == 1)
                   {
                       CommonMethod.ShowMessage("已经是第一页");
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
                return new RelayCommand<object>(async (obj) =>
                {
                    if (IsLoading) return;
                    if (PageIndex == PageCount)
                    {
                        CommonMethod.ShowMessage("已经是最后一页");
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

        private async void OnRefreshCommand(object obj)
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
        public RelayCommand<object> RequestCommand
        {
            get
            {
                return new RelayCommand<object>(OnRequestCommand);
            }
        }

        private async void OnRequestCommand(object obj)
        {
            if (IsLoading) return;
            await LoadPageDataByIndex(PageIndex + 1);
        }

        public RelayCommand<object> NextPageCommand
        {
            get
            {
                return new RelayCommand<object>(OnNextPageCommand);
            }
        }

        private async void OnNextPageCommand(object obj)
        {
            if (IsLoading) return;

            if (PageIndex == PageCount)
            {
                CommonMethod.ShowMessage("已经是最后一页");
                return;
            }
            SetData(PageIndex + 1);
        }

        public RelayCommand<object> PreviousPageCommand
        {
            get
            {
                return new RelayCommand<object>(OnPreviousPageCommand);
            }
        }

        private async void OnPreviousPageCommand(object obj)
        {
            if (IsLoading) return;

            if (PageIndex == 1)
            {
                CommonMethod.ShowMessage("已经是第一页");
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

        public BaseCommand BookChapterSelectedChangedCommand
        {
            get
            {
                return new BaseCommand((obj) =>
                {
                    if (!IsLoading)
                    {
                        BookEntity entity = obj as BookEntity;
                        if (entity != null)
                        {
                            ViewModelInstance.Instance.BookContentPageViewModelInstance.IsLocal = false;
                            NavigationService.NavigateTo(typeof(BookContentPage), entity);
                        }
                    }
                }
                );
            }
        }

    }
}
