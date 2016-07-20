using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDu.Core.HtmlAnalysisService
{
    public interface IAnalysisHtmlContentService
    {
        Task<string> GetContent(HttpHelper http, string catalogUrl);

        string AnalysisHtml(string html, string url);

    }
}
