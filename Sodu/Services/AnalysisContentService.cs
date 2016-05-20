using Sodu.Model;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sodu.Services
{
    public class WebSet
    {
        /// <summary>
        /// 7度书屋
        /// </summary>
        public const string qdsw = "www.7dsw.com";
        /// <summary>
        /// 笔下文学（依依中文网）
        /// </summary>
        public const string bxwx5 = "www.bxwx5.com";
        /// <summary>
        /// 第九中文网
        /// </summary>
        public const string dijiuzww = "www.dijiuzww.com";

        /// <summary>
        /// 清风小说
        /// </summary>
        public const string qfxs = "www.qfxs.cc";
        /// <summary>
        /// 窝窝小说网
        /// </summary>
        public const string wwxsw = "www.quanxiong.org";
        /// <summary>
        /// 55xs（古古小说）
        /// </summary>
        public const string xs55 = "www.55xs.com";

        /// <summary>
        /// 风云小说
        /// </summary>
        public const string fyxs = "www.baoliny.com";

        /// <summary>
        /// 爱上中文
        /// </summary>
        public const string aszw520 = "www.aszw520.com";

        /// <summary>
        /// 大海中文
        /// </summary>
        public const string dhzw = "www.dhzw.com";

        /// <summary>
        /// 酷酷看书
        /// </summary>
        public const string kkks = "www.kukukanshu.cc";

        /// <summary>
        /// 少年文学
        /// </summary>
        public const string snwx = "www.snwx.com";

        /// <summary>
        /// 手牵手小说
        /// </summary>
        public const string sqsxs = "www.sqsxs.com";

        /// <summary>
        /// 大书包
        /// </summary>
        public const string dsb = "www.dashubao.cc";

        /// <summary>
        /// 找书网
        /// </summary>
        public const string zsw = "www.zhaodaoshu.com";

        /// <summary>
        /// 趣笔阁
        /// </summary>
        public const string qubige = "www.qbiquge.com";

        /// <summary>
        /// 倚天中文
        /// </summary>
        public const string ytzww = "www.ytzww.com";

        /// <summary>
        /// 书路小说
        /// </summary>
        public const string shu6 = "www.shu6.cc";

        /// <summary>
        /// 风华居
        /// </summary>
        public const string fenghuaju = "www.fenghuaju.cc";

        /// <summary>
        ///云来阁
        /// </summary>
        public const string ylg = "www.yunlaige.com";

        /// <summary>
        ///4k中文
        /// </summary>
        public const string fourkzw = "www.4kzw.com";

        /// <summary>
        ///幼狮书盟
        /// </summary>
        public const string yssm = "www.youshishumeng.com";

        /// <summary>
        ///80小说
        /// </summary>
        public const string su80 = "www.su80.net";

    }
    public class AnalysisContentService
    {
        public async static Task<string> GetHtmlContent(HttpHelper http, string catalogUrl)
        {
            string html = null;
            string content = null;
            try
            {
                html = await http.WebRequestGet(catalogUrl, false);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                content = Services.AnalysisContentService.AnalysisContentHtml(html, catalogUrl);
                if (string.IsNullOrEmpty(content) || string.IsNullOrWhiteSpace(content))
                {
                    return null;
                }
                List<string> lists = AnalysisPagingUrlFromUrl.GetPagingUrlListFromUrl(html, catalogUrl);
                if (lists != null)
                {
                    foreach (var url in lists)
                    {
                        string temp = await http.WebRequestGet(url, false);
                        temp = await http.WebRequestGet(url, false);
                        if (temp != null)
                        {
                            temp = Services.AnalysisContentService.AnalysisContentHtml(temp, url);
                        }
                        if (temp != null)
                        {
                            content += temp.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                content = null;
            }
            return content;
        }
        public static string AnalysisContentHtml(string html, string url)
        {
            string result = string.Empty;

            Uri tempUrl = new Uri(url);

            string web = tempUrl.Authority;

            switch (web)
            {
                //7度书屋
                case WebSet.qdsw:
                    result = AnalysisSlsxsw(html);
                    break;

                //笔下文学（依依中文网）
                case WebSet.bxwx5:
                    result = AnalysisBxzw5(html);
                    break;

                //第九中文网（有分页）
                case WebSet.dijiuzww:
                    result = AnalysisBxzw5(html);
                    break;

                //清风小说（有分页）
                case WebSet.qfxs:
                    result = AnalysisQfxsw(html);
                    break;

                //窝窝小说网（有分页）
                case WebSet.wwxsw:
                    result = AnalysisBxzw5(html);
                    break;

                //找书网（需要分页）
                case WebSet.zsw:
                    result = AnalysisBxzw5(html);
                    break;

                //55小说（古古小说网）
                case WebSet.xs55:
                    result = Analysis55xs(html);
                    break;

                //风云小说
                case WebSet.fyxs:
                    result = AnalysisWtc(html);
                    break;

                //爱上中文
                case WebSet.aszw520:
                    result = AnalysisAszw(html);
                    break;

                //大海中文
                case WebSet.dhzw:
                    result = AnalysisSlsxsw(html); ;
                    break;

                //酷酷看书
                case WebSet.kkks:
                    result = AnalysisSlsxsw(html);
                    break;

                //少年文学
                case WebSet.snwx:
                    result = AnalysisSlsxsw(html);
                    break;

                //手牵手
                case WebSet.sqsxs:
                    result = AnalysisSlsxsw(html);
                    break;

                //大书包
                case WebSet.dsb:
                    result = AnalysisDsb(html);
                    break;


                //趣笔阁
                case WebSet.qubige:
                    result = AnalysisBxzw5(html);
                    break;

                //书路
                case WebSet.shu6:
                    result = AnalysisShu6(html);
                    break;

                //风华居
                case WebSet.fenghuaju:
                    result = AnalysisBxzw5(html);
                    break;

                //云来阁
                case WebSet.ylg:
                    result = AnalysisYlg(html);
                    break;
                //4k
                case WebSet.fourkzw:
                    result = Analysis4kzw(html);
                    break;
                //4k
                case WebSet.yssm:
                    result = Analysis4kzw(html);
                    break;

                //倚天中文网
                case WebSet.ytzww:
                    result = AnalysisYtzww(html);
                    break;

                //倚天中文网
                case WebSet.su80:
                    result = AnalysisSu80(html);
                    break;

                case "书旗小说":
                    result = AnalysisSq(html);
                    break;

                case "木鱼哥":
                    result = AnalysisMyg(html);
                    break;

                case "无弹窗小说网":
                    result = AnalysisWtc(html);
                    break;
                default:
                    result = ReplaceSymbol(html);
                    break;
            }
            return result;
        }


        /// <summary>
        /// 解析手牵手小说网正文内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisSlsxsw(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"BookText\">.*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = Regex.Replace(result, "阅读本书最新章节请到.*?敬请记住我们最新网址.*?m", "");
                result = ReplaceSymbol(result);
            }
            return result;
        }
        /// <summary>
        /// 解析依依文学（笔下中文5）
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisBxzw5(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"?content\"?.*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
                result = result.Replace("p class=\"pdp\">", "").Replace("pdp\">", "");
            }
            return result;
        }

        /// <summary>
        /// 云来阁
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisYlg(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"content\".*?<div class=\"tc\".*?>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 4k中文网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string Analysis4kzw(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"htmlContent\".*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 倚天中文网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisYtzww(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<!--章节内容开始-->.*?<!--章节内容结束-->", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }


        /// <summary>
        /// 解析55sx 古古小说
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string Analysis55xs(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<dd id=\"contents\".*?</dd>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 解析清风小说网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisQfxsw(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"chapterContent\".*?<center>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = result.Replace("<<", "<");
                result = Regex.Replace(result, "<p class=\"pdp\">清风小说网.*?</p>", "");
                result = Regex.Replace(result, "<p class=\"pdp\" style=\"color:#2E2EFE\">.*?</p>", "");
                //  result = result.Replace("<p class=\"pd</p>", "<p class=\"pd></p>");
                result = ReplaceSymbol(result);
                result = result.Replace("p class=\"pdp\">", "");
                result = result.Replace("p\">", "");

            }
            return result;
        }

        /// <summary>
        /// 解析大书包
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisDsb(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div class=\"hr101\".*?<span id=\"endtips\"></span>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 木鱼哥
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisMyg(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<p class=\"vote\">.*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 爱上中文
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisAszw(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"contents\">.*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }


        /// <summary>
        /// 80书屋
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisSu80(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div id=\"txtright\">.*?<center>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 无弹窗
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisWtc(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<p id=\"?content\"?.*?</p>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 书路
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisShu6(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div class=\"chapter_con\".*?</div>", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString().Replace("记住本站网址：书路（shu6.cc）", "");
                result = ReplaceSymbol(result);
            }
            return result;
        }

        /// <summary>
        /// 书旗
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisSq(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "(?<=<div class=\"bq\">本书关键词.*?</div>).*?(?=<font color=\"red\"><a href=.*?</font>)", RegexOptions.IgnoreCase);
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
            }
            return result;
        }

        public static string ReplaceSymbol(string html)
        {
            string result = string.Empty;
            html = Regex.Replace(html, "<br.*?/>", "\n");
            html = Regex.Replace(html, "<script.*?</script>", "");
            html = Regex.Replace(html, "&nbsp;", " ");
            html = Regex.Replace(html, "<p.*?>", "\n");
            html = Regex.Replace(html, "<.*?>", "");
            html = html.Replace("&lt;/script&gt;", "");
            html = html.Replace("&lt;/div&gt;", "");
            result = html;
            return result;
        }


    }


    public class AnalysisBookCatalogUrl
    {
        public static string GetBookCatalogListUrl(string html, string url)
        {
            string result = string.Empty;

            Uri tempUrl = new Uri(url);

            string web = tempUrl.Authority;

            switch (web)
            {
                //7度书屋
                case WebSet.qdsw:
                    result = AnalysisCommonUrl(url);
                    break;

                //笔下文学（依依中文网）
                case WebSet.bxwx5:
                    result = AnalysisCommonUrl(url);
                    break;

                //第九中文网（有分页）
                case WebSet.dijiuzww:
                    result = AnalysisCommonUrl(url);
                    break;

                //清风小说（有分页）
                case WebSet.qfxs:
                    result = AnalysisCommonUrl(url);
                    break;

                //窝窝小说网（有分页）
                case WebSet.wwxsw:
                    result = AnalysisCommonUrl(url) + "mulu.html";
                    break;

                //找书网（需要分页）
                case WebSet.zsw:
                    result = AnalysisCommonUrl(url);
                    break;

                //55小说（古古小说网）
                case WebSet.xs55:
                    result = AnalysisCommonUrl(url);
                    break;

                //风云小说 //有可能要添加特殊处理  +index.html
                case WebSet.fyxs:
                    result = AnalysisCommonUrl(url) + "index.html";
                    break;

                //爱上中文
                case WebSet.aszw520:
                    result = AnalysisCommonUrl(url);
                    break;

                //大海中文
                case WebSet.dhzw:
                    result = AnalysisCommonUrl(url); ;
                    break;

                //酷酷看书
                case WebSet.kkks:
                    result = AnalysisCommonUrl(url);
                    break;

                //少年文学
                case WebSet.snwx:
                    result = AnalysisCommonUrl(url);
                    break;

                //手牵手
                case WebSet.sqsxs:
                    result = AnalysisCommonUrl(url);
                    break;

                //大书包
                case WebSet.dsb:
                    result = AnalysisCommonUrl(url);
                    break;

                //趣笔阁
                case WebSet.qubige:
                    result = AnalysisCommonUrl(url);
                    break;

                //书路
                case WebSet.shu6:
                    result = AnalysisCommonUrl(url);
                    break;

                //风华居
                case WebSet.fenghuaju:
                    result = AnalysisCommonUrl(url);
                    break;

                //云来阁
                case WebSet.ylg:
                    result = AnalysisCommonUrl(url);
                    break;

                //幼狮
                case WebSet.yssm:
                    result = AnalysisCommonUrl(url);
                    break;

                //4k
                case WebSet.fourkzw:
                    result = AnalysisCommonUrl(url);
                    break;

                case "书旗小说":
                    result = AnalysisSq(url);
                    break;

                case "木鱼哥":
                    result = AnalysisMyg(url);
                    break;

                case "无弹窗小说网":
                    result = AnalysisWtc(url);
                    break;
                default:
                    result = null;
                    break;
            }

            return result;
        }


        /// <summary>
        /// 通用截取目录url的方法
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisCommonUrl(string url)
        {
            string result = url.Substring(0, url.LastIndexOf('/') + 1);

            return result;
        }

        /// <summary>
        /// 云来阁
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisYlg(string html, string url)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "(?<=<a id=\"hlBookName\" href=).*?(?=\".*?</a>)");
            if (match != null)
            {
                result = match.ToString().Trim();
            }
            return result;
        }

        //<a id = "hlBookName" href="http://www.yunlaige.com/book/4351.html">大主宰

        /// <summary>
        /// 无弹窗
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisWtc(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "(?<=<a href=).*?(?=title=.*?>章节目录</a>)", RegexOptions.RightToLeft);
            if (match != null)
            {
                result = match.ToString().Trim().Replace("\"", "");
            }
            return result;
        }

        /// <summary>
        /// 木鱼哥
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisMyg(string html)
        {
            return null;

        }

        /// <summary>
        /// 书旗
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisSq(string html)
        {
            return null;

        }

        /// <summary>
        /// 爱上中文
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisAszw(string html)
        {
            return null;
        }

        /// <summary>
        /// 手牵手小说网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string AnalysisSlsxsw(string html)
        {
            string result = string.Empty;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "(?<=<a href=).*?(?=title=.*?>章节目录</a>)", RegexOptions.RightToLeft);
            if (match != null)
            {
                result = match.ToString().Trim().Replace("\"", "");
            }
            return result;
        }
    }


    public class AnalysisBookCatalogList
    {
        public static async Task<List<BookCatalog>> GetCatalogList(string url, string bookid, HttpHelper http)
        {
            List<BookCatalog> catalogList = null;
            if (string.IsNullOrEmpty(url))
            {
                return catalogList;
            }
            await Task.Run(async () =>
            {
                string html = await http.WebRequestGet(url, true);
                if (html != null)
                {
                    catalogList = AnalysisBookCatalogList.GetCatalogListByHtml(html, url);
                    if (catalogList != null)
                    {
                        foreach (var item in catalogList)
                        {
                            item.BookID = bookid;
                            item.CatalogContentGUID = item.BookID + item.Index.ToString();
                        }
                    }
                }
            });
            return catalogList;
        }

        public static List<BookCatalog> GetCatalogListByHtml(string html, string url)
        {
            List<BookCatalog> result = null;
            Uri tempUrl = new Uri(url);
            string web = tempUrl.Authority;

            switch (web)
            {
                //手牵手
                case WebSet.sqsxs:
                    result = AnalysisSlsxsw(html, url);
                    break;

                //7度
                case WebSet.qdsw:
                    result = AnalysisSlsxsw(html, url);
                    break;

                //大海
                case WebSet.dhzw:
                    result = AnalysisSlsxsw(html, url); ;
                    break;
                //爱上中文
                case WebSet.aszw520:
                    result = AnalysisAszw(html, url);
                    break;

                //笔下文学（依依中文网）
                case WebSet.bxwx5:
                    result = AnalysisYyzww(html);
                    break;

                //第九中文网（有分页）
                case WebSet.dijiuzww:
                    result = AnalysisdJzww(html, web);
                    break;

                //清风小说（有分页）
                case WebSet.qfxs:
                    result = AnalysisdQfxs(html, web);
                    break;

                //窝窝小说网（有分页）
                case WebSet.wwxsw:
                    result = AnalysisdWwxsw(html, web);
                    break;

                //找书网（需要分页）
                case WebSet.zsw:
                    result = AnalysisdJzww(html, web);
                    break;

                //55小说（古古小说网）
                case WebSet.xs55:
                    result = Analysisggxs(html, url);
                    break;

                //风云小说
                case WebSet.fyxs:
                    result = AnalysisFyxs(html, web);
                    break;

                //酷酷看书
                case WebSet.kkks:
                    result = AnalysisKkks(html, web);
                    break;

                //少年文学
                case WebSet.snwx:
                    result = AnalysisSlsxsw(html, url);
                    break;

                //大书包
                case WebSet.dsb:
                    result = AnalysisDefault(html);
                    break;

                //趣笔阁
                case WebSet.qubige:
                    result = AnalysisQbg(html, web);
                    break;

                //书路
                case WebSet.shu6:
                    result = AnalysisSl(html, url);
                    break;

                //风华居
                case WebSet.fenghuaju:
                    result = AnalysisQbg(html, web);
                    break;

                //云来阁
                case WebSet.ylg:
                    result = AnalysisYlg(html, url);
                    break;

                //4k
                case WebSet.fourkzw:
                    result = Analysis4k(html, url);
                    break;

                //幼狮
                case WebSet.yssm:
                    result = Analysis4k(html, url);
                    break;


                case "少年文学":
                    result = AnalysisSlsxsw(html, url);
                    break;

                case "书旗小说":
                    result = AnalysisSq(html);
                    break;

                case "木鱼哥":
                    result = AnalysisMyg(html);
                    break;

                case "无弹窗小说网":
                    result = AnalysisWtc(html);
                    break;

                default:
                    break;
            }
            return result;
        }


        private static List<BookCatalog> AnalysisSlsxsw(string html, string baseUrl)
        {

            List<BookCatalog> list = null;

            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html, "<div id=\"list\">.*?</div>");

            if (t_string != null)
            {
                MatchCollection matches = Regex.Matches(html, "<dd><a href.*?</a></dd>");
                //MatchCollection matches = Regex.Matches(html, "<div style=\"width:188px;float:left;\">.*?</div></div>");
                if (matches.Count == 0)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (var item in matches)
                    {
                        var url_Mathch = Regex.Match(item.ToString(), "(?<=href=\").*?(?=\")");
                        var title_Mathch = Regex.Match(item.ToString(), "(?<=title=\").*?(?=\")");

                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 去笔阁
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisQbg(string html, string baseUrl)
        {

            List<BookCatalog> list = null;

            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html, "<div id=\"list\">.*?</div>");
            if (t_string != null)
            {
                string str = Regex.Replace(t_string.ToString(), "<dt>.*?<dt>", "").ToString();
                MatchCollection matches = Regex.Matches(str.ToString(), "<dd.*?href=\"(.*?)\".*?>(.*?)</a></dd>");
                if (matches.Count == 0)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (Match item in matches)
                    {
                        var groups = item.Groups;
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();

                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 书路
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisSl(string html, string baseUrl)
        {

            List<BookCatalog> list = null;

            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html, "<div id=\"content\">.*?</div>");
            if (t_string != null)
            {
                string str = Regex.Replace(t_string.ToString(), "<dt>.*?<dt>", "").ToString();
                MatchCollection matches = Regex.Matches(str.ToString(), "<dd.*?href='(.*?)'.*?>(.*?)</a></dd>");
                if (matches.Count == 0)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (Match item in matches)
                    {
                        var groups = item.Groups;
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();

                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        private static List<BookCatalog> AnalysisAszw(string html, string baseUrl)
        {
            List<BookCatalog> list = null;

            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            if (html != null)
            {
                MatchCollection matches = Regex.Matches(html, "<td class=\"L\".*?href=\"(.*?)\".*?>(.*?)</a></td>");
                if (matches.Count == 0)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (Match item in matches)
                    {
                        var groups = item.Groups;
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();

                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;
        }


        private static List<BookCatalog> AnalysisSq(string html)
        {
            return null;

        }

        private static List<BookCatalog> AnalysisMyg(string html)
        {
            return null;

        }

        private static List<BookCatalog> AnalysisWtc(string html)
        {
            return null;

        }
        /// <summary>
        /// 依依中文网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisYyzww(string html)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "(?<=<dd>.*?href=\")(.*?)(?=\".*?>(.*?)</a></dd>)");

            //MatchCollection matches = Regex.Matches(html, "<div style=\"width:188px;float:left;\">.*?</div></div>");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 第九中文网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisdJzww(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "(?<=<dd>.*?href=\")(.*?)(?=\".*?>(.*?)</a></dd>)");
            if (matches != null && matches.Count < 4)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                for (int i = 0; i < matches.Count - 4; i++)
                {
                    Match item = matches[i];
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 古古小说网
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> Analysisggxs(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<table.*?</table>");
            if (match == null) return null;
            MatchCollection matches = Regex.Matches(match.ToString(), "(<td>.*?href=\"(.*?)\".*?>(.*?)</a></td>)");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[2].ToString();
                        var title_Mathch = groups[3].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.CatalogUrl = baseUrl + url_Mathch.ToString();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 4f
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> Analysis4k(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<div class=\"book_list\".*?</div>");
            if (match == null) return null;
            MatchCollection matches = Regex.Matches(match.ToString(), "<li>.*?href=\"(.*?)\".*?>(.*?)</a></li>");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;
        }


        private static List<BookCatalog> AnalysisYlg(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match match = Regex.Match(html, "<table.*?</table>");
            if (match == null) return null;
            MatchCollection matches = Regex.Matches(match.ToString(), "(<td>.*?href=\"(.*?)\".*?>(.*?)</a>.*?</td>)");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[2].ToString();
                        var title_Mathch = groups[3].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            string url = baseUrl.Substring(0, baseUrl.LastIndexOf('/') + 1);
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = url + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 风云小说
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisFyxs(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                Match match = Regex.Match(html, "<div class=\"readerListShow\".*?</div>");
                if (match == null) return null;
                MatchCollection matches = Regex.Matches(match.ToString(), "<td.*?href=\"(.*?)\".*?>(.*?)</a></td>");
                if (matches != null && matches.Count < 1)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (Match item in matches)
                    {
                        var groups = item.Groups;
                        if (groups != null && groups.Count > 2)
                        {
                            var url_Mathch = groups[1].ToString();
                            var title_Mathch = groups[2].ToString();
                            if (url_Mathch != null && title_Mathch != null)
                            {
                                BookCatalog catalog = new BookCatalog();
                                catalog.Index = i;
                                i++;
                                catalog.CatalogUrl = url_Mathch.ToString();
                                catalog.CatalogName = title_Mathch.ToString();
                                list.Add(catalog);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        }

        /// <summary>
        /// 酷酷看书
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisKkks(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                Match match = Regex.Match(html, "<div class=\"dirtitone\">.*?<div class=\"blinebgs\"");
                if (match == null) return null;
                MatchCollection matches = Regex.Matches(match.ToString(), "<li.*?href=\"(.*?)\".*?>(.*?)</a></li>");
                if (matches != null && matches.Count < 1)
                {
                    return list;
                }
                else
                {
                    list = new List<BookCatalog>();
                    int i = 0;
                    foreach (Match item in matches)
                    {
                        var groups = item.Groups;
                        if (groups != null && groups.Count > 2)
                        {
                            var url_Mathch = groups[1].ToString();
                            var title_Mathch = groups[2].ToString();
                            if (url_Mathch != null && title_Mathch != null)
                            {
                                BookCatalog catalog = new BookCatalog();
                                catalog.Index = i;
                                i++;
                                catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                                catalog.CatalogName = title_Mathch.ToString();
                                list.Add(catalog);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                list = null;
            }
            return list;
        }

        /// <summary>
        /// 清风小说
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisdQfxs(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "(?<=<td class=\"chapterBean\".*?href=\")(.*?)(?=\".*?>(.*?)</a></td>)");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }


        /// <summary>
        /// 窝窝小说网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisdWwxsw(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "(?<=<li>.*?href=\")(/books/.*?)(?=\".*?>(.*?)</a></li>)");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                int i = 0;
                list = new List<BookCatalog>();
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// 找书网
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<BookCatalog> AnalysisdZsw(string html, string baseUrl)
        {
            List<BookCatalog> list = null;
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "(?<=<li>.*?href=\")(/books/.*?)(?=\".*?>(.*?)</a></li>)");
            if (matches != null && matches.Count < 1)
            {
                return list;
            }
            else
            {
                list = new List<BookCatalog>();
                int i = 0;
                foreach (Match item in matches)
                {
                    var groups = item.Groups;
                    if (groups != null && groups.Count > 2)
                    {
                        var url_Mathch = groups[1].ToString();
                        var title_Mathch = groups[2].ToString();
                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.Index = i;
                            i++;
                            catalog.CatalogUrl = "http://" + baseUrl + url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        private static List<BookCatalog> AnalysisDefault(string html)
        {
            return null;

        }
    }


    public class AnalysisPagingUrlFromUrl
    {
        public static List<string> GetPagingUrlListFromUrl(string html, string pageUrl)
        {
            List<string> result = new List<string>();
            Uri tempUrl = new Uri(pageUrl);
            string web = tempUrl.Authority;

            switch (web)
            {
                //第九中文网（有分页）
                case WebSet.dijiuzww:
                    result = AnalysisDjzw(html, web);
                    break;

                //清风小说（有分页）
                case WebSet.qfxs:
                    result = AnalysisQfxs(html, web);
                    break;

                //窝窝小说网（有分页）
                case WebSet.wwxsw:
                    result = AnalysisWwxs(html, web);
                    break;

                //找书网（需要分页）
                case WebSet.zsw:
                    result = AnalysisDjzw(html, web);
                    break;
            }

            return result;

        }

        private static List<string> AnalysisDjzw(string html, string baseUrl)
        {
            List<string> lists = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                MatchCollection matches = Regex.Matches(html, @"/pdnovel.php\?mod=read.*?page=\d");
                if (matches != null && matches.Count > 1)
                {
                    lists = new List<string>();
                    for (int i = 1; i < matches.Count - 1; i++)
                    {
                        string url = "http://" + baseUrl + matches[i].ToString();
                        lists.Add(url);
                    }
                }
            }
            catch (Exception)
            {
                lists = null;
            }
            return lists;
        }

        /// <summary>
        /// 清风小说
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<string> AnalysisQfxs(string html, string baseUrl)
        {
            List<string> lists = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                MatchCollection matches = Regex.Matches(html, "(?<=<a rel=\"external nofollow\" href=\")/qfxs-\\d+/.*?&page=\\d");
                if (matches != null && matches.Count > 1)
                {
                    lists = new List<string>();
                    for (int i = 1; i < matches.Count - 1; i++)
                    {
                        string url = "http://" + baseUrl + matches[i].ToString();
                        lists.Add(url);
                    }
                }
            }
            catch (Exception)
            {
                lists = null;
            }
            return lists;
        }

        /// <summary>
        /// 窝窝小说
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<string> AnalysisWwxs(string html, string baseUrl)
        {
            List<string> lists = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                MatchCollection matches = Regex.Matches(html, "(?<=<a href=\")/books/\\d+/\\d+\\.html&page=\\d(?=\" class=\"f_\\w\">)");
                if (matches != null && matches.Count > 1)
                {
                    lists = new List<string>();
                    for (int i = 1; i < matches.Count - 1; i++)
                    {
                        string url = "http://" + baseUrl + matches[i].ToString();
                        lists.Add(url);
                    }
                }
            }
            catch (Exception)
            {
                lists = null;
            }
            return lists;
        }

        /// <summary>
        /// 找书网
        /// </summary>
        /// <param name="html"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        private static List<string> AnalysisZsw(string html, string baseUrl)
        {
            List<string> lists = null;
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                MatchCollection matches = Regex.Matches(html, "(?<=<a href=\")/books/\\d+/\\d+\\.html&page=\\d(?=\" class=\"f_\\w\">)");
                if (matches != null && matches.Count > 1)
                {
                    lists = new List<string>();
                    for (int i = 1; i < matches.Count - 1; i++)
                    {
                        string url = "http://" + baseUrl + matches[i].ToString();
                        lists.Add(url);
                    }
                }
            }
            catch (Exception)
            {
                lists = null;
            }
            return lists;
        }
    }
}



