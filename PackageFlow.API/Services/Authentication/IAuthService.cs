using PackageFlow.Shared.DTOs.Authentication;

namespace PackageFlow.API.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegistrationRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}
