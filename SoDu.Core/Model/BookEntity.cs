using Sodu.Core.ViewModel;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace Sodu.Core.Model
{
    public class BookEntity : BaseViewModel
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        ///书ID
        /// </summary>
        [Unique]
        public string BookID { get; set; }

        /// <summary>
        ///书名
        /// </summary>
        public string BookName { get; set; }


        public string m_NewestChapterName;
        /// <summary>
        ///章节名称
        /// </summary>
        public string NewestChapterName
        {
            get
            {
                return m_NewestChapterName;
            }
            set
            {
                SetProperty(ref m_NewestChapterName, value);
            }
        }

        public string m_Cover;
        /// <summary>
        ///封面
        /// </summary>
        public string Cover
        {
            get
            {
                return m_Cover;
            }
            set
            {
                SetProperty(ref m_Cover, value);
            }
        }

        public string m_Description;
        /// <summary>
        ///简介
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                SetProperty(ref m_Description, value);
            }
        }

        /// <summary>
        ///当前章节正文地址
        /// </summary>
        public string NewestChapterUrl { get; set; }


        private string _LastReadChapterName;
        /// <summary>
        ///最后阅读的章节名称
        /// </summary>
        public string LastReadChapterName
        {

            get
            {
                return _LastReadChapterName;
            }
            set
            {
                SetProperty(ref _LastReadChapterName, value);
            }
        }

        private string _LastReadChapterUrl;
        /// <summary>
        ///最后阅读的章节地址
        /// </summary>
        public string LastReadChapterUrl
        {

            get
            {
                return _LastReadChapterUrl;
            }
            set
            {
                SetProperty(ref _LastReadChapterUrl, value);
            }
        }

        private string m_UnReadCountData;
        /// <summary>
        ///未读提示
        /// </summary>
        [Ignore]
        public string UnReadCountData
        {
            get
            {
                return m_UnReadCountData;
            }
            set
            {
                this.SetProperty(ref this.m_UnReadCountData, value);
            }
        }

        /// <summary>   
        ///更新时间
        /// </summary>
        private string m_UpdateTime;

        public string UpdateTime
        {

            get
            {
                return m_UpdateTime;
            }

            set
            {
                try
                {
                    m_UpdateTime = !string.IsNullOrEmpty(value)
                   ? DateTime.Parse(value).ToString("yyyy-MM-dd HH:mm")
                   : DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
                catch (Exception)
                {
                    m_UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                }
            }
        }

        /// <summary>
        ///更新目录地址
        /// </summary>
        public string UpdateCatalogUrl { get; set; }

        /// <summary>
        /// 正文章节列表地址
        /// </summary>
        public string CatalogListUrl
        {
            get; set;
        }

        /// <summary>
        ///作者名称
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        ///来源网站
        /// </summary>
        public string LyWeb { get; set; }

        /// <summary>
        /// 是否是本地图书
        /// </summary>
        private bool m_IsLocal;
        public bool IsLocal
        {
            get
            {
                return m_IsLocal;
            }
            set
            {
                this.SetProperty(ref this.m_IsLocal, value);
            }
        }

        /// <summary>
        /// 是否为历史记录
        /// </summary>
        private bool m_IsHistory;
        public bool IsHistory
        {
            get
            {
                return m_IsHistory;
            }
            set
            {
                this.SetProperty(ref this.m_IsHistory, value);
            }
        }

        private ObservableCollection<BookCatalog> m_CatalogList;
        [Ignore]
        public ObservableCollection<BookCatalog> CatalogList
        {
            get { return m_CatalogList; }

            set
            {
                this.SetProperty(ref this.m_CatalogList, value);
            }
        }

        [Ignore]
        public ObservableCollection<BookCatalog> UnDownloadCatalogList { get; set; }

        /// <summary>
        /// 是否为编辑状态
        /// </summary>
        private bool m_IsInEdit = false;
        [Ignore]
        public bool IsInEdit
        {
            get
            {
                return m_IsInEdit;
            }
            set
            {
                //RaisePropertyChanged("IfBookshelf");
                this.SetProperty(ref this.m_IsInEdit, value);
            }
        }

        /// <summary>
        /// 是否勾选
        /// </summary>
        private bool m_IsSelected;
        [Ignore]
        [XmlIgnore]
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                //RaisePropertyChanged("IfBookshelf");
                this.SetProperty(ref this.m_IsSelected, value);
            }
        }


        public BookEntity Clone()
        {
            BookEntity entity = new BookEntity();

            entity.AuthorName = this.AuthorName;
            entity.BookID = this.BookID;
            entity.BookName = this.BookName;
            entity.CatalogList = this.CatalogList;
            entity.CatalogListUrl = this.CatalogListUrl;
            entity.Id = this.Id;
            entity.IsHistory = this.IsHistory;
            entity.IsInEdit = this.IsInEdit;
            entity.IsLocal = this.IsLocal;
            entity.IsSelected = this.IsSelected;
            entity.LastReadChapterName = this.LastReadChapterName;
            entity.LastReadChapterUrl = this.LastReadChapterUrl;
            entity.LyWeb = this.LyWeb;
            entity.NewestChapterName = this.NewestChapterName;
            entity.NewestChapterUrl = this.NewestChapterUrl;
            entity.UnDownloadCatalogList = this.UnDownloadCatalogList;
            entity.UnReadCountData = this.UnReadCountData;
            entity.UpdateCatalogUrl = this.UpdateCatalogUrl;
            entity.UpdateTime = this.UpdateTime;

            return entity;

        }

    }
}
