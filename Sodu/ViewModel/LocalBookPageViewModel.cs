using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Sodu.ViewModel
{
    public class LocalBookPageViewModel : BaseViewModel, IViewModel
    {
        private ObservableCollection<BookEntity> m_LocalBookList;
        ///本地图书列表
        public ObservableCollection<BookEntity> LocalBookList
        {
            get
            {
                if (m_LocalBookList == null)
                {
                    m_LocalBookList = new ObservableCollection<BookEntity>();
                }
                return m_LocalBookList;
            }
            set
            {
                this.SetProperty(ref this.m_LocalBookList, value);
            }
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
                this.SetProperty(ref this.m_IsLoading, value);
            }
        }

        private string m_ContentTitle = "本地小说";
        public string ContentTitle
        {
            get
            {
                return m_ContentTitle;
            }

            set
            {
                SetProperty(ref m_ContentTitle, value);
            }
        }

        public void GetLocalBook()
        {

            Task.Run(async () =>
        {
            var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
            if (result != null)
            {
                await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    this.LocalBookList.Clear();
                    foreach (var item in result)
                    {
                        item.CatalogList = Database.DBBookCatalog.SelectBookCatalogs(Constants.AppDataPath.GetLocalBookDBPath(), item.BookID);
                        this.LocalBookList.Add(item);
                    }
                });
            }
        });
        }

        public void RefreshData(object obj = null)
        {
            GetLocalBook();
        }

        public void CancleHttpRequest()
        {

        }

        public LocalBookPageViewModel()
        {

        }

        public RelayCommand<object> BookItemSelectedCommand
        {
            get
            {
                return new RelayCommand<object>(OnBookItemSelectedCommand);
            }
        }

        private void OnBookItemSelectedCommand(object obj)
        {
            ///// ViewModelInstance.Instance.MainPageViewModelInstance.OnBooq aswkItemSelectedChangedCommand(obj);
            //CommonMethod.ShowMessage("您点击了。。。");
            BookEntity entity = obj as BookEntity;

            if (entity == null)
            {
                CommonMethod.ShowMessage("获取数据有误");
                return;
            }

            if (entity.CatalogList == null || entity.CatalogList.Count < 1)
            {
                CommonMethod.ShowMessage("获取章节数据有误");
                return;
            }

            BookEntity temp = new BookEntity();
            entity.BookID = entity.BookID;
            entity.BookName = entity.BookName;
            entity.CatalogList = entity.CatalogList;
            entity.CatalogListUrl = entity.CatalogListUrl;

            ///最新章节名称 ，以及地址
            entity.NewestChapterName = entity.NewestChapterName;
            entity.NewestChapterUrl = entity.NewestChapterUrl;

            entity.LastReadChapterName = entity.LastReadChapterName;
            entity.LastReadChapterUrl = entity.LastReadChapterUrl;
            entity.LyWeb = entity.LyWeb;
            entity.UnReadCountData = entity.UnReadCountData;
            entity.UpdateCatalogUrl = entity.UpdateCatalogUrl;
            entity.UpdateTime = entity.UpdateTime;

            if (string.IsNullOrEmpty(entity.LastReadChapterUrl))
            {
                var item = entity.CatalogList.FirstOrDefault();
                if (item != null)
                {
                    entity.LastReadChapterUrl = item.CatalogUrl; ;
                    entity.LastReadChapterName = item.CatalogName;
                }
            }

            MenuModel menu = new MenuModel() { MenuName = null, MenuType = typeof(BookContentPage) };
            ViewModelInstance.Instance.BookContentPageViewModelInstance.IsLocal = true;
            ViewModelInstance.Instance.MainPageViewModelInstance.NavigateToPage(menu, entity);
        }
    }
}
