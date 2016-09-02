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

    public delegate void CloseHanlder();
    public delegate void FontSizeChanged(double value);
    public delegate void LineHeightChanged(double value);

    public sealed partial class UC_ContentSettingPanel : UserControl
    {

        public event CloseHanlder Closed;
        public event FontSizeChanged FontSizeChanged;
        public event LineHeightChanged LineHeightChanged;
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

        private void RangeBase_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double value = (sender as Slider).Value;
            FontSizeChanged?.Invoke(value);
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            double value = (sender as Slider).Value;
            LineHeightChanged?.Invoke(value);
        }
    }
}
