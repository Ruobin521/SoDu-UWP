using GalaSoft.MvvmLight.Command;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookCatalogPageViewModel : BaseViewModel, IViewModel
    {
        private string m_ContentTitle;
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

        private BookEntity m_CurrentBookEntity;
        public BookEntity CurrentBookEntity
        {
            get
            {
                return m_CurrentBookEntity;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentBookEntity, value);
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
                SetProperty(ref m_IsLoading, value);
            }
        }

        private bool m_CanDownLoad;
        public bool CanDownLoad
        {
            get
            {
                return m_CanDownLoad;
            }
            set
            {
                SetProperty(ref m_CanDownLoad, value);
            }
        }


        HttpHelper http;

        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }

        public void InitData(object obj = null)
        {
            BookEntity temp = obj as BookEntity;
            if (temp == null)
            {
                return;
            }

            this.CurrentBookEntity = temp;


            this.ContentTitle = CurrentBookEntity.BookName + "  目录";

            if (CheckIfLoacalExist())
            {
                CanDownLoad = false;
            }
            else
            {
                CanDownLoad = true;
            }
        }

        private bool CheckIfLoacalExist()
        {
            bool rs = false;
            var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());

            if (result != null && result.FirstOrDefault(p => p.BookID == this.CurrentBookEntity.BookID) != null)
            {
                rs = true;
            }

            return rs;
        }



        /// <summary>
        /// 选中目录
        /// </summary>
        public ICommand CatalogSelectedCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
                {
                    if (IsLoading) return;

                    if (str != null)
                    {
                        try
                        {
                            BookCatalog catalog = str as BookCatalog;
                            if (catalog == null) return;

                            NavigationService.GoBack();
                            ViewModelInstance.Instance.BookContentPageViewModelInstance.OnSwtichCommand(catalog);
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            }
        }

        /// </summary>
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return new RelayCommand<object>(OnRefreshCommand);
            }
        }

        private async void OnRefreshCommand(object obj)
        {
            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                IsLoading = true;
                var list = await AnalysisBookCatalogList.GetCatalogList(CurrentBookEntity.CatalogListUrl, this.CurrentBookEntity.BookID, http = new HttpHelper());
                if (list != null && list.Count > 0)
                {
                    this.CurrentBookEntity.CatalogList.Clear();
                    foreach (var item in list)
                    {
                        this.CurrentBookEntity.CatalogList.Add(item);
                    }
                }
                IsLoading = false;
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    NavigationService.GoBack();
                });
            }
        }


        /// </summary>
        public RelayCommand<object> DwonLoadhCommand
        {
            get
            {
                return new RelayCommand<object>(OnDwonLoadhCommand);
            }
        }


        private void OnDwonLoadhCommand(object obj)
        {
            if (IsLoading) return;
            if (this.CurrentBookEntity != null && this.CurrentBookEntity.CatalogList != null && this.CurrentBookEntity.CatalogList.Count > 0)
            {
                var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
                if (result != null && result.FirstOrDefault(p => p.BookID == this.CurrentBookEntity.BookID) != null)
                {
                    ToastHeplper.ShowMessage("该图书已经下载过");
                }

                ViewModelInstance.Instance.DownLoadCenterViewModelInstance.AddNewDownloadItem(this.CurrentBookEntity);
            }

        }
    }
}
