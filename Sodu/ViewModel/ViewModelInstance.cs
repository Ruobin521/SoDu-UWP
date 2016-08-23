using Microsoft.Practices.Unity;
using Sodu.Constants;
using SoDu.Core.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private IUnityContainer m_Container;
        public IUnityContainer Container
        {
            get
            {
                if (m_Container == null)
                {
                    m_Container = new UnityContainer();
                }
                return m_Container;
            }
            set
            {
                SetProperty(ref this.m_Container, value);
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
                    m_UrlService = Container.Resolve<IURLService>();
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


        private RecommendAndHotPageViewModel m_RecommendAndHotPageViewModelInstance;
        public RecommendAndHotPageViewModel RecommendAndHotPageViewModelInstance
        {
            get
            {
                if (m_RecommendAndHotPageViewModelInstance == null)
                {
                    m_RecommendAndHotPageViewModelInstance = new RecommendAndHotPageViewModel();
                }
                return m_RecommendAndHotPageViewModelInstance;
            }
            set
            {
                SetProperty(ref this.m_RecommendAndHotPageViewModelInstance, value);
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

        private HistoryPageViewModel m_EverReadBookPageViewModelInstance;
        public HistoryPageViewModel EverReadBookPageViewModelInstance
        {
            get
            {
                if (m_EverReadBookPageViewModelInstance == null)
                {
                    m_EverReadBookPageViewModelInstance = new HistoryPageViewModel();
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

        private UpdateBookPageViewModel m_UpdateBookPage;
        public UpdateBookPageViewModel UpdateBookPage
        {
            get
            {
                if (m_UpdateBookPage == null)
                {
                    m_UpdateBookPage = new UpdateBookPageViewModel();
                }
                return m_UpdateBookPage;
            }
        }

        private AboutPageViewModel m_AboutPage;
        public AboutPageViewModel AboutPage
        {
            get
            {
                if (m_AboutPage == null)
                {
                    m_AboutPage = new AboutPageViewModel();
                }
                return m_AboutPage;
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
