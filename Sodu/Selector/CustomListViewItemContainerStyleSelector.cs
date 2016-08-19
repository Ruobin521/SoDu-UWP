using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Sodu.Core.Util;

namespace Sodu.Selector
{
    public class CustomListViewItemContainerStyleSelector : StyleSelector
    {
        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            Style st = null;
            //st.TargetType = typeof(ListViewItem);
            //Setter backGroundSetter = new Setter();
            //backGroundSetter.Property = ListViewItem.BackgroundProperty;
            var listView = ItemsControl.ItemsControlFromItemContainer(container) as ListView;
            if (listView != null)
            {
                var index = listView.IndexFromContainer(container);
                if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsPC)
                {
                    st = index % 2 != 0 ? Application.Current.Resources["CustomListViewItemStyleWithDifferentBackColor"] as Style : Application.Current.Resources["CustomListViewItemStyle"] as Style;

                    var rs = (Application.Current.Resources.ThemeDictionaries["Light"] as ResourceDictionary)["HigntLightlBackColor"];

                    var theme = Application.Current.RequestedTheme;
                }
                else
                {
                    st = Application.Current.Resources["CustomListViewItemStyle"] as Style;
                }
            }
            return st;
        }
    }

}
