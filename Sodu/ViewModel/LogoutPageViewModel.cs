using Sodu.Constants;
using Sodu.Core.Util;
using Sodu.Services;

using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

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
            IsLoading = true;

            Task.Run(async () =>
          {
              try
              {
                  HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                  HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(ViewModelInstance.Instance.UrlService.GetHomePage()));
                  var cookieItem = cookieCollection.FirstOrDefault(p => p.Name.Equals("sodu_user"));

                  if (cookieItem == null)
                  {
                      ViewModelInstance.Instance.IsLogin = false;
                  }
                  else
                  {
                      cookieItem.Expires = null;
                      filter.CookieManager.SetCookie(cookieItem);
                  }
                  await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                  {
                      ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                      ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Clear();
                      ToastHeplper.ShowMessage("注销成功");
                  });
              }
              catch (Exception ex)
              {
                  Debug.WriteLine(ex.Message);
              }
              finally
              {
                  await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                  {
                      IsLoading = false;
                  });
              }

          });

        }
        public void InitData2(object obj = null)
        {

            IsLoading = true;

            Task.Run(async () =>
            {
                string html = await GetHtmlData();
                return html;

            }).ContinueWith(async (result) =>
            {
                string html = result.Result;
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
               {
                   try
                   {
                       if (html != null && html.Contains("to delete public domains' cookies"))
                       {
                           ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                           ViewModelInstance.Instance.MyBookShelfViewModelInstance.ShelfBookList.Clear();
                           ToastHeplper.ShowMessage("注销成功");
                       }
                       else
                       {
                           ToastHeplper.ShowMessage("注销失败请重新尝试");
                           NavigationService.GoBack();
                       }
                   }
                   catch (Exception ex)
                   {
                       Debug.WriteLine(ex.Message);
                   }
                   finally
                   {
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           IsLoading = false;
                       });
                   }
               });
            });
        }

        private async Task<string> GetHtmlData()
        {
            string html = null;
            try
            {
                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetLogoutPage());
            }
            catch (Exception)
            {
                return null;
            }
            return html;
        }
    }
}
