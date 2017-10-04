
using System.Web;
namespace Westwind.Web.Mvc
{
    public class BaseViewModel
    {
        public ErrorDisplay ErrorDisplay = null;
        public UserState UserState = null;        
        public string PageTitle = null;

        //public PagingDetails Paging = null;

        public BaseViewModel()
        {
        }
    }
}
