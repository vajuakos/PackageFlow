using PackageFlow.Shared.DTOs.Authentication;

namespace PackageFlow.Client.Services.Authentication
{
    public interface IAuthService
    {
        Task<bool> RegistrateAsync(RegistrationRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task LogoutAsync();
    }
}
