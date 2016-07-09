using Sodu.Constants;
using Sodu.Util;
using Sodu.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Sodu.Services;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using Windows.UI.Core;

namespace Sodu
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public static Frame rootFrame;

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += OnResuming;

            EncodingProvider provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            
        }



        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                //StatusBar statusBar = StatusBar.GetForCurrentView();
                ////statusBar.ForegroundColor = (Color)ColorHelper.FromArgb(255, 0, 77, 0);               
                //statusBar.ForegroundColor = Colors.Black;
                ////statusBar.BackgroundColor = Colors.Red;
                //statusBar.BackgroundOpacity = 1;
                //statusBar.BackgroundColor = Colors.White;

                StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }



            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;

                InitSettingData();

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                }
                else
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
                    SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                }
            }


            if (rootFrame.Content == null)
            {
                // 当导航堆栈尚未还原时，导航到第一页，
                // 并通过将所需信息作为导航参数传入来配置
                // 参数
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }
            // 确保当前窗口处于活动状态
            Window.Current.Activate();
        }


        public void InitSettingData()
        {
            var settingvm = ViewModelInstance.Instance.SettingPageViewModelInstance;
            settingvm.InitSettingData();

            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(Constants.PageUrl.HomePage));
            var cookieItem = cookieCollection.FirstOrDefault(p => p.Name.Equals("sodu_user"));

            if (cookieItem == null)
            {
                ViewModelInstance.Instance.IsLogin = false;
                return;
            }

            if (settingvm.IfAutoLogin)
            {
                cookieItem.Expires = new DateTimeOffset(DateTime.Now.AddDays(365));
                filter.CookieManager.SetCookie(cookieItem, false);
                ViewModelInstance.Instance.IsLogin = true;
            }
            else
            {
                cookieItem.Expires = null;
                filter.CookieManager.SetCookie(cookieItem);
            }
        }


        /// <summary>
        /// 初始化本地缓存的数据
        /// </summary>
        private void InitLocalBook()
        {
            //  Database.DBLocalBook.DeleteAllLocalBooksData(AppDataPath.GetLocalBookDBPath());

            ViewModelInstance.Instance.LocalBookPage.InitData(null);
        }



        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {

            if (e != null)
            {
                e.Handled = true;
            }

            if (NavigationService.ContentFrame != null)
            {
                Page page = NavigationService.ContentFrame.Content as Page;
                if (page != null && (page.DataContext as BookShelfPageViewModel) != null && (page.DataContext as BookShelfPageViewModel).BackpressedHandler())
                {
                    return;
                }
                else
                {
                    NavigationService.GoBack(sender);
                }
            }
        }

        /// <summary>
        /// 点击返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }
            if (NavigationService.ContentFrame != null)
            {
                Page page = NavigationService.ContentFrame.Content as Page;
                if (page != null && (page.DataContext as BookShelfPageViewModel) != null && (page.DataContext as BookShelfPageViewModel).BackpressedHandler())
                {
                    return;
                }
                else
                {
                    NavigationService.GoBack();
                }
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        /// <summary>
        /// 从挂起到恢复时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnResuming(object sender, object e)
        {

        }

    }



}
