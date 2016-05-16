using Sodu.Constants;
using Sodu.Services;
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

        private bool m_IsNeedRefresh = true;
        public bool IsNeedRefresh
        {
            get
            {
                return true;
            }
            set
            {
                m_IsNeedRefresh = true;
            }
        }

        private Util.HttpHelper http = new Util.HttpHelper();

        public void CancleHttpRequest()
        {
            return;
        }

        public void RefreshData(object obj = null)
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
                      CommonMethod.ShowMessage("注销成功");
                  }
                  else
                  {
                      CommonMethod.ShowMessage("注销失败请重新尝试");
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

                html = await http.WebRequestGet(PageUrl.LogoutPage);
            }
            catch (Exception ex)
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
