using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class AboutPageViewModel : BaseViewModel
    {
        private string m_AppVersion;
        public string AppVersion
        {
            get
            {
                return m_AppVersion;
            }
            set
            {
                SetProperty(ref m_AppVersion, value);
            }
        }

        private StringBuilder m_UpdateLog;
        public StringBuilder UpdateLog
        {
            get
            {
                return m_UpdateLog;
            }
            set
            {
                SetProperty(ref m_UpdateLog, value);
            }
        }


        public AboutPageViewModel()
        {
            m_AppVersion = "1.6.2";
            m_UpdateLog = new StringBuilder();
            m_UpdateLog.Append("1.优化章节切换逻辑，在线阅读显示章节索引。\n");
            m_UpdateLog.Append("2.添加行高设置，优化阅读体验。\n");
            m_UpdateLog.Append("3.下方菜单增加下载按钮。\n");
            m_UpdateLog.Append("4.修改删除下载列表软件按崩溃的bug。\n");
        }

    }
}
