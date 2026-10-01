using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PackageFlow.Server.Services.Authentication;
using PackageFlow.Shared.DTOs.Authentication;

namespace PackageFlow.Server.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public Task<bool> Register(RegistrationRequest request)
        {
            return _authService.RegisterAsync(request);
        }

        [HttpPost("login")]
        public Task<LoginResponse> Login(LoginRequest request)
        {
            return _authService.LoginAsync(request);
        }
    }
}
