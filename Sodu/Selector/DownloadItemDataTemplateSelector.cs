using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sodu.Selector
{
    public class DownloadItemDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DowmLoadEntity entity = item as DowmLoadEntity;

            if (entity.IsFast)
            {
                return App.Current.Resources["FastDownLoadTemplate"] as DataTemplate;
            }
            else
            {
                return App.Current.Resources["CommonDownLoadTemplate"] as DataTemplate;
            }
        }
        
    }
}
