﻿using GalaSoft.MvvmLight.Command;
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
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookContentPageViewModel : BaseViewModel, IViewModel
    {
        /// <summary>
        /// 阅读模式， 0 代表单个章节阅读，1 代表从目录章节阅读
        /// </summary>
        private string ReadingMode { get; set; }


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
        private string m_ContentTitle;
        public string ContentTitle
        {
            get
            {
                return m_ContentTitle;
            }
            set
            {
                SetProperty(ref m_ContentTitle, value);
            }
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


        public bool m_DirectionArrowShow;
        public bool DirectionArrowShow
        {
            get
            {
                return m_DirectionArrowShow;
            }
            set
            {
                SetProperty(ref m_DirectionArrowShow, value);
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

        private BookCatalog m_CurrentCatalog;
        /// <summary>
        /// 当前选中的
        /// </summary>
        public BookCatalog CurrentCatalog
        {
            get
            {
                return m_CurrentCatalog;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentCatalog, value);
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
            try
            {
                this.TextContent = string.Empty;
                this.ContentTitle = string.Empty;
                this.IsCatalogMenuShow = false;
                this.DirectionArrowShow = false;

                if (obj == null && (obj as BookEntity) != null)
                {
                    throw new Exception();
                }

                ReadingMode = (obj as object[])[0].ToString();
                BookEntity entity = (obj as object[])[1] as BookEntity;
                if (entity == null) return;

                if (ReadingMode.Equals("1"))
                {
                    this.CatalogList = (obj as object[])[2] as ObservableCollection<BookCatalog>;
                    this.CurrentCatalog = (obj as object[])[3] as BookCatalog;
                }
                this.CurrentBookEntity = entity;
                SetContentByReadingMode();
            }
            catch (Exception ex)
            {
                Services.CommonMethod.ShowMessage("未能获取章节正文。");
            }

        }
        private async void SetContentByReadingMode()
        {
            try
            {
                IsLoading = true;
                string url = null;
                if (ReadingMode.Equals("0"))
                {
                    DirectionArrowShow = false;
                    url = CurrentBookEntity.ChapterUrl;
                    this.ContentTitle = CurrentBookEntity.ChapterName;
                    bool result = await SetContentString(url, CurrentBookEntity);
                    if (!result)
                    {
                        throw new Exception();
                    }
                }
                else if (ReadingMode.Equals("1"))
                {
                    if (CurrentBookEntity == null || CatalogList == null || CatalogList.Count < 1 || CurrentCatalog == null)
                    {
                        return;
                    }
                    url = CurrentCatalog.CatalogUrl;
                    this.DirectionArrowShow = true;
                    Schema.BookCatalog catalog = GetCatafromDatabase(this.CurrentCatalog);
                    if (catalog != null)
                    {

                    }
                    else
                    {
                        bool result = await SetContentString(url, CurrentBookEntity);
                        if (!result)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            this.ContentTitle = CurrentCatalog.CatalogName;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Services.CommonMethod.ShowMessage("未能获取到正文内容。");
            }
            finally
            {
                IsLoading = false;
            }

        }

        private Schema.BookCatalog GetCatafromDatabase(BookCatalog catalog)
        {

            return null;
        }


        private async Task<bool> SetContentString(string url, BookEntity entity)
        {
            try
            {
                string html = await http.WebRequestGet(url, false);
                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception();
                }
                if (Services.WebSetList.AlreadyAnalysisWebList.Contains(entity.LyWeb))
                {
                    CatalogUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogUrl(html, entity.LyWeb);
                    if (string.IsNullOrEmpty(CatalogUrl))
                    {
                        IsCatalogMenuShow = false;
                    }
                    else
                    {
                        IsCatalogMenuShow = true;
                    }
                    html = Services.AnalysisContentHtmlService.AnalysisContentHtml(html, entity.LyWeb);
                    if (string.IsNullOrEmpty(html) || string.IsNullOrWhiteSpace(html))
                    {
                        Services.CommonMethod.ShowMessage("未能解析到正文内容。");
                        return false;
                    }
                    this.TextContent = string.Empty;
                    this.TextContent = html;
                    return true;
                }
                else
                {
                    html = Services.AnalysisContentHtmlService.ReplaceSymbol(html);
                    this.TextContent = html;
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
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
                        SetContentByReadingMode();
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

        /// <summary>
        /// 切换章节
        /// </summary>
        public ICommand SwitchCatalogCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
             {
                 OnSwtichCommand(str);
             });
            }
        }

        private void OnSwtichCommand(object str)
        {
            if (IsLoading) return;

            //上一章
            if (str.ToString().Equals("0"))
            {
                if (this.CatalogList.IndexOf(this.CurrentCatalog) == 0)
                {
                    Services.CommonMethod.ShowMessage("已经是第一章。");
                    return;
                }
                if (this.CatalogList.Count < 2)
                {
                    return;
                }
                this.CurrentCatalog = this.CatalogList[this.CatalogList.IndexOf(this.CurrentCatalog) - 1];
            }
            //下一章
            else /*if (str.ToString().Equals("01"))*/
            {
                if (this.CatalogList.IndexOf(this.CurrentCatalog) == this.CatalogList.Count - 1)
                {
                    Services.CommonMethod.ShowMessage("已经是最后一章。");
                    return;
                }
                if (this.CatalogList.Count < 2)
                {
                    return;
                }
                this.CurrentCatalog = this.CatalogList[this.CatalogList.IndexOf(this.CurrentCatalog) + 1];
            }
            if (this.CurrentCatalog != null && CurrentCatalog.CatalogName != null && this.CurrentCatalog.CatalogUrl != null)
            {
                this.CurrentBookEntity.ChapterName = this.CurrentCatalog.CatalogName;
                this.CurrentBookEntity.ChapterUrl = this.CurrentCatalog.CatalogUrl;
                SetContentByReadingMode();
            }
        }


    }
}
