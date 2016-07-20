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
    public class DBHistory
    {
        public static bool InsertOrUpdateBookHistory(string path, BookEntity book)
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

        public static List<BookEntity> GetBookHistories(string path)
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
                                list = temp.ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            list = null;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                list = null;
            }

            return list;
        }
        public static bool ClearHistories(string path)
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

    }
}
