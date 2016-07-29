using Sodu.Core.Model;
using Sodu.Model;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Core.Database
{
    public class DBBookCatalogContent
    {
        public static bool InsertOrUpdateBookCatalogContent(string path, BookCatalogContent content)
        {
            bool result = true;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalogContent>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalogContent>()
                                    where m.BookID == content.BookID && m.CatalogUrl == content.CatalogUrl
                                    select m
                            ).FirstOrDefault();
                        if (temp == null)
                        {
                            db.Insert(content);
                        }
                        else
                        {
                            db.Delete(temp);
                            db.Insert(content);
                        }
                    });
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        public static bool InsertOrUpdateBookCatalogContents(string path, List<BookCatalogContent> contents)
        {
            bool result = true;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalogContent>();
                    db.RunInTransaction(() =>
                    {
                        foreach (var content in contents)
                        {
                            //var temp = (from m in db.Table<BookCatalogContent>()
                            //            where m.BookID == content.BookID && m.CatalogUrl == content.CatalogUrl
                            //            select m
                            //).FirstOrDefault();

                            //if (temp == null)
                            //{
                            //    db.Insert(content);
                            //}
                            //else
                            //{
                            db.Execute("DELETE FROM BookCatalogContent WHERE BookID = ? AND CatalogUrl = ?", content.BookID, content.CatalogUrl);
                            db.Insert(content);
                            //  }
                        }
                    });
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


        public static BookCatalogContent SelectBookCatalogContent(string path, string catalogUrl)
        {
            BookCatalogContent result = null;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalogContent>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalogContent>()
                                    where m.CatalogUrl == catalogUrl
                                    select m
                                ).FirstOrDefault();
                        if (temp != null)
                        {
                            result = temp;
                        }
                    });
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

    }
}
