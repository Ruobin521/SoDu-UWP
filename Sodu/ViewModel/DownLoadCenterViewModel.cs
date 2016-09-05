using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Core.Util;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;

using SoDu.Core.Util;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;
using System.Net.Http;
using Windows.Storage;

namespace Sodu.ViewModel
{
    public class DownLoadCenterViewModel : BaseViewModel
    {

        private static readonly object isAdd1 = new object();
        private static readonly object isAdd2 = new object();

        private readonly TaskScheduler _syncContextTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

        private int maxDownloadCount = 3;

        public bool IsFrameContent = false;

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

            if (this.DownLoadList.Count == maxDownloadCount)
            {
                ToastHeplper.ShowMessage("最多同时下载" + maxDownloadCount + "本");
                return;
            }

            DowmLoadEntity temp = new DowmLoadEntity();
            temp.Entity = entity.Clone();

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
            StartNewFastDownLoadInstance(temp);
        }

        public void StartNewCommonDownLoadInstance(DowmLoadEntity temp)
        {
            bool result = false;
            IsDownLoading = true;
            temp.IsFast = false;

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
                   HttpHelper http = new HttpHelper();
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
                           DispatcherHelper.CheckBeginInvokeOnUI(() =>
                         {
                             temp.CurrentCatalog = item;
                             temp.CurrentIndex = i + 1;
                             temp.ProgressValue = Math.Round(((double)item.Index / (double)temp.Entity.CatalogList.Count), 3) * 100;
                         });

                           string html = await GetHtmlData(item.CatalogUrl, http);

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

                           DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
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
                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
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


        public async void StartNewFastDownLoadInstance(DowmLoadEntity temp)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            IsDownLoading = true;

            int count = 10;
            temp.IsFast = true;

            if (temp.Entity.CatalogList == null || temp.Entity.CatalogList.Count == 0)
            {
                return;
            }
            temp.ContentList = new List<BookCatalogContent>();

            count = temp.Entity.CatalogList.Count >= count ? count : temp.Entity.CatalogList.Count;
            Task[] tasks = new Task[count];

            var groups = Split<BookCatalog>(temp.Entity.CatalogList, count);
            int index = 0;

            if (!string.IsNullOrEmpty(temp.Entity.Cover))
            {
                try
                {
                    StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(AppDataPath.GetLocalBookCoverFolderPath());
                    StorageFile file = null;
                    string filename = temp.Entity.BookID + ".jpg";
                    if (!File.Exists(filename))
                    {
                        file = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    }
                    file = await storageFolder.GetFileAsync(filename);
                    using (var stream = await file.OpenReadAsync())
                    {
                        if (stream.Size <= 0)
                        {
                            var tmpfile = await storageFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                            var http = new HttpClient();
                            var data = await http.GetByteArrayAsync(temp.Entity.Cover);
                            await FileIO.WriteBytesAsync(tmpfile, data);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(temp.Entity.BookName + "  下载封面失败");
                }
            }

            try
            {
                foreach (var list in groups)
                {
                    tasks[index] = await Task.Factory.StartNew(async () =>
                   {
                       HttpHelper http = new HttpHelper();
                       foreach (var catalog in list)
                       {
                           if (temp.IsPause)
                           {
                               return;
                           }
                           if (string.IsNullOrEmpty(catalog.CatalogUrl))
                           {
                               continue;
                           }
                           try
                           {
                               string html = await GetHtmlData(catalog.CatalogUrl, http);

                               if (string.IsNullOrEmpty(html))
                               {
                                   html = GetHtmlData(catalog.CatalogUrl, http).Result;
                               }
                               if (string.IsNullOrEmpty(html))
                               {
                                   Debug.WriteLine("*******下载失败****** :" + temp.Entity.BookName + "  " + catalog.CatalogName + "   " + catalog.CatalogUrl);
                               }
                               else
                               {
                                   Debug.WriteLine("下载完成 :" + temp.Entity.BookName + "  " + catalog.CatalogName + "   " + catalog.CatalogUrl);
                               }

                               BookCatalogContent content = new BookCatalogContent()
                               {
                                   BookID = catalog.BookID,
                                   CatalogUrl = catalog.CatalogUrl,
                                   Content = html,
                               };
                               temp.ContentList.Add(content);

                               DispatcherHelper.CheckBeginInvokeOnUI(temp.SetProcessValue);

                               //if (IsFrameContent)
                               //{

                               //    await Task.Factory.StartNew((obj) =>
                               //    {
                               //       temp.SetProcessValue();
                               //    }, null, new CancellationTokenSource().Token, TaskCreationOptions.None, _syncContextTaskScheduler);
                               //}
                           }
                           catch (Exception ex)
                           {
                               Debug.WriteLine(ex.Message);
                               continue;
                           }
                       }
                   });
                    index++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                temp.IsPause = true;
            }


            await Task.Factory.ContinueWhenAll(tasks, (obj) =>
           {
               if (temp.IsPause)
               {
                   if (DownLoadList.Contains(temp))
                   {
                       DispatcherHelper.CheckBeginInvokeOnUI(() =>
                       {
                           this.DownLoadList.Remove(temp);
                       });
                   }

                   if (File.Exists(AppDataPath.GetLocalBookCoverPath(temp.Entity.BookID)))
                   {
                       File.Delete(AppDataPath.GetLocalBookCoverPath(temp.Entity.BookID));
                   }
                   return;
               }
               DispatcherHelper.CheckBeginInvokeOnUI(() =>
                          {
                              temp.SetProcessValue();
                              temp.Entity.LastReadChapterName = null;
                              temp.Entity.LastReadChapterUrl = null;
                              temp.Entity.IsLocal = true;

                              var catalog = temp.Entity.CatalogList.OrderBy(p => p.Index).ToList().LastOrDefault();
                              if (catalog != null)
                              {
                                  temp.Entity.NewestChapterName = catalog.CatalogName;
                                  temp.Entity.NewestChapterUrl = catalog.CatalogUrl;
                              }
                          });

               DBBookCatalogContent.InsertOrUpdateBookCatalogContents(AppDataPath.GetBookDBPath(temp.Entity.BookID), temp.ContentList);
               DBBookCatalog.InsertOrUpdateBookCatalogs(AppDataPath.GetBookDBPath(temp.Entity.BookID), temp.Entity.CatalogList.ToList());
               lock (isAdd1)
               {
                   DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), temp.Entity);
               }

               try
               {
                   DispatcherHelper.CheckBeginInvokeOnUI(() =>
                   {
                       this.DownLoadList.Remove(temp);
                       ViewModelInstance.Instance.LocalBookPage.LocalBookList.Add(temp.Entity);
                       ToastHeplper.ShowMessage(temp.Entity.BookName + "下载完毕，点击“本地图书”查看");
                   });
               }
               catch (Exception ex)
               {
                   this.DownLoadList.Remove(temp);
                   ToastHeplper.ShowMessage(temp.Entity.BookName + "下载失败");
                   Debug.WriteLine(ex.Message);
               }
               finally
               {
                   if (this.DownLoadList.Count == 0)
                   {
                       this.IsDownLoading = false;
                   }
               }

               watch.Stop();
               Debug.WriteLine("共用时：" + watch.Elapsed.TotalSeconds);
           });


        }

        public IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> items, int numOfParts)
        {
            int i = 0;
            return items.GroupBy(x => i++ % numOfParts);
        }

