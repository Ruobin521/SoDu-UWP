using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Sodu.Controls
{
    public sealed class NotificationBar : Control
    {
        private TextBlock notifyBlock;
        private Grid mainGrid;
        private Storyboard storyBoard;

        public string Message { get; set; }

        public event EventHandler AnimationCompleted;

        public Grid GridParent { get; set; }
        public NotificationBar()
        {
            this.DefaultStyleKey = typeof(NotificationBar);
            this.Loaded += NotificationBar_Loaded;
        }
        private void NotificationBar_Loaded(object sender, RoutedEventArgs e)
        {
            GetTextBlockControl();
            GetStoryBoardControl("tb_Notify_in");
            ShowMessage(Message);
        }


        private void GetTextBlockControl()
        {
            if (this.notifyBlock == null)
            {
                this.notifyBlock = this.GetTemplateChild("tb_Notify") as TextBlock;
            }
        }
        private void GetStoryBoardControl(string name)
        {
            if (this.storyBoard == null)
            {
                this.storyBoard = this.GetTemplateChild(name) as Storyboard;
                this.storyBoard.Completed += StoryBoard_Completed;
            }
        }
        public void ShowMessage(string message)
        {
            if (notifyBlock != null && storyBoard != null)
            {
                notifyBlock.Text = message;
                storyBoard.Begin();
            }
        }
        /// <summary>
        /// 当动画结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryBoard_Completed(object sender, object e)
        {
            if (AnimationCompleted != null)
            {
                AnimationCompleted(this, new EventArgs());
            }
        }
    }
}
