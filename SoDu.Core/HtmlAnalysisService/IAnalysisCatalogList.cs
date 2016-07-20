using Sodu.Core.Model;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDu.Core.HtmlAnalysisService
{
    interface IAnalysisCatalogList
    {
        Task<List<BookCatalog>> GetCatalogList(string url, string bookid, HttpHelper http);
    }
}
