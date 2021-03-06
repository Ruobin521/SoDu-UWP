﻿using Sodu.Pages;
using Sodu.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sodu.ViewModel;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Sodu
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            NavigationService.ContentFrame = NavigationService.ContentFrame ?? this.ContentFrame;
            this.HightListBack.Visibility = Visibility.Visible;

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(50);
            string version = ViewModelInstance.Instance.AboutPage.AppVersion;
            string version2 = ViewModelInstance.Instance.SettingPageViewModelInstance.GetAppVersion();
            if (version2 == null || !version2.Equals(version))
            {
                ViewModelInstance.Instance.SettingPageViewModelInstance.SetAppVersion(version);
                NavigationService.ContentFrame.Navigate(typeof(AboutPage));
            }

        }
    }
}
