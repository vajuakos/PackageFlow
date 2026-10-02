using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PackageFlow.Desktop.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await SecureStorage.Default.GetAsync("authToken");

                if (string.IsNullOrWhiteSpace(token))
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    SecureStorage.Default.Remove("authToken");
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var identity = new ClaimsIdentity(jwtToken.Claims, "bearer");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task NotifyUserAuthenticatedAsync(string token)
        {
            await SecureStorage.Default.SetAsync("authToken", token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public Task NotifyUserLogoutAsync()
        {
            SecureStorage.Default.Remove("authToken");

            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
            return Task.CompletedTask;
        }
    }
}