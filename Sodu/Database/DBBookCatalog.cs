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
    public class DBBookCatalog
    {
        public static bool InsertOrUpdateBookCatalog(string path, BookCatalog catalog)
        {
            bool result = true;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalog>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalog>()
                                    where m.BookID == catalog.BookID && m.Index == catalog.Index
                                    select m
                            ).FirstOrDefault();
                        if (temp == null)
                        {
                            db.Insert(catalog);
                        }
                        else
                        {
                            catalog.Id = temp.Id;
                            db.Update(catalog);
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
        public static List<BookCatalog> SelectBookCatalogs(string path, string bookID)
        {
            List<BookCatalog> result = null;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalog>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalog>()
                                    where m.BookID == bookID
                                    select m
                            );
                        if (temp != null && temp.Count() > 0)
                        {
                            result = temp.OrderBy(p => p.Index).ToList();
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
        public static List<BookCatalog> SelectAllBookCatalogs(string path)
        {
            List<BookCatalog> result = null;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalog>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalog>()
                                    select m
                            );
                        if (temp != null && temp.Count() > 0)
                        {
                            result = temp.OrderBy(p => p.Index).ToList();
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
        public static bool DeleteAllCatalog(string path)
        {
            bool result = true;

            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookCatalog>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.DeleteAll<BookCatalog>();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }
    }
}
