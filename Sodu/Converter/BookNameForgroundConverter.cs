using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Sodu.Converter
{
    public class BookNameForgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string color = "#FF004D00";
            if (value != null)
            {
                if (value.ToString().Equals("起点中文网"))
                {
                    color = "Black";
                }
                else
                {
                    color = "#FF004D00";
                }
            }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
