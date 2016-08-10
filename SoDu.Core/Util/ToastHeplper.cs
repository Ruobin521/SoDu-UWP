using SoDu.Core.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;

namespace Sodu.Core.Util
{
    public class ToastHeplper
    {
        public static void ToastMessage(string message)
        {
            string xml = "<toast lang=\"zh-CN\">" +
                "<visual>" +
                    "<binding template=\"ToastGeneric\">" +
                         "<text>" + message + "</text>" +
                   //"<text>看，桃花开了。</text>" +
                   //"<image placement=\"inline\" src=\"ms-appx:///Images/Logo.jpg\" />" +
                   //"<text>这桃花好看吧？</text>" +
                   "</binding>" +
               "</visual>" +
            "</toast>";
            // 创建XML文档
            XmlDocument doc = new XmlDocument();
            // 加载XML
            doc.LoadXml(xml);
            // 创建通知实例
            ToastNotification notification = new ToastNotification(doc);
            // 显示通知
            ToastNotifier nt = ToastNotificationManager.CreateToastNotifier();
            nt.Show(notification);

        }
        public static void ShowMessage(string message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                PopupWindow popup = new PopupWindow(message);
                popup.ShowWindow();
            });
        }
    }
}
