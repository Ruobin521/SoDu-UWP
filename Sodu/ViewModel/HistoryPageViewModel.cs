using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Core.Util;
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
using GalaSoft.MvvmLight.Threading;

namespace Sodu.ViewModel
{
    public class HistoryPageViewModel : BaseViewModel, IViewModel
    {
        private string _ContentTitle = "历史记录";
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

        private bool m_IsEditing;
        public bool IsEditing
        {
            get
            {
                return m_IsEditing;
            }
            set
            {
                SetProperty(ref this.m_IsEditing, value);
            }
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
            if (IsLoading) return;
            InitHitoryData();
        }


        public void InitHitoryData()
        {
            if (IsLoading) return;
            IsLoading = true;

            BookList.Clear();

            Task.Run(() =>
          {
              try
              {
                  var list = DBHistory.GetBookHistories(AppDataPath.GetHistoryDBPath());
                  if (list != null)
                  {
                      DispatcherHelper.CheckBeginInvokeOnUI(() =>
                      {
                          list.ForEach(x => this.BookList.Add(x));
                      });
                  }
              }
              catch (Exception)
              {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                      ToastHeplper.ShowMessage("加载历史记录有误");
                  });
              }
              finally
              {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                      IsLoading = false;
                  });
              }
          });
        }

        public HistoryPageViewModel()
        {
            //InitData();
        }

        public void AddToHistoryList(BookEntity entity)
        {
            Task.Run(() =>
          {
              var temp = entity.Clone();
              temp.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

              if (BookList.ToList().Find(p => p.BookID == entity.BookID) == null)
              {
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    BookList.Insert(0, temp);
                });
              }
              bool result = DBHistory.InsertOrUpdateBookHistory(AppDataPath.GetHistoryDBPath(), temp);
          });
        }
        /// <summary>
        /// 全选，全不选
        /// </summary>
        private RelayCommand<object> m_EditCommand;
        public RelayCommand<object> EditCommand
        {
            get
            {
                return m_EditCommand ?? (m_EditCommand = new RelayCommand<object>(
                 (obj) =>
                 {

                     if (IsLoading) return;
                     OnEditCommand();
                 }
                    ));
            }
        }

        public void OnEditCommand()
        {
            if (IsLoading) return;

            if (this.BookList == null || this.BookList.Count < 1)
            {
                IsEditing = false;
                return;
            }

            SetBookEditStatus(!IsEditing);
        }

        private void SetBookEditStatus(bool value)
        {
            foreach (var item in BookList)
            {
                item.IsInEdit = value;
                item.IsSelected = false;
            }

            IsEditing = value;
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
            BookEntity entity = obj as BookEntity;

            if (entity != null)
            {
                if (IsEditing)
                {
                    entity.IsSelected = !entity.IsSelected;
                }
                else
                {
                    entity.IsHistory = true;
                    NavigationService.NavigateTo(typeof(BookContentPage), entity);
                }

            }
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

            if (!IsEditing) return;

            int count = 0;

            ObservableCollection<BookEntity> tempList = new ObservableCollection<BookEntity>();
            foreach (var item in this.BookList)
            {
                if (item.IsSelected)
                {
                    tempList.Add(item);
                    count++;
                }
            }
            if (count == 0)
            {
                ToastHeplper.ShowMessage("请选择需要删除的记录");
                return;
            }

            var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定删除历史记录？") { Title = "历史记录" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
            {
                foreach (var item in tempList)
                {
                    this.BookList.Remove(item);
                    DBHistory.DeleteHistory(AppDataPath.GetHistoryDBPath(), item);
                }

                this.InitData();
                SetBookEditStatus(false);

            }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
            {
                return;
            }));
            await msgDialog.ShowAsync();
        }

    }
}
