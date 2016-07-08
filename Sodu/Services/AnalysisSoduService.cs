using Sodu.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sodu.Services
{
    public class AnalysisSoduService
    {
        public static ObservableCollection<BookEntity>[] GetHomePageBookList(string html)
        {
            ObservableCollection<BookEntity>[] listArray = new ObservableCollection<BookEntity>[3];
            try
            {
                html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

                string html0 = html;
                string html1 = html;
                string html2 = html;

                //整体
                Match match = Regex.Match(html0, "<div class=\"main-head\">.*?<table");

                if (match == null)
                {
                    return null;
                }

                //  MatchCollection matches = Regex.Matches(match.ToString(), "<div class=\"main-head\">.*?<div class=\"main-head\">");

                var matches = match.ToString().Split(new string[] { "<div class=\"main-head\">" }, StringSplitOptions.RemoveEmptyEntries);
                if (matches.Length < 3)
                {
                    return null;
                }

                ///站长推荐
                ObservableCollection<BookEntity> recommendList = null;
                try
                {
                    if (matches[1] != null && !string.IsNullOrEmpty(matches[1].ToString()))
                    {
                        recommendList = CommonGetEntityList(matches[1].ToString());
                    }
                }
                catch (Exception)
                {
                    recommendList = null;
                }


                //热门小说
                ObservableCollection<BookEntity> hotList = null;
                try
                {
                    if (matches[2] != null && !string.IsNullOrEmpty(matches[2].ToString()))
                    {
                        hotList = CommonGetEntityList(matches[2].ToString());
                    }
                }
                catch (Exception)
                {
                    hotList = null;
                }

                try
                {

                }
                catch (Exception)
                {

                }

                //更新列表
                html2 = html2.Replace("\r", "").Replace("\t", "").Replace("\n", "");
                Match t_string = Regex.Match(html2, "<form name=\"form2\".*?</form>");

                if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
                {
                    html2 = html2.Replace(t_string.ToString(), "");
                }
                ObservableCollection<BookEntity> updateList = CommonGetEntityList(html2);

                listArray[0] = null;
                listArray[1] = recommendList;
                listArray[2] = hotList;
            }
            catch (Exception)
            {
                listArray = null;
            }

            return listArray;
        }
        public static ObservableCollection<BookEntity> GetRankListFromHtml(string html)
        {
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            ObservableCollection<BookEntity> t_list = new ObservableCollection<BookEntity>();

            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?<div style=\"width:88px;float:left;\">.*?</div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            BookEntity t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                t_entity = new BookEntity();

                try
                {
                    Match match = Regex.Match(matches[i].ToString(), "<a href=\"javascript.*?</a>");


                    t_entity.BookName = Regex.Match(match.ToString(), "(?<=addToFav\\(.*?').*?(?=')").ToString();
                    t_entity.UpdateCatalogUrl = Regex.Match(matches[i].ToString(), "(?<=<a href=\").*?(?=\">.*?</a>)").ToString();
                    t_entity.BookID = Regex.Match(match.ToString(), "(?<=id=\").*?(?=\")").ToString().Replace("a", "");
                    t_entity.NewestChapterName = Regex.Match(matches[i].ToString(), "(?<=<a href.*?>).*?(?=</a>)", RegexOptions.RightToLeft).ToString();
                    Match match2 = Regex.Match(matches[i].ToString(), "(<div.*?>).*?(?=</div>)", RegexOptions.RightToLeft);
                    t_entity.UpdateTime = Regex.Replace(match2.ToString(), "<.*?>", "");
                    t_list.Add(t_entity);
                }
                catch
                {
                    t_list = null;
                    return t_list;
                }
            }
            return t_list;
        }

        public static ObservableCollection<BookEntity> GetUpdatePageBookList(string html)
        {
            ObservableCollection<BookEntity>[] listArray = new ObservableCollection<BookEntity>[3];
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            //更新列表
            Match t_string = Regex.Match(html, "<form name=\"form2\".*?</form>");
            if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
            {
                html = html.Replace(t_string.ToString(), "");
            }
            ObservableCollection<BookEntity> updateList = CommonGetEntityList(html);
            return updateList;
        }

        public static ObservableCollection<BookEntity> GetBookShelftListFromHtml(string html)
        {
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            //个人书架
            ObservableCollection<BookEntity> t_list = new ObservableCollection<BookEntity>();

            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=\"clearSc\".*?</div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            BookEntity t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i] == null) continue;
                t_entity = new BookEntity();
                try
                {
                    MatchCollection divmatches = Regex.Matches(matches[i].ToString(), "<div.*?</div>");
                    t_entity.BookName = Regex.Replace(divmatches[0].ToString(), "<.*?>", "").ToString();
                    t_entity.NewestChapterName = Regex.Replace(divmatches[1].ToString(), "<.*?>", "").ToString();
                    t_entity.UpdateTime = Regex.Replace(divmatches[2].ToString(), "<.*?>", "");
                    t_entity.UpdateCatalogUrl = Regex.Match(divmatches[0].ToString(), "(?<=<a href=\").*?(?=\")").ToString();
                    t_entity.BookID = Regex.Match(divmatches[3].ToString(), "(?<=id=).*?(?=\")").ToString();
                    t_list.Add(t_entity);
                }
                catch
                {
                    t_list = null;
                    return t_list;
                }
            }
            return t_list;
        }
        public static ObservableCollection<BookEntity> GetSearchResultkListFromHtml(string html)
        {
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html, "<form name=\"form2\".*?</form>");

            if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
            {
                html = html.Replace(t_string.ToString(), "");
            }
            ObservableCollection<BookEntity> t_list = CommonGetEntityList(html);
            return t_list;
        }


        public static ObservableCollection<BookEntity> CommonGetEntityList(string html)
        {
            ObservableCollection<BookEntity> t_list = new ObservableCollection<BookEntity>();
            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=xt1.*?</div>");
            //MatchCollection matches = Regex.Matches(html, "<div style=\"width:188px;float:left;\">.*?</div></div>");
            if (matches.Count == 0)
            {
                t_list = null;
                return t_list;
            }

            BookEntity t_entity;
            for (int i = 0; i < matches.Count; i++)
            {
                t_entity = new BookEntity();

                try
                {
                    Match match = Regex.Match(matches[i].ToString(), "<div style=\"width:482px;float:left;\">.*?</div>");
                    t_entity.BookName = Regex.Match(match.ToString(), "(?<=alt=\").*?(?=\")").ToString();
                    t_entity.UpdateCatalogUrl = Regex.Match(match.ToString(), "(?<=<a href=\").*?(?=\")").ToString();
                    t_entity.BookID = Regex.Match(matches[i].ToString(), "(?<=.*?id=\").*?(?=\")").ToString();
                    t_entity.NewestChapterName = Regex.Replace(match.ToString(), "<.*?>", "");
                    Match match2 = Regex.Match(matches[i].ToString(), "(?<=<.*?class=xt1>).*?(?=</div>)");
                    t_entity.UpdateTime = match2.ToString();

                    t_list.Add(t_entity);
                }
                catch
                {
                    t_list = null;
                    return t_list;
                }

            }
            return t_list;
        }


        public static List<BookEntity> GetBookUpdateChapterList(string html)
        {
            List<BookEntity> list = new List<BookEntity>();
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            MatchCollection matches = Regex.Matches(html, "<div class=\"main-html\".*?class=\"xt1.*?</div>");

            foreach (var item in matches)
            {
                MatchCollection matches2 = Regex.Matches(item.ToString(), "<a href.*?</a>");
                BookEntity t_entity = new BookEntity();
                t_entity.NewestChapterUrl = Regex.Match(matches2[0].ToString(), "(?<=&chapterurl=).*?(?=\")").ToString();

                bool value = WebSet.CheckUrl(t_entity.NewestChapterUrl);
                if (!value)
                {
                    continue;
                }
                t_entity.NewestChapterName = Regex.Match(matches2[0].ToString(), "(?<=alt=\").*?(?=\")").ToString();

                //  t_entity.ChapterName = Regex.Replace(matches2[0].ToString(), "<.*?>", "").ToString();
                t_entity.LyWeb = Regex.Replace(matches2[1].ToString(), "<.*?>", "").ToString();

                Match match2 = Regex.Match(item.ToString(), "(?<=<.*?class=\"xt1\">).*?(?=</div>)");
                t_entity.UpdateTime = match2.ToString();

                list.Add(t_entity);
            }

            return list;

        }
    }
}
