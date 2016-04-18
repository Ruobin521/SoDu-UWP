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
        /// 启动加载动画
        /// </summary>
        public static void StartLoading2()
        {
            //  ViewModelInstance.Instance.MainPageViewModelInstance.StartLoading();
        }

        /// <summary>
        /// 结束加载动画
        /// </summary>
        public static void StopLoading2()
        {
            //ViewModelInstance.Instance.MainPageViewModelInstance.StopLoading();
        }

        /// <summary>
        /// ShowMessage
        /// </summary>
        public static void ShowMessage(string message)
        {
            ToastHeplper.ShowMessage(message);
        }
    }
}
