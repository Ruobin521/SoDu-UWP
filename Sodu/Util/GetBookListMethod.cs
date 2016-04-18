using Sodu.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sodu.Util
{
    public class GetBookListMethod
    {
        public static ObservableCollection<BookEntity>[] GetHomePageBookList(string html)
        {
            ObservableCollection<BookEntity>[] listArray = new ObservableCollection<BookEntity>[3];
            html = html.Replace("\r", "").Replace("\t", "").Replace("\n", "");

            string html0 = html;
            string html1 = html;
            string html2 = html;


            //曾经看过的小说
            ObservableCollection<BookEntity> lookedList = new ObservableCollection<BookEntity>();
            Match readedMatch0 = Regex.Match(html0, "<form name=\"form1\".*?</form>");

            if (readedMatch0 != null && !string.IsNullOrEmpty(readedMatch0.ToString()))
            {
                lookedList = CommonGetEntityList(readedMatch0.ToString(), false);
            }

            //个人书架
            ObservableCollection<BookEntity> personalList = new ObservableCollection<BookEntity>();
            Match readedMatch = Regex.Match(html1, "<form name=\"form2\".*?</form>");

            if (readedMatch != null && !string.IsNullOrEmpty(readedMatch.ToString()))
            {
                personalList = CommonGetEntityList(readedMatch.ToString(), true);
            }


            //更新列表
            html2 = html2.Replace("\r", "").Replace("\t", "").Replace("\n", "");
            Match t_string = Regex.Match(html2, "<form name=\"form2\".*?</form>");

            if (t_string != null && !string.IsNullOrWhiteSpace(t_string.ToString()))
            {
                html2 = html2.Replace(t_string.ToString(), "");
            }
            ObservableCollection<BookEntity> updateList = CommonGetEntityList(html2);

            listArray[0] = lookedList;
            listArray[1] = personalList;
            listArray[2] = updateList;
            return listArray;
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
            Match t_string = Regex.Match(html, "<form name=\"form2\".*?</form>");
            //个人书架
            ObservableCollection<BookEntity> personalList = new ObservableCollection<BookEntity>();
            Match readedMatch = Regex.Match(html, "<form name=\"form2\".*?</form>");

            if (readedMatch != null && !string.IsNullOrEmpty(readedMatch.ToString()))
            {
                personalList = CommonGetEntityList(readedMatch.ToString(), true);
            }
            return personalList;
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


        public static ObservableCollection<BookEntity> CommonGetEntityList(string html, bool flag = false)
        {
            ObservableCollection<BookEntity> t_list = new ObservableCollection<BookEntity>();
            MatchCollection matches = Regex.Matches(html, "<a href=\"/mulu.*?>.*?</div></div>");
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
                string t_string = "<div>" + matches[i].ToString();
                try
                {
                    MatchCollection matches2 = Regex.Matches(t_string, "<div.*?>.*?</div>");
                    if (matches.Count == 0)
                    {
                        continue;
                    }
                    t_entity.BookName = Regex.Replace(matches2[0].ToString(), "<.*?>", "");
                    t_entity.ContentsUrl = Regex.Match(matches2[0].ToString(), "(?<=<a href=\").*?(?=\">)").ToString();

                    t_entity.BookID = Regex.Match(matches2[0].ToString(), "(?<=<a href=.*?_).*?(?=.html)").ToString();



                    t_entity.ChapterName = Regex.Replace(matches2[1].ToString(), "<.*?>", "");

                    Match str_Unread = Regex.Match(t_entity.ChapterName, @"\(未读.*?\)");
                    if (str_Unread != null && !string.IsNullOrEmpty(str_Unread.ToString()))
                    {
                        t_entity.UnReadCountData = str_Unread.ToString();
                        t_entity.ChapterName = t_entity.ChapterName.Replace(t_entity.UnReadCountData, "");
                    }
                    t_entity.UpdateTime = Regex.Replace(matches2[2].ToString(), "<.*?>", "");
                    //if (flag)
                    //{
                    //    t_entity.IfBookshelf = true;
                    //}
                    //else
                    //{
                    //    t_entity.IfBookshelf = false;
                    //}
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
            Match str = Regex.Match(html, "<center><script.*?>.*</div></div>");
            MatchCollection collections = Regex.Matches(str.ToString(), "<a href=.*?>.*?</div></div>");

            foreach (var item in collections)
            {
                BookEntity t_entity = new BookEntity();
                t_entity.ChapterUrl = Regex.Match(item.ToString(), "(?<=<a href=').*?(?=')").ToString();
                t_entity.ChapterName = Regex.Match(item.ToString(), "(?<=<a href=.*?>).*?(?=</a>)").ToString();
                t_entity.LyUrl = Regex.Match(item.ToString(), "(?<=<a href=.*?>).*?(?=</a>)", RegexOptions.RightToLeft).ToString();
                t_entity.UpdateTime = Regex.Match(item.ToString(), "(?<=<div.*?>).*?(?=</div></div>)", RegexOptions.RightToLeft).ToString();

                list.Add(t_entity);
            }

            return list;

        }
    }
}
