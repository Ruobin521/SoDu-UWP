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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UC
{
    public sealed partial class PopupWindow : UserControl
    {

        private Storyboard storyBoard;

        private Popup m_Popup;

        private string txtMessage;
        public PopupWindow()
        {
            this.InitializeComponent();
            m_Popup = new Popup();
            m_Popup.Child = this;
            this.Margin = new Thickness(Window.Current.Bounds.Width, Windows.UI.ViewManagement.StatusBar.GetForCurrentView().OccludedRect.Height, 0, 0);
            this.Loaded += PopupWindow_Loaded;
            //this.Unloaded += PopupWindow_Unloaded;
        }
        public PopupWindow(string message) : this()
        {
            this.txtMessage = message;
        }


        private void PopupWindow_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PopupWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tb_Notify.Text = this.txtMessage;
            this.storyBoard = this.tb_Notify_in;
            this.easeKeyframe.Value = -this.ActualWidth;
            this.storyBoard.Completed += StoryBoard_Completed;
            this.storyBoard.Begin();
        }


        private void StoryBoard_Completed(object sender, object e)
        {
            m_Popup.IsOpen = false;
        }
        public void ShowWindow()
        {
            m_Popup.IsOpen = true;
        }


    }
}