        private async Task<string> GetHtmlData(string catalogUrl, HttpHelper http)
        {
            string html = null;
            try
            {
                html = await AnalysisContentService.GetHtmlContent(http, catalogUrl);
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

                StartNewCommonDownLoadInstance(entity);
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
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
          {
              DeleteDownLoadItem(entity);
          }));
            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
            {
                return;
            }));
            await msgDialog.ShowAsync();
        }

        private async void DeleteDownLoadItem(DowmLoadEntity entity)
        {
            entity.IsPause = true;
            await Task.Delay(800);
            if (entity.IsCompleted)
            {
                ToastHeplper.ShowMessage(entity.Entity.BookName + "已下载完毕，正在处理数据，无法取消");
                return;
            }
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
                            msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                          {
                              foreach (var item in DownLoadList)
                              {
                                  DeleteDownLoadItem(item);
                              }
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
                                StartNewCommonDownLoadInstance(item);
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

        private int m_DownLoadCount;
        public int DownLoadCount
        {
            get
            {
                return m_DownLoadCount;
            }
            set
            {
                SetProperty(ref m_DownLoadCount, value);
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

        /// <summary>
        /// 快速下载模式
        /// </summary>
        private bool m_IsFast = false;
        public bool IsFast
        {
            get
            {
                return m_IsFast;
            }
            set
            {
                SetProperty(ref m_IsFast, value);
            }
        }

        public List<BookCatalogContent> ContentList { get; set; }

        private string m_Note;
        public string Note
        {
            get
            {
                return m_Note;
            }
            set
            {
                SetProperty(ref m_Note, value);
            }
        }

        public bool IsCompleted = false;

        public void SetProcessValue()
        {
            this.DownLoadCount = this.ContentList.Count;
            this.ProgressValue = Math.Round(((double)this.DownLoadCount / (double)this.Entity.CatalogList.Count), 3) * 100;

            if (this.DownLoadCount == 0)
            {
                this.Note = "数据初始化中...";
            }
            else if (this.DownLoadCount == this.Entity.CatalogList.Count)
            {
                this.IsCompleted = true;
                this.Note = "下载完毕，数据处理中...";
            }
            else
            {
                this.Note = null;
            }
        }
    }
}
