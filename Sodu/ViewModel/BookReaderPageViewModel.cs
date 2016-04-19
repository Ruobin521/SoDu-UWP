using Sodu.Model;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class BookReaderPageViewModel : BaseViewModel, IViewModel
    {

        public BookEntity CurrentBookEntity { get; set; }

        public string ContentTitle
        {
            get; set;
        }

        public bool IsLoading
        {
            get; set;
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

        public HttpHelper http = new HttpHelper();

        public BookReaderPageViewModel()
        {
            this.ContentFontSzie = ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie;
        }

        public void SetCurrentCatalogIndex()
        {
            int index = CatalogList.IndexOf(CurrentCatalog);
        }

        public void CancleHttpRequest()
        {
            //  throw new NotImplementedException();
        }

        public async void RefreshData(object obj = null, bool IsRefresh = true)
        {

            if (obj == null) return;

            CurrentBookEntity = (obj as object[])[0] as BookEntity;
            this.CatalogList = (obj as object[])[1] as ObservableCollection<BookCatalog>;
            CurrentCatalog = (obj as object[])[2] as BookCatalog;

            if (CurrentBookEntity == null || CatalogList == null || CatalogList.Count < 1 || CurrentCatalog == null)
            {
                return;
            }

            Schema.BookCatalog catalog = GetCatafromDatabase();
            if (catalog != null)
            {

            }
            else
            {
                string html = await http.HttpClientGetRequest(CurrentBookEntity.ChapterUrl, false);
                if (string.IsNullOrEmpty(html))
                {
                    throw new Exception();
                }
                if (Services.WebSetList.AlreadyAnalysisWebList.Contains(CurrentBookEntity.LyUrl))
                {
                    html = Services.AnalysisContentHtmlService.AnalysisContentHtml(html, CurrentBookEntity.LyUrl);
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

            //new object[] { CurrentBookEntity, this.CatalogList, catalog }




            //throw new NotImplementedException();
        }



        private Schema.BookCatalog GetCatafromDatabase()
        {

            return null;
        }
    }
}
