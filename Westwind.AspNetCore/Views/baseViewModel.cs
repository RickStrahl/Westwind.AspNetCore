using Westwind.AspNetCore.Components;
using Westwind.Web;

namespace Westwind.AspNetCore
{
    public class BaseViewModel
    {
        public ErrorDisplayModel ErrorDisplay = null;
        public UserState UserState = null;        
        public string PageTitle = null;

        //public PagingDetails Paging = null;

        public BaseViewModel()
        {
        }
    }
}
