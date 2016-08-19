using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sodu.Core.Model;
using SQLite.Net;
using SQLite.Net.Platform.WinRT;

namespace SoDu.Core.Database
{
    public class DBBookShelf
    {
        public static bool InsertOrUpdateBook(string path, BookEntity book)
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
                    catch (Exception)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }
        public static bool InsertOrUpdateBooks(string path, List<BookEntity> books)
        {
            bool result = true;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookEntity>();
                db.RunInTransaction(() =>
                {
                    foreach (var book in books)
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
                        catch (Exception)
                        {
                            result = false;
                        }
                    }
                });
            }
            return result;
        }


        public static List<BookEntity> GetBooks(string path)
        {
            List<BookEntity> list = new List<BookEntity>();
            try
            {
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
                            if (temp == null || temp.Count() < 1)
                            {
                                list = null;
                            }
                            else
                            {
                                list = temp.ToList().OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
                            }
                        }
                        catch (Exception)
                        {
                            list = null;
                        }
                    });
                }
            }
            catch (Exception)
            {
                list = null;
            }

            return list;
        }
        public static bool ClearBooks(string path)
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
                    catch (Exception)
                    {
                        result = false;
                    }

                });
            }
            return result;
        }

        public static bool DeleteBook(string path, BookEntity entity)
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
                                    where m.BookID == entity.BookID
                                    select m
                                   ).FirstOrDefault();
                        if (temp != null)
                        {
                            db.Delete(temp);
                        }
                    }
                    catch (Exception)
                    {
                        result = false;
                    }
                });
            }
            return result;
        }

        public static BookEntity GetBook(string path, BookEntity entity)
        {
            BookEntity result = null;
            using (var db = new SQLiteConnection(new SQLitePlatformWinRT(), path))
            {
                db.CreateTable<BookEntity>();
                db.RunInTransaction(() =>
                {
                    try
                    {
                        var temp = (from m in db.Table<BookEntity>()
                                    where m.BookID == entity.BookID
                                    select m
                                   ).FirstOrDefault();

                        result = temp;
                    }
                    catch (Exception)
                    {
                        result = null;
                    }
                });
            }
            return result;
        }

    }
}
