using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Constants
{
    public static class AppDataPath
    {
        /// <summary>
        /// 历史记录数据库
        /// </summary>
        public const string HistoryDBPath = "HistoryDatabase.db";

        /// <summary>
        /// 本地缓存小说数据库
        /// </summary>
        public const string LocalBookDBPath = "LoacalBookDatabase.db";



        public const string SettingFileName = "SettingPageViewModel.xml";


        public static string GetHistoryDBPath()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, AppDataPath.HistoryDBPath);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            return path;
        }

        public static string GetLocalBookDBPath()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, AppDataPath.LocalBookDBPath);
            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            return path;
        }
        public static string GetSettingFilePath()
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, AppDataPath.SettingFileName);
            return path;
        }


    }
}
