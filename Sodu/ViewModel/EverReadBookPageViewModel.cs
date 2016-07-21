using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public class EverReadBookPageViewModel : BaseViewModel, IViewModel
    {
        private string _ContentTitle = "阅读记录";
        public string ContentTitle
        {
            get
            {
                return _ContentTitle;
            }

            set
            {
                SetProperty(ref _ContentTitle, value);
            }
        }

        public bool IsLoading
        {
            get; set;
        }

        private bool m_IsShow = true;
        public bool IsShow
        {
            get
            {
                return m_IsShow;
            }
            set
            {
                SetProperty(ref this.m_IsShow, value);
            }
        }

        private ObservableCollection<BookEntity> m_BookList;
        public ObservableCollection<BookEntity> BookList
        {
            get
            {
                if (m_BookList == null)
                {
                    m_BookList = new ObservableCollection<BookEntity>();
                }
                return m_BookList;
            }
            set
            {
                this.SetProperty(ref this.m_BookList, value);
            }
        }

        public void CancleHttpRequest()
        {
            return;
        }
        public void InitData(object obj = null)
        {
            if (BookList == null || BookList.Count <= 0)
            {
                ToastHeplper.ShowMessage("你还没有阅读记录");
            }
        }

        public EverReadBookPageViewModel()
        {
            var list = DBHistory.GetBookHistories(AppDataPath.GetHistoryDBPath());
            if (list != null)
            {
                list.ForEach(x => this.BookList.Add(x));
            }
        }

        public void AddToHistoryList(BookEntity entity)
        {
            if (BookList.ToList().Find(p => p.BookID == entity.BookID) != null)
            {
                return;
            }
            else
            {
                BookList.Add(entity);
                bool result = DBHistory.InsertOrUpdateBookHistory(AppDataPath.GetHistoryDBPath(), entity);
            }
        }

        private RelayCommand<object> m_BookItemSelectedCommand;
        public RelayCommand<object> BookItemSelectedCommand
        {
            get
            {
                return m_BookItemSelectedCommand ?? (m_BookItemSelectedCommand = new RelayCommand<object>(OnBookItemSelectedCommand));
            }
        }
        private void OnBookItemSelectedCommand(object obj)
        {
            ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
        }

        private RelayCommand<object> m_ClearCommand;
        public RelayCommand<object> ClearCommand
        {
            get
            {
                return m_ClearCommand ?? (m_ClearCommand = new RelayCommand<object>(OnClearCommand));
            }
        }
        private async void OnClearCommand(object obj)
        {
            if (this.BookList.Count < 1) return;

            var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定清空历史记录？") { Title = "历史记录" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
            {
                this.BookList.Clear();
                DBHistory.ClearHistories(AppDataPath.GetHistoryDBPath());
            }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
            {
                return;
            }));
            await msgDialog.ShowAsync();
        }

    }
}
