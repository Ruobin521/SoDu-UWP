using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Sodu.Core.Util;

namespace Sodu.Converter
{
    public class CommandBarBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsMobile)
            {
                if (value != null && (bool)value)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return false;
            // throw new NotImplementedException();
        }
    }
}
