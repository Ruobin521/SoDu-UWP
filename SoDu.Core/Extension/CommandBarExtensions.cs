using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SoDu.Core.Extension
{
    public static class CommandBarExtensions
    {
        public static readonly DependencyProperty HideMoreButtonProperty =
            DependencyProperty.RegisterAttached("HideMoreButton", typeof(bool), typeof(CommandBarExtensions),
                new PropertyMetadata(false, OnHideMoreButtonChanged));

        public static bool GetHideMoreButton(UIElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            return (bool)element.GetValue(HideMoreButtonProperty);
        }

        public static void SetHideMoreButton(UIElement element, bool value)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));
            element.SetValue(HideMoreButtonProperty, value);
        }

        private static void OnHideMoreButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var commandBar = d as CommandBar;
            if (e == null || commandBar == null || e.NewValue == null) return;
            var morebutton = FindChild<Button>(commandBar, "MoreButton") as Button;
            if (morebutton != null)
            {
                var value = GetHideMoreButton(commandBar);
                morebutton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                commandBar.Loaded += CommandBarLoaded;
            }
        }

        private static void CommandBarLoaded(object o, object args)
        {
            var commandBar = o as CommandBar;
            if (commandBar == null) return;
            var morebutton = FindChild<Button>(commandBar, "MoreButton") as Button;
            if (morebutton == null) return;
            var value = GetHideMoreButton(commandBar);
            morebutton.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
            commandBar.Loaded -= CommandBarLoaded;
        }


        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null)
                        break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;

                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }




}
