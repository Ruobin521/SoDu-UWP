using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class DownLoadCenterViewModel : BaseViewModel
    {

        private ObservableCollection<BookEntity> downLoadedBookList = new ObservableCollection<BookEntity>();
        private ObservableCollection<BookCatalog> downLoadedBookCatalogList = new ObservableCollection<BookCatalog>();
        private ObservableCollection<BookCatalogContent> downLoadedBookContentList = new ObservableCollection<BookCatalogContent>();

        private bool m_IsDownLoading;
        public bool IsDownLoading
        {
            get
            {

                return m_IsDownLoading;
            }
            set
            {
                this.SetProperty(ref this.m_IsDownLoading, value);
            }
        }


        private ObservableCollection<DowmLoadEntity> m_DownLoadList;
        //下载列表
        public ObservableCollection<DowmLoadEntity> DownLoadList
        {
            get
            {
                if (m_DownLoadList == null)
                {
                    m_DownLoadList = new ObservableCollection<DowmLoadEntity>();
                }
                return m_DownLoadList;
            }
            set
            {
                this.SetProperty(ref this.m_DownLoadList, value);
            }
        }

        public DownLoadCenterViewModel()
        {

            //downLoadedBookList.CollectionChanged += DownLoadedBookList_CollectionChanged;
            //downLoadedBookCatalogList.CollectionChanged += DownLoadedBookCatalogList_CollectionChanged;
            //downLoadedBookContentList.CollectionChanged += DownLoadedBookContentList_CollectionChanged;
        }



        public async void AddNewDownloadItem(BookEntity entity)
        {
            DowmLoadEntity temp = new DowmLoadEntity();
            temp.Entity = entity;

            if (this.DownLoadList.ToList().FirstOrDefault(p => p.Entity.BookID == entity.BookID) != null)
            {
                Services.CommonMethod.ShowMessage("正在下载图书，请耐心等待。");

            }
            if (NetworkManager.Current.Network == 4)  //无网络
            {
                Services.CommonMethod.ShowMessage("无网络连接，请检查网络后重试");
                return;
            }
            else if (NetworkManager.Current.Network == 2)
            {
                Services.CommonMethod.ShowMessage("开始下载图书，请耐心等待。");
                this.DownLoadList.Add(temp);
                StartNewDownLoadInstance(temp);
            }
            else if (NetworkManager.Current.Network == 3)
            //   else
            {
                if (!ViewModelInstance.Instance.SettingPageViewModelInstance.IfDownloadInWAAN)
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("你现在使用的是手机流量，确定下载？\n你可以在设置中取消此提示。") { Title = "使用流量下载" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("我是土豪", uiCommand =>
                    {
                        Services.CommonMethod.ShowMessage("开始下载图书，请耐心等待。");
                        this.DownLoadList.Add(temp);
                        StartNewDownLoadInstance(temp);
                    }));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("还是算了吧", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
                else
                {
                    Services.CommonMethod.ShowMessage("开始下载图书，请耐心等待。");
                    this.DownLoadList.Add(temp);
                    StartNewDownLoadInstance(temp);
                }
            }
        }


        public void StartNewDownLoadInstance(DowmLoadEntity temp)
        {
            bool result = false;
            IsDownLoading = true;
            Task task = Task.Run(async () =>
           {
               try
               {
                   int startIndex = 0;

                   //适用于暂停然后重新开始
                   if (temp.CurrentCatalog != null)
                   {
                       if (temp.Entity.CatalogList != null && temp.Entity.CatalogList.Count > 0)
                       {
                           var catalog = temp.Entity.CatalogList.FirstOrDefault(p => p.CatalogUrl == temp.CurrentCatalog.CatalogUrl);
                           if (catalog != null)
                           {
                               startIndex = temp.Entity.CatalogList.IndexOf(catalog);
                           }
                       }
                   }

                   BookEntity entity = temp.Entity;
                   for (int i = startIndex; i < temp.Entity.CatalogList.Count; i++)
                   //  for (int i = startIndex; i < 20; i++)
                   {
                       if (temp.IsPause)
                       {
                           return;
                           // break;
                       }
                       try
                       {
                           var item = temp.Entity.CatalogList[i];
                           await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                           {
                               temp.CurrentCatalog = item;
                               temp.CurrentIndex = i + 1;
                               temp.ProgressValue = Math.Round(((double)item.Index / (double)temp.Entity.CatalogList.Count), 3) * 100;
                           });
                           string html = await GetHtmlData(item.CatalogUrl);
                           //  item.CatalogContent = html;
                           item.CatalogContentGUID = item.BookID + item.Index.ToString();
                           BookCatalogContent content = new BookCatalogContent()
                           {
                               BookID = item.BookID,
                               CatalogContentGUID = item.CatalogContentGUID,
                               Content = html
                           };
                           lock (isAdd2)
                           {
                               Database.DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetLocalBookDBPath(), item);
                               Database.DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetLocalBookDBPath(), content);
                           }
                       }
                       catch (Exception ex)
                       {
                           continue;
                       }
                   }
                   result = true;
               }
               catch (Exception)
               {
                   result = false;
               }
               finally
               {
                   if (result)
                   {
                       AddBookEntityToDatabase(temp.Entity);
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           this.DownLoadList.Remove(temp);
                           if (this.DownLoadList.Count == 0)
                           {
                               this.IsDownLoading = false;
                           }
                           ViewModelInstance.Instance.LocalBookPage.LocalBookList.Add(temp.Entity);
                           Services.CommonMethod.ShowMessage(temp.Entity.BookName + "下载完毕，点击“本地图书”查看");
                       });
                   }
                   else
                   {
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           if (temp.IsPause)
                           {
                               //  Services.CommonMethod.ShowMessage(temp.Entity.BookName + "下载已暂停");
                           }
                           else
                           {
                               this.DownLoadList.Remove(temp);
                               Services.CommonMethod.ShowMessage(temp.Entity.BookName + "下载失败");
                           }
                       });
                   }
               }
           });
        }

        private static readonly object isAdd1 = new object();
        private static readonly object isAdd2 = new object();
        private static readonly object isAdd3 = new object();

        public async void AddBookEntityToDatabase(BookEntity entity)
        {
            this.downLoadedBookList.Add(entity);
            while (true)
            {
                if (downLoadedBookList.Count > 0)
                {
                    while (downLoadedBookList.Count > 0)
                    {
                        Database.DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), downLoadedBookList[0]);
                        downLoadedBookList.RemoveAt(0);
                    }
                }
                else
                {
                    await Task.Delay(3000);
                }
            }
        }

        public void AddBookCatalogToDatabase(BookCatalog entity)
        {

            try
            {
                bool result = Database.DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetLocalBookDBPath(), entity);
            }
            catch (Exception ex)
            {

            }
        }


        public void AddBookCatalogContentToDatabase(BookCatalogContent entity)
        {
            try
            {
                var result = Database.DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetLocalBookDBPath(), entity);
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<string> GetHtmlData(string catalogUrl)
        {
            string html = null;
            try
            {
                html = await AnalysisContentHtmlService.GetHtmlContent(new HttpHelper(), catalogUrl);
            }
            catch (Exception ex)
            {
                html = null;
            }
            return html;
        }

        ///暂停
        /// </summary>
        public RelayCommand<object> PauseCommand
        {
            get
            {
                return new RelayCommand<object>(OnPauseCommand);
            }
        }

        private void OnPauseCommand(object obj)
        {
            DowmLoadEntity entity = obj as DowmLoadEntity;
            if (entity == null)
            {
                return;
            }
            if (!entity.IsPause)
            {
                entity.IsPause = true;
                this.IsDownLoading = false;
                foreach (var item in DownLoadList)
                {
                    if (entity.IsPause == false)
                    {
                        this.IsDownLoading = true;
                        break;
                    }
                }
            }
            else
            {
                entity.IsPause = false;

                StartNewDownLoadInstance(entity);
            }
        }
        ///
        ///删除
        /// </summary>
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                return new RelayCommand<object>(OnDeleteCommand);
            }
        }

        private async void OnDeleteCommand(object obj)
        {
            DowmLoadEntity entity = obj as DowmLoadEntity;
            if (entity == null)
            {
                return;
            }

            var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定取消下载 " + entity.Entity.BookName + "？") { Title = "取消下载" };
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
            {
                this.DownLoadList.Remove(entity);
                if (this.DownLoadList.Count == 0)
                {
                    IsDownLoading = false;
                }
            }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
            {
                return;
            }));
            await msgDialog.ShowAsync();
        }
        ///
        ///删除全部
        /// </summary>
        public RelayCommand<object> DeleteAllCommand
        {
            get
            {
                return new RelayCommand<object>(
                  async (obj) =>
                    {
                        if (DownLoadList.Count > 0)
                        {
                            var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定取消所有下载？") { Title = "取消下载" };
                            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                            {
                                this.DownLoadList.Clear();
                                IsDownLoading = false;
                            }));
                            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                            {
                                return;
                            }));
                            await msgDialog.ShowAsync();
                        }
                    });
            }
        }
        ///
        ///暂停全部
        /// </summary>
        public RelayCommand<object> PauseAllCommand
        {
            get
            {
                return new RelayCommand<object>(
                    (obj) =>
                    {
                        if (DownLoadList.Count == 0)
                        {
                            IsDownLoading = false;
                            return;
                        }

                        if (IsDownLoading)
                        {
                            foreach (var item in DownLoadList)
                            {
                                item.IsPause = true;
                            }
                            IsDownLoading = false;
                        }
                        else
                        {
                            foreach (var item in DownLoadList)
                            {
                                item.IsPause = false;
                                StartNewDownLoadInstance(item);
                            }
                            IsDownLoading = true;
                        }
                    });
            }
        }
    }

    public class DowmLoadEntity : BaseViewModel
    {

        private BookEntity m_Entity;
        public BookEntity Entity
        {
            get
            {
                return m_Entity;
            }
            set
            {
                SetProperty(ref m_Entity, value);
            }
        }

        private BookCatalog m_CurrentCatalog;
        public BookCatalog CurrentCatalog
        {
            get
            {
                return m_CurrentCatalog;
            }
            set
            {
                SetProperty(ref m_CurrentCatalog, value);
            }
        }

        private double m_ProgressValue;
        public double ProgressValue
        {
            get
            {
                return m_ProgressValue;
            }
            set
            {
                SetProperty(ref m_ProgressValue, value);
            }
        }

        private int m_CurrentIndex;
        public int CurrentIndex
        {
            get
            {
                return m_CurrentIndex;
            }
            set
            {
                SetProperty(ref m_CurrentIndex, value);
            }
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        private bool m_IsPause;
        public bool IsPause
        {
            get
            {
                return m_IsPause;
            }
            set
            {
                SetProperty(ref m_IsPause, value);
            }
        }
    }
}
