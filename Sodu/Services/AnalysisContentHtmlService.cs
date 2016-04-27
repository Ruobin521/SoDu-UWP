using Sodu.Model;
using System;
using System.Collections.Generic;
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
        public const string wwxsw = " www.quanxiong.org";
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
        public const string qbiquge = "www.qbiquge.com";

        /// <summary>
        /// 书路小说
        /// </summary>
        public const string shu6 = "www.shu6.cc";

        /// <summary>
        /// 风华居
        /// </summary>
        public const string fenghuaju = "www.fenghuaju.com";
    }
    public class AnalysisContentHtmlService
    {
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
                    result = ReplaceSymbol(html);
                    break;

                case WebSet.sqsxs:
                    result = AnalysisSlsxsw(html);
                    break;

                case WebSet.dhzw:
                    result = AnalysisSlsxsw(html); ;
                    break;

                case WebSet.aszw520:
                    result = AnalysisAszw(html);
                    break;

                case WebSet.snwx:
                    result = AnalysisSlsxsw(html);
                    break;

                //case "书旗小说":
                //    result = AnalysisSq(html);
                //    break;

                //case "木鱼哥":
                //    result = AnalysisMyg(html);
                //    break;

                //case "无弹窗小说网":
                //    result = AnalysisWtc(html);
                //    break;
                default:
                    result = ReplaceSymbol(html);
                    break;
            }
            if (!string.IsNullOrEmpty(result))
            {
                result = result + "\n\n\n\n\n\n\n\n";
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
            Match match = Regex.Match(html, "<div id=\"BookText\">.*?</div>");
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
            Match match = Regex.Match(html, "<p class=\"vote\">.*?</div>");
            if (match != null)
            {
                result = match.ToString();
                result = ReplaceSymbol(result);
                result = result;
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
            Match match = Regex.Match(html, "<div id=\"contents\">.*?</div>");
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
            Match match = Regex.Match(html, "<p id=\"?content\"?.*?</p>");
            if (match != null)
            {
                result = match.ToString();
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
            Match match = Regex.Match(html, "(?<=<div class=\"bq\">本书关键词.*?</div>).*?(?=<font color=\"red\"><a href=.*?</font>)");
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
            html = Regex.Replace(html, "&nbsp;", " ");
            html = Regex.Replace(html, "</p.*?>", "\n");
            html = Regex.Replace(html, "<.*?>", "");
            result = html;
            return result;
        }


    }


    public class AnalysisBookCatalogUrl
    {
        public static string GetBookCatalogListUrl(string html, string webName)
        {
            string result = string.Empty;
            try
            {
                switch (webName)
                {
                    case "手牵手小说网":
                        result = AnalysisSlsxsw(html);
                        break;

                    case "七度书屋":
                        result = AnalysisSlsxsw(html);
                        break;

                    case "大海中文":
                        result = AnalysisSlsxsw(html); ;
                        break;

                    case "爱上中文":
                        result = AnalysisAszw(html);
                        break;

                    case "少年文学":
                        result = AnalysisSlsxsw(html);
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
                        result = AnalysisWtc(html);
                        break;
                }
            }
            catch (Exception)
            {
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
            return null;
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
        public static List<BookCatalog> GetCatalogListByHtml(string html, string webName)
        {
            List<BookCatalog> result = null;

            switch (webName)
            {
                case "手牵手小说网":
                    result = AnalysisSlsxsw(html);
                    break;

                case "七度书屋":
                    result = AnalysisSlsxsw(html);
                    break;

                case "大海中文":
                    result = AnalysisSlsxsw(html); ;
                    break;

                case "爱上中文":
                    result = AnalysisAszw(html);
                    break;

                case "少年文学":
                    result = AnalysisSlsxsw(html);
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

                //case " 天天看书网":
                //    result = "";
                //    break;

                //case "  二六九小说网":
                //    result = "";
                //    break;
                default:
                    break;
            }
            return result;
        }


        private static List<BookCatalog> AnalysisSlsxsw(string html)
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
                    foreach (var item in matches)
                    {
                        var url_Mathch = Regex.Match(item.ToString(), "(?<=href=\").*?(?=\")");
                        var title_Mathch = Regex.Match(item.ToString(), "(?<=title=\").*?(?=\")");

                        if (url_Mathch != null && title_Mathch != null)
                        {
                            BookCatalog catalog = new BookCatalog();
                            catalog.CatalogUrl = url_Mathch.ToString();
                            catalog.CatalogName = title_Mathch.ToString();
                            list.Add(catalog);
                        }
                    }
                }
            }

            return list;

        }

        private static List<BookCatalog> AnalysisAszw(string html)
        {
            throw new NotImplementedException();
        }


        private static List<BookCatalog> AnalysisSq(string html)
        {
            throw new NotImplementedException();
        }

        private static List<BookCatalog> AnalysisMyg(string html)
        {
            throw new NotImplementedException();
        }

        private static List<BookCatalog> AnalysisWtc(string html)
        {
            throw new NotImplementedException();
        }
    }
}
