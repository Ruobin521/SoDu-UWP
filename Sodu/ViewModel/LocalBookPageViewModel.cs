using GalaSoft.MvvmLight.Command;
using Sodu.Model;
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

        public bool IsLoading
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
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

            Task.Run(() =>
         {
             var result = Database.DBLocalBook.GetAllLocalBookList(Constants.AppDataPath.GetLocalBookDBPath());
             if (result != null)
             {
                 this.LocalBookList.Clear();
                 foreach (var item in result)
                 {
                     this.LocalBookList.Add(item);
                 }
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
            /// ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
            CommonMethod.ShowMessage("您点击了。。。");
        }
    }
}
