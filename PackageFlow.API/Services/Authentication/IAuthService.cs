using PackageFlow.Shared.DTOs.Authentication;

namespace PackageFlow.Server.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegistrationRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
