﻿using Sodu.ViewModel;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string NewestChapterName { get; set; }

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

        /// <summary>
        ///未读提示
        /// </summary>
        public string UnReadCountData { get; set; }
        /// <summary>   
        ///更新时间
        /// </summary>
        public string UpdateTime { get; set; }


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

        [Ignore]
        public ObservableCollection<BookCatalog> CatalogList { get; set; }


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
