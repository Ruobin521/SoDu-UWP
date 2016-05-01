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

namespace Sodu.ViewModel
{
    public class DownLoadCenterViewModel : BaseViewModel
    {
        private bool isDownLoading { get; set; }

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

        public void AddNewDownloadItem(BookEntity entity)
        {
            if (this.DownLoadList.ToList().FirstOrDefault(p => p.Entity.BookID == entity.BookID) != null)
            {
                Services.CommonMethod.ShowMessage("正在下载图书，请耐心等待。");
                return;
            }
            DowmLoadEntity temp = new DowmLoadEntity();
            temp.Entity = entity;
            this.DownLoadList.Add(temp);
            StartNewDownLoadInstance();
        }

        public void StartNewDownLoadInstance()
        {
            if (isDownLoading) return;

            Task.Run(async () =>
           {
               isDownLoading = true;
               while (true)
               {
                   if (this.DownLoadList == null || this.DownLoadList.Count < 1)
                   {
                       break;
                   }
                   bool result = true;

                   DowmLoadEntity temp = DownLoadList[0];
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

                       //for (int i = 0; i < temp.Entity.CatalogList.Count; i++)
                       for (int i = startIndex; i < 50; i++)
                       {
                           try
                           {
                               var item = temp.Entity.CatalogList[i];
                               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                               {
                                   temp.CurrentCatalog = item;
                                   temp.CurrentIndex = i + 1;
                                   temp.ProgressValue = Math.Round(((double)item.Index / (double)temp.Entity.CatalogList.Count), 3, MidpointRounding.AwayFromZero) * 100;
                               });
                               string html = await GetHtmlData(item.CatalogUrl);
                               item.CatalogContent = html;
                               Database.DBLocalBook.InsertOrUpdateBookCatalog(AppDataPath.GetLocalBookDBPath(), item);
                           }
                           catch (Exception ex)
                           {
                               continue;
                           }
                       }
                   }
                   catch (Exception)
                   {
                       result = false;
                   }
                   if (result)
                   {
                       Database.DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), temp.Entity);
                       await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           this.DownLoadList.Remove(temp);
                           Services.CommonMethod.ShowMessage(temp.Entity.BookName + "下载完毕，您可在左侧导航“本地图书”中查看");
                           Services.CommonMethod.ShowMessage(temp.Entity.BookName + "下载完毕，您可在左侧导航“本地图书”中查看");
                       });
                   }
               }
           }).ContinueWith((result) =>
           {
               isDownLoading = false;
           });
        }

        private async Task<string> GetHtmlData(string catalogUrl)
        {
            string html = null;
            try
            {
                html = await new HttpHelper().WebRequestGet(catalogUrl, false);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                html = Services.AnalysisContentHtmlService.AnalysisContentHtml(html, catalogUrl);
                if (string.IsNullOrEmpty(html) || string.IsNullOrWhiteSpace(html))
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                html = null;
            }

            return html;
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
        public bool IsPause { get; set; }
    }
}
