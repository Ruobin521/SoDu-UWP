using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

    public sealed partial class UC_DownLoadItem : UserControl
    {
        public ICommand PasueCommand
        {
            get { return (ICommand)GetValue(PasueCommandProperty); }
            set { SetValue(PasueCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasueCommandProperty =
            DependencyProperty.Register("PasueCommand", typeof(ICommand), typeof(UC_DownLoadItem), new PropertyMetadata(null));



        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeleteCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register("DeleteCommand", typeof(ICommand), typeof(UC_DownLoadItem), new PropertyMetadata(null, IdPropertyChangedCallback));



        private static void IdPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var ddo = dependencyObject as ICommand;
        }
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(UC_DownLoadItem), new PropertyMetadata(null));




        public UC_DownLoadItem()
        {
            this.InitializeComponent();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.DeleteCommand != null)
            {
                this.DeleteCommand.Execute(this.CommandParameter);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (this.PasueCommand != null)
            {
                this.PasueCommand.Execute(this.CommandParameter);
            }
        }
    }
}
