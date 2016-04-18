using Sodu.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class BookReaderPageViewModel : BaseViewModel, IViewModel
    {
        public string ContentTitle
        {
            get; set;
        }

        public bool IsLoading
        {
            get; set;
        }


        private ObservableCollection<BookCatalog> m_CatalogList;
        /// <summary>
        /// 目录集合
        /// </summary>
        public ObservableCollection<BookCatalog> CatalogList
        {
            get
            {
                return m_CatalogList;
            }
            set
            {
                this.SetProperty(ref this.m_CatalogList, value);
            }
        }

        private BookCatalog m_CurrentCatalog;
        /// <summary>
        /// 当前选中的
        /// </summary>
        public BookCatalog CurrentCatalog
        {
            get
            {
                return m_CurrentCatalog;
            }
            set
            {
                this.SetProperty(ref this.m_CurrentCatalog, value);
            }
        }


        public void SetCurrentCatalogIndex()
        {
            int index = CatalogList.IndexOf(CurrentCatalog);
        }

        public void CancleHttpRequest()
        {
            //  throw new NotImplementedException();
        }

        public void RefreshData(object obj = null, bool IsRefresh = true)
        {
            //throw new NotImplementedException();
        }
    }
}
