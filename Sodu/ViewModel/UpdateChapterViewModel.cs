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

        public string ContentTitle { get; set; }
        public string CurrentPageUrl { get; set; }


        HttpHelper HttpHelper = new HttpHelper();
        public async void RefreshData(object obj = null, bool IsRefresh = false)
        {
            try
            {

                if (obj == null || (obj as BookEntity) == null || (obj == null && this.ChapterList.Count > 0))
                {
                    return;
                }

                if (CurrentEntity == obj as BookEntity)
                {
                    return;
                }

                string html = string.Empty;

                //  如果正在加载，或者提示不需要刷新，这时候不需要刷新了
                if (IsLoading || !IsRefresh)
                {
                    return;
                }

                if (this.ChapterList != null)
                {
                    this.ChapterList.Clear();
                }
                // IsLoading = true;
                CurrentEntity = (obj as BookEntity);
                CurrentPageUrl = (obj as BookEntity).CatalogUrl;
                ContentTitle = (obj as BookEntity).BookName;

                bool result = await LoadPageDataByIndex(1);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsLoading = false;
            }
        }

        public void CancleHttpRequest()
        {
            this.HttpHelper.HttpClientCancleRequest();
            IsLoading = false;
        }

        async Task<bool> LoadPageDataByIndex(int nextPageIndex)
        {
            string html = string.Empty;
            IsLoading = true;
            try
            {
                if (PageIndex == 1)
                {
                    html = await HttpHelper.WebRequestGet(CurrentPageUrl, true);

                }
                else
                {
                    html = await HttpHelper.WebRequestGet(CurrentPageUrl.Insert(CurrentPageUrl.Length - 5, "_" + PageIndex), true);
                }
                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception();
                }
                CommonMethod.ShowMessage("第" + PageIndex + "页，共20页");
                bool result = await SetBookList(html);
                PageIndex = nextPageIndex;
                return true;
            }
            catch (Exception ex)
            {
                html = null;
                CommonMethod.ShowMessage("未能获取数据");
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<bool> SetBookList(string html)
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

                    foreach (var item in arrary)
                    {
                        item.BookName = this.ContentTitle;
                        this.ChapterList.Add(item);
                        await Task.Delay(1);
                    }
                    return true;
                }
            }

            return false;
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

        private async void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                await LoadPageDataByIndex(1);
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

            if (PageIndex == 20)
            {
                CommonMethod.ShowMessage("已经是最后一页");
                return;
            }
            await LoadPageDataByIndex(PageIndex + 1);
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
            await LoadPageDataByIndex(PageIndex - 1);
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

        public BaseCommand BookChapterSelectedChangedCommand
        {
            get
            {
                return new BaseCommand((obj) =>
                {
                    if (!IsLoading)
                    {
                        ViewModelInstance.Instance.MainPageViewModelInstance.OnBookChapterSelectedChangedCommand(obj);
                    }
                }
                );
            }
        }

        #endregion


    }
}
