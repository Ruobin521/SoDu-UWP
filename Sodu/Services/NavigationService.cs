﻿using Sodu.Model;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.Services
{
    public static class NavigationService
    {
        private static DateTime FirstTime { get; set; }
        private static DateTime SecondTime { get; set; }

        private static Frame _ContentFrame;
        public static Frame ContentFrame
        {
            get
            {
                return _ContentFrame;
            }

            set
            {
                if (_ContentFrame == null)
                {
                    _ContentFrame = value;
                    _ContentFrame.Navigated -= _ContentFrame_Navigated;
                    _ContentFrame.Navigated += _ContentFrame_Navigated;
                }
            }
        }



        public static void CancleHttpRequest()
        {
            Page page = ContentFrame.Content as Page;
            if (page != null)
            {
                IViewModel viewModel = page.DataContext as IViewModel;
                if (viewModel != null)
                {
                    if (viewModel.IsLoading)
                    {
                        viewModel.CancleHttpRequest();
                    }
                }
            }
        }
        public static void NavigateTo(Type type, object para = null)
        {
            ContentFrame.Navigate(type, para);
        }

        public static void GoBack()
        {
            GoBack(null, null);
        }

        public static void GoBack(object sender, BackPressedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }
            if (ContentFrame.CanGoBack)
            {
                //     CancleHttpRequest();
                ContentFrame.GoBack();
            }
            else
            {
                if (CheckIfShutDown())
                {
                    App.Current.Exit();
                }
                else
                {
                    CommonMethod.ShowMessage("再按一次返回键退出");
                }
            }
        }
        private static bool CheckIfShutDown()
        {
            if (FirstTime == DateTime.MinValue)
            {
                FirstTime = DateTime.Now;
                return false;
            }

            else if (FirstTime != DateTime.MinValue)
            {
                SecondTime = DateTime.Now;
                if ((SecondTime - FirstTime).TotalSeconds > 2.5)
                {
                    FirstTime = DateTime.Now;
                    return false;
                }
                else if ((SecondTime - FirstTime).TotalSeconds <= 2.5)
                {
                    return true;
                }
            }
            return false;
        }


        private static void _ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            Page page = ContentFrame.Content as Page;
            if (e.NavigationMode == Windows.UI.Xaml.Navigation.NavigationMode.New)
            {
                if (page != null)
                {
                    IViewModel viewModel = page.DataContext as IViewModel;
                    if (viewModel != null)
                    {
                        viewModel.RefreshData(e.Parameter);
                    }
                }
            }
            ViewModelInstance.Instance.MainPageViewModelInstance.SetCurrentMenu(page.GetType());
        }

    }
}
