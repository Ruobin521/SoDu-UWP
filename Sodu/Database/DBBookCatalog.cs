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

                      try
                      {
                          if (temp == null)
                          {
                              db.Insert(catalog);
                          }
                          else
                          {
                              db.Delete(temp);
                              db.Insert(catalog);
                          }
                      }
                      catch (Exception ex)
                      {

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
            catch (Exception)
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
            catch (Exception)
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

        public static List<BookCatalog> DeteleBookCatalogByBookID(string path, string bookID)
        {
            List<BookCatalog> result = null;
            try
            {
                using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
                {
                    db.CreateTable<BookCatalog>();
                    db.CreateTable<BookCatalogContent>();
                    db.RunInTransaction(() =>
                    {
                        var temp = (from m in db.Table<BookCatalog>()
                                    where m.BookID == bookID
                                    select m
                            );

                        var temp2 = (from m in db.Table<BookCatalogContent>()
                                     where m.BookID == bookID
                                     select m
                            );

                        if (temp != null && temp.Count() > 0)
                        {
                            foreach (var item in temp)
                            {
                                db.Delete(item);
                            }
                        }
                        if (temp2 != null && temp2.Count() > 0)
                        {
                            foreach (var item in temp2)
                            {
                                db.Delete(item);
                            }
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
