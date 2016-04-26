using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.ViewModel
{
    public interface IViewModel
    {
        void RefreshData(object obj = null, bool IsRefresh = true);
        bool IsNeedRefresh { get; set; }

        bool IsLoading { get; set; }
        string ContentTitle { get; set; }


        void CancleHttpRequest();
    }
}
