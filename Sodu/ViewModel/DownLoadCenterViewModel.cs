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
                       BookEntity entity = temp.Entity;
                       entity.Guid = Guid.NewGuid().ToString();
                       foreach (var item in temp.Entity.CatalogList)
                       {
                           try
                           {
                               item.Guid = entity.Guid;
                               await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                               {
                                   temp.CurrentCatalogName = item.CatalogName;
                                   temp.CurrentIndex = item.Index;
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

        private string m_CurrentCatalogName;
        public string CurrentCatalogName
        {
            get
            {
                return m_CurrentCatalogName;
            }
            set
            {
                SetProperty(ref m_CurrentCatalogName, value);
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
