using PackageFlow.Core.Enums;
using PackageFlow.Core.Models;
using PackageFlow.Core.Helpers;
using PackageFlow.Data.Context;

namespace PackageFlow.Data.Seed
{
    public static class DatabaseSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Users.Any()) return;

            var defaultUsers = new List<AppUser>
            {
                new()
                {
                    Username = "admin",
                    Email = "admin@packageflow.local",
                    PasswordHash = PasswordHelper.Hash("Admin123!"),
                    Role = UserRole.Admin
                },
                new()
                {
                    Username = "courier1",
                    Email = "courier@packageflow.local",
                    PasswordHash = PasswordHelper.Hash("Courier123!"),
                    Role = UserRole.Courier
                },
                new()
                {
                    Username = "warehouse1",
                    Email = "warehouse@packageflow.local",
                    PasswordHash = PasswordHelper.Hash("Warehouse123!"),
                    Role = UserRole.Warehouseman
                },
                new()
                {
                    Username = "customer1",
                    Email = "customer@packageflow.local",
                    PasswordHash = PasswordHelper.Hash("Customer123!"),
                    Role = UserRole.Customer
                }
            };

            context.Users.AddRange(defaultUsers);
            context.SaveChanges();
        }
    }
}
