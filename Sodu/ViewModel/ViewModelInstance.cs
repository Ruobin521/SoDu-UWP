using Microsoft.Practices.Unity;
using Sodu.Constants;
using SoDu.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Windows.Storage;

namespace Sodu.ViewModel
{
    public class ViewModelInstance : BaseViewModel
    {
        public ViewModelInstance()
        {

        }
        private static readonly object SynObject = new object();

        private static ViewModelInstance m_Instance = new ViewModelInstance();
        public static ViewModelInstance Instance
        {
            get
            {
                // Syn operation.
                lock (SynObject)
                {
                    return m_Instance;
                }
            }
        }


        private MainPageViewModel m_MainPageViewModelInstance;
        public MainPageViewModel MainPageViewModelInstance
        {
            get
            {
                if (m_MainPageViewModelInstance == null)
                {
                    m_MainPageViewModelInstance = new MainPageViewModel();
                }
                return m_MainPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_MainPageViewModelInstance, value);
            }
        }

        private IURLService m_UrlService;
        public IURLService UrlService
        {
            get
            {
                if (m_UrlService == null)
                {
                    m_UrlService = App.Container.Resolve<IURLService>();
                }
                return m_UrlService;
            }
        }



        private BookShelfPageViewModel m_MyBookShelfViewModelInstance;
        public BookShelfPageViewModel MyBookShelfViewModelInstance
        {
            get
            {
                if (m_MyBookShelfViewModelInstance == null)
                {
                    m_MyBookShelfViewModelInstance = new BookShelfPageViewModel();
                }
                return m_MyBookShelfViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_MyBookShelfViewModelInstance, value);
            }
        }


        private LoginViewModel m_LoginViewModelInstance;
        public LoginViewModel LoginViewModelInstance
        {
            get
            {
                if (m_LoginViewModelInstance == null)
                {
                    m_LoginViewModelInstance = new LoginViewModel();
                }
                return m_LoginViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_LoginViewModelInstance, value);
            }
        }
        private LogoutPageViewModel m_LogoutPageViewModelInstance;
        public LogoutPageViewModel LogoutPageViewModelInstance
        {
            get
            {
                if (m_LogoutPageViewModelInstance == null)
                {
                    m_LogoutPageViewModelInstance = new LogoutPageViewModel();
                }
                return m_LogoutPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_LogoutPageViewModelInstance, value);
            }
        }


        private HomePageViewModel m_HomePageViewModelInstance;
        public HomePageViewModel HomePageViewModelInstance
        {
            get
            {
                if (m_HomePageViewModelInstance == null)
                {
                    m_HomePageViewModelInstance = new HomePageViewModel();
                }
                return m_HomePageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_HomePageViewModelInstance, value);
            }
        }

        private HotPageViewModel m_HotPageViewModelInstance;
        public HotPageViewModel HotPageViewModelInstance
        {
            get
            {
                if (m_HotPageViewModelInstance == null)
                {
                    m_HotPageViewModelInstance = new HotPageViewModel();
                }
                return m_HotPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_HotPageViewModelInstance, value);
            }
        }

        private RankListPageViewModel m_RankListPageViewModelInstance;
        public RankListPageViewModel RankListPageViewModelInstance
        {
            get
            {
                if (m_RankListPageViewModelInstance == null)
                {
                    m_RankListPageViewModelInstance = new RankListPageViewModel();
                }
                return m_RankListPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_RankListPageViewModelInstance, value);
            }
        }

        private EverReadBookPageViewModel m_EverReadBookPageViewModelInstance;
        public EverReadBookPageViewModel EverReadBookPageViewModelInstance
        {
            get
            {
                if (m_EverReadBookPageViewModelInstance == null)
                {
                    m_EverReadBookPageViewModelInstance = new EverReadBookPageViewModel();
                }
                return m_EverReadBookPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_EverReadBookPageViewModelInstance, value);
            }
        }

        private UpdateChapterViewModel m_UpdataChapterPageViewModelInstance;
        public UpdateChapterViewModel UpdataChapterPageViewModelInstance
        {
            get
            {
                if (m_UpdataChapterPageViewModelInstance == null)
                {
                    m_UpdataChapterPageViewModelInstance = new UpdateChapterViewModel();
                }
                return m_UpdataChapterPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_UpdataChapterPageViewModelInstance, value);
            }
        }


        private RegiserPageViewModel m_RegiserPageViewModelInstance;
        public RegiserPageViewModel RegiserPageViewModelInstance
        {
            get
            {
                if (m_RegiserPageViewModelInstance == null)
                {
                    m_RegiserPageViewModelInstance = new RegiserPageViewModel();
                }
                return m_RegiserPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_RegiserPageViewModelInstance, value);
            }
        }


        private SettingPageViewModel m_SettingPageViewModelInstance;
        public SettingPageViewModel SettingPageViewModelInstance
        {
            get
            {
                if (m_SettingPageViewModelInstance == null)
                {
                    m_SettingPageViewModelInstance = new SettingPageViewModel();
                }
                return m_SettingPageViewModelInstance;
            }
            set
            {
                SetProperty(ref m_SettingPageViewModelInstance, value);
            }
        }


        private SearchResultPageViewModel m_SearchResultPageViewModelInstance;
        public SearchResultPageViewModel SearchResultPageViewModelInstance
        {
            get
            {
                if (m_SearchResultPageViewModelInstance == null)
                {
                    m_SearchResultPageViewModelInstance = new SearchResultPageViewModel();
                }
                return m_SearchResultPageViewModelInstance;
            }
            set
            {
                SetProperty(ref m_SearchResultPageViewModelInstance, value);
            }
        }

        private BookContentPageViewModel m_BookContentPageViewModelInstance;
        public BookContentPageViewModel BookContentPageViewModelInstance
        {
            get
            {
                if (m_BookContentPageViewModelInstance == null)
                {
                    m_BookContentPageViewModelInstance = new BookContentPageViewModel();
                }
                return m_BookContentPageViewModelInstance;
            }
            set
            {
                SetProperty(ref m_BookContentPageViewModelInstance, value);
            }
        }

        private BookCatalogPageViewModel m_BookCatalogPageViewModelInstance;
        public BookCatalogPageViewModel BookCatalogPageViewModelInstance
        {
            get
            {
                if (m_BookCatalogPageViewModelInstance == null)
                {
                    m_BookCatalogPageViewModelInstance = new BookCatalogPageViewModel();
                }
                return m_BookCatalogPageViewModelInstance;
            }
            set
            {
                SetProperty(ref m_BookCatalogPageViewModelInstance, value);
            }
        }


        private DownLoadCenterViewModel m_DownLoadCenterViewModelInstance;
        public DownLoadCenterViewModel DownLoadCenterViewModelInstance
        {
            get
            {
                if (m_DownLoadCenterViewModelInstance == null)
                {
                    m_DownLoadCenterViewModelInstance = new DownLoadCenterViewModel();
                }
                return m_DownLoadCenterViewModelInstance;
            }
            set
            {
                SetProperty(ref m_DownLoadCenterViewModelInstance, value);
            }
        }

        private LocalBookPageViewModel m_LocalBookPage;
        public LocalBookPageViewModel LocalBookPage
        {
            get
            {
                if (m_LocalBookPage == null)
                {
                    m_LocalBookPage = new LocalBookPageViewModel();
                }
                return m_LocalBookPage;
            }
            set
            {
                SetProperty(ref m_LocalBookPage, value);
            }
        }

        private bool m_IsLogin;
        public bool IsLogin
        {
            get
            {
                return m_IsLogin;
            }
            set
            {
                SetProperty(ref m_IsLogin, value);
            }
        }

    }





}
