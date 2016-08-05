using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Sodu.Converter
{
    public class TabControlSelectedForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return App.Current.Resources["TabTitleTextBlockSelectedStyle"] as Style;

            }
            else
            {
                return App.Current.Resources["TabTitleTextBlockCommonStyle"] as Style;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class TabControlSelectedBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
            {
                return App.Current.Resources["TabBorderSelectedStyle"] as Style;

            }
            else
            {
                return App.Current.Resources["TabBorderCommonStyle"] as Style;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
