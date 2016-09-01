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
    public class PreLoadBtnForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = value as string;
            if (value == null || string.IsNullOrEmpty(str.ToString()))
            {
                if (ViewModel.ViewModelInstance.Instance.SettingPageViewModelInstance.IsNightModel)
                {
                    return new SolidColorBrush(Colors.White);
                }
                else
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                return new SolidColorBrush(Colors.Green);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
