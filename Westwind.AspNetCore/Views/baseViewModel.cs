using Newtonsoft.Json;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Security;

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

    public class BaseViewModel<TUserState> : BaseViewModel
        where TUserState : UserState, new()
    {

        public TUserState UserState { get; set; }


        public BaseViewModel()
        {

        }

        public BaseViewModel(TUserState userState)
        {
            UserState = userState ?? new TUserState();
        }
    }
}
