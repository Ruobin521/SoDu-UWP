using Sodu.Core.Util;
using Sodu.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Sodu.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9nblggh4sk4v"));
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigateTo(typeof(HelpPage));
        }

        private void QQTextButton_Click(object sender, RoutedEventArgs e)
        {
            string str = "568856882";
            DataPackage dp = new DataPackage();
            dp.SetText(str);
            Clipboard.SetContent(dp);
            ToastHeplper.ShowMessage("已复制群号到剪切板");
        }

        private void VersionButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.updateText.Visibility == Visibility.Collapsed)
            {
                this.updateText.Visibility = Visibility.Visible;

                this.transform.Rotation = -90;
            }
            else
            {
                this.updateText.Visibility = Visibility.Collapsed;
                this.transform.Rotation = 90;
            }
        }

        private void AlipayTextButton_Click(object sender, RoutedEventArgs e)
        {
            string str = "83250112@qq.com";
            DataPackage dp = new DataPackage();
            dp.SetText(str);
            Clipboard.SetContent(dp);
            ToastHeplper.ShowMessage("已复制支付婊账号到剪切板");
        }
    }
}
