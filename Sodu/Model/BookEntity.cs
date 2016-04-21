using Sodu.ViewModel;
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
        /// <summary>
        ///书ID
        /// </summary>
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
        ///来源网站
        /// </summary>
        public string LyWeb { get; set; }



        /// <summary>
        /// 是否在书架上，默认不在
        /// </summary>
        private bool m_IfBookshelf;
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
