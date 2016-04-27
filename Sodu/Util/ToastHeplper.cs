using Sodu.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sodu.Util
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
            UC.PopupWindow popup = new UC.PopupWindow(message);
            popup.ShowWindow();
        }
        public static void ShowMessage_1(string message)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) return;
            Page page = rootFrame.Content as Page;
            if (page == null) return;
            Grid grid = page.Content as Grid;
            if (grid == null) return;

            NotificationBar notiBar = new NotificationBar();
            notiBar.Message = message;
            notiBar.Margin = new Thickness(0, 40, 0, 0);
            notiBar.GridParent = grid;
            notiBar.AnimationCompleted += NotiBar_AnimationCompleted;
            grid.Children.Add(notiBar);
            Grid.SetRowSpan(notiBar, grid.RowDefinitions.Count);
        }

        private static void NotiBar_AnimationCompleted(object sender, EventArgs e)
        {
            (sender as NotificationBar).GridParent.Children.Remove(sender as UIElement);

        }
    }
}
