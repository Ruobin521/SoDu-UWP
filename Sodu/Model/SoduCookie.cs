using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Model
{
    public class SoduCookie
    {
        public string CookieName { get; set; }
        public string CookieDomain { get; set; }
        public string CookieValue { get; set; }

        public DateTime CookieDateTime { get; set; }

        //HttpCookie cookie = new HttpCookie("loginname", "www.soduso.com", "/");
        //cookie.Value = "logname=918201&logid=20005";
        //    ///设置cookie存活时间，如果为null，则表示只在一个会话中生效。
        //    cookie.Expires = new DateTimeOffset(DateTime.Now.AddDays(30));
        //    filter.CookieManager.SetCookie(cookie, false);
    }
}
