using Microsoft.EntityFrameworkCore;
using PackageFlow.Core.Helpers;
using PackageFlow.Core.Models;
using PackageFlow.Data.Context;

namespace PackageFlow.Data.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _dbContext;

        public AuthService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public UserSessionModel Login(string username, string? password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(password))
            {
                return new UserSessionModel { IsLoginSucceed = false };
            }

            string hashedPassword = PasswordHelper.Hash(password);

            // Compare input password with pawword stored in database
            var user = _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == username &&
                    u.PasswordHash == hashedPassword);

            if (user == null) new UserSessionModel { IsLoginSucceed = false };

            return new UserSessionModel
            {
                IsLoginSucceed = true,
                CurrentUser = user
            };
        }
    }
}
