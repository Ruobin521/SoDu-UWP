using Sodu.Util;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Services
{
    public class CommonMethod
    {

        /// <summary>
        /// ShowMessage
        /// </summary>
        public static void ShowMessage(string message)
        {
            ToastHeplper.ShowMessage(message);
        }
    }
}
