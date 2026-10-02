using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using PackageFlow.Shared.DTOs.Authentication;

namespace PackageFlow.Desktop.Services.Authentication
{
    public class AuthService : IAuthService
    {
        private const string BaseUrl = "api/auth";

        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authStateProvider = authStateProvider ?? throw new ArgumentNullException(nameof(authStateProvider));
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                if (result != null && result.IsSuccess && !string.IsNullOrWhiteSpace(result.Token))
                {
                    if (_authStateProvider is CustomAuthenticationStateProvider customAuthStateProvider)
                    {
                        await customAuthStateProvider.NotifyUserAuthenticatedAsync(result.Token);
                    }

                    return result;
                }

                return new LoginResponse { IsSuccess = false };
            }

            return new LoginResponse { IsSuccess = false };
        }

        public async Task<bool> RegistrateAsync(RegistrationRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

            if (!response.IsSuccessStatusCode) return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task LogoutAsync()
        {
            if (_authStateProvider is CustomAuthenticationStateProvider customAuthStateProvider)
            {
                await customAuthStateProvider.NotifyUserLogoutAsync();
            }
        }
    }
}