using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Model
{
    public class BookCatalog
    {

        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }
        public string BookID { get; set; }
        public string CatalogName { get; set; }
        public string CatalogUrl { get; set; }
        public string LyWeb { get; set; }
        public int Index { get; set; }

    }

    public class BookCatalogContent
    {

        [PrimaryKey]// 主键。
        [AutoIncrement]// 自动增长。
        public int Id { get; set; }
        public string BookID { get; set; }
        public string Content { get; set; }
        public string CatalogUrl { get; set; }

    }

}
