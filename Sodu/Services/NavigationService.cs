﻿using Sodu.Core.Util;
using Sodu.Model;

using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static void ClearStack()
        {
            if (ContentFrame.CanGoBack || ContentFrame.CanGoForward)
            {
                ContentFrame.BackStack.Clear();
                ContentFrame.ForwardStack.Clear();
            }
        }
        public static void NavigateTo(Type type, object para = null)
        {
            if (ContentFrame.Content != null && ContentFrame.Content.GetType().Equals(type))
            {
                return;
            }
            //  CancleHttpRequest();
            ContentFrame.Navigate(type, para);
        }
        public static void GoBack(object sender = null)
        {
            if (ContentFrame.CanGoBack)
            {
                try
                {
                    //CancleHttpRequest();
                    ContentFrame.GoBack();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                //  bool result = await App.CheckIfHasDownloadTasks();

                if (true)
                {
                    if (CheckIfShutDown())
                    {
                        App.Current.Exit();
                    }
                    else
                    {
                        ToastHeplper.ShowMessage("再按一次返回键退出");
                    }
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
                if ((SecondTime - FirstTime).TotalSeconds > 2)
                {
                    FirstTime = DateTime.Now;
                    return false;
                }
                else if ((SecondTime - FirstTime).TotalSeconds < 2)
                {
                    return true;
                }
            }
            return false;
        }

        private static void _ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (ViewModelInstance.Instance.MainPageViewModelInstance.IsLeftPanelOpen)
            {
                ViewModelInstance.Instance.MainPageViewModelInstance.IsLeftPanelOpen = false;
                return;
            }

            if (PlatformHelper.GetPlatform() == PlatformHelper.Platform.IsPC)
            {
                if (ContentFrame.CanGoBack)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
                else
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
            }
        }

    }
}
