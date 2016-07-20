using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Config;
using Sodu.Core.Database;
using Sodu.Core.Model;
using Sodu.Model;
using Sodu.Pages;
using Sodu.Services;
using Sodu.Util;
using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sodu.ViewModel
{
    public class BookContentPageViewModel : BaseViewModel, IViewModel
    {
        #region 属性，字段

        HttpHelper htmlHttp = new HttpHelper();
        HttpHelper catalogsHttp = new HttpHelper();


        private bool isClickCtalog = false;

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
        private bool m_IsLoadingCatalogList;
        public bool IsLoadingCatalogList
        {
            get
            {
                return m_IsLoadingCatalogList;
            }
            set
            {
                SetProperty(ref m_IsLoadingCatalogList, value);
            }
        }

        private string m_ContentTitle;
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


        public string m_TextContent;
        public string TextContent
        {
            get
            {
                return m_TextContent;
            }
            set
            {
                SetProperty(ref m_TextContent, value);
            }
        }

        public ObservableCollection<string> m_ContentListt;
        public ObservableCollection<string> ContentListt
        {
            get
            {
                if (m_ContentListt == null)
                {
                    m_ContentListt = new ObservableCollection<string>();
                }
                return m_ContentListt;
            }
            set
            {
                SetProperty(ref m_ContentListt, value);
            }
        }




        public bool m_IsSwitchButtonShow;
        public bool IsSwitchButtonShow
        {
            get
            {
                return m_IsSwitchButtonShow;
            }
            set
            {
                SetProperty(ref m_IsSwitchButtonShow, value);
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

        private BookEntity m_BookEntity;
        public BookEntity BookEntity
        {
            get
            {
                return m_BookEntity;
            }
            set
            {
                this.SetProperty(ref this.m_BookEntity, value);
            }
        }

        #endregion


        #region 方法

        public BookContentPageViewModel()
        {

        }

        public void InitData(object obj)
        {
            CancleHttpRequest();


            BookEntity entity = obj as BookEntity;
            if (entity == null)
            {
                return;
            }

            this.TextContent = null;

            if (!entity.IsLocal)
            {
                this.IsSwitchButtonShow = false;
            }
            else
            {
                this.IsSwitchButtonShow = true;

            }

            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }

            this.BookEntity = entity;

            if (this.BookEntity.IsLocal)
            {
                this.CurrentCatalog = entity.CatalogList.FirstOrDefault(p => p.CatalogUrl == entity.LastReadChapterUrl);
            }
            else
            {
                this.BookEntity.CatalogList = null;
                this.CurrentCatalog = new BookCatalog() { BookID = entity.BookID, CatalogName = entity.NewestChapterName, CatalogUrl = entity.NewestChapterUrl, LyWeb = new Uri(entity.NewestChapterUrl).Authority };
            }

            this.ContentTitle = CurrentCatalog.CatalogName;

            SetData(CurrentCatalog);
            BookEntity.CatalogListUrl = SetBookCataologListUrl(CurrentCatalog.CatalogUrl);
            SetBookCatalogList();
        }

        public void SetData(BookCatalog catalog)
        {
            IsLoading = true;

            Task.Run(async () =>
           {
               string resultHtml = null;
               try
               {
                   if (this.BookEntity.IsLocal)
                   {
                       resultHtml = await GetCatafromDatabase(catalog);
                       if (!string.IsNullOrEmpty(resultHtml))
                       {
                           return resultHtml;
                       }
                   }
                   resultHtml = await AnalysisContentService.GetHtmlContent(htmlHttp, catalog.CatalogUrl);
               }
               catch (Exception ex)
               {
                   resultHtml = null;
               }
               finally
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoading = false;
                   });
               }
               return resultHtml;

           }).ContinueWith(async (result) =>
          {
              string html = result.Result;
              bool rs = false;
              await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
              {
                  if (html != null)
                  {

                      this.CurrentCatalog = catalog;

                      this.BookEntity.LastReadChapterName = catalog.CatalogName;
                      this.BookEntity.LastReadChapterUrl = catalog.CatalogUrl;

                      //  TextContent = html;

                      SetTextContent(html);
                      this.ContentTitle = BookEntity.LastReadChapterName;
                      if (!this.BookEntity.IsLocal)
                      {
                          // ToastHeplper.ShowMessage("正文加载完毕");
                      }
                      else
                      {
                          try
                          {
                              DBLocalBook.InsertOrUpdateBookEntity(AppDataPath.GetLocalBookDBPath(), BookEntity);
                          }
                          catch (Exception ex)
                          {

                          }
                      }
                      rs = true;
                  }
                  else
                  {
                      ToastHeplper.ShowMessage("未能获取正文内容");
                      rs = false;
                  }
              });
              return rs;
          });
        }

        private void SetBookCatalogList()
        {
            Task.Run(async () =>
           {
               try
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoadingCatalogList = true;
                   });
                   if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                   {
                       return;
                   }
                   else
                   {
                       if (string.IsNullOrEmpty(this.BookEntity.CatalogListUrl))
                       {
                           return;
                       }

                       List<BookCatalog> list = await Services.AnalysisBookCatalogList.GetCatalogList(BookEntity.CatalogListUrl, this.BookEntity.BookID, catalogsHttp);
                       if (list != null && list.Count > 0)
                       {
                           if (this.BookEntity.CatalogList == null)
                           {
                               this.BookEntity.CatalogList = new ObservableCollection<BookCatalog>();
                           }
                           this.BookEntity.CatalogList.Clear();
                           foreach (var item in list)
                           {
                               this.BookEntity.CatalogList.Add(item);
                           }
                           await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                           {
                               IsSwitchButtonShow = true;
                           });
                       }
                   }
               }
               catch (Exception ex)
               {
               }
               finally
               {
                   await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                   {
                       IsLoadingCatalogList = false;
                       IsLoading = false;
                       if (isClickCtalog && IsSwitchButtonShow)
                       {
                           NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                           isClickCtalog = false;
                       }
                   });
               }
           });

        }

        private string SetBookCataologListUrl(string catalogUrl)
        {
            var catalogListUrl = Services.AnalysisBookCatalogUrl.GetBookCatalogListUrl(catalogUrl);

            return catalogListUrl;
        }

        private async void SetTextContent(string html)
        {
            this.ContentTitle = this.BookEntity.BookName + "_" + this.CurrentCatalog.CatalogName;

            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height;

            if (this.ContentListt != null)
            {
                this.ContentListt.Clear();
            }
            List<string> strList = SplitString(html);
            this.ContentListt.Add(strList[0]);

            for (int i = 1; i < strList.Count; i++)
            {
                this.ContentListt.Add(strList[i]);
                await Task.Delay(1);
            }
        }

        private List<string> SplitString(string str)
        {
            List<string> strList = new List<string>();

            string[] lists = str.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lists.Count(); i++)
            {
                strList.Add(lists[i]);
            }
            return strList;
        }

        /// <summary>
        /// 从数据库获取数据
        /// <param name="catalog"></param>
        /// <param name="isBackGround"></param>
        /// <returns></returns>
        private async Task<string> GetCatafromDatabase(BookCatalog catalog, bool isBackGround = true)
        {
            string html = null;
            await Task.Run(async () =>
            {
                try
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = true;
                    });

                    var item = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == catalog.CatalogUrl);
                    if (item != null)
                    {
                        var content = DBBookCatalogContent.SelectBookCatalogContent(AppDataPath.GetBookDBPath(BookEntity.BookID), item.CatalogUrl);
                        if (content != null)
                        {
                            html = content.Content;
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        IsLoading = false;
                    });
                }

            });
            return html;
        }

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            if (htmlHttp != null)
            {
                htmlHttp.HttpClientCancleRequest();
            }
            if (catalogsHttp != null)
            {
                catalogsHttp.HttpClientCancleRequest();
            }
            IsLoading = false;
        }

        #endregion

        #region 命令


        /// <summary>
        /// 字体增大
        /// </summary>
        public ICommand FontIncreaseCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
              {
                  ViewModelInstance.Instance.SettingPageViewModelInstance.SetTextSize(ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie + 2);
              });
            }
        }

        /// <summary>
        /// 字体减小
        /// </summary>
        public ICommand FontDecreaseCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
             {
                 ViewModelInstance.Instance.SettingPageViewModelInstance.SetTextSize(ViewModelInstance.Instance.SettingPageViewModelInstance.TextFontSzie - 2);
             });
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    if (IsLoading)
                    {
                        CancleHttpRequest();
                    }
                    else
                    {
                        SetData(CurrentCatalog);
                    }
                });
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        public ICommand GoBackCommand
        {
            get
            {
                return new RelayCommand<bool>((str) =>
                {
                    NavigationService.GoBack();
                });
            }
        }


        /// <summary>
        /// 目录
        /// </summary>
        public ICommand CatalogCommand
        {
            get
            {
                return new RelayCommand<bool>(async (str) =>
              {
                  try
                  {
                      await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                      {
                          IsLoading = true;
                      });
                      if (IsLoadingCatalogList)
                      {
                          ToastHeplper.ShowMessage("正在加载目录,请稍候");
                          isClickCtalog = true;
                          return;
                      }

                      bool rs = false;
                      if (this.BookEntity.CatalogList != null && this.BookEntity.CatalogList.Count > 0)
                      {
                          rs = true;
                      }
                      else
                      {

                          ToastHeplper.ShowMessage("正在加载目录,请稍候");
                          isClickCtalog = true;
                          SetBookCatalogList();
                      }
                      if (rs)
                      {
                          NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                      }
                      else
                      {
                          ToastHeplper.ShowMessage("目录加载失败");
                      }

                      await NavigationService.ContentFrame.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                      {
                          IsLoading = false;
                      });
                  }
                  catch (Exception ex)
                  {

                  }
              });
            }
        }

        /// <summary>
        /// 切换章节
        /// </summary>
        public ICommand SwitchCatalogCommand
        {
            get
            {
                return new RelayCommand<object>((str) =>
                {
                    OnSwtichCommand(str);
                });
            }
        }

        public void OnSwtichCommand(object str)
        {
            if (IsLoading) return;

            try
            {
                if (this.BookEntity.CatalogList == null || this.BookEntity.CatalogList.Count <= 1)
                {
                    ToastHeplper.ShowMessage("暂未获取到章节列表，无法切换");
                    return;
                }
                var tempcatalog = this.BookEntity.CatalogList.FirstOrDefault(p => p.CatalogUrl == this.CurrentCatalog.CatalogUrl);
                var ooo = this.BookEntity.CatalogList.LastOrDefault();
                int index = this.BookEntity.CatalogList.IndexOf(tempcatalog);
                //上一章
                if (str.ToString().Equals("0"))
                {
                    if (this.BookEntity.CatalogList.IndexOf(this.CurrentCatalog) == 0)
                    {
                        ToastHeplper.ShowMessage("已经是第一章。");
                        return;
                    }
                    if (this.BookEntity.CatalogList.Count < 2)
                    {
                        return;
                    }
                    if (tempcatalog != null)
                    {
                        tempcatalog = this.BookEntity.CatalogList[this.BookEntity.CatalogList.IndexOf(tempcatalog) - 1];
                    }
                }
                //下一章
                else if (str.ToString().Equals("1"))
                {
                    if (this.BookEntity.CatalogList.Count < 2)
                    {
                        return;
                    }
                    else if (index == this.BookEntity.CatalogList.Count - 1 || index == -1)
                    {
                        NavigationService.NavigateTo(typeof(BookCatalogPage), this.BookEntity);
                        return;
                    }
                    tempcatalog = this.BookEntity.CatalogList[index + 1];
                }
                else if (str is BookCatalog)
                {
                    BookCatalog catalog = str as BookCatalog;
                    tempcatalog = catalog;
                }

                if (tempcatalog != null && tempcatalog.CatalogName != null && tempcatalog.CatalogUrl != null)
                {

                    SetData(tempcatalog);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
