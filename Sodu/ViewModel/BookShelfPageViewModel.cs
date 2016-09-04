using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
using Sodu.Core.Model;
using Sodu.Model;
using Sodu.Services;

using SoDu.Core.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.Unity;
using Microsoft.VisualBasic.CompilerServices;
using Sodu.Core.Config;
using SoDu.Core.API;
using Sodu.Core.Util;
using SoDu.Core.Database;

namespace Sodu.ViewModel
{
    public class BookShelfPageViewModel : BaseViewModel, IViewModel
    {


        #region  属性 字段
        private string _ContentTitle = "个人书架";
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
        private bool m_IsAllSelected;
        public bool IsAllSelected
        {
            get
            {
                return m_IsAllSelected;
            }
            set
            {
                SetProperty(ref this.m_IsAllSelected, value);
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

        private bool m_IsShow = false;

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

        private ObservableCollection<BookEntity> m_ShelfBookList;
        //我的书架
        public ObservableCollection<BookEntity> ShelfBookList
        {
            get
            {
                return m_ShelfBookList ?? (m_ShelfBookList = new ObservableCollection<BookEntity>());
            }
            set
            {
                this.SetProperty(ref this.m_ShelfBookList, value);
            }
        }


        private ObservableCollection<BookEntity> m_HistoryBookList;
        //上次记录的数据
        public ObservableCollection<BookEntity> HistoryBookList
        {
            get
            {
                return m_HistoryBookList ?? (m_HistoryBookList = new ObservableCollection<BookEntity>());
            }
            set
            {
                this.SetProperty(ref this.m_HistoryBookList, value);
            }
        }


        HttpHelper http = new HttpHelper();

        #endregion

        public BookShelfPageViewModel()
        {

        }

        #region 方法

        /// <summary>
        /// 取消请求
        /// </summary>
        public void CancleHttpRequest()
        {
            try
            {
                if (http != null)
                {
                    http.HttpClientCancleRequest();
                }
                IsLoading = false;
            }
            catch (Exception)
            {

            }

        }

        public bool BackpressedHandler()
        {
            CancleHttpRequest();

            if (IsEditing)
            {
                OnEditCommand(false);
                return true;
            }

            if (IsLoading)
            {
                CancleHttpRequest();
                return true;
            }

            return false;
        }

        public void InitData(object obj = null)
        {
            this.IsShow = false;
            //if (this.ShelfBookList.Count > 0)
            //{
            //    return;
            //}

            SetData();
        }

        public void SetData()
        {
            //设置编辑模式为false
            IsEditing = false;
            Task.Run(async () =>
            {
                string html = await GetHtmlData();
                return html;
            }).ContinueWith((result) =>
          {
              if (result.Result != null)
              {
                  string html = result.Result;
                  DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                      if (html.Contains("退出") && html.Contains("站长留言") && html.Contains("我的书架"))
                      {
                          SetBookList(html);
                      }
                      else if (html.Contains("您还没有登录"))
                      {
                          ToastHeplper.ShowMessage("您还没有登录");
                          ViewModelInstance.Instance.MainPageViewModelInstance.ChangeLoginState(false);
                      }
                      else
                      {
                          ToastHeplper.ShowMessage("获取数据失败");
                      }
                  });
              }
          });
        }


