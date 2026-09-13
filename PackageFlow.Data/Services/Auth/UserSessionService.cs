using PackageFlow.Core.Models;

namespace PackageFlow.Data.Services.Auth
{
    public class UserSessionService : IUserSessionService
    {
        public AppUser? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public void SetSession(AppUser user)
        {
            CurrentUser = user;
        }

        public void ClearSession()
        {
            CurrentUser = null;
        }
    }
}
