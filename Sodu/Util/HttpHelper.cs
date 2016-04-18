using Sodu.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Sodu.Util
{

    public delegate void HandleResult(string result);
    public class HttpHelper
    {
        /// <summary>
        /// 异步控制
        /// </summary>
        //    public  ManualResetEvent allDone = new ManualResetEvent(false);
        private static CookieContainer m_CurrBaseCookie;
        public static CookieContainer CurrBaseCookie
        {
            get
            {
                return m_CurrBaseCookie;
            }
            set
            {
                m_CurrBaseCookie = value;
            }
        }


        //取消网络请求
        public CancellationTokenSource Cts = new CancellationTokenSource();

        Encoding DefauleEncoding = Encoding.GetEncoding("gb2312");

        /// <summary>
        /// 获取html代码
        /// </summary>
        public string Html = string.Empty;

        public HttpHelper()
        {
        }

        public string HttpGetRequest(string Url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Method = "Get";
            webRequest.Headers[HttpRequestHeader.IfModifiedSince] = DateTime.Now.ToString();
            webRequest.CookieContainer = CurrBaseCookie;
            webRequest.BeginGetResponse(new AsyncCallback(GetHandleResponse), webRequest);

            //allDone.Reset();
            //allDone.WaitOne();

            return Html;
        }
        public void GetHandleResponse(IAsyncResult asyncResult)
        {
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            try
            {
                httpRequest = (HttpWebRequest)asyncResult.AsyncState;
                httpResponse = (HttpWebResponse)httpRequest.EndGetResponse(asyncResult);

                using (var reader = new StreamReader(httpResponse.GetResponseStream(), DefauleEncoding))
                {
                    Html = reader.ReadToEnd();
                    reader.Dispose();
                }
            }
            catch
            {
                Html = "";
            }
            finally
            {
                if (httpRequest != null) httpRequest.Abort();
                if (httpResponse != null) httpResponse.Dispose();
                //  allDone.Set();
            }
        }
        /// <summary>
        /// Http Get Request
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public async Task<string> HttpClientGetRequest(string url, bool isAddTime = true, HttpCookie cookie = null, Encoding encode = null)
        {
            string html = string.Empty;
            try
            {
                Cts = new CancellationTokenSource();
                Cts.CancelAfter(TimeSpan.FromSeconds(15));

                HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
                HttpClient httpclient = new HttpClient(filter);
                //filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.MostRecent;
                filter.UseProxy = false;

                if (isAddTime)
                {
                    url = url + "?time=" + GetTimeStamp();
                }
                HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(url));
                try
                {
                    HttpResponseMessage response = await httpclient.GetAsync(new Uri(url)).AsTask(Cts.Token);
                    Cts.Token.ThrowIfCancellationRequested();
                    using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                    {
                        using (var reader = new StreamReader(responseStream, encode == null ? DefauleEncoding : encode))
                        {
                            html = reader.ReadToEnd();
                            reader.Dispose();
                            responseStream.Dispose();
                        }

                    }
                }
                catch (TaskCanceledException ex)
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                html = null;
            }
            return html;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        public async Task<string> HttpClientPostRequest(string url, string postData, Encoding encode = null)
        {
            string html = string.Empty;
            try
            {
                await Window.Current.Content.Dispatcher.RunIdleAsync((e) =>
                {
                    CommonMethod.StartLoading2();
                });

                HttpClient httpclient = new HttpClient();
                HttpStringContent httpStringContent = new HttpStringContent(postData);
                //就这个问题让我找了好几个小时
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");
                // httpStringContent.Headers["Connection"] = "Keep-Alive";
                //  HttpResponseMessage response = await httpclient.PostAsync(new Uri(url), httpStringContent).AsTask(Cts.Token);
                Cts = new CancellationTokenSource();
                Cts.CancelAfter(TimeSpan.FromSeconds(15));

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                request.Content = httpStringContent;
                request.Headers["Connection"] = "Keep-Alive";
                HttpResponseMessage response = await httpclient.SendRequestAsync(request).AsTask(Cts.Token);
                Cts.Token.ThrowIfCancellationRequested();
                using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                {
                    using (var reader = new StreamReader(responseStream, DefauleEncoding))
                    {
                        html = reader.ReadToEnd();
                        reader.Dispose();
                        responseStream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                html = null;
            }
            finally
            {
                await Window.Current.Content.Dispatcher.RunIdleAsync((e) =>
                {
                    CommonMethod.StopLoading2();
                });
            }
            return html;
        }

        //登陆时用
        public async Task<string> HttpClientPostLoginRequest(string url, string postData, bool ifAutoLogin = false, Encoding encode = null)
        {
            string html = string.Empty;
            try
            {
                await Window.Current.Content.Dispatcher.RunIdleAsync((e) =>
                {
                    CommonMethod.StartLoading2();
                });
                CancellationTokenSource cts = new CancellationTokenSource();
                HttpClient httpclient = new HttpClient();
                HttpStringContent httpStringContent = new HttpStringContent(postData);
                //就这个问题让我找了好几个小时
                httpStringContent.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");

                HttpResponseMessage response = await httpclient.PostAsync(new Uri(url), httpStringContent).AsTask(cts.Token);

                //  HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                //  request.Content = httpStringContent;
                //  HttpResponseMessage response = await httpclient.SendRequestAsync(request).AsTask(cts.Token);

                using (Stream responseStream = (await response.Content.ReadAsInputStreamAsync()).AsStreamForRead())
                {
                    using (var reader = new StreamReader(responseStream, DefauleEncoding))
                    {
                        html = reader.ReadToEnd();
                        reader.Dispose();
                        responseStream.Dispose();
                        SetCookie(url, ifAutoLogin);
                    }
                }
                cts.Token.ThrowIfCancellationRequested();
            }

            catch (Exception ex)
            {
                html = "";
            }
            finally
            {
                await Window.Current.Content.Dispatcher.RunIdleAsync((e) =>
                {
                    CommonMethod.StartLoading2();
                });
            }
            return html;
        }
        public void SetCookie(string url, bool ifAutoLogin)
        {
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            HttpCookieCollection cookieCollection = filter.CookieManager.GetCookies(new Uri(url));
            foreach (var cookieItem in cookieCollection)
            {
                if (cookieItem.Name == "loginname")
                {
                    // cookie = new HttpCookie(cookieItem.Name, cookieItem.Path, "/");
                    // cookie.Value = cookieItem.Value;
                    if (ifAutoLogin)
                    {
                        ///设置cookie存活时间，如果为null，则表示只在一个会话中生效。
                        cookieItem.Expires = new DateTimeOffset(DateTime.Now.AddDays(365));
                    }
                    filter.CookieManager.SetCookie(cookieItem, false);
                }
            }
        }


        public void HttpClientCancleRequest()
        {
            if (Cts.Token.CanBeCanceled)
            {
                this.Cts.Cancel();
                CommonMethod.StopLoading2();
            }
        }
    }
}
