using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace SoDu.Core.Util
{
    public class ColorBrushHelper
    {
        public static SolidColorBrush ConverterFromString(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            str = str.Replace("#", "").Replace(" ", "");

            var a = (byte)Convert.ToInt32(str.Substring(0, 2), 16);
            var r = (byte)Convert.ToInt32(str.Substring(2, 2), 16);
            var g = (byte)Convert.ToInt32(str.Substring(4, 2), 16);
            var b = (byte)Convert.ToInt32(str.Substring(6, 2), 16);

            SolidColorBrush brush = new SolidColorBrush((Color)Color.FromArgb(a, r, g, b));

            return brush;
        }
    }
}
