using System.Text;

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
            m_UpdateLog.Append("1.修改本地图书更新的bug。\n");
            m_UpdateLog.Append("2.优化细节，快捷键进行了微调（主要是PC端）。\n");
            m_UpdateLog.Append("3.修复偶尔分页不正常的bug。\n");
        }

    }
}
