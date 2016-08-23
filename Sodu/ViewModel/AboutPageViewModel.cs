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

        private string m_UpdateLog;
        public string UpdateLog
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
            m_AppVersion = "1.6.0";

            m_UpdateLog = "1.列表项隔行换色，跟PC版保持一致，不喜欢可以反馈给我，下个版本去掉。" + "\n"
                          + "2.个人书架添加更新提醒。" + "\n"
                          + "3.正文页面添加分页阅读(可关闭)。" + "\n"
                          + "4.PC版添加快捷键（左右键切换页面（章节），上下滚动页面，Esc打开菜单。全局使用Ctrl+Back 返回上个页面。）" + "\n"
                          + "5.正文页面打开添加双击打开菜单。";

        }

    }
}
