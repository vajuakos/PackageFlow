using System.Security.Claims;

namespace PackageFlow.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal? principal)
        {
            if (principal is null)
                return null;

            var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(claim))
                return null;

            return int.TryParse(claim, out var userId) ? userId : null;
        }
    }
}
