using System.Security.Claims;
using Newtonsoft.Json;
using Westwind.AspNetCore.Components;
using Westwind.AspNetCore.Security;

namespace Westwind.AspNetCore
{
    public class BaseViewModel
    {
        [JsonIgnore] public ErrorDisplayModel ErrorDisplay;

        [JsonIgnore] public ClaimsPrincipal IdentityUser;

        [JsonIgnore] public string PageTitle;
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
