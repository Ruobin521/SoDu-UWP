using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Schema
{
    public class Book
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id
        {
            get;
            set;
        }

        [Unique]
        public string BookName
        {
            get;
            set;
        }

        public string BookCover
        {
            get;
            set;
        }

        public string BookNote
        {
            get;
            set;
        }

        /// <summary>
        /// 来源网站
        /// </summary>
        public string lywz
        {
            get;
            set;
        }

        /// <summary>
        /// 目录地址
        /// </summary>
        public string CatalogUrl
        {
            get;
            set;
        }

        public string LastUpdateTime
        {
            get;
            set;
        }

        public string LastCatalog
        {
            get;
            set;
        }

    }

    public class BookCatalog
    {
        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id
        {
            get;
            set;
        }

        [Unique]
        public string BookName
        {
            get;
            set;
        }

        public string CatalogName
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
    }

}
