using Newtonsoft.Json;
using Westwind.AspNetCore.Components;
using Westwind.Web;

namespace Westwind.AspNetCore
{
    public class BaseViewModel
    {
        [JsonIgnore]
        public ErrorDisplayModel ErrorDisplay = null;
        
        [JsonIgnore]
        public string PageTitle = null;

        //public PagingDetails Paging = null;

        public BaseViewModel()
        {
        }
    }
}
