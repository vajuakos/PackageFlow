using PackageFlow.Core.Models;

namespace PackageFlow.Data.Services.Auth
{
    public interface IUserSessionService
    {
        AppUser? CurrentUser { get; }
        bool IsAuthenticated { get; }
        void SetSession(AppUser user);
        void ClearSession();
    }
}
