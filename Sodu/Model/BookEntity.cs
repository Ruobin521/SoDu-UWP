using Sodu.ViewModel;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.UI.Xaml;

namespace Sodu.Model
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
        /// <summary>
        ///章节名称
        /// </summary>
        public string ChapterName { get; set; }

        /// <summary>
        ///最后阅读的章节
        /// </summary>
        public string LastReadChapterName { get; set; }

        /// <summary>
        ///未读提示
        /// </summary>
        public string UnReadCountData { get; set; }
        /// <summary>   
        ///更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        ///章节地址
        /// </summary>
        public string ChapterUrl { get; set; }
        /// <summary>
        ///目录地址
        /// </summary>
        public string CatalogUrl { get; set; }

        /// <summary>
        ///作者名称
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        ///来源网站
        /// </summary>
        public string LyWeb { get; set; }

        [Ignore]
        public List<BookCatalog> CatalogList { get; set; }

        /// <summary>
        /// 是否为编辑状态
        /// </summary>
        private bool m_IfBookshelf;
        [Ignore]
        public bool IfBookshelf
        {
            get
            {
                return m_IfBookshelf;
            }
            set
            {
                //RaisePropertyChanged("IfBookshelf");
                this.SetProperty(ref this.m_IfBookshelf, value);
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

    }
}
