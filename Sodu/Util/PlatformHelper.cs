using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Util
{
    public class PlatformHelper
    {
        public enum Platform
        {
            IsMobile,
            IsPC
        }

        public static Platform GetPlatform()
        {
            var api = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            if (api.Equals("Windows.Desktop"))
            {
                return Platform.IsPC;
            }
            else if (api.Equals("Windows.Mobile"))
            {
                return Platform.IsMobile;
            }

            return Platform.IsPC;
        }
    }
}
