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
    public class DBLocalBook
    {
        public static bool InsertOrUpdateBookEntity(string path, BookEntity book)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookEntity>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookEntity>()
                                    where m.BookID == book.BookID
                                    select m
                                ).FirstOrDefault();
                        if (temp == null)
                        {
                            db.Insert(book);
                        }
                        else
                        {
                            book.Id = temp.Id;
                            db.Update(book);
                        }
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }


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

        public static List<BookEntity> GetAllLocalBookList(string path)
        {
            List<BookEntity> result = new List<BookEntity>();
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookEntity>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookEntity>()
                                    select m
                                );
                        if (temp != null && temp.Count() > 0)
                        {
                            result = temp.ToList();
                        }
                        else
                        {
                            result = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        result = null;
                    }
                });
            }
            return result;
        }


        public static bool DeleteAllLocalBooks(string path)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookEntity>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        db.DeleteAll<BookEntity>();
                    }
                    catch (Exception ex)
                    {
                        result = false;
                    }
                });
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
                        db.DeleteAll<BookEntity>();
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
