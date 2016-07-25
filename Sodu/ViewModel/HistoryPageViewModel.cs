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

            Task.Run(async () =>
            {
                try
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = true;
                        BookList.Clear();
                    });


                    var list = DBHistory.GetBookHistories(AppDataPath.GetHistoryDBPath());
                    if (list != null)
                    {
                        await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            list.ForEach(x => this.BookList.Add(x));
                        });

                    }

                }
                catch (Exception)
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ToastHeplper.ShowMessage("加载历史记录有误");
                    });
                }
                finally
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
            Task.Run(async () =>
            {
                var temp = entity.Clone();
                temp.UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (BookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        BookList.Insert(0, temp);
                    });
                }
                bool result = DBHistory.InsertOrUpdateBookHistory(AppDataPath.GetHistoryDBPath(), temp);
            });
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
                entity.IsHistory = true;
                NavigationService.NavigateTo(typeof(BookContentPage), entity);
            }
            //  ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);
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
