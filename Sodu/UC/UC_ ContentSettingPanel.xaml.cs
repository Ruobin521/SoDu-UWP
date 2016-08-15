﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UC
{

    public delegate void CloseHanlder();
    public sealed partial class UC_ContentSettingPanel : UserControl
    {

        public event CloseHanlder Closed;
        public UC_ContentSettingPanel()
        {
            this.InitializeComponent();
        }


        public void Close()
        {
            CloseStoryboard.Begin();
            Closed?.Invoke();
        }

        public void Show()
        {
            ShowStoryboard.Begin();
        }

        private void BtnColose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}