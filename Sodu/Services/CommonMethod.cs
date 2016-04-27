using Sodu.Util;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Sodu.Services
{
    public class CommonMethod
    {

        /// <summary>
        /// ShowMessage
        /// </summary>
        public async static void ShowMessage(string message)
        {
            await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
          {
              ToastHeplper.ShowMessage(message);
          });

        }
    }
}
