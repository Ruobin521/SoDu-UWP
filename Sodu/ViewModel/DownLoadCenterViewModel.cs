using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class DownLoadCenterViewModel : BaseViewModel
    {

        private static readonly object isAdd1 = new object();
        private static readonly object isAdd2 = new object();


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

        }

        public async void AddNewDownloadItem(BookEntity entity)
        {
            if (this.DownLoadList.ToList().FirstOrDefault(p => p.Entity.BookID == entity.BookID) != null)
            {
                ToastHeplper.ShowMessage("已经在下载队列中");
                return;
            }

            DowmLoadEntity temp = new DowmLoadEntity();
            temp.Entity = new BookEntity()
            {
                AuthorName = entity.AuthorName,
                BookName = entity.BookName,
                BookID = entity.BookID,
                CatalogList = entity.CatalogList,
                CatalogListUrl = entity.CatalogListUrl,
                IsLocal = false,
                LastReadChapterName = entity.LastReadChapterName,
                LastReadChapterUrl = entity.LastReadChapterUrl,
                LyWeb = entity.LyWeb,
                NewestChapterName = entity.NewestChapterName,
                NewestChapterUrl = entity.NewestChapterUrl,
                UnReadCountData = entity.UnReadCountData,
                UpdateCatalogUrl = entity.UpdateCatalogUrl,
                UpdateTime = entity.UpdateTime,
            };
            //无网络
            if (NetworkHelper.Current.Network == 4)
            {
                ToastHeplper.ShowMessage("无网络连接，请检查网络后重试");
                return;
            }
            //wifi
            else if (NetworkHelper.Current.Network == 3)
            {
                StartNew(temp);
            }
            //流量
            else
            {
                if (!ViewModelInstance.Instance.SettingPageViewModelInstance.IfDownloadInWAAN)
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("你现在使用的是手机流量，确定下载？\n你可以在设置中取消此提示。") { Title = "使用流量下载" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("我是土豪", uiCommand =>
                    {
                        StartNew(temp);
                    }));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("还是算了吧", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
                else
                {
                    StartNew(temp);
                }
            }
        }

        private void StartNew(DowmLoadEntity temp)
        {
            ToastHeplper.ShowMessage("开始下载图书，请耐心等待。");
            this.DownLoadList.Add(temp);
            StartNewDownLoadInstance(temp);
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
                   int count = temp.Entity.CatalogList.Count;

                   for (int i = startIndex; i < count; i++)
                   {
                       if (temp.IsPause)
                       {
                           // break;
                           return;
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

                           BookCatalogContent content = new BookCatalogContent()
                           {
                               BookID = item.BookID,
                               CatalogUrl = item.CatalogUrl,
                               Content = html
                           };

                           lock (isAdd1)
                           {
                               if (!string.IsNullOrEmpty(item.CatalogUrl))
                               {
                                   DBBookCatalog.InsertOrUpdateBookCatalog(AppDataPath.GetBookDBPath(temp.Entity.BookID), item);
                               }
                               if (!string.IsNullOrEmpty(content.CatalogUrl))
                               {
                                   DBBookCatalogContent.InsertOrUpdateBookCatalogContent(AppDataPath.GetBookDBPath(temp.Entity.BookID), content);
                               }
                           }

                           await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                           {
                               temp.Entity.NewestChapterName = item.CatalogName;
                               temp.Entity.NewestChapterUrl = item.CatalogUrl;
                           });
                       }
                       catch (Exception)
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
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           lock (isAdd2)
                           {
                               temp.Entity.LastReadChapterName = null;
                               temp.Entity.LastReadChapterUrl = null;
                               DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), temp.Entity);
                           }
                           this.DownLoadList.Remove(temp);
                           if (this.DownLoadList.Count == 0)
                           {
                               this.IsDownLoading = false;
                           }
                           temp.Entity.IsLocal = true;
                           ViewModelInstance.Instance.LocalBookPage.LocalBookList.Add(temp.Entity);
                           ToastHeplper.ShowMessage(temp.Entity.BookName + "下载完毕，点击“本地图书”查看");
                       });
                   }
                   else
                   {
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           if (!temp.IsPause)
                           {
                               this.DownLoadList.Remove(temp);
                               ToastHeplper.ShowMessage(temp.Entity.BookName + "下载失败");
                           }
                           else
                           {
                               //  Services.ToastHeplper.ShowMessage(temp.Entity.BookName + "下载已暂停");
                           }
                       });
                   }
               }
           });
        }

        private async Task<string> GetHtmlData(string catalogUrl)
        {
            string html = null;
            try
            {
                html = await AnalysisContentService.GetHtmlContent(new HttpHelper(), catalogUrl);
            }
            catch (Exception)
            {
                html = null;
            }
            return html;
        }

        ///暂停
        /// </summary>
        private RelayCommand<object> m_PauseCommand;
        public RelayCommand<object> PauseCommand
        {
            get
            {
                return m_PauseCommand ?? (m_PauseCommand = new RelayCommand<object>(OnPauseCommand));
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
        private RelayCommand<object> m_DeleteCommand;
        public RelayCommand<object> DeleteCommand
        {
            get
            {
                return m_DeleteCommand ?? (m_DeleteCommand = new RelayCommand<object>(OnDeleteCommand));
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
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", async uiCommand =>
            {
                entity.IsPause = true;
                await Task.Delay(800);
                this.DownLoadList.Remove(entity);
                try
                {
                    string path = Path.Combine(AppDataPath.GetLocalBookFolderPath(), entity.Entity.BookID + ".db");
                    if (File.Exists(path))
                    {
                        System.IO.File.Delete(AppDataPath.GetBookDBPath(entity.Entity.BookID));
                    }
                }
                catch (Exception)
                {

                }
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
        private RelayCommand<object> m_DeleteAllCommand;
        public RelayCommand<object> DeleteAllCommand
        {
            get
            {
                return m_DeleteAllCommand ?? (m_DeleteAllCommand = new RelayCommand<object>(
                  async (obj) =>
                    {
                        if (DownLoadList.Count > 0)
                        {
                            var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定取消所有下载？") { Title = "取消下载" };
                            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", async uiCommand =>
                            {
                                foreach (var item in DownLoadList)
                                {
                                    item.IsPause = true;
                                }
                                await Task.Delay(500);
                                foreach (var item in DownLoadList)
                                {
                                    try
                                    {
                                        string path = Path.Combine(AppDataPath.GetLocalBookFolderPath(), item.Entity.BookID + ".db");

                                        if (File.Exists(path))
                                        {
                                            System.IO.File.Delete(path);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }
                                }
                                this.DownLoadList.Clear();
                                IsDownLoading = false;
                            }));
                            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                            {
                                return;
                            }));
                            await msgDialog.ShowAsync();
                        }
                    }));
            }
        }
        ///
        ///暂停全部
        /// </summary>
        private RelayCommand<object> m_PauseAllCommand;
        public RelayCommand<object> PauseAllCommand
        {
            get
            {
                return m_PauseAllCommand ?? (m_PauseAllCommand = new RelayCommand<object>(
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
                    }));
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
