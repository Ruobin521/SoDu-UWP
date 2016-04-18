using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookContentPageViewModel : BaseViewModel, IViewModel
    {

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
        public string ContentTitle
        {
            get; set;
        }

        public string CatalogUrl
        {
            get; set;
        }


        public bool m_IsCatalogMenuShow = true;
        public bool IsCatalogMenuShow
        {
            get
            {
                return m_IsCatalogMenuShow;
            }
            set
            {
                SetProperty(ref m_IsCatalogMenuShow, value);
            }
        }

        public string m_TextContent;
        public string TextContent
        {
            get
            {
                return m_TextContent;
            }
            set
            {
                SetProperty(ref m_TextContent, value);
            }
        }




        public int m_FontSize;
        public int ContentFontSzie
        {
            get
            {
                return m_FontSize;
            }
            set
            {


                SetProperty(ref m_FontSize, value);
            }
        }


        public BookEntity CurrentBookEntity { get; set; }


        public BookContentPageViewModel()
        {
            this.ContentFontSzie = ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie;

        }

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }
        HttpHelper http = new HttpHelper();


        public async void RefreshData(object obj = null, bool IsRefresh = true)
        {
            if (obj == null && (obj as BookEntity) != null)
            {
                Services.CommonMethod.ShowMessage("传递数据有误，请重新操作");
                return;
            }

            this.TextContent = string.Empty;
            BookEntity entity = obj as BookEntity;
            this.CurrentBookEntity = entity;
            ContentTitle = entity.ChapterName;
            string url = null;
            if (!entity.ChapterUrl.StartsWith("http"))
            {
                url = Constants.PageUrl.HomePage + entity.ChapterUrl;
            }
            else
            {
                url = entity.ChapterUrl;
            }
            try
            {
                IsLoading = true;
                string html = await http.HttpClientGetRequest(url, false);
                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception();
                }
                if (Services.WebSetList.AlreadyAnalysisWebList.Contains(entity.LyUrl))
                {
                    CatalogUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogUrl(html, entity.LyUrl);
                    if (string.IsNullOrEmpty(CatalogUrl))
                    {
                        IsCatalogMenuShow = false;
                    }
                    else
                    {
                        IsCatalogMenuShow = true;
                    }
                    html = Services.AnalysisContentHtmlService.AnalysisContentHtml(html, entity.LyUrl);
                    if (string.IsNullOrEmpty(html) || string.IsNullOrWhiteSpace(html))
                    {
                        Services.CommonMethod.ShowMessage("未能解析到正文内容。");
                        return;
                    }
                    this.TextContent = html;
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                Services.CommonMethod.ShowMessage("未能获取章节正文。");
            }
            finally
            {
                IsLoading = false;
            }

        }

        /// <summary>
        /// 字体增大
        /// </summary>
        public ICommand FontIncreaseCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    if (ContentFontSzie >= 26) return;
                    string temp = this.TextContent;
                    this.TextContent = string.Empty;
                    this.ContentFontSzie += 2;
                    //this.TextContent = temp;
                    SetTextContent(temp);
                });
            }
        }


        /// <summary>
        /// 字体减小
        /// </summary>
        public ICommand FontDecreaseCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    if (ContentFontSzie <= 16) return;
                    string temp = this.TextContent;
                    this.TextContent = string.Empty;
                    this.ContentFontSzie -= 2;
                    SetTextContent(temp);
                });
            }
        }

        public async void SetTextContent(string temp)
        {
            var group = Split<char>(temp.ToList(), 5);
            foreach (var list in group)
            {
                foreach (var item in list)
                {
                    this.TextContent = string.Format(this.TextContent, item.ToString());
                }
                await Task.Delay(1);
            }

            this.TextContent = temp;
        }

        public IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> items,
                                                       int numOfParts)
        {
            int i = 0;
            return items.GroupBy(x => i++ % numOfParts);
        }


        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    if (IsLoading)
                    {
                        CancleHttpRequest();
                    }
                    else
                    {
                        RefreshData(this.CurrentBookEntity);
                    }
                });
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


        /// <summary>
        /// 目录
        /// </summary>
        public ICommand CatalogCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    MenuModel menu = new MenuModel() { MenuName = CurrentBookEntity.BookName, MenuType = typeof(BookCatalogPage) };
                    NavigationService.NavigateTo(menu, new object[] { this.CatalogUrl, this.CurrentBookEntity });
                });
            }
        }

    }
}
