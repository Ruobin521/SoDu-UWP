using Sodu.Core.Config;
using Sodu.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Sodu.Converter
{
    public class LocalBookCoverConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BookEntity entity = value as BookEntity;
            if (entity != null)
            {
                string path = AppDataPath.GetLocalBookCoverPath(entity.BookID);
                if (File.Exists(path))
                {
                    return path;
                }
                else
                {
                    return entity.Cover;
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
