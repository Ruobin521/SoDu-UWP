using Sodu.Model;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Database
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

    }
}
