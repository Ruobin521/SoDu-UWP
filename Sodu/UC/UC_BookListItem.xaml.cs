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
using Windows.UI.Xaml.Shapes;
using Sodu.Core.Util;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Sodu.UC
{
    public sealed partial class UC_BookListItem : UserControl
    {

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
        "Command", typeof(ICommand), typeof(UC_BookListItem), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }


        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            "CommandParameter", typeof(object), typeof(UC_BookListItem), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public UC_BookListItem()
        {
            this.InitializeComponent();
            //  this.Loaded += UC_BookListItem_Loaded;
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Command?.Execute(CommandParameter);
        }
    }
}
