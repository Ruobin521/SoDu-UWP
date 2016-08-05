using Sodu.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sodu.Model
{
    public class MenuModel : BaseViewModel
    {
        public string MenuIcon { get; set; }
        public string MenuName { get; set; }
        public Type MenuType { get; set; }


        private bool m_IsSelected;
        public bool IsSelected
        {
            get
            {
                return m_IsSelected;
            }
            set
            {
                SetProperty(ref m_IsSelected, value);
            }
        }
    }
}
