﻿using System;
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
    public class PreLoadBtnForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return App.Current.Resources["CustomPreLoadedBtnCommonAppBarButtonStyle"] as Style;

            }
            else
            {
                return App.Current.Resources["CustomPreLoadedAppBarButtonStyle"] as Style;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
