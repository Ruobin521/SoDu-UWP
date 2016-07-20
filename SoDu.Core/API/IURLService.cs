using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoDu.Core.API
{
    public interface IURLService
    {
        string GetHomePage();

        string GetBookShelfPage();

        string GetRankListPage(string pageIndex = null);

        string GetLoginPage();

        string GetLogoutPage();

        string GetRegisterPostPage();

        string GetSearchPage();

        string GetAddToShelfPage();

    }
}
