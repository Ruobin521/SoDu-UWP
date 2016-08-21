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

    public delegate void StoryBoardCompleted();

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


        public static readonly DependencyProperty LefSwitchValueProperty = DependencyProperty.Register(
            "LefSwitchValue", typeof(double), typeof(UC_ContentControl), new PropertyMetadata(default(double)));

        public double LefSwitchValue
        {
            get { return (double)GetValue(LefSwitchValueProperty); }
            set { SetValue(LefSwitchValueProperty, value); }
        }

        public static readonly DependencyProperty RightSwitchValueProperty = DependencyProperty.Register(
            "RightSwitchValue", typeof(double), typeof(UC_ContentControl), new PropertyMetadata(default(double)));

        public double RightSwitchValue
        {
            get { return (double)GetValue(RightSwitchValueProperty); }
            set { SetValue(RightSwitchValueProperty, value); }
        }


        public TextBlock Textblock
        {
            get { return this.txt; }
        }

        public UC_ContentControl()
        {
            this.InitializeComponent();

            this.Loaded += UC_ContentControl_Loaded;
        }

        private void UC_ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            LefSwitchValue = -this.ActualWidth;
            RightSwitchValue = this.ActualWidth;
        }

        public void StartToLeft()
        {
            this.leftStartValue.Value = (this.RenderTransform as CompositeTransform).TranslateX;
            this.leftEndValue.Value = LefSwitchValue;
            StoryboardToLeft.Begin();
        }


        public void StartToRight()
        {
            this.rightStartValue.Value = (this.RenderTransform as CompositeTransform).TranslateX;
            this.rightEndValue.Value = RightSwitchValue;
            StoryboardToRight.Begin();
        }

    }
}