        private async Task<string> GetHtmlData()
        {
            string html = null;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
          {
              IsLoading = true;
          });
            try
            {
                html = await http.WebRequestGet(ViewModelInstance.Instance.UrlService.GetBookShelfPage(), true);
            }
            catch (Exception)
            {
                html = null;
            }
            finally
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
              {
                  IsLoading = false;
              });
            }
            return html;
        }

        public void SetBookList(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                var list = AnalysisSoduService.GetBookShelftListFromHtml(html);
                if (list == null)
                {
                    this.IsShow = true;
                }
                else
                {
                    this.ShelfBookList?.Clear();
                    this.IsShow = false;

                    if (list.Count > 0)
                    {
                        var temp = list.OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
                        foreach (var item in temp)
                        {
                            var entity = DBBookShelf.GetBook(AppDataPath.GetBookShelfDBPath(), item);
                            if (!string.IsNullOrEmpty(entity?.LastReadChapterName))
                            {
                                item.LastReadChapterName = entity.LastReadChapterName;
                                var sim = LevenshteinDistancePercent(item.LastReadChapterName, item.NewestChapterName);
                                if (sim)
                                {
                                    item.UnReadCountData = "";
                                }
                                else
                                {
                                    item.UnReadCountData = "(有更新)";
                                }
                            }
                            else
                            {
                                item.UnReadCountData = "(有更新)";
                                item.LastReadChapterName = "无";
                            }
                            ShelfBookList.Add(item);
                        }
                        DBBookShelf.ClearBooks(AppDataPath.GetBookShelfDBPath());
                        DBBookShelf.InsertOrUpdateBooks(AppDataPath.GetBookShelfDBPath(), ShelfBookList.ToList());
                    }
                }
                ToastHeplper.ShowMessage("个人书架已更新");
            }
        }

        /// <summary>
        /// 计算字符串相似度
        /// </summary>
        /// <param name=”str1″></param>
        /// <param name=”str2″></param>
        /// <returns></returns>
        public bool LevenshteinDistancePercent(string str1, string str2)
        {

            str1 = str1.ToLower().Replace(" ", "").Replace("　", "");
            str2 = str2.ToLower().Replace(" ", "").Replace("　", "");

            if (str1.Equals(str2) || str1.Contains(str2) || str2.Contains(str1))
            {
                return true;
            }

            return false;
        }
        public void RemoveBookList(List<BookEntity> removeBookList)
        {

            if (IsLoading) return;
            IsLoading = true;

            Task.Run(async () =>
        {
            bool result = true;
            foreach (var item in removeBookList)
            {
                if (!IsLoading)
                {
                    result = false;
                    break;
                }
                string url = ViewModelInstance.Instance.UrlService.GetBookShelfPage() + "?id=" + item.BookID;
                string html = await http.WebRequestGet(url);
                if (html.Contains("取消收藏成功"))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                  {
                      ShelfBookList.Remove(item);
                  });
                }
                else
                {
                    result = false;
                }
            }
            return result;

        }).ContinueWith((result) =>
     {
         DispatcherHelper.CheckBeginInvokeOnUI(() =>
         {
             IsLoading = false;
             if (result.Result)
             {
                 //   ToastHeplper.ShowMessage("操作完毕");
                 removeBookList.Clear();
                 OnEditCommand(false);
                 RefreshList();
             }
             else
             {
                 ToastHeplper.ShowMessage("取消收藏失败。");
             }
         }
          );
     });
        }


        public void AddEntityToList(BookEntity entity)
        {
            Task.Run(async () =>
            {
                if (ShelfBookList.ToList().Find(p => p.BookID == entity.BookID) == null)
                {
                    string html = await (new HttpHelper()).WebRequestGet(string.Format(ViewModelInstance.Instance.UrlService.GetAddToShelfPage(), entity.BookID));
                    if (html.Contains("{\"success\":true}"))
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var temp = entity.Clone();
                            temp.LastReadChapterName = temp.NewestChapterName;
                            temp.UnReadCountData = null;
                            var result = DBBookShelf.InsertOrUpdateBook(AppDataPath.GetBookShelfDBPath(), temp);
                            ShelfBookList.Insert(0, temp);
                            RefreshList();
                        });
                    }
                    else
                    {
                        ToastHeplper.ShowMessage(entity.BookName + " 添加至个人书架失败");
                    }
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        var temp = ShelfBookList.ToList().Find(p => p.BookID == entity.BookID);
                        var sim = LevenshteinDistancePercent(temp.LastReadChapterName, entity.NewestChapterName);
                        if (sim)
                        {
                            temp.LastReadChapterName = temp.NewestChapterName;
                            temp.UnReadCountData = null;
                            var result = DBBookShelf.InsertOrUpdateBook(AppDataPath.GetBookShelfDBPath(), temp);
                        }
                    });
                }
            });


        }

        public void RefreshList()
        {
            if (this.ShelfBookList.Count > 0)
            {
                List<BookEntity> list = new List<BookEntity>();
                this.ShelfBookList.ToList().ForEach(p => list.Add(p));
                this.ShelfBookList.Clear();
                list = list.OrderByDescending(p => DateTime.Parse(p.UpdateTime)).ToList();
                list.ForEach(p => ShelfBookList.Add(p));
            }
            else
            {
                IsShow = true;
            }

        }


        #endregion


        #region  命令

        /// <summary>
        /// 全选，全不选
        /// </summary>
        private RelayCommand<bool> m_EditCommand;
        public RelayCommand<bool> EditCommand
        {
            get
            {
                return m_EditCommand ?? (m_EditCommand = new RelayCommand<bool>(
                 (obj) =>
                 {

                     if (IsLoading) return;
                     OnEditCommand(!IsEditing);
                 }
                    ));
            }
        }

        public void OnEditCommand(bool isEdit)
        {
            if (IsLoading) return;

            if (this.ShelfBookList == null || this.ShelfBookList.Count < 1)
            {
                IsEditing = false;
                return;
            }
            SetBookShelfEditStatus(isEdit);
        }

        public void SetBookShelfEditStatus(bool vale)
        {
            IsEditing = vale;

            if (!vale)
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsInEdit = false;
                    item.IsSelected = false;
                }
                IsEditing = false;
                IsAllSelected = false;
            }
            else
            {
                IsEditing = true;
                foreach (var item in ShelfBookList)
                {
                    item.IsInEdit = true;
                }
            }
        }


        /// <summary>
        /// 全选，全不选
        /// </summary>
        private RelayCommand<object> m_SelectAllCommand;
        public RelayCommand<object> SelectAllCommand
        {
            get
            {
                return m_SelectAllCommand ?? (m_SelectAllCommand = new RelayCommand<object>(OnSelectAllCommand));
            }
        }
        public void OnSelectAllCommand(object obj)
        {
            if (!IsEditing) return;

            if (!IsAllSelected)
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = true;
                }
                IsAllSelected = true;
            }
            else
            {
                foreach (var item in ShelfBookList)
                {
                    item.IsSelected = false;
                }
                IsAllSelected = false;
            }
        }


        /// <summary>
        /// 下架
        /// </summary>
        private RelayCommand<object> m_RemoveBookFromShelfCommand;
        public RelayCommand<object> RemoveBookFromShelfCommand
        {
            get
            {
                return m_RemoveBookFromShelfCommand ?? (m_RemoveBookFromShelfCommand = new RelayCommand<object>(OnRemoveBookFromShelfCommand));
            }
        }
        private async void OnRemoveBookFromShelfCommand(object obj)
        {

            if (!IsEditing)
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog(" \n请点击编辑按钮，并选择需要取消收藏的小说。") { Title = "取消收藏" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                {
                    return;
                }));
                await msgDialog.ShowAsync();
            }
            else
            {

                List<BookEntity> removeList = new List<BookEntity>();

                foreach (var item in ShelfBookList)
                {
                    if (item.IsSelected == true)
                    {
                        removeList.Add(item);
                    }
                }

                if (removeList.Count > 0)
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n确定取消收藏" + removeList.Count + "本小说？") { Title = "取消收藏" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        RemoveBookList(removeList);
                    }));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();

                }
                else
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("\n请勾选需要取消收藏的小说。") { Title = "取消收藏" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定", uiCommand =>
                    {
                        return;
                    }));
                    await msgDialog.ShowAsync();
                }
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
            if (IsLoading) return;
            BookEntity entity = obj as BookEntity;
            if (entity != null)
            {
                if (IsEditing)
                {
                    entity.IsSelected = !entity.IsSelected;

                    IsAllSelected = true;
                    foreach (var item in ShelfBookList)
                    {
                        if (!item.IsSelected)
                        {
                            IsAllSelected = false;
                            break;
                        }
                    }
                }
                else
                {
                    ViewModelInstance.Instance.MainPageViewModelInstance.OnBookItemSelectedChangedCommand(obj);

                    if (!string.IsNullOrEmpty(entity.UnReadCountData) || !entity.LastReadChapterName.Equals(entity.NewestChapterName))
                    {
                        entity.UnReadCountData = string.Empty;
                        entity.LastReadChapterName = entity.NewestChapterName;
                        var result = DBBookShelf.InsertOrUpdateBook(AppDataPath.GetBookShelfDBPath(), entity);
                    }

                }
            }
        }


        ///跳转到相应页数
        /// </summary>
        private RelayCommand<object> m_RefreshCommand;
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return m_RefreshCommand ?? (m_RefreshCommand = new RelayCommand<object>(OnRefreshCommand));
            }
        }

        private void OnRefreshCommand(object obj)
        {

            if (IsLoading)
            {
                CancleHttpRequest();
            }
            else
            {
                SetData();
            }
        }


        #endregion

    }
}
