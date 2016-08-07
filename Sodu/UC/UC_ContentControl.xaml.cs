using System;
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
    public sealed partial class UC_ContentControl : UserControl
    {
        public static readonly DependencyProperty TextFontSizeProperty = DependencyProperty.Register(
            "TextFontSize", typeof(double), typeof(UC_ContentControl), new PropertyMetadata(15.0));

        public double TextFontSize
        {
            get { return (double)GetValue(TextFontSizeProperty); }
            set { SetValue(TextFontSizeProperty, value); }
        }

        public static readonly DependencyProperty TextPropertyProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(UC_ContentControl), new PropertyMetadata(default(string)));

        public string Text
        {
            get { return (string)GetValue(TextPropertyProperty); }
            set { SetValue(TextPropertyProperty, value); }
        }

        public UC_ContentControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
