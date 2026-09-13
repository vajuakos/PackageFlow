using PackageFlow.Core.Models;

namespace PackageFlow.Data.Services.Auth
{
    public interface IAuthService
    {
        UserSessionModel Login(string username, string? password);
    }
}
