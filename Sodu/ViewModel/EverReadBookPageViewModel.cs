using GalaSoft.MvvmLight.Command;
using Sodu.Constants;
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
        public bool IsNeedRefresh { get; set; } = true;

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

        private int m_PageIndex = 1;
        public int PageIndex
        {
            get
            {
                return m_PageIndex;
            }
            set
            {
                SetProperty(ref this.m_PageIndex, value);
            }
        }

        private ObservableCollection<BookEntity> m_BookList = new ObservableCollection<BookEntity>();
        public ObservableCollection<BookEntity> BookList
        {
            get
            {
                return m_BookList;
            }
            set
            {
                this.SetProperty(ref this.m_BookList, value);
            }
        }

        HttpHelper http = new HttpHelper();
        public void CancleHttpRequest()
        {
            http.HttpClientCancleRequest();
            IsLoading = false;
        }
        public void RefreshData(object obj = null, bool IsRefresh = true)
        {
            //string html = string.Empty; ;
            try
            {
                //    IsLoading = true;
                //    if (obj != null)
                //    {
                //        this.PageIndex = Convert.ToInt32(obj);
                //    }

                //    html = await http.HttpClientGetRequest(string.Format(PageUrl.EverReadPage, PageIndex));
                //    SetBookList(html);

                //    if (this.BookList != null && this.BookList.Count > 0)
                //    {
                //        IsShow = false;
                //        CommonMethod.ShowMessage("已刷新，共" + BookList.Count + "条数据");
                //    }
                //    else
                //    {
                //        IsShow = true;
                //        CommonMethod.ShowMessage("你还没有阅读记录");

                //    }

                if (BookList == null || BookList.Count <= 0)
                {
                    CommonMethod.ShowMessage("你还没有阅读记录");
                }
            }
            catch (Exception ex)
            {
                CommonMethod.ShowMessage("获取数据失败，请重新尝试");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async void SetBookList(string html)
        {
            if (!string.IsNullOrEmpty(html))
            {
                ObservableCollection<BookEntity> arrary = GetBookListMethod.GetUpdatePageBookList(html);
                if (arrary == null)
                {
                    return;
                }
                else
                {
                    if (this.PageIndex == 1)
                    {
                        this.BookList.Clear();
                    }
                    foreach (var item in arrary)
                    {
                        this.BookList.Add(item);
                        await Task.Delay(1);
                    }
                }
            }
        }


        public void AddToHistoryList(BookEntity entity)
        {
            if (BookList == null)
            {
                BookList = new ObservableCollection<BookEntity>();
            }

            if (BookList.ToList().Find(p => p.BookName == entity.BookName) != null)
            {
                return;
            }
            BookList.Add(entity);
        }

        #region  上拉刷新,下拉加载

        ///跳转到相应页数
        /// </summary>
        public RelayCommand<object> RefreshCommand
        {
            get
            {
                return new RelayCommand<object>(OnRefreshCommand);
            }
        }

        private void OnRefreshCommand(object obj)
        {
            RefreshData(1);
        }




        public RelayCommand<object> ClearCommand
        {
            get
            {
                return new RelayCommand<object>((e) =>
                    {
                        if (BookList != null && BookList.Count > 0)
                        {
                            BookList.Clear();
                        }
                    });
            }
        }
        #endregion
    }
}
