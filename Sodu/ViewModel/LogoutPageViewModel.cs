using Sodu.Constants;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Sodu.ViewModel
{
    public class LogoutPageViewModel : BaseViewModel, IViewModel
    {
        public string ContentTitle
        {
            get; set;
        }

        private bool m_IsLoading;
        public bool IsLoading
        {
            get
            {
                return m_IsLoading;
            }
            set
            {
                SetProperty(ref m_IsLoading, value);
            }
        }

        private HttpHelper http = new HttpHelper();

        public void CancleHttpRequest()
        {
            return;
        }

        public void InitData(object obj = null)
        {
            Task.Run(async () =>
            {
                string html = await GetHtmlData();
                return html;

            }).ContinueWith(async (result) =>
            {
                string html = result.Result;
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  if (html != null && html.Contains("to delete public domains' cookies"))
                  {
                      ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                      ToastHeplper.ShowMessage("注销成功");
                  }
                  else
                  {
                      ToastHeplper.ShowMessage("注销失败请重新尝试");
                      NavigationService.GoBack();
                  }
              });
            });
        }

        private async Task<string> GetHtmlData()
        {
            string html = null;
            try
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = true;
                });

                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetLogoutPage());
            }
            catch (Exception  )
            {
                return null;
            }
            finally
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsLoading = false;
                });
            }
            return html;
        }
    }
}
