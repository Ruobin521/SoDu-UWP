using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookCatalogPageViewModel : BaseViewModel, IViewModel
    {
        public string ContentTitle
        {
            get; set;
        }

        public string BaseUrl
        {
            get; set;
        }

        //public string WebName
        //{
        //    get; set;
        //}

        public BookEntity CurrentBookEntity { get; set; }



        private ObservableCollection<BookCatalog> m_CatalogList;
        /// <summary>
        /// 目录集合
        /// </summary>
        public ObservableCollection<BookCatalog> CatalogList
        {
            get
            {
                return m_CatalogList;
            }
            set
            {
                this.SetProperty(ref this.m_CatalogList, value);
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

        HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
            //throw new NotImplementedException();
        }

        public async void RefreshData(object obj = null, bool IsRefresh = true)
        {
            try
            {
                if (obj == null)
                {
                    return;
                }
                object[] para = obj as object[];
                if (this.CurrentBookEntity != null && this.CurrentBookEntity.BookName == (para[1] as BookEntity).BookName)
                {
                    return;
                }
                this.BaseUrl = para[0].ToString();
                BookEntity temp = para[1] as BookEntity;
                this.CurrentBookEntity = new BookEntity()
                {
                    BookID = temp.BookID,
                    BookName = temp.BookName,
                    ChapterName = temp.ChapterName,
                    ChapterUrl = temp.ChapterUrl,
                    ContentsUrl = temp.ContentsUrl,
                    LyUrl = temp.LyUrl,
                    UpdateTime = temp.UpdateTime,
                };

                this.ContentTitle = CurrentBookEntity.BookName;

                IsLoading = true;
                bool result = await SetData(BaseUrl);

                if (!result)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Services.CommonMethod.ShowMessage("未能获取到目录数据，请重新尝试");
            }
            finally
            {
                IsLoading = false;
            }

        }


        private async Task<bool> SetData(string url)
        {
            try
            {
                string html = await http.HttpClientGetRequest(url, true);
                bool result = await SetCatalogList(html, CurrentBookEntity.LyUrl);
                return result;
            }
            catch (Exception)
            {
                Services.CommonMethod.ShowMessage("未能获取到目录数据，请重新尝试");
                return false;
            }
            return false;
        }

        private async Task<bool> SetCatalogList(string html, string webname)
        {
            var result = AnalysisBookCatalogList.GetCatalogListByHtml(html, webname);
            if (result != null)
            {
                if (this.CatalogList != null)
                {
                    this.CatalogList.Clear();
                }
                else
                {
                    this.CatalogList = new ObservableCollection<BookCatalog>();
                }


                foreach (var item in result)
                {
                    this.CatalogList.Add(item);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 选中目录
        /// </summary>
        public ICommand CatalogSelectedCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
                {
                    if (IsLoading) return;

                    if (str != null)
                    {
                        try
                        {
                            BookCatalog catalog = str as BookCatalog;
                            if (catalog == null) return;
                            string url = BaseUrl + "/" + catalog.CatalogUrl;
                            CurrentBookEntity.ChapterUrl = BaseUrl + "/" + catalog.CatalogUrl;
                            CurrentBookEntity.ChapterName = catalog.CatalogName;
                            MenuModel menu = new MenuModel() { MenuName = CurrentBookEntity.ChapterName, MenuType = typeof(BookContentPage) };
                            NavigationService.NavigateTo(menu, new object[] { "1", CurrentBookEntity, this.CatalogList, catalog });
                        }
                        catch (Exception)
                        {

                        }

                    }
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
                await SetData(BaseUrl);
            }
        }
        /// <summary>
        /// 返回
        /// </summary>
        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    NavigationService.GoBack(null, null);
                });
            }
        }

    }
}
