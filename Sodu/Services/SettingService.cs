using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Sodu.Services
{
    public static class SettingService
    {
        private static ApplicationDataContainer _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static string GetSetting(string key)
        {
            string value = _localSettings.Values[key].ToString();
            return value;
        }

        public static void SetSetting(string key, object value)
        {
            _localSettings.Values[key] = value;
        }

        public static bool CheckKeyExist(string key)
        {
            return _localSettings.Values.ContainsKey(key);
        }
    }
}
